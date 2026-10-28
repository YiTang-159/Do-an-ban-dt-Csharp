-- ============================================================
--  CSDL QuanLyBanDienThoai (SQL Server) - theo ERD hiện tại
--  15 bảng = 14 bảng ERD + THAMSO. Có thể chạy lại nhiều lần.
--  Không có PHIENBAN, KHO, TONKHO: RAM, BoNho, MauSac, GiaNiemYet, SoLuongTon nằm trong DIENTHOAI.
--  Khóa chính INT IDENTITY(1,1); bảng chi tiết dùng khóa chính kép.
-- ============================================================
IF DB_ID(N'QuanLyBanDienThoai') IS NULL
    CREATE DATABASE QuanLyBanDienThoai;
GO
USE QuanLyBanDienThoai;
GO

-- Xóa bảng cũ (bảng con xóa trước bảng cha) để chạy lại không bị lỗi.
-- Gồm cả TONKHO, PHIENBAN, KHO của phiên bản ERD cũ (nếu máy nào còn).
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
    MoTa NVARCHAR(255) NULL
);

CREATE TABLE HANGDT (
    MaHang INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    TenHang NVARCHAR(100) NOT NULL,
    QuocGia NVARCHAR(50) NULL
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
    MaHang INT NULL,
    MaLoai INT NULL,
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
    CONSTRAINT CK_KM_Ngay CHECK (NgayKetThuc >= NgayBatDau)
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
(N'Khuyến mãi quý 4', '2026-10-01', '2026-12-31', N'Giảm giá cuối năm'),
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
