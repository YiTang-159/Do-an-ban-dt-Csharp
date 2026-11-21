-- ============================================================================
--  BỔ SUNG DỮ LIỆU MẪU CHO CSDL QuanLyBanDienThoai
--  Chạy SAU Database/QuanLyBanDienThoai.sql (không cần chạy Du_lieu_demo_20_dong.sql).
--  Chạy trong SSMS / Azure Data Studio (một cửa sổ truy vấn, bấm Execute một lần).
--
--  Sau khi chạy, mỗi bảng đạt mức yêu cầu của đề:
--    (1) Điện thoại, khách hàng, nhân viên, hãng, loại, nhà cung cấp, khuyến mãi,
--        chi tiết khuyến mãi, tài khoản, bảo hành, giỏ hàng  : tối thiểu 10 dòng
--    (2) Hóa đơn, phiếu nhập hàng                              : tối thiểu 20 dòng
--    (3) Chi tiết hóa đơn, chi tiết phiếu nhập hàng            : tối thiểu 30 dòng
--  (THAMSO chỉ có 4 tham số nghiệp vụ nên không tính vào yêu cầu số dòng.)
--
--  Nguyên tắc: không đụng vào dữ liệu có sẵn; chỉ thêm dòng còn thiếu.
--   - Hóa đơn và phiếu nhập trải đều 4 quý của năm 2026 (để bảng chéo Q1..Q4 ở FormThongKe có số liệu).
--   - Hóa đơn có nhiều nhân viên, nhiều khách hàng, nhiều dòng chi tiết, có khuyến mãi còn hiệu lực
--     tại ngày bán. ThanhTien dùng đúng công thức của trigger TR_CTHD_KiemTraTien; TongTien do trigger tính.
--   - SoLuongTon = tồn cũ + số nhập - số bán của các phiếu/hóa đơn bổ sung (không sửa tay).
--   - Điểm tích lũy cộng theo THAMSO.SoTienMotDiem. Bảo hành theo THAMSO.ThangBaoHanhMacDinh.
--   - Chạy lại lần 2 sẽ tự bỏ qua (không nhân đôi dữ liệu). Có lỗi thì hoàn tác toàn bộ.
--  Tài khoản khách hàng mới (mật khẩu kh123456): tuan, thao, hung, ngoc, danh, yen.
-- ============================================================================
USE QuanLyBanDienThoai;
GO
SET ANSI_NULLS ON;
SET ANSI_PADDING ON;
SET ANSI_WARNINGS ON;
SET ARITHABORT ON;
SET CONCAT_NULL_YIELDS_NULL ON;
SET QUOTED_IDENTIFIER ON;
SET NUMERIC_ROUNDABORT OFF;
GO
SET NOCOUNT ON;
SET XACT_ABORT ON;
BEGIN TRY
    IF EXISTS (SELECT 1 FROM dbo.KHUYENMAI WHERE TenKM = N'Chào xuân 2026')
    BEGIN
        PRINT N'Dữ liệu bổ sung đã được nạp trước đó. Bỏ qua, không thêm lần nữa.';
        RETURN;
    END;

    DECLARE @i int, @n int, @MaPN int, @MaHD int, @MaKH int, @Tong decimal(18,0), @Diem int,
            @SoTien decimal(18,2), @Thang int;
    SELECT @SoTien = GiaTri FROM dbo.THAMSO WHERE TenThamSo = 'SoTienMotDiem' AND GiaTri >= 1;
    SELECT @Thang = CONVERT(int, GiaTri) FROM dbo.THAMSO WHERE TenThamSo = 'ThangBaoHanhMacDinh' AND GiaTri BETWEEN 1 AND 120;
    SET @SoTien = ISNULL(@SoTien, 100000);
    SET @Thang  = ISNULL(@Thang, 12);

    -- ===================== DỮ LIỆU NGUỒN (bảng tạm trong bộ nhớ) =====================
    DECLARE @Hang TABLE (TenHang nvarchar(100), QuocGia nvarchar(50));
    DECLARE @Loai TABLE (TenLoai nvarchar(100), MoTa nvarchar(255));
    DECLARE @NCC  TABLE (Ma varchar(10), TenNCC nvarchar(100), DiaChi nvarchar(255), SDT varchar(20), Email varchar(100));
    DECLARE @NV   TABLE (Ma varchar(10), Ho nvarchar(50), Ten nvarchar(50), GioiTinh nvarchar(10), NgaySinh date, SDT varchar(20), Email varchar(100), ChucVu nvarchar(50), NgayVaoLam date);
    DECLARE @KH   TABLE (Ma varchar(10), Ho nvarchar(50), Ten nvarchar(50), SDT varchar(20), Email varchar(100), DiaChi nvarchar(255), NgaySinh date);
    DECLARE @SP   TABLE (Ma varchar(10), TenDT nvarchar(100), HeDieuHanh nvarchar(50), NamRaMat int, RAM int, BoNho int, MauSac nvarchar(50), GiaNiemYet decimal(18,0), MoTa nvarchar(255), Hang nvarchar(100), Loai nvarchar(100));
    DECLARE @KM   TABLE (TenKM nvarchar(100), NgayBatDau date, NgayKetThuc date, MoTa nvarchar(255));
    DECLARE @CTKM TABLE (TenKM nvarchar(100), Sp varchar(10), PhanTram decimal(5,2));
    DECLARE @PN   TABLE (Stt int, NgayNhap datetime, NCC varchar(10), NV varchar(10));
    DECLARE @CTPN TABLE (Stt int, Sp varchar(10), SoLuong int, DonGiaNhap decimal(18,0));
    DECLARE @HD   TABLE (Stt int, NgayLap datetime, KH varchar(10), NV varchar(10), PTTT nvarchar(50));
    DECLARE @CTHD TABLE (Stt int, Sp varchar(10), SoLuong int, TenKM nvarchar(100) NULL);
    DECLARE @BH   TABLE (HD int, Sp varchar(10), TrangThai nvarchar(50), GhiChu nvarchar(255));
    DECLARE @GH   TABLE (KH varchar(10), Sp varchar(10), SoLuong int, NgayThem date);
    DECLARE @TK   TABLE (TenDangNhap varchar(50), KH varchar(10), MatKhau char(64));
    INSERT @Hang VALUES
    (N'Apple', N'Mỹ'),
    (N'Samsung', N'Hàn Quốc'),
    (N'Xiaomi', N'Trung Quốc'),
    (N'OPPO', N'Trung Quốc'),
    (N'Vivo', N'Trung Quốc'),
    (N'Realme', N'Trung Quốc'),
    (N'Nokia', N'Phần Lan'),
    (N'Google', N'Mỹ'),
    (N'Sony', N'Nhật Bản'),
    (N'ASUS', N'Đài Loan');
    INSERT @Loai VALUES
    (N'Smartphone', N'Điện thoại thông minh cao cấp và tầm trung'),
    (N'Feature Phone', N'Điện thoại cơ bản, bàn phím phổ thông'),
    (N'Gaming Phone', N'Điện thoại chuyên dụng chơi game cấu hình mạnh'),
    (N'Điện thoại gập', N'Màn hình gập, thiết kế nhỏ gọn khi cất túi'),
    (N'Tầm trung', N'Cấu hình cân bằng, giá từ 7 đến 15 triệu'),
    (N'Giá rẻ', N'Dưới 7 triệu, đáp ứng nhu cầu cơ bản'),
    (N'Cao cấp', N'Flagship, camera và hiệu năng hàng đầu'),
    (N'Chụp ảnh chuyên nghiệp', N'Ưu tiên cụm camera và xử lý hình ảnh'),
    (N'Pin trâu', N'Dung lượng pin lớn, dùng nhiều ngày'),
    (N'Dành cho người lớn tuổi', N'Chữ lớn, giao diện đơn giản, loa to');
    INSERT @NCC VALUES
    ('NCC01', N'Công ty TNHH Apple Việt Nam', N'Quận 1, TP.HCM', '0901112223', 'apple.vn@email.com'),
    ('NCC02', N'Công ty Cổ phần Samsung Vina', N'TP. Thủ Đức, TP.HCM', '0904445556', 'samsung.vina@email.com'),
    ('NCC03', N'Công ty TNHH Công nghệ Xiaomi VN', N'Quận 3, TP.HCM', '0907778889', 'xiaomi.vn@email.com'),
    ('NCC04', N'Công ty TNHH Phân phối Di Động Sài Gòn', N'Quận Tân Bình, TP.HCM', '0908123401', 'didong.saigon@email.com'),
    ('NCC05', N'Công ty CP Viễn Thông Miền Nam', N'Quận 10, TP.HCM', '0908123402', 'vienthong.mn@email.com'),
    ('NCC06', N'Công ty TNHH Điện Tử Hoàng Long', N'Quận 5, TP.HCM', '0908123403', 'hoanglong.dt@email.com'),
    ('NCC07', N'Công ty TNHH Thương Mại Minh Phát', N'Quận Bình Thạnh, TP.HCM', '0908123404', 'minhphat.tm@email.com'),
    ('NCC08', N'Công ty CP Công Nghệ Việt Tiến', N'TP. Thủ Đức, TP.HCM', '0908123405', 'viettien.cn@email.com'),
    ('NCC09', N'Công ty TNHH Mobile Plaza', N'Quận 1, TP.HCM', '0908123406', 'mobileplaza@email.com'),
    ('NCC10', N'Công ty TNHH Nhập Khẩu Thiên Hòa', N'Quận 6, TP.HCM', '0908123407', 'thienhoa.nk@email.com');
    INSERT @NV VALUES
    ('NV01', N'Lê Văn', N'Cường', N'Nam', '1996-03-15', '0933111222', 'cuong.lv@email.com', N'Quản lý', '2022-05-10'),
    ('NV02', N'Phạm Thị', N'Dung', N'Nữ', '2000-08-22', '0944333444', 'dung.pt@email.com', N'Nhân viên bán hàng', '2023-01-15'),
    ('NV03', N'Hoàng Văn', N'Đức', N'Nam', '1998-11-10', '0955666777', 'duc.hv@email.com', N'Nhân viên kho', '2023-06-20'),
    ('NV04', N'Nguyễn Thị', N'Hạnh', N'Nữ', '1999-02-14', '0961234501', 'hanh.nt@email.com', N'Nhân viên bán hàng', '2023-09-01'),
    ('NV05', N'Trần Quốc', N'Bảo', N'Nam', '1997-07-30', '0961234502', 'bao.tq@email.com', N'Nhân viên bán hàng', '2022-11-07'),
    ('NV06', N'Võ Thị', N'Mai', N'Nữ', '2001-04-18', '0961234503', 'mai.vt@email.com', N'Thu ngân', '2024-02-19'),
    ('NV07', N'Đặng Minh', N'Khoa', N'Nam', '1995-12-09', '0961234504', 'khoa.dm@email.com', N'Nhân viên kho', '2021-08-23'),
    ('NV08', N'Bùi Thị', N'Lan', N'Nữ', '1994-09-25', '0961234505', 'lan.bt@email.com', N'Kế toán', '2020-03-16'),
    ('NV09', N'Phan Văn', N'Tài', N'Nam', '2000-01-05', '0961234506', 'tai.pv@email.com', N'Nhân viên bán hàng', '2024-06-10'),
    ('NV10', N'Ngô Thị', N'Thu', N'Nữ', '1998-06-12', '0961234507', 'thu.nt@email.com', N'Nhân viên bán hàng', '2023-04-03'),
    ('NV11', N'Đỗ Hữu', N'Phước', N'Nam', '1993-10-21', '0961234508', 'phuoc.dh@email.com', N'Nhân viên kho', '2022-01-17'),
    ('NV12', N'Lý Thanh', N'Tùng', N'Nam', '1996-03-27', '0961234509', 'tung.lt@email.com', N'Quản lý', '2023-12-04');
    INSERT @KH VALUES
    ('KH01', N'Nguyễn Văn', N'An', '0912345678', 'an.nv@email.com', N'Quận 3, TP.HCM', '1995-05-12'),
    ('KH02', N'Trần Thị', N'Bình', '0987654321', 'binh.tt@email.com', N'Quận 10, TP.HCM', '1998-10-20'),
    ('KH03', N'Lê Hoàng', N'Nam', '0935556677', 'nam.lh@email.com', N'Quận Tân Bình, TP.HCM', '2001-12-05'),
    ('KH04', N'Phạm Minh', N'Tuấn', '0971234501', 'tuan.pm@email.com', N'Quận 7, TP.HCM', '1992-03-08'),
    ('KH05', N'Nguyễn Thị', N'Thảo', '0971234502', 'thao.nt@email.com', N'TP. Thủ Đức, TP.HCM', '1999-11-23'),
    ('KH06', N'Võ Văn', N'Hùng', '0971234503', 'hung.vv@email.com', N'Quận Gò Vấp, TP.HCM', '1990-05-17'),
    ('KH07', N'Lê Thị', N'Ngọc', '0971234504', 'ngoc.lt@email.com', N'Quận Phú Nhuận, TP.HCM', '2002-08-01'),
    ('KH08', N'Đinh Công', N'Danh', '0971234505', 'danh.dc@email.com', N'Quận 5, TP.HCM', '1988-01-29'),
    ('KH09', N'Hồ Thị', N'Yến', '0971234506', 'yen.ht@email.com', N'Quận Bình Thạnh, TP.HCM', '1996-12-14'),
    ('KH10', N'Trương Quốc', N'Việt', '0971234507', 'viet.tq@email.com', N'TP. Biên Hòa, Đồng Nai', '1985-07-07'),
    ('KH11', N'Mai Thị', N'Hồng', '0971234508', 'hong.mt@email.com', N'Quận 12, TP.HCM', '2000-02-20'),
    ('KH12', N'Cao Đức', N'Huy', '0971234509', 'huy.cd@email.com', N'TP. Dĩ An, Bình Dương', '1994-09-09');
    INSERT @SP VALUES
    ('P01', N'iPhone 15 Pro Max', N'iOS', 2023, 8, 256, N'Titan Tự Nhiên', 34990000, N'Flagship Apple chip A17 Pro', N'Apple', N'Smartphone'),
    ('P02', N'iPhone 15 Pro Max', N'iOS', 2023, 8, 512, N'Titan Xanh', 40990000, N'Flagship Apple bản 512GB', N'Apple', N'Smartphone'),
    ('P03', N'Samsung Galaxy S24 Ultra', N'Android', 2024, 12, 256, N'Đen Titan', 29990000, N'Flagship Samsung hỗ trợ AI', N'Samsung', N'Smartphone'),
    ('P04', N'Samsung Galaxy S24 Ultra', N'Android', 2024, 12, 512, N'Xám Titan', 33990000, N'Flagship Samsung bản 512GB', N'Samsung', N'Smartphone'),
    ('P05', N'Xiaomi 14 Pro', N'Android', 2023, 12, 256, N'Đen', 21990000, N'Cấu hình mạnh giá tốt', N'Xiaomi', N'Smartphone'),
    ('P06', N'Xiaomi Redmi Note 13', N'Android', 2024, 8, 128, N'Xanh Dương', 6490000, N'Tầm trung pin lớn', N'Xiaomi', N'Smartphone'),
    ('P07', N'OPPO Find X7', N'Android', 2024, 12, 256, N'Nâu', 24990000, N'Camera chụp ảnh sắc nét', N'OPPO', N'Smartphone'),
    ('P08', N'OPPO Reno11', N'Android', 2024, 8, 256, N'Xám', 10990000, N'Thiết kế mỏng nhẹ', N'OPPO', N'Smartphone'),
    ('P09', N'iPhone 15', N'iOS', 2023, 6, 128, N'Hồng', 19990000, N'Dynamic Island, camera 48MP', N'Apple', N'Cao cấp'),
    ('P10', N'iPhone 14', N'iOS', 2022, 6, 128, N'Xanh', 16990000, N'Chip A15 Bionic, pin bền', N'Apple', N'Cao cấp'),
    ('P11', N'iPhone 15 Plus', N'iOS', 2023, 6, 256, N'Vàng', 25990000, N'Màn hình lớn 6.7 inch', N'Apple', N'Cao cấp'),
    ('P12', N'Samsung Galaxy A55 5G', N'Android', 2024, 8, 128, N'Xanh Navy', 9990000, N'Khung kim loại, chống nước IP67', N'Samsung', N'Tầm trung'),
    ('P13', N'Samsung Galaxy Z Flip5', N'Android', 2023, 8, 256, N'Tím', 21990000, N'Điện thoại gập vỏ sò', N'Samsung', N'Điện thoại gập'),
    ('P14', N'Samsung Galaxy A15', N'Android', 2024, 4, 128, N'Đen', 4990000, N'Màn hình AMOLED giá rẻ', N'Samsung', N'Giá rẻ'),
    ('P15', N'Xiaomi 13T', N'Android', 2023, 12, 256, N'Xanh', 12990000, N'Camera hợp tác Leica', N'Xiaomi', N'Tầm trung'),
    ('P16', N'Xiaomi Redmi 13C', N'Android', 2023, 4, 128, N'Xanh', 3290000, N'Điện thoại phổ thông giá tốt', N'Xiaomi', N'Giá rẻ'),
    ('P17', N'OPPO A78', N'Android', 2023, 8, 128, N'Đen', 6290000, N'Sạc nhanh 33W', N'OPPO', N'Giá rẻ'),
    ('P18', N'vivo V29 5G', N'Android', 2023, 12, 256, N'Xanh', 10990000, N'Camera chân dung Aura Light', N'Vivo', N'Tầm trung'),
    ('P19', N'realme 11 Pro+', N'Android', 2023, 12, 512, N'Be', 11990000, N'Camera 200MP', N'Realme', N'Tầm trung'),
    ('P20', N'realme C67', N'Android', 2023, 8, 128, N'Xanh', 4990000, N'Pin 5000mAh', N'Realme', N'Pin trâu'),
    ('P21', N'Nokia G42 5G', N'Android', 2023, 6, 128, N'Tím', 5990000, N'Dễ sửa chữa, pin 3 ngày', N'Nokia', N'Giá rẻ'),
    ('P22', N'ASUS ROG Phone 8', N'Android', 2024, 16, 512, N'Đen', 26990000, N'Chuyên game, tản nhiệt chủ động', N'ASUS', N'Gaming Phone'),
    ('P23', N'Google Pixel 8', N'Android', 2023, 8, 128, N'Đen', 16990000, N'Camera AI, cập nhật 7 năm', N'Google', N'Chụp ảnh chuyên nghiệp'),
    ('P24', N'Sony Xperia 1 V', N'Android', 2023, 12, 256, N'Đen', 31990000, N'Quay phim chuyên nghiệp 4K', N'Sony', N'Chụp ảnh chuyên nghiệp');
    INSERT @KM VALUES
    (N'Khuyến mãi quý 4', '2026-10-01', '2027-06-30', N'Giảm giá cuối năm'),
    (N'Chào xuân 2026', '2026-01-15', '2026-02-28', N'Giảm giá đầu năm mới'),
    (N'Mừng 8/3', '2026-03-01', '2026-03-10', N'Ưu đãi Quốc tế Phụ nữ'),
    (N'Siêu sale 30/4 - 1/5', '2026-04-25', '2026-05-05', N'Ưu đãi dịp lễ 30/4 và 1/5'),
    (N'Hè sôi động', '2026-06-01', '2026-07-15', N'Giảm giá mùa hè'),
    (N'Khai giảng 2026', '2026-08-20', '2026-09-10', N'Ưu đãi học sinh sinh viên năm học mới'),
    (N'Sale đôi 9/9', '2026-09-09', '2026-09-12', N'Ngày hội mua sắm 9/9'),
    (N'Black Friday 2026', '2026-11-20', '2026-11-30', N'Giảm sâu Black Friday'),
    (N'Tri ân khách hàng thân thiết', '2026-01-01', '2026-12-31', N'Ưu đãi cả năm cho một số dòng máy');
    INSERT @CTKM VALUES
    (N'Khuyến mãi quý 4', 'P01', 10),
    (N'Khuyến mãi quý 4', 'P03', 5),
    (N'Chào xuân 2026', 'P09', 5),
    (N'Chào xuân 2026', 'P12', 8),
    (N'Chào xuân 2026', 'P14', 10),
    (N'Chào xuân 2026', 'P16', 10),
    (N'Mừng 8/3', 'P12', 5),
    (N'Mừng 8/3', 'P17', 10),
    (N'Mừng 8/3', 'P19', 7),
    (N'Mừng 8/3', 'P13', 6),
    (N'Siêu sale 30/4 - 1/5', 'P05', 8),
    (N'Siêu sale 30/4 - 1/5', 'P15', 12),
    (N'Siêu sale 30/4 - 1/5', 'P18', 10),
    (N'Siêu sale 30/4 - 1/5', 'P06', 5),
    (N'Hè sôi động', 'P03', 5),
    (N'Hè sôi động', 'P10', 7),
    (N'Hè sôi động', 'P12', 10),
    (N'Hè sôi động', 'P20', 12),
    (N'Hè sôi động', 'P22', 5),
    (N'Khai giảng 2026', 'P16', 15),
    (N'Khai giảng 2026', 'P08', 8),
    (N'Khai giảng 2026', 'P14', 12),
    (N'Khai giảng 2026', 'P17', 10),
    (N'Sale đôi 9/9', 'P10', 10),
    (N'Sale đôi 9/9', 'P21', 12),
    (N'Sale đôi 9/9', 'P18', 8),
    (N'Black Friday 2026', 'P22', 8),
    (N'Black Friday 2026', 'P11', 6),
    (N'Black Friday 2026', 'P24', 5),
    (N'Black Friday 2026', 'P23', 7),
    (N'Tri ân khách hàng thân thiết', 'P01', 3),
    (N'Tri ân khách hàng thân thiết', 'P02', 3),
    (N'Tri ân khách hàng thân thiết', 'P07', 5),
    (N'Tri ân khách hàng thân thiết', 'P11', 4);
    INSERT @PN VALUES
    (1, '2026-01-05T08:30:00', 'NCC01', 'NV03'),
    (2, '2026-01-08T09:15:00', 'NCC02', 'NV07'),
    (3, '2026-01-12T10:00:00', 'NCC03', 'NV03'),
    (4, '2026-01-20T14:00:00', 'NCC04', 'NV07'),
    (5, '2026-02-03T09:00:00', 'NCC05', 'NV11'),
    (6, '2026-02-18T11:30:00', 'NCC06', 'NV03'),
    (7, '2026-03-02T09:45:00', 'NCC07', 'NV07'),
    (8, '2026-03-15T15:20:00', 'NCC01', 'NV11'),
    (9, '2026-04-06T08:50:00', 'NCC02', 'NV03'),
    (10, '2026-04-22T10:30:00', 'NCC03', 'NV07'),
    (11, '2026-05-09T13:10:00', 'NCC08', 'NV11'),
    (12, '2026-05-27T09:25:00', 'NCC04', 'NV03'),
    (13, '2026-06-10T14:40:00', 'NCC01', 'NV07'),
    (14, '2026-07-01T08:35:00', 'NCC09', 'NV11'),
    (15, '2026-07-21T10:05:00', 'NCC02', 'NV03'),
    (16, '2026-08-12T09:20:00', 'NCC10', 'NV07'),
    (17, '2026-09-02T11:00:00', 'NCC01', 'NV11'),
    (18, '2026-09-25T15:30:00', 'NCC03', 'NV03'),
    (19, '2026-10-18T09:40:00', 'NCC05', 'NV07'),
    (20, '2026-11-10T10:15:00', 'NCC07', 'NV11');
    INSERT @CTPN VALUES
    (1, 'P09', 20, 15590000),
    (1, 'P10', 15, 13080000),
    (1, 'P11', 10, 20270000),
    (2, 'P12', 25, 7890000),
    (2, 'P13', 10, 17150000),
    (2, 'P14', 30, 3990000),
    (3, 'P15', 15, 10260000),
    (3, 'P16', 30, 2630000),
    (4, 'P17', 20, 4970000),
    (4, 'P18', 15, 8570000),
    (5, 'P19', 12, 9350000),
    (5, 'P20', 20, 3990000),
    (6, 'P21', 15, 4730000),
    (6, 'P22', 8, 20780000),
    (7, 'P23', 10, 13250000),
    (7, 'P24', 6, 24630000),
    (8, 'P01', 10, 26590000),
    (8, 'P02', 5, 31150000),
    (9, 'P03', 10, 23090000),
    (9, 'P04', 8, 26170000),
    (10, 'P05', 10, 17150000),
    (10, 'P06', 20, 5190000),
    (11, 'P07', 8, 19240000),
    (11, 'P08', 12, 8680000),
    (12, 'P17', 15, 4970000),
    (13, 'P09', 10, 15590000),
    (13, 'P11', 6, 20010000),
    (14, 'P12', 15, 7890000),
    (14, 'P14', 20, 3990000),
    (14, 'P16', 15, 2630000),
    (15, 'P13', 8, 17150000),
    (15, 'P03', 5, 23090000),
    (16, 'P18', 10, 8570000),
    (16, 'P19', 8, 9350000),
    (16, 'P21', 10, 4730000),
    (17, 'P10', 10, 13080000),
    (17, 'P01', 6, 26590000),
    (18, 'P15', 10, 10260000),
    (18, 'P05', 8, 17150000),
    (18, 'P16', 20, 2630000),
    (19, 'P20', 15, 3990000),
    (19, 'P22', 5, 20780000),
    (19, 'P23', 8, 13250000),
    (20, 'P24', 4, 24630000),
    (20, 'P12', 10, 7890000);
    INSERT @HD VALUES
    (1, '2026-01-18T10:20:00', 'KH04', 'NV04', N'Tiền mặt'),
    (2, '2026-02-07T15:45:00', 'KH05', 'NV05', N'Chuyển khoản'),
    (3, '2026-02-26T11:10:00', 'KH06', 'NV02', N'Thẻ'),
    (4, '2026-03-05T09:30:00', 'KH07', 'NV09', N'Tiền mặt'),
    (5, '2026-03-22T16:05:00', 'KH08', 'NV04', N'Chuyển khoản'),
    (6, '2026-04-12T10:00:00', 'KH09', 'NV05', N'Thẻ'),
    (7, '2026-04-28T14:25:00', 'KH10', 'NV10', N'Chuyển khoản'),
    (8, '2026-05-16T09:50:00', 'KH11', 'NV02', N'Tiền mặt'),
    (9, '2026-06-08T13:35:00', 'KH12', 'NV09', N'Chuyển khoản'),
    (10, '2026-06-24T17:15:00', 'KH04', 'NV10', N'Thẻ'),
    (11, '2026-07-14T10:40:00', 'KH05', 'NV04', N'Tiền mặt'),
    (12, '2026-08-03T11:55:00', 'KH01', 'NV05', N'Chuyển khoản'),
    (13, '2026-08-25T15:00:00', 'KH06', 'NV09', N'Thẻ'),
    (14, '2026-09-09T09:20:00', 'KH08', 'NV02', N'Chuyển khoản'),
    (15, '2026-09-21T14:10:00', 'KH02', 'NV10', N'Tiền mặt'),
    (16, '2026-10-06T10:30:00', 'KH09', 'NV04', N'Thẻ'),
    (17, '2026-10-19T16:20:00', 'KH10', 'NV05', N'Chuyển khoản'),
    (18, '2026-10-30T11:45:00', 'KH03', 'NV09', N'Tiền mặt'),
    (19, '2026-11-12T09:55:00', 'KH07', 'NV02', N'Thẻ'),
    (20, '2026-11-20T14:30:00', 'KH11', 'NV10', N'Chuyển khoản');
    INSERT @CTHD VALUES
    (1, 'P09', 1, N'Chào xuân 2026'),
    (1, 'P12', 1, N'Chào xuân 2026'),
    (2, 'P14', 2, N'Chào xuân 2026'),
    (2, 'P16', 1, N'Chào xuân 2026'),
    (3, 'P15', 1, NULL),
    (3, 'P13', 1, NULL),
    (4, 'P12', 1, N'Mừng 8/3'),
    (4, 'P17', 1, N'Mừng 8/3'),
    (4, 'P19', 1, N'Mừng 8/3'),
    (5, 'P01', 1, N'Tri ân khách hàng thân thiết'),
    (5, 'P23', 1, NULL),
    (6, 'P03', 1, NULL),
    (6, 'P04', 1, NULL),
    (7, 'P05', 1, N'Siêu sale 30/4 - 1/5'),
    (7, 'P15', 1, N'Siêu sale 30/4 - 1/5'),
    (7, 'P06', 2, N'Siêu sale 30/4 - 1/5'),
    (8, 'P07', 1, N'Tri ân khách hàng thân thiết'),
    (8, 'P08', 1, NULL),
    (9, 'P10', 1, N'Hè sôi động'),
    (9, 'P20', 2, N'Hè sôi động'),
    (9, 'P12', 1, N'Hè sôi động'),
    (10, 'P22', 1, N'Hè sôi động'),
    (10, 'P03', 1, N'Hè sôi động'),
    (11, 'P18', 1, NULL),
    (11, 'P21', 1, NULL),
    (12, 'P11', 1, N'Tri ân khách hàng thân thiết'),
    (12, 'P09', 1, NULL),
    (13, 'P16', 2, N'Khai giảng 2026'),
    (13, 'P14', 1, N'Khai giảng 2026'),
    (13, 'P17', 1, N'Khai giảng 2026'),
    (14, 'P10', 1, N'Sale đôi 9/9'),
    (14, 'P21', 1, N'Sale đôi 9/9'),
    (14, 'P18', 2, N'Sale đôi 9/9'),
    (15, 'P13', 1, NULL),
    (15, 'P24', 1, NULL),
    (16, 'P01', 1, N'Khuyến mãi quý 4'),
    (16, 'P03', 1, N'Khuyến mãi quý 4'),
    (16, 'P22', 1, NULL),
    (17, 'P20', 2, NULL),
    (17, 'P23', 1, NULL),
    (17, 'P05', 1, NULL),
    (18, 'P12', 1, NULL),
    (18, 'P16', 1, NULL),
    (19, 'P11', 1, N'Tri ân khách hàng thân thiết'),
    (19, 'P07', 1, N'Tri ân khách hàng thân thiết'),
    (20, 'P22', 1, N'Black Friday 2026'),
    (20, 'P24', 1, N'Black Friday 2026'),
    (20, 'P23', 1, N'Black Friday 2026');
    INSERT @BH VALUES
    (1, 'P09', N'Đã hoàn thành', N'Đã xử lý lỗi sạc, trả máy cho khách'),
    (1, 'P12', N'Đang bảo hành', N'Bảo hành chính hãng 12 tháng'),
    (2, 'P14', N'Đang bảo hành', N'Bảo hành chính hãng 12 tháng'),
    (2, 'P14', N'Đang bảo hành', N'Bảo hành chính hãng 12 tháng'),
    (3, 'P13', N'Đang bảo hành', N'Bảo hành chính hãng 12 tháng'),
    (4, 'P17', N'Đã từ chối', N'Máy rơi vỡ màn hình, không thuộc diện bảo hành'),
    (5, 'P23', N'Đang bảo hành', N'Bảo hành chính hãng 12 tháng'),
    (6, 'P03', N'Đã hoàn thành', N'Thay pin theo bảo hành, trả máy sau 3 ngày'),
    (7, 'P05', N'Đang bảo hành', N'Bảo hành chính hãng 12 tháng'),
    (8, 'P07', N'Đang bảo hành', N'Bảo hành chính hãng 12 tháng'),
    (9, 'P10', N'Đang bảo hành', N'Bảo hành chính hãng 12 tháng'),
    (10, 'P22', N'Đang bảo hành', N'Bảo hành chính hãng 12 tháng'),
    (12, 'P11', N'Đang bảo hành', N'Bảo hành chính hãng 12 tháng');
    INSERT @GH VALUES
    ('KH05', 'P09', 1, '2026-11-15'),
    ('KH05', 'P14', 2, '2026-11-15'),
    ('KH06', 'P22', 1, '2026-11-16'),
    ('KH08', 'P11', 1, '2026-11-17'),
    ('KH09', 'P18', 1, '2026-11-18'),
    ('KH10', 'P12', 1, '2026-11-18'),
    ('KH11', 'P15', 1, '2026-11-19'),
    ('KH12', 'P10', 1, '2026-11-19'),
    ('KH12', 'P21', 1, '2026-11-19'),
    ('KH04', 'P24', 1, '2026-11-20');
    INSERT @TK VALUES
    ('tuan', 'KH04', 'b9edb8982854b5e79a890577927de0c29af629a14dd8c1d326bab0056ad0a313'),
    ('thao', 'KH05', 'b9edb8982854b5e79a890577927de0c29af629a14dd8c1d326bab0056ad0a313'),
    ('hung', 'KH06', 'b9edb8982854b5e79a890577927de0c29af629a14dd8c1d326bab0056ad0a313'),
    ('ngoc', 'KH07', 'b9edb8982854b5e79a890577927de0c29af629a14dd8c1d326bab0056ad0a313'),
    ('danh', 'KH08', 'b9edb8982854b5e79a890577927de0c29af629a14dd8c1d326bab0056ad0a313'),
    ('yen', 'KH09', 'b9edb8982854b5e79a890577927de0c29af629a14dd8c1d326bab0056ad0a313');

    -- ===================== GHI VÀO CSDL (một transaction) =====================
    BEGIN TRANSACTION;

    -- 1. Danh mục (chỉ thêm dòng chưa có, so theo khóa duy nhất của CSDL)
    INSERT dbo.HANGDT(TenHang, QuocGia)
    SELECT h.TenHang, h.QuocGia FROM @Hang h
    WHERE NOT EXISTS (SELECT 1 FROM dbo.HANGDT x WHERE x.TenHang = h.TenHang);

    INSERT dbo.LOAIDIENTHOAI(TenLoai, MoTa)
    SELECT l.TenLoai, l.MoTa FROM @Loai l
    WHERE NOT EXISTS (SELECT 1 FROM dbo.LOAIDIENTHOAI x WHERE x.TenLoai = l.TenLoai);

    INSERT dbo.NHACUNGCAP(TenNCC, DiaChi, SDT, Email)
    SELECT c.TenNCC, c.DiaChi, c.SDT, c.Email FROM @NCC c
    WHERE NOT EXISTS (SELECT 1 FROM dbo.NHACUNGCAP x WHERE x.SDT = c.SDT OR x.Email = c.Email);

    INSERT dbo.NHANVIEN(Ho, Ten, GioiTinh, NgaySinh, SDT, Email, ChucVu, NgayVaoLam)
    SELECT n.Ho, n.Ten, n.GioiTinh, n.NgaySinh, n.SDT, n.Email, n.ChucVu, n.NgayVaoLam FROM @NV n
    WHERE NOT EXISTS (SELECT 1 FROM dbo.NHANVIEN x WHERE x.SDT = n.SDT OR x.Email = n.Email);

    INSERT dbo.KHACHHANG(Ho, Ten, SDT, Email, DiaChi, NgaySinh, DiemTichLuy)
    SELECT k.Ho, k.Ten, k.SDT, k.Email, k.DiaChi, k.NgaySinh, 0 FROM @KH k
    WHERE NOT EXISTS (SELECT 1 FROM dbo.KHACHHANG x WHERE x.SDT = k.SDT OR x.Email = k.Email);

    -- Điện thoại mới có SoLuongTon = 0; tồn chỉ tăng nhờ phiếu nhập ở bước 3
    INSERT dbo.DIENTHOAI(TenDT, HeDieuHanh, NamRaMat, RAM, BoNho, MauSac, GiaNiemYet, SoLuongTon, MoTa, MaHang, MaLoai)
    SELECT s.TenDT, s.HeDieuHanh, s.NamRaMat, s.RAM, s.BoNho, s.MauSac, s.GiaNiemYet, 0, s.MoTa, h.MaHang, l.MaLoai
    FROM @SP s
    JOIN dbo.HANGDT h ON h.TenHang = s.Hang
    JOIN dbo.LOAIDIENTHOAI l ON l.TenLoai = s.Loai
    WHERE NOT EXISTS (SELECT 1 FROM dbo.DIENTHOAI x
                      WHERE x.TenDT = s.TenDT AND x.RAM = s.RAM AND x.BoNho = s.BoNho AND x.MauSac = s.MauSac);

    INSERT dbo.KHUYENMAI(TenKM, NgayBatDau, NgayKetThuc, MoTa)
    SELECT k.TenKM, k.NgayBatDau, k.NgayKetThuc, k.MoTa FROM @KM k
    WHERE NOT EXISTS (SELECT 1 FROM dbo.KHUYENMAI x WHERE x.TenKM = k.TenKM);

    -- Bảng ánh xạ mã nội bộ (P01, NV01...) sang mã thật trong CSDL
    DECLARE @MapDT  TABLE (Ma varchar(10) PRIMARY KEY, MaDT  int);
    DECLARE @MapNV  TABLE (Ma varchar(10) PRIMARY KEY, MaNV  int);
    DECLARE @MapKH  TABLE (Ma varchar(10) PRIMARY KEY, MaKH  int);
    DECLARE @MapNCC TABLE (Ma varchar(10) PRIMARY KEY, MaNCC int);
    DECLARE @MapHD  TABLE (Stt int PRIMARY KEY, MaHD int);

    INSERT @MapDT SELECT s.Ma, d.MaDT FROM @SP s
        JOIN dbo.DIENTHOAI d ON d.TenDT = s.TenDT AND d.RAM = s.RAM AND d.BoNho = s.BoNho AND d.MauSac = s.MauSac;
    INSERT @MapNV SELECT n.Ma, x.MaNV FROM @NV n JOIN dbo.NHANVIEN x ON x.Email = n.Email;
    INSERT @MapKH SELECT k.Ma, x.MaKH FROM @KH k JOIN dbo.KHACHHANG x ON x.Email = k.Email;
    INSERT @MapNCC SELECT c.Ma, x.MaNCC FROM @NCC c JOIN dbo.NHACUNGCAP x ON x.Email = c.Email;

    IF (SELECT COUNT(*) FROM @MapDT)  <> (SELECT COUNT(*) FROM @SP)  THROW 50201, N'Không khớp được điện thoại mẫu với CSDL (có thể trùng khóa với dữ liệu khác).', 1;
    IF (SELECT COUNT(*) FROM @MapNV)  <> (SELECT COUNT(*) FROM @NV)  THROW 50202, N'Không khớp được nhân viên mẫu với CSDL (SDT hoặc Email đã dùng cho người khác).', 1;
    IF (SELECT COUNT(*) FROM @MapKH)  <> (SELECT COUNT(*) FROM @KH)  THROW 50203, N'Không khớp được khách hàng mẫu với CSDL (SDT hoặc Email đã dùng cho người khác).', 1;
    IF (SELECT COUNT(*) FROM @MapNCC) <> (SELECT COUNT(*) FROM @NCC) THROW 50204, N'Không khớp được nhà cung cấp mẫu với CSDL (SDT hoặc Email đã dùng cho người khác).', 1;

    INSERT dbo.CHITIETKHUYENMAI(MaKM, MaDT, PhanTramGiam)
    SELECT k.MaKM, m.MaDT, c.PhanTram FROM @CTKM c
    JOIN dbo.KHUYENMAI k ON k.TenKM = c.TenKM
    JOIN @MapDT m ON m.Ma = c.Sp
    WHERE NOT EXISTS (SELECT 1 FROM dbo.CHITIETKHUYENMAI x WHERE x.MaKM = k.MaKM AND x.MaDT = m.MaDT);

    -- 2. Tài khoản khách hàng mới (mật khẩu kh123456, đã băm SHA-256)
    INSERT dbo.TAIKHOAN(TenDangNhap, MatKhau, VaiTro, MaNV, MaKH, TrangThai)
    SELECT t.TenDangNhap, t.MatKhau, N'KhachHang', NULL, k.MaKH, 1 FROM @TK t
    JOIN @MapKH k ON k.Ma = t.KH
    WHERE NOT EXISTS (SELECT 1 FROM dbo.TAIKHOAN x WHERE x.TenDangNhap = t.TenDangNhap OR x.MaKH = k.MaKH);

    -- 3. Phiếu nhập hàng: mỗi phiếu một transaction chung; trigger tự cộng TongTien từ chi tiết
    SET @i = 1; SELECT @n = COUNT(*) FROM @PN;
    WHILE @i <= @n
    BEGIN
        INSERT dbo.PHIEUNHAPHANG(NgayNhap, TongTien, MaNCC, MaNV)
        SELECT p.NgayNhap, 0, c.MaNCC, v.MaNV FROM @PN p
        JOIN @MapNCC c ON c.Ma = p.NCC
        JOIN @MapNV  v ON v.Ma = p.NV
        WHERE p.Stt = @i;
        IF @@ROWCOUNT <> 1 THROW 50210, N'Không tạo được phiếu nhập mẫu.', 1;
        SET @MaPN = CONVERT(int, SCOPE_IDENTITY());

        INSERT dbo.CHITIETPHIEUNHAPHANG(MaPN, MaDT, SoLuong, DonGiaNhap, ThanhTien)
        SELECT @MaPN, m.MaDT, l.SoLuong, l.DonGiaNhap, l.SoLuong * l.DonGiaNhap
        FROM @CTPN l JOIN @MapDT m ON m.Ma = l.Sp
        WHERE l.Stt = @i;
        SET @i += 1;
    END;

    -- 4. Hóa đơn: giá lấy từ DIENTHOAI, % giảm lấy từ CHITIETKHUYENMAI, công thức khớp trigger TR_CTHD_KiemTraTien
    SET @i = 1; SELECT @n = COUNT(*) FROM @HD;
    WHILE @i <= @n
    BEGIN
        INSERT dbo.HOADON(NgayLap, PhuongThucTT, TongTien, MaKH, MaNV)
        SELECT h.NgayLap, h.PTTT, 0, k.MaKH, v.MaNV FROM @HD h
        JOIN @MapKH k ON k.Ma = h.KH
        JOIN @MapNV v ON v.Ma = h.NV
        WHERE h.Stt = @i;
        IF @@ROWCOUNT <> 1 THROW 50211, N'Không tạo được hóa đơn mẫu.', 1;
        SET @MaHD = CONVERT(int, SCOPE_IDENTITY());
        INSERT @MapHD VALUES (@i, @MaHD);

        INSERT dbo.CHITIETHOADON(MaHD, MaDT, SoLuong, DonGia, ThanhTien, MaKM)
        SELECT @MaHD, m.MaDT, c.SoLuong, d.GiaNiemYet,
               ROUND(CAST(d.GiaNiemYet AS DECIMAL(38,6)) * c.SoLuong * (100 - ISNULL(ct.PhanTramGiam, 0)) / 100, 0),
               km.MaKM
        FROM @CTHD c
        JOIN @MapDT m ON m.Ma = c.Sp
        JOIN dbo.DIENTHOAI d ON d.MaDT = m.MaDT
        LEFT JOIN dbo.KHUYENMAI km ON km.TenKM = c.TenKM
        LEFT JOIN dbo.CHITIETKHUYENMAI ct ON ct.MaKM = km.MaKM AND ct.MaDT = d.MaDT
        WHERE c.Stt = @i;

        -- Cộng điểm tích lũy theo tổng tiền hóa đơn (trigger đã đồng bộ TongTien)
        SELECT @Tong = TongTien, @MaKH = MaKH FROM dbo.HOADON WHERE MaHD = @MaHD;
        SET @Diem = CONVERT(int, FLOOR(@Tong / @SoTien));
        UPDATE dbo.KHACHHANG SET DiemTichLuy = DiemTichLuy + @Diem WHERE MaKH = @MaKH;
        SET @i += 1;
    END;

    -- 5. Cập nhật tồn kho một lần: + số nhập - số bán của các phiếu/hóa đơn vừa thêm
    EXEC sp_set_session_context N'cho_phep_sua_ton', 1;
    UPDATE d SET SoLuongTon = d.SoLuongTon + x.Delta
    FROM dbo.DIENTHOAI d
    JOIN (SELECT MaDT, SUM(Delta) AS Delta FROM (
              SELECT m.MaDT, l.SoLuong AS Delta FROM @CTPN l JOIN @MapDT m ON m.Ma = l.Sp
              UNION ALL
              SELECT m.MaDT, -c.SoLuong FROM @CTHD c JOIN @MapDT m ON m.Ma = c.Sp) t
          GROUP BY MaDT) x ON x.MaDT = d.MaDT;
    EXEC sp_set_session_context N'cho_phep_sua_ton', 0;

    -- 6. Bảo hành (cặp hóa đơn, điện thoại có trong chi tiết hóa đơn; số phiếu không vượt số máy đã mua)
    ;WITH b AS (SELECT ROW_NUMBER() OVER (ORDER BY x.HD, x.Sp) AS rn, x.* FROM @BH x)
    INSERT dbo.BAOHANH(SoIMEI, NgayBatDau, NgayKetThuc, TrangThai, GhiChu, MaHD, MaDT)
    SELECT '35880' + RIGHT('0000000000' + CONVERT(varchar(10), b.rn), 10),
           CONVERT(date, h.NgayLap), DATEADD(month, @Thang, CONVERT(date, h.NgayLap)),
           b.TrangThai, b.GhiChu, mh.MaHD, md.MaDT
    FROM b
    JOIN @MapHD mh ON mh.Stt = b.HD
    JOIN dbo.HOADON h ON h.MaHD = mh.MaHD
    JOIN @MapDT md ON md.Ma = b.Sp;

    -- 7. Giỏ hàng
    INSERT dbo.GIOHANG(MaKH, MaDT, SoLuong, NgayThem)
    SELECT k.MaKH, m.MaDT, g.SoLuong, g.NgayThem FROM @GH g
    JOIN @MapKH k ON k.Ma = g.KH
    JOIN @MapDT m ON m.Ma = g.Sp
    WHERE NOT EXISTS (SELECT 1 FROM dbo.GIOHANG x WHERE x.MaKH = k.MaKH AND x.MaDT = m.MaDT);

    -- ===================== KIỂM TRA TRƯỚC KHI LƯU =====================
    DECLARE @KQ TABLE (Bang sysname, SoDong int, YeuCau int);
    INSERT @KQ
    SELECT N'DIENTHOAI', COUNT(*), 10 FROM dbo.DIENTHOAI
    UNION ALL SELECT N'KHACHHANG', COUNT(*), 10 FROM dbo.KHACHHANG
    UNION ALL SELECT N'NHANVIEN', COUNT(*), 10 FROM dbo.NHANVIEN
    UNION ALL SELECT N'HANGDT', COUNT(*), 10 FROM dbo.HANGDT
    UNION ALL SELECT N'LOAIDIENTHOAI', COUNT(*), 10 FROM dbo.LOAIDIENTHOAI
    UNION ALL SELECT N'NHACUNGCAP', COUNT(*), 10 FROM dbo.NHACUNGCAP
    UNION ALL SELECT N'KHUYENMAI', COUNT(*), 10 FROM dbo.KHUYENMAI
    UNION ALL SELECT N'CHITIETKHUYENMAI', COUNT(*), 10 FROM dbo.CHITIETKHUYENMAI
    UNION ALL SELECT N'TAIKHOAN', COUNT(*), 10 FROM dbo.TAIKHOAN
    UNION ALL SELECT N'BAOHANH', COUNT(*), 10 FROM dbo.BAOHANH
    UNION ALL SELECT N'GIOHANG', COUNT(*), 10 FROM dbo.GIOHANG
    UNION ALL SELECT N'HOADON', COUNT(*), 20 FROM dbo.HOADON
    UNION ALL SELECT N'PHIEUNHAPHANG', COUNT(*), 20 FROM dbo.PHIEUNHAPHANG
    UNION ALL SELECT N'CHITIETHOADON', COUNT(*), 30 FROM dbo.CHITIETHOADON
    UNION ALL SELECT N'CHITIETPHIEUNHAPHANG', COUNT(*), 30 FROM dbo.CHITIETPHIEUNHAPHANG;
    IF EXISTS (SELECT 1 FROM @KQ WHERE SoDong < YeuCau)
        THROW 50299, N'Có bảng chưa đủ số dòng yêu cầu. Toàn bộ thay đổi đã được hoàn tác.', 1;

    IF OBJECT_ID(N'dbo.vw_DoiSoatTongTien', N'V') IS NOT NULL
    BEGIN
        DECLARE @Lech int;
        EXEC sp_executesql N'SELECT @x = COUNT(*) FROM dbo.vw_DoiSoatTongTien', N'@x int OUTPUT', @Lech OUTPUT;
        IF @Lech > 0 THROW 50298, N'Có hóa đơn hoặc phiếu nhập có TongTien lệch tổng chi tiết. Đã hoàn tác.', 1;
    END;

    COMMIT TRANSACTION;

    SELECT Bang, SoDong, YeuCau, CASE WHEN SoDong >= YeuCau THEN N'Đạt' ELSE N'Thiếu' END AS KetQua
    FROM @KQ ORDER BY YeuCau DESC, Bang;
END TRY
BEGIN CATCH
    IF XACT_STATE() <> 0 ROLLBACK TRANSACTION;
    EXEC sp_set_session_context N'cho_phep_sua_ton', 0;
    THROW;
END CATCH;
GO
