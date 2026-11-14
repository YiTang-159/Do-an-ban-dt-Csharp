-- ============================================================
--  VÁ CSDL QuanLyBanDienThoai (chạy SAU script gốc, KHÔNG xóa dữ liệu, chạy lại nhiều lần được)
--  1. BAOHANH (MaHD, MaDT) phải nằm trong CHITIETHOADON
--  2. CHITIETHOADON (MaKM, MaDT) phải nằm trong CHITIETKHUYENMAI
--  3. Ràng buộc tiền: ThanhTien theo khuyến mãi; TongTien hóa đơn / phiếu nhập tự bằng tổng chi tiết
--  4. SĐT 10 số bắt đầu bằng 0; ThanhTien phiếu nhập = SoLuong * DonGiaNhap
--  5. MaHang, MaLoai của DIENTHOAI bắt buộc
--  6. Chặn sửa tay SoLuongTon (chỉ đổi qua nhập/bán hàng)
--  7. Không đổi phần trăm khuyến mãi đã dùng trong hóa đơn; số bảo hành không vượt số máy đã mua
--  8. View đối soát tồn kho và tổng tiền
--  Nếu dữ liệu cũ đang vi phạm, script báo lỗi và dừng trước khi sửa gì.
-- ============================================================
USE QuanLyBanDienThoai;
GO
SET QUOTED_IDENTIFIER ON;
SET XACT_ABORT ON;
GO

-- ---------- Kiểm tra dữ liệu cũ (lỗi thì tắt thực thi các batch sau, không sửa gì) ----------
BEGIN TRY
IF EXISTS (SELECT 1 FROM BAOHANH b WHERE NOT EXISTS
           (SELECT 1 FROM CHITIETHOADON c WHERE c.MaHD = b.MaHD AND c.MaDT = b.MaDT))
    THROW 50101, N'Có bảo hành cho máy không thuộc hóa đơn. Sửa BAOHANH rồi chạy lại.', 1;
IF EXISTS (SELECT 1 FROM CHITIETHOADON c WHERE c.MaKM IS NOT NULL AND NOT EXISTS
           (SELECT 1 FROM CHITIETKHUYENMAI k WHERE k.MaKM = c.MaKM AND k.MaDT = c.MaDT))
    THROW 50102, N'Có dòng hóa đơn dùng khuyến mãi không áp dụng cho máy đó. Sửa rồi chạy lại.', 1;
IF EXISTS (SELECT 1 FROM CHITIETHOADON c
           LEFT JOIN CHITIETKHUYENMAI k ON k.MaKM = c.MaKM AND k.MaDT = c.MaDT
           WHERE c.ThanhTien <> ROUND(CAST(c.DonGia AS DECIMAL(38,6)) * c.SoLuong
                                      * (100 - ISNULL(k.PhanTramGiam, 0)) / 100, 0))
    THROW 50103, N'Có dòng hóa đơn có ThanhTien sai công thức. Sửa rồi chạy lại.', 1;
IF EXISTS (SELECT 1 FROM HOADON h WHERE h.TongTien <>
           (SELECT ISNULL(SUM(c.ThanhTien), 0) FROM CHITIETHOADON c WHERE c.MaHD = h.MaHD))
    THROW 50104, N'Có hóa đơn TongTien khác tổng chi tiết. Sửa rồi chạy lại.', 1;
IF EXISTS (SELECT 1 FROM PHIEUNHAPHANG p WHERE p.TongTien <>
           (SELECT ISNULL(SUM(c.ThanhTien), 0) FROM CHITIETPHIEUNHAPHANG c WHERE c.MaPN = p.MaPN))
    THROW 50105, N'Có phiếu nhập TongTien khác tổng chi tiết. Sửa rồi chạy lại.', 1;
IF EXISTS (SELECT 1 FROM DIENTHOAI WHERE MaHang IS NULL OR MaLoai IS NULL)
    THROW 50106, N'Có điện thoại chưa có hãng hoặc loại. Điền MaHang/MaLoai rồi chạy lại.', 1;
END TRY
BEGIN CATCH
    PRINT N'DỪNG: ' + ERROR_MESSAGE();
    SET NOEXEC ON;
END CATCH;
GO

-- ---------- 1. BAOHANH -> CHITIETHOADON (giữ hai FK cũ để ERD không đổi) ----------
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_BH_CTHD')
    ALTER TABLE BAOHANH ADD CONSTRAINT FK_BH_CTHD
        FOREIGN KEY (MaHD, MaDT) REFERENCES CHITIETHOADON(MaHD, MaDT);
GO

-- ---------- 2. CHITIETHOADON (MaKM, MaDT) -> CHITIETKHUYENMAI ----------
-- MaKM NULL thì SQL Server bỏ qua FK này (không áp dụng khuyến mãi).
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_CTHD_CTKM')
    ALTER TABLE CHITIETHOADON ADD CONSTRAINT FK_CTHD_CTKM
        FOREIGN KEY (MaKM, MaDT) REFERENCES CHITIETKHUYENMAI(MaKM, MaDT);
GO

-- ---------- 3. Ràng buộc tiền bằng trigger ----------
-- ThanhTien = làm tròn(DonGia x SoLuong x (100 - %) / 100), cùng công thức GiaBanHelper.ThanhTien.
CREATE OR ALTER TRIGGER TR_CTHD_KiemTraTien ON CHITIETHOADON
AFTER INSERT, UPDATE
AS
BEGIN
    SET NOCOUNT ON;
    IF EXISTS (SELECT 1 FROM inserted i
               LEFT JOIN CHITIETKHUYENMAI k ON k.MaKM = i.MaKM AND k.MaDT = i.MaDT
               WHERE i.ThanhTien <> ROUND(CAST(i.DonGia AS DECIMAL(38,6)) * i.SoLuong
                                          * (100 - ISNULL(k.PhanTramGiam, 0)) / 100, 0))
    BEGIN
        ROLLBACK TRANSACTION;
        THROW 50111, N'ThanhTien không khớp DonGia x SoLuong x (100 - % khuyến mãi) / 100.', 1;
    END;
END;
GO

-- HOADON.TongTien luôn = tổng ThanhTien các dòng: mỗi lần dòng chi tiết đổi, DB tự tính lại.
CREATE OR ALTER TRIGGER TR_CTHD_DongBoTongTien ON CHITIETHOADON
AFTER INSERT, UPDATE, DELETE
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE h SET TongTien = ISNULL((SELECT SUM(c.ThanhTien) FROM CHITIETHOADON c WHERE c.MaHD = h.MaHD), 0)
    FROM HOADON h
    WHERE h.MaHD IN (SELECT MaHD FROM inserted UNION SELECT MaHD FROM deleted);
END;
GO

-- Đã có dòng chi tiết thì không cho sửa tay TongTien lệch khỏi tổng.
CREATE OR ALTER TRIGGER TR_HOADON_KiemTraTong ON HOADON
AFTER UPDATE
AS
BEGIN
    SET NOCOUNT ON;
    IF UPDATE(TongTien) AND EXISTS (SELECT 1 FROM inserted i
        WHERE EXISTS (SELECT 1 FROM CHITIETHOADON c WHERE c.MaHD = i.MaHD)
          AND i.TongTien <> (SELECT SUM(c.ThanhTien) FROM CHITIETHOADON c WHERE c.MaHD = i.MaHD))
    BEGIN
        ROLLBACK TRANSACTION;
        THROW 50114, N'TongTien hóa đơn phải bằng tổng ThanhTien các dòng chi tiết.', 1;
    END;
END;
GO

CREATE OR ALTER TRIGGER TR_CTPN_DongBoTongTien ON CHITIETPHIEUNHAPHANG
AFTER INSERT, UPDATE, DELETE
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE p SET TongTien = ISNULL((SELECT SUM(c.ThanhTien) FROM CHITIETPHIEUNHAPHANG c WHERE c.MaPN = p.MaPN), 0)
    FROM PHIEUNHAPHANG p
    WHERE p.MaPN IN (SELECT MaPN FROM inserted UNION SELECT MaPN FROM deleted);
END;
GO

CREATE OR ALTER TRIGGER TR_PHIEUNHAP_KiemTraTong ON PHIEUNHAPHANG
AFTER UPDATE
AS
BEGIN
    SET NOCOUNT ON;
    IF UPDATE(TongTien) AND EXISTS (SELECT 1 FROM inserted i
        WHERE EXISTS (SELECT 1 FROM CHITIETPHIEUNHAPHANG c WHERE c.MaPN = i.MaPN)
          AND i.TongTien <> (SELECT SUM(c.ThanhTien) FROM CHITIETPHIEUNHAPHANG c WHERE c.MaPN = i.MaPN))
    BEGIN
        ROLLBACK TRANSACTION;
        THROW 50115, N'TongTien phiếu nhập phải bằng tổng ThanhTien các dòng chi tiết.', 1;
    END;
END;
GO

-- SoLuongTon chỉ được đổi trong luồng nhập hàng / bán hàng của ứng dụng (và script dữ liệu mẫu).
-- Luồng đó bật cờ SESSION_CONTEXT 'cho_phep_sua_ton' = 1 trên kết nối của nó (cần SQL Server 2016 trở lên).
-- Sửa tay trong SSMS không có cờ nên bị chặn. Cần sửa tay thật sự thì chạy:
--   EXEC sp_set_session_context N'cho_phep_sua_ton', 1;  (rồi UPDATE, rồi đặt lại 0)
CREATE OR ALTER TRIGGER TR_DIENTHOAI_BaoVeTonKho ON DIENTHOAI
AFTER UPDATE
AS
BEGIN
    SET NOCOUNT ON;
    IF UPDATE(SoLuongTon)
       AND EXISTS (SELECT 1 FROM inserted i JOIN deleted d ON d.MaDT = i.MaDT WHERE i.SoLuongTon <> d.SoLuongTon)
       AND ISNULL(TRY_CONVERT(int, SESSION_CONTEXT(N'cho_phep_sua_ton')), 0) <> 1
    BEGIN
        ROLLBACK TRANSACTION;
        THROW 50116, N'SoLuongTon chỉ đổi qua nhập hàng hoặc bán hàng, không sửa tay.', 1;
    END;
END;
GO

-- Đã có hóa đơn dùng khuyến mãi (MaKM, MaDT) thì không đổi phần trăm giảm, để lịch sử không bị lệch.
-- Muốn giảm khác: tạo khuyến mãi mới thay vì sửa khuyến mãi cũ.
CREATE OR ALTER TRIGGER TR_CTKM_GiuLichSu ON CHITIETKHUYENMAI
AFTER UPDATE
AS
BEGIN
    SET NOCOUNT ON;
    IF UPDATE(PhanTramGiam) AND EXISTS (
        SELECT 1 FROM inserted i
        JOIN deleted d ON d.MaKM = i.MaKM AND d.MaDT = i.MaDT
        WHERE i.PhanTramGiam <> d.PhanTramGiam
          AND EXISTS (SELECT 1 FROM CHITIETHOADON c WHERE c.MaKM = i.MaKM AND c.MaDT = i.MaDT))
    BEGIN
        ROLLBACK TRANSACTION;
        THROW 50117, N'Khuyến mãi này đã được dùng trong hóa đơn, không đổi phần trăm giảm. Hãy tạo khuyến mãi mới.', 1;
    END;
END;
GO

-- Số phiếu bảo hành của (hóa đơn, điện thoại) không vượt số máy đã mua.
-- UPDLOCK + HOLDLOCK trên dòng chi tiết hóa đơn để hai phiên cùng thêm bảo hành phải xếp hàng, không vượt qua nhau.
CREATE OR ALTER TRIGGER TR_BAOHANH_KiemTraSoLuong ON BAOHANH
AFTER INSERT, UPDATE
AS
BEGIN
    SET NOCOUNT ON;
    IF EXISTS (
        SELECT 1
        FROM (SELECT DISTINCT MaHD, MaDT FROM inserted) i
        CROSS APPLY (SELECT c.SoLuong FROM CHITIETHOADON c WITH (UPDLOCK, HOLDLOCK)
                     WHERE c.MaHD = i.MaHD AND c.MaDT = i.MaDT) x
        WHERE (SELECT COUNT(*) FROM BAOHANH b WHERE b.MaHD = i.MaHD AND b.MaDT = i.MaDT) > x.SoLuong)
    BEGIN
        ROLLBACK TRANSACTION;
        THROW 50118, N'Số phiếu bảo hành vượt quá số máy đã mua trong hóa đơn.', 1;
    END;
END;
GO

-- ---------- 4. SĐT và ThanhTien phiếu nhập ----------
IF NOT EXISTS (SELECT 1 FROM sys.check_constraints WHERE name = 'CK_NV_SDT_TV2')
    ALTER TABLE NHANVIEN ADD CONSTRAINT CK_NV_SDT_TV2
        CHECK (LEN(SDT) = 10 AND SDT LIKE '0%' AND SDT NOT LIKE '%[^0-9]%');
IF NOT EXISTS (SELECT 1 FROM sys.check_constraints WHERE name = 'CK_KH_SDT_TV2')
    ALTER TABLE KHACHHANG ADD CONSTRAINT CK_KH_SDT_TV2
        CHECK (LEN(SDT) = 10 AND SDT LIKE '0%' AND SDT NOT LIKE '%[^0-9]%');
IF NOT EXISTS (SELECT 1 FROM sys.check_constraints WHERE name = 'CK_CTPN_ThanhTien_TV2')
    ALTER TABLE CHITIETPHIEUNHAPHANG ADD CONSTRAINT CK_CTPN_ThanhTien_TV2
        CHECK (ThanhTien = SoLuong * DonGiaNhap);
GO

-- ---------- 5. Hãng và loại bắt buộc ----------
IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('DIENTHOAI') AND name = 'MaHang' AND is_nullable = 1)
    ALTER TABLE DIENTHOAI ALTER COLUMN MaHang INT NOT NULL;
IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('DIENTHOAI') AND name = 'MaLoai' AND is_nullable = 1)
    ALTER TABLE DIENTHOAI ALTER COLUMN MaLoai INT NOT NULL;
GO

-- ---------- 8. View đối soát (chỉ đọc) ----------
-- SoLuongTon là cột lưu sẵn, chỉ đổi qua nhập (+) và bán (-). Cột ChenhLech khác 0 nghĩa là
-- tồn đầu kỳ khác 0 hoặc có người sửa tay SoLuongTon; xem lại nếu cột này thay đổi bất thường.
CREATE OR ALTER VIEW vw_DoiSoatTonKho AS
SELECT d.MaDT, d.TenDT, d.SoLuongTon,
       ISNULL(n.TongNhap, 0) AS TongNhap, ISNULL(b.TongBan, 0) AS TongBan,
       d.SoLuongTon - (ISNULL(n.TongNhap, 0) - ISNULL(b.TongBan, 0)) AS ChenhLech
FROM DIENTHOAI d
LEFT JOIN (SELECT MaDT, SUM(SoLuong) AS TongNhap FROM CHITIETPHIEUNHAPHANG GROUP BY MaDT) n ON n.MaDT = d.MaDT
LEFT JOIN (SELECT MaDT, SUM(SoLuong) AS TongBan FROM CHITIETHOADON GROUP BY MaDT) b ON b.MaDT = d.MaDT;
GO

-- Hóa đơn / phiếu nhập có TongTien khác tổng chi tiết (kỳ vọng: không có dòng nào)
CREATE OR ALTER VIEW vw_DoiSoatTongTien AS
SELECT N'HOADON' AS Bang, h.MaHD AS Ma, h.TongTien, ISNULL(s.Tong, 0) AS TongChiTiet
FROM HOADON h
LEFT JOIN (SELECT MaHD, SUM(ThanhTien) AS Tong FROM CHITIETHOADON GROUP BY MaHD) s ON s.MaHD = h.MaHD
WHERE h.TongTien <> ISNULL(s.Tong, 0)
UNION ALL
SELECT N'PHIEUNHAPHANG', p.MaPN, p.TongTien, ISNULL(s.Tong, 0)
FROM PHIEUNHAPHANG p
LEFT JOIN (SELECT MaPN, SUM(ThanhTien) AS Tong FROM CHITIETPHIEUNHAPHANG GROUP BY MaPN) s ON s.MaPN = p.MaPN
WHERE p.TongTien <> ISNULL(s.Tong, 0);
GO
SET NOEXEC OFF;
GO
