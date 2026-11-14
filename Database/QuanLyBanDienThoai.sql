-- ============================================================
--  CSDL QuanLyBanDienThoai (SQL Server) - theo ERD hiện tại
--  16 bảng = 14 bảng ERD + THAMSO + TAIKHOAN (đăng nhập). Có thể chạy lại nhiều lần.
--  Không có PHIENBAN, KHO, TONKHO: RAM, BoNho, MauSac, GiaNiemYet, SoLuongTon nằm trong DIENTHOAI.
--  Khóa chính INT IDENTITY(1,1); bảng chi tiết dùng khóa chính kép.
-- ============================================================
IF DB_ID(N'QuanLyBanDienThoai') IS NULL
    CREATE DATABASE QuanLyBanDienThoai;
GO
USE QuanLyBanDienThoai;
GO

-- CHỐNG XÓA NHẦM: script này DROP toàn bộ bảng. Nếu database đã có hóa đơn, phiếu nhập hoặc tài khoản
-- thì script dừng ngay, không đụng vào dữ liệu. Muốn tạo lại từ đầu thật sự, chạy dòng sau trong CÙNG phiên rồi chạy lại script:
--     EXEC sp_set_session_context N'cho_phep_xoa_csdl', 1;
DECLARE @DaCoDuLieu int = 0, @n int;
IF OBJECT_ID(N'dbo.HOADON', N'U') IS NOT NULL      BEGIN EXEC sp_executesql N'SELECT @n = COUNT(*) FROM dbo.HOADON', N'@n int OUTPUT', @n OUTPUT;        SET @DaCoDuLieu += @n; END;
IF OBJECT_ID(N'dbo.PHIEUNHAPHANG', N'U') IS NOT NULL BEGIN EXEC sp_executesql N'SELECT @n = COUNT(*) FROM dbo.PHIEUNHAPHANG', N'@n int OUTPUT', @n OUTPUT; SET @DaCoDuLieu += @n; END;
IF OBJECT_ID(N'dbo.TAIKHOAN', N'U') IS NOT NULL    BEGIN EXEC sp_executesql N'SELECT @n = COUNT(*) FROM dbo.TAIKHOAN', N'@n int OUTPUT', @n OUTPUT;    SET @DaCoDuLieu += @n; END;
IF @DaCoDuLieu > 0 AND ISNULL(TRY_CONVERT(int, SESSION_CONTEXT(N'cho_phep_xoa_csdl')), 0) <> 1
BEGIN
    PRINT N'DỪNG: database QuanLyBanDienThoai đang có dữ liệu. Script này sẽ xóa sạch nên không chạy. Dùng Database/VaLoiCSDL.sql để vá, hoặc đọc hướng dẫn ở đầu mục CHỐNG XÓA NHẦM.';
    SET NOEXEC ON;
END;
GO

-- Xóa bảng cũ (bảng con xóa trước bảng cha) để chạy lại không bị lỗi.
-- Gồm cả TONKHO, PHIENBAN, KHO của phiên bản ERD cũ (nếu máy nào còn).
DROP TABLE IF EXISTS TAIKHOAN;
DROP TABLE IF EXISTS BAOHANH;
DROP TABLE IF EXISTS CHITIETHOADON;
DROP TABLE IF EXISTS CHITIETPHIEUNHAPHANG;
DROP TABLE IF EXISTS CHITIETKHUYENMAI;
DROP TABLE IF EXISTS GIOHANG;
DROP TABLE IF EXISTS TONKHO;
DROP TABLE IF EXISTS HOADON;
DROP TABLE IF EXISTS PHIEUNHAPHANG;
DROP TABLE IF EXISTS PHIENBAN;
DROP TABLE IF EXISTS NHANVIEN;
DROP TABLE IF EXISTS KHO;
DROP TABLE IF EXISTS KHUYENMAI;
DROP TABLE IF EXISTS DIENTHOAI;
DROP TABLE IF EXISTS KHACHHANG;
DROP TABLE IF EXISTS NHACUNGCAP;
DROP TABLE IF EXISTS HANGDT;
DROP TABLE IF EXISTS LOAIDIENTHOAI;
DROP TABLE IF EXISTS THAMSO;
GO

-- ===================== TẠO BẢNG =====================

-- Bảng tham số (ngoài ERD): hằng số nghiệp vụ, BUS đọc bằng ThamSoBUS.LayGiaTri
CREATE TABLE THAMSO (
    TenThamSo VARCHAR(50) NOT NULL PRIMARY KEY,
    GiaTri DECIMAL(18, 2) NOT NULL,
    MoTa NVARCHAR(255) NULL,
    CONSTRAINT CK_TS_GiaTri CHECK (GiaTri > 0)
);

CREATE TABLE LOAIDIENTHOAI (
    MaLoai INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    TenLoai NVARCHAR(100) NOT NULL,
    MoTa NVARCHAR(255) NULL,
    CONSTRAINT UQ_LDT_TenLoai UNIQUE (TenLoai)
);

CREATE TABLE HANGDT (
    MaHang INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    TenHang NVARCHAR(100) NOT NULL,
    QuocGia NVARCHAR(50) NULL,
    CONSTRAINT UQ_HDT_TenHang UNIQUE (TenHang)
);

CREATE TABLE NHACUNGCAP (
    MaNCC INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    TenNCC NVARCHAR(100) NOT NULL,
    DiaChi NVARCHAR(255) NULL,
    SDT VARCHAR(20) NOT NULL,
    Email VARCHAR(100) NOT NULL,
    CONSTRAINT UQ_NCC_SDT UNIQUE (SDT),
    CONSTRAINT UQ_NCC_Email UNIQUE (Email)
);

CREATE TABLE DIENTHOAI (
    MaDT INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    TenDT NVARCHAR(100) NOT NULL,
    HeDieuHanh NVARCHAR(50) NULL,
    NamRaMat INT NULL,
    RAM INT NOT NULL,                       -- GB
    BoNho INT NOT NULL,                     -- GB
    MauSac NVARCHAR(50) NOT NULL,
    GiaNiemYet DECIMAL(18, 0) NOT NULL,
    SoLuongTon INT NOT NULL CONSTRAINT DF_DT_SoLuongTon DEFAULT 0,  -- chỉ đổi qua nhập hàng (+) và bán hàng (-)
    MoTa NVARCHAR(255) NULL,
    MaHang INT NOT NULL,
    MaLoai INT NOT NULL,
    CONSTRAINT UQ_DT_BienThe UNIQUE (TenDT, RAM, BoNho, MauSac),
    CONSTRAINT FK_DT_Hang FOREIGN KEY (MaHang) REFERENCES HANGDT(MaHang),
    CONSTRAINT FK_DT_Loai FOREIGN KEY (MaLoai) REFERENCES LOAIDIENTHOAI(MaLoai),
    CONSTRAINT CK_DT_NamRaMat CHECK (NamRaMat IS NULL OR NamRaMat > 1900),
    CONSTRAINT CK_DT_RAM CHECK (RAM > 0),
    CONSTRAINT CK_DT_BoNho CHECK (BoNho > 0),
    CONSTRAINT CK_DT_GiaNiemYet CHECK (GiaNiemYet > 0),
    CONSTRAINT CK_DT_SoLuongTon CHECK (SoLuongTon >= 0)
);

CREATE TABLE NHANVIEN (
    MaNV INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    Ho NVARCHAR(50) NOT NULL,
    Ten NVARCHAR(50) NOT NULL,
    GioiTinh NVARCHAR(10) NULL,
    NgaySinh DATE NULL,
    SDT VARCHAR(20) NOT NULL,
    Email VARCHAR(100) NOT NULL,
    ChucVu NVARCHAR(50) NULL,
    NgayVaoLam DATE NULL CONSTRAINT DF_NV_NgayVaoLam DEFAULT GETDATE(),
    CONSTRAINT UQ_NV_SDT UNIQUE (SDT),
    CONSTRAINT UQ_NV_Email UNIQUE (Email),
    CONSTRAINT CK_NV_NgayVaoLam CHECK (NgayVaoLam IS NULL OR NgaySinh IS NULL OR NgayVaoLam >= NgaySinh)
);

CREATE TABLE KHACHHANG (
    MaKH INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    Ho NVARCHAR(50) NOT NULL,
    Ten NVARCHAR(50) NOT NULL,
    SDT VARCHAR(20) NOT NULL,
    Email VARCHAR(100) NOT NULL,
    DiaChi NVARCHAR(255) NOT NULL,
    NgaySinh DATE NULL,
    DiemTichLuy INT NOT NULL CONSTRAINT DF_KH_DiemTichLuy DEFAULT 0,
    CONSTRAINT UQ_KH_SDT UNIQUE (SDT),
    CONSTRAINT UQ_KH_Email UNIQUE (Email),
    CONSTRAINT CK_KH_DiemTichLuy CHECK (DiemTichLuy >= 0)
);

CREATE TABLE KHUYENMAI (
    MaKM INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    TenKM NVARCHAR(100) NOT NULL,
    NgayBatDau DATE NOT NULL,
    NgayKetThuc DATE NOT NULL,
    MoTa NVARCHAR(255) NULL,
    CONSTRAINT CK_KM_Ngay CHECK (NgayKetThuc >= NgayBatDau),
    CONSTRAINT UQ_KM_TenKM UNIQUE (TenKM)
);

-- Khuyến mãi chỉ tính theo phần trăm (không có số tiền giảm)
CREATE TABLE CHITIETKHUYENMAI (
    MaKM INT NOT NULL,
    MaDT INT NOT NULL,
    PhanTramGiam DECIMAL(5, 2) NOT NULL,
    PRIMARY KEY (MaKM, MaDT),
    CONSTRAINT FK_CTKM_KM FOREIGN KEY (MaKM) REFERENCES KHUYENMAI(MaKM),
    CONSTRAINT FK_CTKM_DT FOREIGN KEY (MaDT) REFERENCES DIENTHOAI(MaDT),
    CONSTRAINT CK_CTKM_PhanTram CHECK (PhanTramGiam >= 0 AND PhanTramGiam <= 100)
);

CREATE TABLE PHIEUNHAPHANG (
    MaPN INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    NgayNhap DATETIME NOT NULL CONSTRAINT DF_PN_NgayNhap DEFAULT GETDATE(),
    TongTien DECIMAL(18, 0) NOT NULL CONSTRAINT DF_PN_TongTien DEFAULT 0,
    MaNCC INT NOT NULL,
    MaNV INT NOT NULL,
    CONSTRAINT FK_PN_NCC FOREIGN KEY (MaNCC) REFERENCES NHACUNGCAP(MaNCC),
    CONSTRAINT FK_PN_NV FOREIGN KEY (MaNV) REFERENCES NHANVIEN(MaNV),
    CONSTRAINT CK_PN_TongTien CHECK (TongTien >= 0)
);

CREATE TABLE CHITIETPHIEUNHAPHANG (
    MaPN INT NOT NULL,
    MaDT INT NOT NULL,
    SoLuong INT NOT NULL,
    DonGiaNhap DECIMAL(18, 0) NOT NULL,
    ThanhTien DECIMAL(18, 0) NOT NULL,
    PRIMARY KEY (MaPN, MaDT),
    CONSTRAINT FK_CTPN_PN FOREIGN KEY (MaPN) REFERENCES PHIEUNHAPHANG(MaPN),
    CONSTRAINT FK_CTPN_DT FOREIGN KEY (MaDT) REFERENCES DIENTHOAI(MaDT),
    CONSTRAINT CK_CTPN_SoLuong CHECK (SoLuong > 0),
    CONSTRAINT CK_CTPN_DonGiaNhap CHECK (DonGiaNhap > 0),
    CONSTRAINT CK_CTPN_ThanhTien CHECK (ThanhTien >= 0)
);

CREATE TABLE HOADON (
    MaHD INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    NgayLap DATETIME NOT NULL CONSTRAINT DF_HD_NgayLap DEFAULT GETDATE(),
    PhuongThucTT NVARCHAR(50) NULL CONSTRAINT DF_HD_PTTT DEFAULT N'Tiền mặt',
    TongTien DECIMAL(18, 0) NOT NULL CONSTRAINT DF_HD_TongTien DEFAULT 0,
    MaKH INT NOT NULL,
    MaNV INT NOT NULL,
    CONSTRAINT FK_HD_KH FOREIGN KEY (MaKH) REFERENCES KHACHHANG(MaKH),
    CONSTRAINT FK_HD_NV FOREIGN KEY (MaNV) REFERENCES NHANVIEN(MaNV),
    CONSTRAINT CK_HD_TongTien CHECK (TongTien >= 0)
);

CREATE TABLE CHITIETHOADON (
    MaHD INT NOT NULL,
    MaDT INT NOT NULL,
    SoLuong INT NOT NULL,
    DonGia DECIMAL(18, 0) NOT NULL,
    ThanhTien DECIMAL(18, 0) NOT NULL,
    MaKM INT NULL,                          -- NULL nếu không áp dụng khuyến mãi
    PRIMARY KEY (MaHD, MaDT),
    CONSTRAINT FK_CTHD_HD FOREIGN KEY (MaHD) REFERENCES HOADON(MaHD),
    CONSTRAINT FK_CTHD_DT FOREIGN KEY (MaDT) REFERENCES DIENTHOAI(MaDT),
    CONSTRAINT FK_CTHD_KM FOREIGN KEY (MaKM) REFERENCES KHUYENMAI(MaKM),
    CONSTRAINT CK_CTHD_SoLuong CHECK (SoLuong > 0),
    CONSTRAINT CK_CTHD_DonGia CHECK (DonGia > 0),
    CONSTRAINT CK_CTHD_ThanhTien CHECK (ThanhTien >= 0)
);

CREATE TABLE GIOHANG (
    MaKH INT NOT NULL,
    MaDT INT NOT NULL,
    SoLuong INT NOT NULL CONSTRAINT DF_GH_SoLuong DEFAULT 1,
    NgayThem DATE NOT NULL CONSTRAINT DF_GH_NgayThem DEFAULT GETDATE(),
    PRIMARY KEY (MaKH, MaDT),
    CONSTRAINT FK_GH_KH FOREIGN KEY (MaKH) REFERENCES KHACHHANG(MaKH),
    CONSTRAINT FK_GH_DT FOREIGN KEY (MaDT) REFERENCES DIENTHOAI(MaDT),
    CONSTRAINT CK_GH_SoLuong CHECK (SoLuong > 0)
);

-- Hai khóa ngoại riêng: MaHD -> HOADON, MaDT -> DIENTHOAI.
-- Việc cặp (MaHD, MaDT) có trong CHITIETHOADON do BaoHanhBUS kiểm tra.
CREATE TABLE BAOHANH (
    MaBH INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    SoIMEI VARCHAR(15) NOT NULL,
    NgayBatDau DATE NOT NULL,
    NgayKetThuc DATE NOT NULL,
    TrangThai NVARCHAR(50) NULL CONSTRAINT DF_BH_TrangThai DEFAULT N'Đang bảo hành',
    GhiChu NVARCHAR(255) NULL,
    MaHD INT NOT NULL,
    MaDT INT NOT NULL,
    CONSTRAINT UQ_BH_SoIMEI UNIQUE (SoIMEI),
    CONSTRAINT FK_BH_HD FOREIGN KEY (MaHD) REFERENCES HOADON(MaHD),
    CONSTRAINT FK_BH_DT FOREIGN KEY (MaDT) REFERENCES DIENTHOAI(MaDT),
    CONSTRAINT CK_BH_IMEI CHECK (LEN(SoIMEI) = 15 AND SoIMEI NOT LIKE '%[^0-9]%'),
    CONSTRAINT CK_BH_Ngay CHECK (NgayKetThuc >= NgayBatDau)
);
GO

-- Tài khoản đăng nhập (ngoài ERD, giống THAMSO). Mỗi tài khoản gắn với ĐÚNG MỘT người có sẵn trong ERD:
--   Admin, NhanVien -> MaNV (NHANVIEN);  KhachHang -> MaKH (KHACHHANG).
-- Nhờ vậy hóa đơn, giỏ hàng, bảo hành của khách và hóa đơn do nhân viên lập luôn dùng chung MaKH, MaNV với ERD.
-- MatKhau lưu mã băm SHA-256 (chữ thường, 64 ký tự), không lưu mật khẩu thật.
CREATE TABLE TAIKHOAN (
    TenDangNhap VARCHAR(50) NOT NULL PRIMARY KEY,
    MatKhau CHAR(64) NOT NULL,
    VaiTro NVARCHAR(20) NOT NULL,
    MaNV INT NULL,
    MaKH INT NULL,
    TrangThai BIT NOT NULL CONSTRAINT DF_TK_TrangThai DEFAULT 1,   -- 1 = hoạt động, 0 = bị khóa
    CONSTRAINT FK_TK_NV FOREIGN KEY (MaNV) REFERENCES NHANVIEN(MaNV),
    CONSTRAINT FK_TK_KH FOREIGN KEY (MaKH) REFERENCES KHACHHANG(MaKH),
    CONSTRAINT CK_TK_VaiTro CHECK (VaiTro IN (N'Admin', N'NhanVien', N'KhachHang')),
    CONSTRAINT CK_TK_LienKet CHECK (
        (VaiTro = N'KhachHang' AND MaKH IS NOT NULL AND MaNV IS NULL) OR
        (VaiTro IN (N'Admin', N'NhanVien') AND MaNV IS NOT NULL AND MaKH IS NULL))
);
GO
-- Index lọc cần QUOTED_IDENTIFIER ON (sqlcmd mặc định là OFF nếu không có -I)
SET QUOTED_IDENTIFIER ON;
GO
-- Mỗi nhân viên và mỗi khách hàng chỉ có tối đa một tài khoản
CREATE UNIQUE INDEX UQ_TK_MaNV ON TAIKHOAN(MaNV) WHERE MaNV IS NOT NULL;
CREATE UNIQUE INDEX UQ_TK_MaKH ON TAIKHOAN(MaKH) WHERE MaKH IS NOT NULL;
GO

-- ===================== DỮ LIỆU MẪU =====================
-- Khóa chính IDENTITY bắt đầu từ 1 theo đúng thứ tự INSERT bên dưới
-- (script xóa bảng rồi tạo lại nên mã luôn khớp với các khóa ngoại ở dưới).

INSERT INTO THAMSO (TenThamSo, GiaTri, MoTa) VALUES
('SoTienMotDiem', 100000, N'Số tiền mua hàng để được 1 điểm tích lũy (đồng)'),
('TuoiNhanVienToiThieu', 18, N'Tuổi tối thiểu của nhân viên'),
('NamRaMatToiThieu', 2000, N'Năm ra mắt nhỏ nhất của điện thoại'),
('ThangBaoHanhMacDinh', 12, N'Số tháng bảo hành mặc định');

INSERT INTO LOAIDIENTHOAI (TenLoai, MoTa) VALUES          -- 1..3
(N'Smartphone', N'Điện thoại thông minh cao cấp và tầm trung'),
(N'Feature Phone', N'Điện thoại cơ bản, bàn phím phổ thông'),
(N'Gaming Phone', N'Điện thoại chuyên dụng chơi game cấu hình mạnh');

INSERT INTO HANGDT (TenHang, QuocGia) VALUES              -- 1..4
(N'Apple', N'Mỹ'),
(N'Samsung', N'Hàn Quốc'),
(N'Xiaomi', N'Trung Quốc'),
(N'OPPO', N'Trung Quốc');

INSERT INTO NHACUNGCAP (TenNCC, DiaChi, SDT, Email) VALUES   -- 1..3
(N'Công ty TNHH Apple Việt Nam', N'Quận 1, TP.HCM', '0901112223', 'apple.vn@email.com'),
(N'Công ty Cổ phần Samsung Vina', N'TP. Thủ Đức, TP.HCM', '0904445556', 'samsung.vina@email.com'),
(N'Công ty TNHH Công nghệ Xiaomi VN', N'Quận 3, TP.HCM', '0907778889', 'xiaomi.vn@email.com');

-- SoLuongTon ở đây là tồn sau khi đã tính phiếu nhập PN1 (+) và hóa đơn HD1 (-) bên dưới
INSERT INTO DIENTHOAI (TenDT, HeDieuHanh, NamRaMat, RAM, BoNho, MauSac, GiaNiemYet, SoLuongTon, MoTa, MaHang, MaLoai) VALUES   -- 1..8
(N'iPhone 15 Pro Max', N'iOS', 2023, 8, 256, N'Titan Tự Nhiên', 34990000, 24, N'Flagship Apple chip A17 Pro', 1, 1),
(N'iPhone 15 Pro Max', N'iOS', 2023, 8, 512, N'Titan Xanh', 40990000, 10, N'Flagship Apple bản 512GB', 1, 1),
(N'Samsung Galaxy S24 Ultra', N'Android', 2024, 12, 256, N'Đen Titan', 29990000, 22, N'Flagship Samsung hỗ trợ AI', 2, 1),
(N'Samsung Galaxy S24 Ultra', N'Android', 2024, 12, 512, N'Xám Titan', 33990000, 15, N'Flagship Samsung bản 512GB', 2, 1),
(N'Xiaomi 14 Pro', N'Android', 2023, 12, 256, N'Đen', 21990000, 18, N'Cấu hình mạnh giá tốt', 3, 1),
(N'Xiaomi Redmi Note 13', N'Android', 2024, 8, 128, N'Xanh Dương', 6490000, 40, N'Tầm trung pin lớn', 3, 1),
(N'OPPO Find X7', N'Android', 2024, 12, 256, N'Nâu', 24990000, 12, N'Camera chụp ảnh sắc nét', 4, 1),
(N'OPPO Reno11', N'Android', 2024, 8, 256, N'Xám', 10990000, 25, N'Thiết kế mỏng nhẹ', 4, 1);

INSERT INTO NHANVIEN (Ho, Ten, GioiTinh, NgaySinh, SDT, Email, ChucVu, NgayVaoLam) VALUES   -- 1..3
(N'Lê Văn', N'Cường', N'Nam', '1996-03-15', '0933111222', 'cuong.lv@email.com', N'Quản lý', '2022-05-10'),
(N'Phạm Thị', N'Dung', N'Nữ', '2000-08-22', '0944333444', 'dung.pt@email.com', N'Nhân viên bán hàng', '2023-01-15'),
(N'Hoàng Văn', N'Đức', N'Nam', '1998-11-10', '0955666777', 'duc.hv@email.com', N'Nhân viên kho', '2023-06-20');

-- HD1 = 75.471.000 đồng -> 754 điểm (100.000 đồng = 1 điểm) cho KH1
INSERT INTO KHACHHANG (Ho, Ten, SDT, Email, DiaChi, NgaySinh, DiemTichLuy) VALUES   -- 1..3
(N'Nguyễn Văn', N'An', '0912345678', 'an.nv@email.com', N'Quận 3, TP.HCM', '1995-05-12', 754),
(N'Trần Thị', N'Bình', '0987654321', 'binh.tt@email.com', N'Quận 10, TP.HCM', '1998-10-20', 0),
(N'Lê Hoàng', N'Nam', '0935556677', 'nam.lh@email.com', N'Quận Tân Bình, TP.HCM', '2001-12-05', 0);

-- KM1 còn hiệu lực (dùng thử bán hàng); KM2 đã hết hạn (dùng thử báo "không áp dụng được")
INSERT INTO KHUYENMAI (TenKM, NgayBatDau, NgayKetThuc, MoTa) VALUES   -- 1..2
(N'Khuyến mãi quý 4', '2026-10-01', '2027-06-30', N'Giảm giá cuối năm'),
(N'Back to School', '2026-08-15', '2026-09-05', N'Giảm cho học sinh sinh viên');

INSERT INTO CHITIETKHUYENMAI (MaKM, MaDT, PhanTramGiam) VALUES
(1, 1, 10.00),
(1, 3, 5.00),
(2, 6, 15.00),
(2, 8, 10.00);

-- Phiếu nhập PN1: 10 x iPhone 256GB (giá nhập 30.000.000) + 10 x S24 Ultra 256GB (25.000.000)
INSERT INTO PHIEUNHAPHANG (NgayNhap, TongTien, MaNCC, MaNV) VALUES
('2026-10-05 09:30:00', 550000000, 1, 1);

INSERT INTO CHITIETPHIEUNHAPHANG (MaPN, MaDT, SoLuong, DonGiaNhap, ThanhTien) VALUES
(1, 1, 10, 30000000, 300000000),
(1, 3, 10, 25000000, 250000000);

-- Hóa đơn HD1: 1 x iPhone (KM1 giảm 10%: 34.990.000 x 0,9 = 31.491.000) + 2 x Xiaomi 14 Pro (không KM: 43.980.000)
INSERT INTO HOADON (NgayLap, PhuongThucTT, TongTien, MaKH, MaNV) VALUES
('2026-10-12 14:20:00', N'Chuyển khoản', 75471000, 1, 2);

INSERT INTO CHITIETHOADON (MaHD, MaDT, SoLuong, DonGia, ThanhTien, MaKM) VALUES
(1, 1, 1, 34990000, 31491000, 1),
(1, 5, 2, 21990000, 43980000, NULL);

-- Bảo hành 12 tháng (ThangBaoHanhMacDinh) cho iPhone của HD1
INSERT INTO BAOHANH (SoIMEI, NgayBatDau, NgayKetThuc, TrangThai, GhiChu, MaHD, MaDT) VALUES
('359876543210123', '2026-10-12', '2027-10-12', N'Đang bảo hành', N'Bảo hành chính hãng 12 tháng', 1, 1);

INSERT INTO GIOHANG (MaKH, MaDT, SoLuong, NgayThem) VALUES
(2, 4, 1, '2026-10-20'),
(3, 8, 2, '2026-10-21');
GO

-- Tài khoản mẫu (mật khẩu: admin = admin123; nhân viên = nv123456; khách hàng = kh123456)
--   admin -> NV1 Lê Văn Cường (Quản lý);  dung -> NV2 (bán hàng);  duc -> NV3 (kho)
--   an -> KH1 (có HD1 và bảo hành);  binh -> KH2, nam -> KH3 (có giỏ hàng)
INSERT INTO TAIKHOAN (TenDangNhap, MatKhau, VaiTro, MaNV, MaKH, TrangThai) VALUES
('admin', '240be518fabd2724ddb6f04eeb1da5967448d7e831c08c8fa822809f74c720a9', N'Admin', 1, NULL, 1),
('dung', 'a5d21a2fa99d15d6c13d848008559574fd222b2a51415c72abf3bae221c41f71', N'NhanVien', 2, NULL, 1),
('duc', 'a5d21a2fa99d15d6c13d848008559574fd222b2a51415c72abf3bae221c41f71', N'NhanVien', 3, NULL, 1),
('an', 'b9edb8982854b5e79a890577927de0c29af629a14dd8c1d326bab0056ad0a313', N'KhachHang', NULL, 1, 1),
('binh', 'b9edb8982854b5e79a890577927de0c29af629a14dd8c1d326bab0056ad0a313', N'KhachHang', NULL, 2, 1),
('nam', 'b9edb8982854b5e79a890577927de0c29af629a14dd8c1d326bab0056ad0a313', N'KhachHang', NULL, 3, 1);
GO

-- ===================== VÁ RÀNG BUỘC (cùng nội dung Database/VaLoiCSDL.sql) =====================
SET QUOTED_IDENTIFIER ON;
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
