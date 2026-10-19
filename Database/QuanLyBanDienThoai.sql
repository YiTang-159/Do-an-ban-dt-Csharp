-- ============================================================
--  CSDL QuanLyBanDienThoai (SQL Server)
--  Bản sửa lỗi từ file của TV2. Có thể chạy lại nhiều lần.
-- ============================================================
IF DB_ID(N'QuanLyBanDienThoai') IS NULL
    CREATE DATABASE QuanLyBanDienThoai;
GO
USE QuanLyBanDienThoai;
GO

-- Xóa bảng cũ (bảng con xóa trước bảng cha) để chạy lại không bị lỗi
DROP TABLE IF EXISTS BAOHANH;
DROP TABLE IF EXISTS CHITIETHOADON;
DROP TABLE IF EXISTS HOADON;
DROP TABLE IF EXISTS CHITIETKHUYENMAI;
DROP TABLE IF EXISTS GIOHANG;
DROP TABLE IF EXISTS TONKHO;
DROP TABLE IF EXISTS NHANVIEN;
DROP TABLE IF EXISTS PHIENBAN;
DROP TABLE IF EXISTS DIENTHOAI;
DROP TABLE IF EXISTS KHACHHANG;
DROP TABLE IF EXISTS KHUYENMAI;
DROP TABLE IF EXISTS KHO;
DROP TABLE IF EXISTS NHACUNGCAP;
DROP TABLE IF EXISTS HANGDT;
DROP TABLE IF EXISTS LOAIDIENTHOAI;
GO

-- ===================== TẠO BẢNG =====================
CREATE TABLE LOAIDIENTHOAI (
    MaLoai VARCHAR(20) NOT NULL PRIMARY KEY,
    TenLoai NVARCHAR(100) NOT NULL,
    MoTa NVARCHAR(255) NULL
);

CREATE TABLE HANGDT (
    MaHang VARCHAR(20) NOT NULL PRIMARY KEY,
    TenHang NVARCHAR(100) NOT NULL,
    QuocGia NVARCHAR(50) NULL
);

CREATE TABLE NHACUNGCAP (
    MaNCC VARCHAR(20) NOT NULL PRIMARY KEY,
    TenNCC NVARCHAR(100) NOT NULL,
    DiaChi NVARCHAR(255) NULL,
    SDT VARCHAR(20) NOT NULL,
    Email VARCHAR(100) NOT NULL
);

CREATE TABLE KHO (
    MaKho VARCHAR(20) NOT NULL PRIMARY KEY,
    TenKho NVARCHAR(100) NOT NULL,
    DiaChi NVARCHAR(255) NULL,
    SDT VARCHAR(20) NOT NULL
);

CREATE TABLE KHUYENMAI (
    MaKM VARCHAR(20) NOT NULL PRIMARY KEY,
    TenKM NVARCHAR(100) NOT NULL,
    LoaiKM NVARCHAR(50) NULL,
    NgayBatDau DATE NOT NULL,
    NgayKetThuc DATE NOT NULL,
    MoTa NVARCHAR(255) NULL,
    CONSTRAINT CK_KM_Ngay CHECK (NgayKetThuc >= NgayBatDau)
);

CREATE TABLE KHACHHANG (
    MaKH VARCHAR(20) NOT NULL PRIMARY KEY,
    Ho NVARCHAR(50) NOT NULL,
    Ten NVARCHAR(50) NOT NULL,
    SDT VARCHAR(20) NOT NULL,
    Email VARCHAR(100) NOT NULL,
    DiaChi NVARCHAR(255) NOT NULL,
    NgaySinh DATE NULL,
    DiemTichLuy INT NULL CONSTRAINT DF_KH_DiemTichLuy DEFAULT 0,
    CONSTRAINT CK_KH_DiemTichLuy CHECK (DiemTichLuy >= 0)
);

CREATE TABLE DIENTHOAI (
    MaDT VARCHAR(20) NOT NULL PRIMARY KEY,
    TenDT NVARCHAR(100) NOT NULL,
    HeDieuHanh NVARCHAR(50) NULL,
    NamRaMat INT NULL,
    MoTa NVARCHAR(255) NULL,
    MaHang VARCHAR(20) NULL,
    MaLoai VARCHAR(20) NULL,
    MaNCC VARCHAR(20) NULL,
    CONSTRAINT FK_DT_Hang FOREIGN KEY (MaHang) REFERENCES HANGDT(MaHang),
    CONSTRAINT FK_DT_Loai FOREIGN KEY (MaLoai) REFERENCES LOAIDIENTHOAI(MaLoai),
    CONSTRAINT FK_DT_NCC FOREIGN KEY (MaNCC) REFERENCES NHACUNGCAP(MaNCC),
    CONSTRAINT CK_DT_NamRaMat CHECK (NamRaMat > 1900)
);

CREATE TABLE PHIENBAN (
    MaPB VARCHAR(20) NOT NULL PRIMARY KEY,
    TenPB NVARCHAR(100) NOT NULL,
    RAM VARCHAR(20) NULL,
    BoNho VARCHAR(20) NULL,
    MauSac NVARCHAR(50) NULL,
    GiaNiemYet DECIMAL(18, 2) NULL,
    TrangThai NVARCHAR(50) NULL CONSTRAINT DF_PB_TrangThai DEFAULT N'Đang kinh doanh',
    MaDT VARCHAR(20) NULL,
    CONSTRAINT FK_PB_DT FOREIGN KEY (MaDT) REFERENCES DIENTHOAI(MaDT),
    CONSTRAINT CK_PB_GiaNiemYet CHECK (GiaNiemYet > 0)
);

CREATE TABLE TONKHO (
    MaKho VARCHAR(20) NOT NULL,
    MaPB VARCHAR(20) NOT NULL,
    SoLuongTon INT NOT NULL CONSTRAINT DF_TK_SoLuongTon DEFAULT 0,
    NgayCapNhat DATE NULL CONSTRAINT DF_TK_NgayCapNhat DEFAULT GETDATE(),
    PRIMARY KEY (MaKho, MaPB),
    CONSTRAINT FK_TK_Kho FOREIGN KEY (MaKho) REFERENCES KHO(MaKho),
    CONSTRAINT FK_TK_PB FOREIGN KEY (MaPB) REFERENCES PHIENBAN(MaPB),
    CONSTRAINT CK_TK_SoLuongTon CHECK (SoLuongTon >= 0)
);

CREATE TABLE NHANVIEN (
    MaNV VARCHAR(20) NOT NULL PRIMARY KEY,
    Ho NVARCHAR(50) NOT NULL,
    Ten NVARCHAR(50) NOT NULL,
    GioiTinh NVARCHAR(10) NULL,
    NgaySinh DATE NULL,
    SDT VARCHAR(20) NOT NULL,
    Email VARCHAR(100) NOT NULL,
    ChucVu NVARCHAR(50) NULL,
    NgayVaoLam DATE NULL CONSTRAINT DF_NV_NgayVaoLam DEFAULT GETDATE(),
    MaKho VARCHAR(20) NULL,
    CONSTRAINT FK_NV_Kho FOREIGN KEY (MaKho) REFERENCES KHO(MaKho)
);

CREATE TABLE HOADON (
    MaHD VARCHAR(20) NOT NULL PRIMARY KEY,
    NgayLap DATE NULL CONSTRAINT DF_HD_NgayLap DEFAULT GETDATE(),
    PhuongThucTT NVARCHAR(50) NULL CONSTRAINT DF_HD_PTTT DEFAULT N'Tiền mặt',
    TongTien DECIMAL(18, 2) NOT NULL CONSTRAINT DF_HD_TongTien DEFAULT 0,
    MaKH VARCHAR(20) NULL,
    MaNV VARCHAR(20) NULL,
    MaKho VARCHAR(20) NULL,
    CONSTRAINT FK_HD_KH FOREIGN KEY (MaKH) REFERENCES KHACHHANG(MaKH),
    CONSTRAINT FK_HD_NV FOREIGN KEY (MaNV) REFERENCES NHANVIEN(MaNV),
    CONSTRAINT FK_HD_Kho FOREIGN KEY (MaKho) REFERENCES KHO(MaKho),
    CONSTRAINT CK_HD_TongTien CHECK (TongTien >= 0)
);

CREATE TABLE CHITIETKHUYENMAI (
    MaKM VARCHAR(20) NOT NULL,
    MaPB VARCHAR(20) NOT NULL,
    PhanTramGiam DECIMAL(5, 2) NULL CONSTRAINT DF_CTKM_PhanTram DEFAULT 0,
    SoTienGiam DECIMAL(18, 2) NULL CONSTRAINT DF_CTKM_SoTien DEFAULT 0,
    PRIMARY KEY (MaKM, MaPB),
    CONSTRAINT FK_CTKM_KM FOREIGN KEY (MaKM) REFERENCES KHUYENMAI(MaKM),
    CONSTRAINT FK_CTKM_PB FOREIGN KEY (MaPB) REFERENCES PHIENBAN(MaPB),
    CONSTRAINT CK_CTKM_PhanTram CHECK (PhanTramGiam >= 0 AND PhanTramGiam <= 100),
    CONSTRAINT CK_CTKM_SoTien CHECK (SoTienGiam >= 0)
);

CREATE TABLE GIOHANG (
    MaKH VARCHAR(20) NOT NULL,
    MaPB VARCHAR(20) NOT NULL,
    SoLuong INT NOT NULL CONSTRAINT DF_GH_SoLuong DEFAULT 1,
    NgayThem DATE NULL CONSTRAINT DF_GH_NgayThem DEFAULT GETDATE(),
    PRIMARY KEY (MaKH, MaPB),
    CONSTRAINT FK_GH_KH FOREIGN KEY (MaKH) REFERENCES KHACHHANG(MaKH),
    CONSTRAINT FK_GH_PB FOREIGN KEY (MaPB) REFERENCES PHIENBAN(MaPB),
    CONSTRAINT CK_GH_SoLuong CHECK (SoLuong > 0)
);

CREATE TABLE CHITIETHOADON (
    MaHD VARCHAR(20) NOT NULL,
    MaPB VARCHAR(20) NOT NULL,
    SoLuong INT NOT NULL,
    DonGia DECIMAL(18, 2) NOT NULL,
    ThanhTien DECIMAL(18, 2) NOT NULL,
    MaKM VARCHAR(20) NULL,
    PRIMARY KEY (MaHD, MaPB),
    CONSTRAINT FK_CTHD_HD FOREIGN KEY (MaHD) REFERENCES HOADON(MaHD),
    CONSTRAINT FK_CTHD_PB FOREIGN KEY (MaPB) REFERENCES PHIENBAN(MaPB),
    CONSTRAINT FK_CTHD_KM FOREIGN KEY (MaKM) REFERENCES KHUYENMAI(MaKM),
    CONSTRAINT CK_CTHD_SoLuong CHECK (SoLuong > 0),
    CONSTRAINT CK_CTHD_DonGia CHECK (DonGia > 0),
    CONSTRAINT CK_CTHD_ThanhTien CHECK (ThanhTien >= 0)
);

CREATE TABLE BAOHANH (
    MaBH VARCHAR(20) NOT NULL PRIMARY KEY,
    SoIMEI VARCHAR(50) NOT NULL,
    NgayBatDau DATE NOT NULL,
    NgayKetThuc DATE NOT NULL,
    TrangThai NVARCHAR(50) NULL CONSTRAINT DF_BH_TrangThai DEFAULT N'Đang bảo hành',
    GhiChu NVARCHAR(255) NULL,
    MaHD VARCHAR(20) NULL,
    MaPB VARCHAR(20) NULL,
    CONSTRAINT FK_BH_HD FOREIGN KEY (MaHD) REFERENCES HOADON(MaHD),
    CONSTRAINT FK_BH_PB FOREIGN KEY (MaPB) REFERENCES PHIENBAN(MaPB),
    CONSTRAINT CK_BH_Ngay CHECK (NgayKetThuc >= NgayBatDau)
);
GO

-- ===================== DỮ LIỆU MẪU =====================
INSERT INTO LOAIDIENTHOAI (MaLoai, TenLoai, MoTa) VALUES
('L01', N'Smartphone', N'Điện thoại thông minh cao cấp và tầm trung'),
('L02', N'Feature Phone', N'Điện thoại cơ bản, bàn phím phổ thông'),
('L03', N'Gaming Phone', N'Điện thoại chuyên dụng chơi game cấu hình mạnh');

INSERT INTO HANGDT (MaHang, TenHang, QuocGia) VALUES
('H01', N'Apple', N'Mỹ'),
('H02', N'Samsung', N'Hàn Quốc'),
('H03', N'Xiaomi', N'Trung Quốc'),
('H04', N'OPPO', N'Trung Quốc');

INSERT INTO NHACUNGCAP (MaNCC, TenNCC, DiaChi, SDT, Email) VALUES
('NCC01', N'Công ty TNHH Apple Việt Nam', N'Quận 1, TP.HCM', '0901112223', 'apple.vn@email.com'),
('NCC02', N'Công ty Cổ phần Samsung Vina', N'TP. Thủ Đức, TP.HCM', '0904445556', 'samsung.vina@email.com'),
('NCC03', N'Công ty TNHH Công nghệ Xiaomi VN', N'Quận 3, TP.HCM', '0907778889', 'xiaomi.vn@email.com');

INSERT INTO KHO (MaKho, TenKho, DiaChi, SDT) VALUES
('K01', N'Kho Tổng TP.HCM', N'Quận 7, TP.HCM', '0281234567'),
('K02', N'Kho Hà Nội', N'Quận Cầu Giấy, Hà Nội', '0247654321'),
('K03', N'Kho Đà Nẵng', N'Quận Hải Châu, Đà Nẵng', '0236333444');

-- KM01..KM03 là khuyến mãi cũ (đã hết hạn); KM04 còn hiệu lực để thử bán hàng
INSERT INTO KHUYENMAI (MaKM, TenKM, LoaiKM, NgayBatDau, NgayKetThuc, MoTa) VALUES
('KM01', N'Giảm giá khai trương', N'Phần trăm', '2026-01-01', '2026-02-01', N'Giảm 10% giá sản phẩm'),
('KM02', N'Siêu sale hè', N'Tiền mặt', '2026-06-01', '2026-06-30', N'Giảm trực tiếp 500k'),
('KM03', N'Back to School', N'Phần trăm', '2026-08-15', '2026-09-05', N'Giảm 15% cho học sinh sinh viên'),
('KM04', N'Khuyến mãi quý 4', N'Phần trăm', '2026-10-01', '2026-12-31', N'Giảm giá cuối năm');

INSERT INTO KHACHHANG (MaKH, Ho, Ten, SDT, Email, DiaChi, NgaySinh, DiemTichLuy) VALUES
('KH01', N'Nguyễn Văn', N'An', '0912345678', 'an.nv@email.com', N'Quận 3, TP.HCM', '1995-05-12', 150),
('KH02', N'Trần Thị', N'Bình', '0987654321', 'binh.tt@email.com', N'Quận 10, TP.HCM', '1998-10-20', 80),
('KH03', N'Lê Hoàng', N'Nam', '0935556677', 'nam.lh@email.com', N'Quận Tân Bình, TP.HCM', '2001-12-05', 210),
('KH04', N'Phạm Thị Mai', N'Linh', '0978889900', 'linh.ptm@email.com', N'Quận 1, TP.HCM', '1999-03-18', 50);

INSERT INTO DIENTHOAI (MaDT, TenDT, HeDieuHanh, NamRaMat, MoTa, MaHang, MaLoai, MaNCC) VALUES
('DT01', N'iPhone 15 Pro Max', N'iOS', 2023, N'Flagship Apple chip A17 Pro', 'H01', 'L01', 'NCC01'),
('DT02', N'Samsung Galaxy S24 Ultra', N'Android', 2024, N'Flagship Samsung hỗ trợ AI', 'H02', 'L01', 'NCC02'),
('DT03', N'Xiaomi 14 Pro', N'Android', 2023, N'Điện thoại cấu hình mạnh giá tốt', 'H03', 'L01', 'NCC03'),
('DT04', N'OPPO Find X7', N'Android', 2024, N'Camera chụp ảnh sắc nét', 'H04', 'L01', 'NCC03');

INSERT INTO NHANVIEN (MaNV, Ho, Ten, GioiTinh, NgaySinh, SDT, Email, ChucVu, NgayVaoLam, MaKho) VALUES
('NV01', N'Lê Văn', N'Cường', N'Nam', '1996-03-15', '0933111222', 'cuong.lv@email.com', N'Quản lý kho', '2022-05-10', 'K01'),
('NV02', N'Phạm Thị', N'Dung', N'Nữ', '2000-08-22', '0944333444', 'dung.pt@email.com', N'Nhân viên bán hàng', '2023-01-15', 'K01'),
('NV03', N'Hoàng Văn', N'Đức', N'Nam', '1998-11-10', '0955666777', 'duc.hv@email.com', N'Nhân viên kho', '2023-06-20', 'K02');

INSERT INTO PHIENBAN (MaPB, TenPB, RAM, BoNho, MauSac, GiaNiemYet, TrangThai, MaDT) VALUES
('PB01', N'iPhone 15 Pro Max 256GB', '8GB', '256GB', N'Titan Tự Nhiên', 34990000, N'Còn hàng', 'DT01'),
('PB02', N'iPhone 15 Pro Max 512GB', '8GB', '512GB', N'Titan Xanh', 40990000, N'Còn hàng', 'DT01'),
('PB03', N'Galaxy S24 Ultra 256GB', '12GB', '256GB', N'Đen Titan', 29990000, N'Còn hàng', 'DT02'),
('PB04', N'Galaxy S24 Ultra 512GB', '12GB', '512GB', N'Xám Titan', 33990000, N'Còn hàng', 'DT02'),
('PB05', N'Xiaomi 14 Pro 256GB', '12GB', '256GB', N'Đen', 21990000, N'Còn hàng', 'DT03');

INSERT INTO TONKHO (MaKho, MaPB, SoLuongTon, NgayCapNhat) VALUES
('K01', 'PB01', 25, '2026-06-01'),
('K01', 'PB02', 10, '2026-06-01'),
('K01', 'PB03', 15, '2026-06-01'),
('K02', 'PB04', 20, '2026-06-01'),
('K02', 'PB05', 30, '2026-06-01');

-- Ngày lập hóa đơn nằm trong thời gian hiệu lực của khuyến mãi tương ứng
INSERT INTO HOADON (MaHD, NgayLap, PhuongThucTT, TongTien, MaKH, MaNV, MaKho) VALUES
('HD01', '2026-01-15', N'Tiền mặt', 31491000, 'KH01', 'NV02', 'K01'),
('HD02', '2026-06-11', N'Chuyển khoản', 29490000, 'KH02', 'NV02', 'K01'),
('HD03', '2026-08-20', N'Thẻ ngân hàng', 34841500, 'KH03', 'NV03', 'K02');

-- Mỗi dòng chỉ dùng MỘT kiểu giảm: phần trăm hoặc số tiền (không điền cả hai)
INSERT INTO CHITIETKHUYENMAI (MaKM, MaPB, PhanTramGiam, SoTienGiam) VALUES
('KM01', 'PB01', 10.00, 0),
('KM02', 'PB03', 0.00, 500000),
('KM03', 'PB02', 15.00, 0),
('KM04', 'PB05', 10.00, 0),
('KM04', 'PB04', 0.00, 1000000);

INSERT INTO GIOHANG (MaKH, MaPB, SoLuong, NgayThem) VALUES
('KH04', 'PB04', 1, '2026-06-12'),
('KH02', 'PB05', 2, '2026-06-12');

INSERT INTO CHITIETHOADON (MaHD, MaPB, SoLuong, DonGia, ThanhTien, MaKM) VALUES
('HD01', 'PB01', 1, 34990000, 31491000, 'KM01'),
('HD02', 'PB03', 1, 29990000, 29490000, 'KM02'),
('HD03', 'PB02', 1, 40990000, 34841500, 'KM03');

INSERT INTO BAOHANH (MaBH, SoIMEI, NgayBatDau, NgayKetThuc, TrangThai, GhiChu, MaHD, MaPB) VALUES
('BH01', '359876543210123', '2026-01-15', '2027-01-15', N'Đang bảo hành', N'Bảo hành chính hãng 1 năm', 'HD01', 'PB01'),
('BH02', '359876543210456', '2026-06-11', '2027-06-11', N'Đang bảo hành', N'Bảo hành tiêu chuẩn', 'HD02', 'PB03'),
('BH03', '359876543210789', '2026-08-20', '2027-08-20', N'Đang bảo hành', N'Bảo hành vàng rơi vỡ', 'HD03', 'PB02');
GO
