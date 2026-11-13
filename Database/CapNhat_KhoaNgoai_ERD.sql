USE [QuanLyBanDienThoai];
GO
SET XACT_ABORT ON;

BEGIN TRY
    BEGIN TRANSACTION;

    IF EXISTS (
        SELECT 1 FROM dbo.BAOHANH b
        WHERE NOT EXISTS (
            SELECT 1 FROM dbo.CHITIETHOADON c
            WHERE c.MaHD = b.MaHD AND c.MaDT = b.MaDT
        )
    )
        THROW 50001, N'Có bảo hành trỏ đến điện thoại không nằm trong hóa đơn. Hãy kiểm tra MaHD và MaDT trong BAOHANH.', 1;

    IF EXISTS (
        SELECT 1 FROM dbo.CHITIETHOADON c
        WHERE c.MaKM IS NOT NULL AND NOT EXISTS (
            SELECT 1 FROM dbo.CHITIETKHUYENMAI k
            WHERE k.MaKM = c.MaKM AND k.MaDT = c.MaDT
        )
    )
        THROW 50002, N'Có dòng hóa đơn áp khuyến mãi sai điện thoại. Hãy kiểm tra MaKM và MaDT trong CHITIETHOADON.', 1;

    IF OBJECT_ID(N'dbo.FK_BH_CTHD', N'F') IS NULL
        ALTER TABLE dbo.BAOHANH WITH CHECK ADD CONSTRAINT FK_BH_CTHD
            FOREIGN KEY (MaHD, MaDT) REFERENCES dbo.CHITIETHOADON(MaHD, MaDT);

    IF OBJECT_ID(N'dbo.FK_CTHD_CTKM', N'F') IS NULL
        ALTER TABLE dbo.CHITIETHOADON WITH CHECK ADD CONSTRAINT FK_CTHD_CTKM
            FOREIGN KEY (MaKM, MaDT) REFERENCES dbo.CHITIETKHUYENMAI(MaKM, MaDT);

    ALTER TABLE dbo.BAOHANH WITH CHECK CHECK CONSTRAINT FK_BH_CTHD;
    ALTER TABLE dbo.CHITIETHOADON WITH CHECK CHECK CONSTRAINT FK_CTHD_CTKM;

    IF OBJECT_ID(N'dbo.FK_BH_HD', N'F') IS NOT NULL
        ALTER TABLE dbo.BAOHANH DROP CONSTRAINT FK_BH_HD;
    IF OBJECT_ID(N'dbo.FK_BH_DT', N'F') IS NOT NULL
        ALTER TABLE dbo.BAOHANH DROP CONSTRAINT FK_BH_DT;
    IF OBJECT_ID(N'dbo.FK_CTHD_KM', N'F') IS NOT NULL
        ALTER TABLE dbo.CHITIETHOADON DROP CONSTRAINT FK_CTHD_KM;


    -- Khóa dữ liệu trong lúc đối chiếu và cài trigger.
    DECLARE @Khoa bigint;
    SELECT @Khoa = COUNT_BIG(*) FROM dbo.HOADON WITH (TABLOCKX, HOLDLOCK);
    SELECT @Khoa = COUNT_BIG(*) FROM dbo.PHIEUNHAPHANG WITH (TABLOCKX, HOLDLOCK);
    SELECT @Khoa = COUNT_BIG(*) FROM dbo.CHITIETHOADON WITH (TABLOCKX, HOLDLOCK);
    SELECT @Khoa = COUNT_BIG(*) FROM dbo.CHITIETPHIEUNHAPHANG WITH (TABLOCKX, HOLDLOCK);
    SELECT @Khoa = COUNT_BIG(*) FROM dbo.BAOHANH WITH (TABLOCKX, HOLDLOCK);
    SELECT @Khoa = COUNT_BIG(*) FROM dbo.KHUYENMAI WITH (TABLOCKX, HOLDLOCK);
    SELECT @Khoa = COUNT_BIG(*) FROM dbo.CHITIETKHUYENMAI WITH (TABLOCKX, HOLDLOCK);

    IF EXISTS (
        SELECT 1 FROM dbo.BAOHANH b JOIN dbo.HOADON h ON h.MaHD = b.MaHD
        WHERE b.NgayBatDau < CONVERT(date, h.NgayLap)
    ) OR EXISTS (
        SELECT 1 FROM dbo.CHITIETHOADON c
        WHERE c.SoLuong < (SELECT COUNT_BIG(*) FROM dbo.BAOHANH b WHERE b.MaHD = c.MaHD AND b.MaDT = c.MaDT)
    )
        THROW 50110, N'Dữ liệu bảo hành cũ có ngày hoặc số lượng không hợp lệ.', 1;

    IF EXISTS (
        SELECT 1 FROM dbo.CHITIETHOADON c
        JOIN dbo.HOADON h ON h.MaHD = c.MaHD
        LEFT JOIN dbo.KHUYENMAI km ON km.MaKM = c.MaKM
        LEFT JOIN dbo.CHITIETKHUYENMAI k ON k.MaKM = c.MaKM AND k.MaDT = c.MaDT
        WHERE CONVERT(date, h.NgayLap) NOT BETWEEN km.NgayBatDau AND km.NgayKetThuc
           OR c.ThanhTien <> ROUND(c.DonGia * c.SoLuong * (100 - COALESCE(k.PhanTramGiam, 0)) / 100, 0)
    )
        THROW 50111, N'Dữ liệu hóa đơn cũ có thành tiền hoặc thời hạn khuyến mãi không hợp lệ.', 1;

    IF EXISTS (SELECT 1 FROM dbo.CHITIETPHIEUNHAPHANG WHERE ThanhTien <> SoLuong * DonGiaNhap)
        THROW 50112, N'Dữ liệu chi tiết phiếu nhập cũ có thành tiền không hợp lệ.', 1;

    IF OBJECT_ID(N'dbo.CK_CTPN_ThanhTien_TV2', N'C') IS NULL
        ALTER TABLE dbo.CHITIETPHIEUNHAPHANG WITH CHECK ADD CONSTRAINT CK_CTPN_ThanhTien_TV2
            CHECK (ThanhTien = SoLuong * DonGiaNhap);
    ALTER TABLE dbo.CHITIETPHIEUNHAPHANG WITH CHECK CHECK CONSTRAINT CK_CTPN_ThanhTien_TV2;

    UPDATE h SET TongTien = t.TongTien
    FROM dbo.HOADON h
    CROSS APPLY (SELECT COALESCE(SUM(c.ThanhTien), 0) AS TongTien
                 FROM dbo.CHITIETHOADON c WHERE c.MaHD = h.MaHD) t
    WHERE h.TongTien <> t.TongTien;

    UPDATE h SET TongTien = t.TongTien
    FROM dbo.PHIEUNHAPHANG h
    CROSS APPLY (SELECT COALESCE(SUM(c.ThanhTien), 0) AS TongTien
                 FROM dbo.CHITIETPHIEUNHAPHANG c WHERE c.MaPN = h.MaPN) t
    WHERE h.TongTien <> t.TongTien;

    EXEC sys.sp_executesql N'CREATE OR ALTER TRIGGER dbo.TR_CHITIETHOADON_TongTien
ON dbo.CHITIETHOADON
AFTER INSERT, UPDATE, DELETE
AS
BEGIN
    SET NOCOUNT ON;
    IF EXISTS (
        SELECT 1 FROM inserted c
        JOIN dbo.HOADON h WITH (UPDLOCK, HOLDLOCK) ON h.MaHD = c.MaHD
        JOIN dbo.KHUYENMAI k WITH (HOLDLOCK) ON k.MaKM = c.MaKM
        WHERE CONVERT(date, h.NgayLap) NOT BETWEEN k.NgayBatDau AND k.NgayKetThuc
    )
        THROW 50102, N''Ngày lập hóa đơn nằm ngoài thời hạn khuyến mãi.'', 1;

    IF EXISTS (
        SELECT 1 FROM inserted c
        LEFT JOIN dbo.CHITIETKHUYENMAI k WITH (HOLDLOCK) ON k.MaKM = c.MaKM AND k.MaDT = c.MaDT
        WHERE c.ThanhTien <> ROUND(c.DonGia * c.SoLuong * (100 - COALESCE(k.PhanTramGiam, 0)) / 100, 0)
    )
        THROW 50104, N''Thành tiền không khớp đơn giá, số lượng và mức giảm.'', 1;

    IF EXISTS (
        SELECT 1 FROM inserted c WHERE c.SoLuong <
            (SELECT COUNT_BIG(*) FROM dbo.BAOHANH b WITH (UPDLOCK, HOLDLOCK)
             WHERE b.MaHD = c.MaHD AND b.MaDT = c.MaDT)
    )
        THROW 50105, N''Số máy đã mua không thể ít hơn số bảo hành đã đăng ký.'', 1;

    UPDATE h SET TongTien = t.TongTien
    FROM dbo.HOADON h
    JOIN (SELECT MaHD FROM inserted UNION SELECT MaHD FROM deleted) a ON a.MaHD = h.MaHD
    CROSS APPLY (
        SELECT COALESCE(SUM(c.ThanhTien), 0) AS TongTien
        FROM dbo.CHITIETHOADON c WITH (UPDLOCK, HOLDLOCK) WHERE c.MaHD = h.MaHD
    ) t;
END;';

    EXEC sys.sp_executesql N'CREATE OR ALTER TRIGGER dbo.TR_CHITIETPHIEUNHAPHANG_TongTien
ON dbo.CHITIETPHIEUNHAPHANG
AFTER INSERT, UPDATE, DELETE
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE h SET TongTien = t.TongTien
    FROM dbo.PHIEUNHAPHANG h
    JOIN (SELECT MaPN FROM inserted UNION SELECT MaPN FROM deleted) a ON a.MaPN = h.MaPN
    CROSS APPLY (
        SELECT COALESCE(SUM(c.ThanhTien), 0) AS TongTien
        FROM dbo.CHITIETPHIEUNHAPHANG c WITH (UPDLOCK, HOLDLOCK) WHERE c.MaPN = h.MaPN
    ) t;
END;';

    EXEC sys.sp_executesql N'CREATE OR ALTER TRIGGER dbo.TR_PHIEUNHAPHANG_KiemTra
ON dbo.PHIEUNHAPHANG
AFTER INSERT, UPDATE
AS
BEGIN
    SET NOCOUNT ON;
    IF EXISTS (
        SELECT 1 FROM inserted h
        WHERE h.TongTien <> (SELECT COALESCE(SUM(c.ThanhTien), 0)
                              FROM dbo.CHITIETPHIEUNHAPHANG c WITH (UPDLOCK, HOLDLOCK) WHERE c.MaPN = h.MaPN)
    )
        THROW 50101, N''Tổng tiền phải bằng tổng thành tiền các dòng chi tiết.'', 1;
END;';

    EXEC sys.sp_executesql N'CREATE OR ALTER TRIGGER dbo.TR_HOADON_KiemTra
ON dbo.HOADON
AFTER INSERT, UPDATE
AS
BEGIN
    SET NOCOUNT ON;
    IF EXISTS (
        SELECT 1 FROM inserted h
        WHERE h.TongTien <> (SELECT COALESCE(SUM(c.ThanhTien), 0)
                              FROM dbo.CHITIETHOADON c WITH (UPDLOCK, HOLDLOCK) WHERE c.MaHD = h.MaHD)
    )
        THROW 50101, N''Tổng tiền phải bằng tổng thành tiền các dòng chi tiết.'', 1;
    IF UPDATE(NgayLap)
    BEGIN
        IF EXISTS (
            SELECT 1 FROM inserted h
            JOIN dbo.CHITIETHOADON c ON c.MaHD = h.MaHD
            JOIN dbo.KHUYENMAI k WITH (HOLDLOCK) ON k.MaKM = c.MaKM
            WHERE CONVERT(date, h.NgayLap) NOT BETWEEN k.NgayBatDau AND k.NgayKetThuc
        )
            THROW 50102, N''Ngày lập hóa đơn nằm ngoài thời hạn khuyến mãi.'', 1;

        IF EXISTS (
            SELECT 1 FROM inserted h JOIN dbo.BAOHANH b ON b.MaHD = h.MaHD
            WHERE b.NgayBatDau < CONVERT(date, h.NgayLap)
        )
            THROW 50103, N''Ngày bảo hành không thể trước ngày lập hóa đơn.'', 1;
    END;
END;';

    EXEC sys.sp_executesql N'CREATE OR ALTER TRIGGER dbo.TR_BAOHANH_KiemTra
ON dbo.BAOHANH
AFTER INSERT, UPDATE
AS
BEGIN
    SET NOCOUNT ON;
    IF EXISTS (
        SELECT 1 FROM inserted b JOIN dbo.HOADON h WITH (HOLDLOCK) ON h.MaHD = b.MaHD
        WHERE b.NgayBatDau < CONVERT(date, h.NgayLap)
    )
        THROW 50103, N''Ngày bảo hành không thể trước ngày lập hóa đơn.'', 1;

    IF EXISTS (
        SELECT 1 FROM (SELECT DISTINCT MaHD, MaDT FROM inserted) a
        JOIN dbo.CHITIETHOADON c WITH (UPDLOCK, HOLDLOCK) ON c.MaHD = a.MaHD AND c.MaDT = a.MaDT
        WHERE c.SoLuong <
            (SELECT COUNT_BIG(*) FROM dbo.BAOHANH b WITH (UPDLOCK, HOLDLOCK)
             WHERE b.MaHD = a.MaHD AND b.MaDT = a.MaDT)
    )
        THROW 50105, N''Số bảo hành vượt số máy đã mua.'', 1;
END;';

    EXEC sys.sp_executesql N'CREATE OR ALTER TRIGGER dbo.TR_KHUYENMAI_KiemTraNgay
ON dbo.KHUYENMAI
AFTER UPDATE
AS
BEGIN
    SET NOCOUNT ON;
    IF (UPDATE(NgayBatDau) OR UPDATE(NgayKetThuc)) AND EXISTS (
        SELECT 1 FROM inserted k JOIN dbo.CHITIETHOADON c ON c.MaKM = k.MaKM
        JOIN dbo.HOADON h ON h.MaHD = c.MaHD
        WHERE CONVERT(date, h.NgayLap) NOT BETWEEN k.NgayBatDau AND k.NgayKetThuc
    )
        THROW 50102, N''Thời hạn mới loại bỏ hóa đơn đã áp dụng khuyến mãi.'', 1;
END;';

    EXEC sys.sp_executesql N'CREATE OR ALTER TRIGGER dbo.TR_CHITIETKHUYENMAI_KiemTraMucGiam
ON dbo.CHITIETKHUYENMAI
AFTER UPDATE
AS
BEGIN
    SET NOCOUNT ON;
    IF UPDATE(PhanTramGiam) AND EXISTS (
        SELECT 1 FROM inserted k JOIN deleted d ON d.MaKM = k.MaKM AND d.MaDT = k.MaDT
        JOIN dbo.CHITIETHOADON c ON c.MaKM = k.MaKM AND c.MaDT = k.MaDT
        WHERE k.PhanTramGiam <> d.PhanTramGiam
    )
        THROW 50106, N''Chi tiết đã dùng trong hóa đơn. Hãy tạo chương trình mới để đổi mức giảm.'', 1;
END;';

    COMMIT TRANSACTION;
    PRINT N'Đã cập nhật khóa ngoại và ràng buộc nghiệp vụ.';
END TRY
BEGIN CATCH
    IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
    THROW;
END CATCH;
