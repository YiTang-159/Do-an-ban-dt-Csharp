-- =====================================================================
--  DỮ LIỆU MẪU: THÊM 20 DÒNG MỚI CHO MỖI ĐỐI TƯỢNG (QuanLyBanDienThoai)
--  Chạy SAU Database/QuanLyBanDienThoai.sql (và TV2_BoSungRangBuoc.sql).
--  - Không xóa, không sửa dữ liệu cũ; chạy lần 2 sẽ tự dừng (không nhân đôi).
--  - Tồn kho và điểm tích lũy được tính lại từ phiếu nhập / hóa đơn cho khớp.
--  - Mật khẩu tài khoản mới: nhân viên = nv123456, khách hàng = kh123456.
--  - Một giao dịch duy nhất: lỗi ở đâu thì hoàn tác toàn bộ.
-- =====================================================================
USE [QuanLyBanDienThoai];
GO
SET NOCOUNT ON;
SET XACT_ABORT ON;
SET ANSI_NULLS ON;
SET ANSI_PADDING ON;
SET ANSI_WARNINGS ON;
SET ARITHABORT ON;
SET CONCAT_NULL_YIELDS_NULL ON;
SET QUOTED_IDENTIFIER ON;
SET NUMERIC_ROUNDABORT OFF;
GO
IF EXISTS (SELECT 1 FROM dbo.KHACHHANG WHERE SDT = '0765001001')
BEGIN
    PRINT N'Dữ liệu mẫu 20 dòng đã có sẵn, không thêm lại.';
    RETURN;
END;
BEGIN TRY
BEGIN TRANSACTION;
DECLARE @dt1 int, @dt2 int, @dt3 int, @dt4 int, @dt5 int, @dt6 int, @dt7 int, @dt8 int, @dt9 int, @dt10 int, @dt11 int, @dt12 int, @dt13 int, @dt14 int, @dt15 int, @dt16 int, @dt17 int, @dt18 int, @dt19 int, @dt20 int;
DECLARE @kh1 int, @kh2 int, @kh3 int, @kh4 int, @kh5 int, @kh6 int, @kh7 int, @kh8 int, @kh9 int, @kh10 int, @kh11 int, @kh12 int, @kh13 int, @kh14 int, @kh15 int, @kh16 int, @kh17 int, @kh18 int, @kh19 int, @kh20 int;
DECLARE @nv1 int, @nv2 int, @nv3 int, @nv4 int, @nv5 int, @nv6 int, @nv7 int, @nv8 int, @nv9 int, @nv10 int, @nv11 int, @nv12 int, @nv13 int, @nv14 int, @nv15 int, @nv16 int, @nv17 int, @nv18 int, @nv19 int, @nv20 int;
DECLARE @ncc1 int, @ncc2 int, @ncc3 int, @ncc4 int, @ncc5 int, @ncc6 int, @ncc7 int, @ncc8 int, @ncc9 int, @ncc10 int, @ncc11 int, @ncc12 int, @ncc13 int, @ncc14 int, @ncc15 int, @ncc16 int, @ncc17 int, @ncc18 int, @ncc19 int, @ncc20 int;
DECLARE @km1 int, @km2 int, @km3 int, @km4 int, @km5 int, @km6 int, @km7 int, @km8 int, @km9 int, @km10 int, @km11 int, @km12 int, @km13 int, @km14 int, @km15 int, @km16 int, @km17 int, @km18 int, @km19 int, @km20 int;
DECLARE @pn int, @hd int, @soTien decimal(18,2);
SELECT @soTien = GiaTri FROM dbo.THAMSO WHERE TenThamSo = 'SoTienMotDiem' AND GiaTri >= 1 AND GiaTri = FLOOR(GiaTri);
SET @soTien = COALESCE(@soTien, 100000);

-- 1. LOAIDIENTHOAI (20)
INSERT dbo.LOAIDIENTHOAI(TenLoai, MoTa) VALUES
(N'Điện thoại cao cấp', N'Flagship, cấu hình và camera hàng đầu'),
(N'Điện thoại tầm trung', N'Cân bằng giữa giá và hiệu năng'),
(N'Điện thoại giá rẻ', N'Dưới 6 triệu, đáp ứng nhu cầu cơ bản'),
(N'Điện thoại gập màn hình', N'Màn hình gập trong, mở ra như máy tính bảng'),
(N'Điện thoại gập vỏ sò', N'Thiết kế gập dọc nhỏ gọn'),
(N'Điện thoại chuyên chụp ảnh', N'Cụm camera lớn, xử lý ảnh mạnh'),
(N'Điện thoại pin trâu', N'Pin từ 5000 mAh, dùng nhiều ngày'),
(N'Điện thoại 5G', N'Hỗ trợ mạng 5G'),
(N'Điện thoại chống nước bụi', N'Chuẩn kháng nước IP67 trở lên'),
(N'Điện thoại siêu bền', N'Khung và kính chịu va đập'),
(N'Điện thoại cho người cao tuổi', N'Chữ lớn, loa to, nút SOS'),
(N'Điện thoại học sinh sinh viên', N'Giá mềm, đủ dùng học tập'),
(N'Điện thoại doanh nhân', N'Bảo mật cao, hai SIM'),
(N'Điện thoại selfie', N'Camera trước độ phân giải cao'),
(N'Điện thoại nhỏ gọn', N'Màn hình dưới 6 inch'),
(N'Điện thoại gaming cao cấp', N'Tản nhiệt, màn hình 120Hz trở lên'),
(N'Điện thoại gaming tầm trung', N'Chip chơi game, giá dễ tiếp cận'),
(N'Điện thoại hai màn hình', N'Có màn hình phụ mặt sau'),
(N'Điện thoại màn hình lớn', N'Màn hình từ 6.8 inch'),
(N'Điện thoại trẻ em', N'Có chế độ giám sát của phụ huynh');

-- 2. HANGDT (20)
INSERT dbo.HANGDT(TenHang, QuocGia) VALUES
(N'Realme', N'Trung Quốc'),
(N'Vivo', N'Trung Quốc'),
(N'Nokia', N'Phần Lan'),
(N'Sony', N'Nhật Bản'),
(N'Google', N'Hoa Kỳ'),
(N'Motorola', N'Hoa Kỳ'),
(N'Huawei', N'Trung Quốc'),
(N'Honor', N'Trung Quốc'),
(N'OnePlus', N'Trung Quốc'),
(N'ASUS', N'Đài Loan'),
(N'Tecno', N'Trung Quốc'),
(N'Infinix', N'Hồng Kông'),
(N'ZTE', N'Trung Quốc'),
(N'Nothing', N'Anh'),
(N'Lenovo', N'Trung Quốc'),
(N'Meizu', N'Trung Quốc'),
(N'VinSmart', N'Việt Nam'),
(N'Itel', N'Trung Quốc'),
(N'BlackBerry', N'Canada'),
(N'Nubia', N'Trung Quốc');

-- 3. NHACUNGCAP (20)
INSERT dbo.NHACUNGCAP(TenNCC, DiaChi, SDT, Email) VALUES
(N'Công ty TNHH Phân phối Di Động Sài Gòn', N'12 Nguyễn Huệ, Quận 1, TP.HCM', '0283001001', 'lienhe01@tnhhphanphoidi.vn'),
(N'Công ty CP Viễn Thông Miền Nam', N'45 Lê Lợi, Quận 1, TP.HCM', '0283001002', 'lienhe02@cpvienthongmien.vn'),
(N'Công ty TNHH Thương mại Điện tử Á Châu', N'88 Cách Mạng Tháng Tám, Quận 3, TP.HCM', '0283001003', 'lienhe03@tnhhthuongmaidi.vn'),
(N'Công ty CP Công nghệ Phương Nam', N'27 Võ Văn Tần, Quận 3, TP.HCM', '0283001004', 'lienhe04@cpcongnghephuon.vn'),
(N'Công ty TNHH Mobile Pro Việt Nam', N'310 Điện Biên Phủ, Quận Bình Thạnh, TP.HCM', '0283001005', 'lienhe05@tnhhmobileprovi.vn'),
(N'Công ty CP Phân phối Kỹ thuật số Thành Công', N'15 Trường Chinh, Quận Tân Bình, TP.HCM', '0283001006', 'lienhe06@cpphanphoikyth.vn'),
(N'Công ty TNHH Điện máy Hoàng Gia', N'201 Nguyễn Văn Linh, Quận 7, TP.HCM', '0283001007', 'lienhe07@tnhhdienmayhoan.vn'),
(N'Công ty CP Đầu tư Công nghệ Minh Long', N'76 Phạm Văn Đồng, TP. Thủ Đức, TP.HCM', '0283001008', 'lienhe08@cpdautucongngh.vn'),
(N'Công ty TNHH Viễn Thông Đại Phát', N'9 Nguyễn Thị Minh Khai, Quận 1, TP.HCM', '0283001009', 'lienhe09@tnhhvienthongda.vn'),
(N'Công ty CP Thiết bị Di động Bình Minh', N'132 Lý Thường Kiệt, Quận 10, TP.HCM', '0283001010', 'lienhe10@cpthietbididon.vn'),
(N'Công ty TNHH Nhập khẩu Phụ kiện Tín Nghĩa', N'58 Hoàng Văn Thụ, Quận Phú Nhuận, TP.HCM', '0283001011', 'lienhe11@tnhhnhapkhauphu.vn'),
(N'Công ty CP Công nghệ Số Việt Tiến', N'24 Láng Hạ, Quận Đống Đa, Hà Nội', '0283001012', 'lienhe12@cpcongnghesovi.vn'),
(N'Công ty TNHH Thương mại Hải Phòng Mobile', N'33 Lạch Tray, Quận Ngô Quyền, Hải Phòng', '0283001013', 'lienhe13@tnhhthuongmaiha.vn'),
(N'Công ty CP Phân phối Đà Nẵng Tech', N'101 Nguyễn Văn Linh, Quận Hải Châu, Đà Nẵng', '0283001014', 'lienhe14@cpphanphoidana.vn'),
(N'Công ty TNHH Cần Thơ Digital', N'19 Trần Hưng Đạo, Quận Ninh Kiều, Cần Thơ', '0283001015', 'lienhe15@tnhhcanthodigit.vn'),
(N'Công ty CP Biên Hòa Mobile', N'66 Phạm Văn Thuận, TP. Biên Hòa, Đồng Nai', '0283001016', 'lienhe16@cpbienhoamobile.vn'),
(N'Công ty TNHH Dương Gia Telecom', N'5 Hùng Vương, TP. Nha Trang, Khánh Hòa', '0283001017', 'lienhe17@tnhhduonggiatel.vn'),
(N'Công ty CP Bảo An Công nghệ', N'92 Hai Bà Trưng, Quận 1, TP.HCM', '0283001018', 'lienhe18@cpbaoancongngh.vn'),
(N'Công ty TNHH Kim Long Mobile', N'40 Nguyễn Trãi, Quận 5, TP.HCM', '0283001019', 'lienhe19@tnhhkimlongmobi.vn'),
(N'Công ty CP Tân Thế Giới Di Động', N'180 Cộng Hòa, Quận Tân Bình, TP.HCM', '0283001020', 'lienhe20@cptanthegioidi.vn');
SELECT @ncc1 = MaNCC FROM dbo.NHACUNGCAP WHERE SDT = '0283001001';
SELECT @ncc2 = MaNCC FROM dbo.NHACUNGCAP WHERE SDT = '0283001002';
SELECT @ncc3 = MaNCC FROM dbo.NHACUNGCAP WHERE SDT = '0283001003';
SELECT @ncc4 = MaNCC FROM dbo.NHACUNGCAP WHERE SDT = '0283001004';
SELECT @ncc5 = MaNCC FROM dbo.NHACUNGCAP WHERE SDT = '0283001005';
SELECT @ncc6 = MaNCC FROM dbo.NHACUNGCAP WHERE SDT = '0283001006';
SELECT @ncc7 = MaNCC FROM dbo.NHACUNGCAP WHERE SDT = '0283001007';
SELECT @ncc8 = MaNCC FROM dbo.NHACUNGCAP WHERE SDT = '0283001008';
SELECT @ncc9 = MaNCC FROM dbo.NHACUNGCAP WHERE SDT = '0283001009';
SELECT @ncc10 = MaNCC FROM dbo.NHACUNGCAP WHERE SDT = '0283001010';
SELECT @ncc11 = MaNCC FROM dbo.NHACUNGCAP WHERE SDT = '0283001011';
SELECT @ncc12 = MaNCC FROM dbo.NHACUNGCAP WHERE SDT = '0283001012';
SELECT @ncc13 = MaNCC FROM dbo.NHACUNGCAP WHERE SDT = '0283001013';
SELECT @ncc14 = MaNCC FROM dbo.NHACUNGCAP WHERE SDT = '0283001014';
SELECT @ncc15 = MaNCC FROM dbo.NHACUNGCAP WHERE SDT = '0283001015';
SELECT @ncc16 = MaNCC FROM dbo.NHACUNGCAP WHERE SDT = '0283001016';
SELECT @ncc17 = MaNCC FROM dbo.NHACUNGCAP WHERE SDT = '0283001017';
SELECT @ncc18 = MaNCC FROM dbo.NHACUNGCAP WHERE SDT = '0283001018';
SELECT @ncc19 = MaNCC FROM dbo.NHACUNGCAP WHERE SDT = '0283001019';
SELECT @ncc20 = MaNCC FROM dbo.NHACUNGCAP WHERE SDT = '0283001020';

-- 4. DIENTHOAI (20) - SoLuongTon được tính lại ở cuối script
INSERT dbo.DIENTHOAI(TenDT, HeDieuHanh, NamRaMat, RAM, BoNho, MauSac, GiaNiemYet, SoLuongTon, MoTa, MaHang, MaLoai) VALUES
(N'Realme 12 Pro+ 5G', N'Android', 2024, 12, 256, N'Xanh Navy', 12990000, 0, N'Camera tele periscope 64MP', (SELECT MaHang FROM dbo.HANGDT WHERE TenHang = N'Realme'), (SELECT MaLoai FROM dbo.LOAIDIENTHOAI WHERE TenLoai = N'Điện thoại chuyên chụp ảnh')),
(N'Realme C67', N'Android', 2023, 8, 128, N'Xanh Lá', 4990000, 0, N'Pin 5000 mAh sạc nhanh 33W', (SELECT MaHang FROM dbo.HANGDT WHERE TenHang = N'Realme'), (SELECT MaLoai FROM dbo.LOAIDIENTHOAI WHERE TenLoai = N'Điện thoại giá rẻ')),
(N'Vivo V30', N'Android', 2024, 12, 256, N'Tím Lavender', 11990000, 0, N'Camera selfie 50MP lấy nét tự động', (SELECT MaHang FROM dbo.HANGDT WHERE TenHang = N'Vivo'), (SELECT MaLoai FROM dbo.LOAIDIENTHOAI WHERE TenLoai = N'Điện thoại selfie')),
(N'Vivo Y36', N'Android', 2023, 8, 128, N'Đen', 5490000, 0, N'Pin 5000 mAh, sạc 44W', (SELECT MaHang FROM dbo.HANGDT WHERE TenHang = N'Vivo'), (SELECT MaLoai FROM dbo.LOAIDIENTHOAI WHERE TenLoai = N'Điện thoại pin trâu')),
(N'Nokia G42 5G', N'Android', 2023, 6, 128, N'Hồng', 5290000, 0, N'Dễ sửa chữa, cập nhật Android 3 năm', (SELECT MaHang FROM dbo.HANGDT WHERE TenHang = N'Nokia'), (SELECT MaLoai FROM dbo.LOAIDIENTHOAI WHERE TenLoai = N'Điện thoại 5G')),
(N'Nokia XR21', N'Android', 2023, 6, 128, N'Đen', 9990000, 0, N'Chuẩn quân đội MIL-STD-810H', (SELECT MaHang FROM dbo.HANGDT WHERE TenHang = N'Nokia'), (SELECT MaLoai FROM dbo.LOAIDIENTHOAI WHERE TenLoai = N'Điện thoại siêu bền')),
(N'Sony Xperia 1 V', N'Android', 2023, 12, 256, N'Đen', 31990000, 0, N'Màn hình 4K OLED 120Hz', (SELECT MaHang FROM dbo.HANGDT WHERE TenHang = N'Sony'), (SELECT MaLoai FROM dbo.LOAIDIENTHOAI WHERE TenLoai = N'Điện thoại cao cấp')),
(N'Sony Xperia 10 V', N'Android', 2023, 8, 128, N'Xanh Lá', 10990000, 0, N'Nhẹ 159g, pin hai ngày', (SELECT MaHang FROM dbo.HANGDT WHERE TenHang = N'Sony'), (SELECT MaLoai FROM dbo.LOAIDIENTHOAI WHERE TenLoai = N'Điện thoại tầm trung')),
(N'Google Pixel 8', N'Android', 2023, 8, 128, N'Đen Obsidian', 17990000, 0, N'Chip Tensor G3, 7 năm cập nhật', (SELECT MaHang FROM dbo.HANGDT WHERE TenHang = N'Google'), (SELECT MaLoai FROM dbo.LOAIDIENTHOAI WHERE TenLoai = N'Điện thoại chuyên chụp ảnh')),
(N'Google Pixel 8 Pro', N'Android', 2023, 12, 256, N'Xanh Bay', 24990000, 0, N'Camera 50MP, tính năng AI chỉnh ảnh', (SELECT MaHang FROM dbo.HANGDT WHERE TenHang = N'Google'), (SELECT MaLoai FROM dbo.LOAIDIENTHOAI WHERE TenLoai = N'Điện thoại cao cấp')),
(N'Motorola Edge 40 Neo', N'Android', 2023, 12, 256, N'Xanh Dương', 8990000, 0, N'Màn hình pOLED 144Hz', (SELECT MaHang FROM dbo.HANGDT WHERE TenHang = N'Motorola'), (SELECT MaLoai FROM dbo.LOAIDIENTHOAI WHERE TenLoai = N'Điện thoại tầm trung')),
(N'Huawei Pura 70 Pro', N'HarmonyOS', 2024, 12, 512, N'Trắng', 26990000, 0, N'Camera XMAGE, zoom quang học', (SELECT MaHang FROM dbo.HANGDT WHERE TenHang = N'Huawei'), (SELECT MaLoai FROM dbo.LOAIDIENTHOAI WHERE TenLoai = N'Điện thoại chuyên chụp ảnh')),
(N'Honor Magic6 Pro', N'Android', 2024, 12, 512, N'Đen', 28990000, 0, N'Pin silicon-carbon 5600 mAh', (SELECT MaHang FROM dbo.HANGDT WHERE TenHang = N'Honor'), (SELECT MaLoai FROM dbo.LOAIDIENTHOAI WHERE TenLoai = N'Điện thoại cao cấp')),
(N'Honor X8b', N'Android', 2024, 8, 256, N'Bạc', 6990000, 0, N'Mỏng 7.45 mm, sạc 35W', (SELECT MaHang FROM dbo.HANGDT WHERE TenHang = N'Honor'), (SELECT MaLoai FROM dbo.LOAIDIENTHOAI WHERE TenLoai = N'Điện thoại tầm trung')),
(N'OnePlus 12', N'Android', 2024, 16, 512, N'Xanh Lục', 22990000, 0, N'Sạc 100W, Snapdragon 8 Gen 3', (SELECT MaHang FROM dbo.HANGDT WHERE TenHang = N'OnePlus'), (SELECT MaLoai FROM dbo.LOAIDIENTHOAI WHERE TenLoai = N'Điện thoại cao cấp')),
(N'ASUS ROG Phone 8', N'Android', 2024, 16, 512, N'Đen', 26990000, 0, N'Màn hình 165Hz, quạt tản nhiệt rời', (SELECT MaHang FROM dbo.HANGDT WHERE TenHang = N'ASUS'), (SELECT MaLoai FROM dbo.LOAIDIENTHOAI WHERE TenLoai = N'Điện thoại gaming cao cấp')),
(N'Tecno Camon 30', N'Android', 2024, 8, 256, N'Xanh Lam', 6490000, 0, N'Camera RGBW 50MP', (SELECT MaHang FROM dbo.HANGDT WHERE TenHang = N'Tecno'), (SELECT MaLoai FROM dbo.LOAIDIENTHOAI WHERE TenLoai = N'Điện thoại selfie')),
(N'Infinix Note 40 Pro', N'Android', 2024, 8, 256, N'Vàng', 5990000, 0, N'Sạc không dây 20W, sạc nhanh 45W', (SELECT MaHang FROM dbo.HANGDT WHERE TenHang = N'Infinix'), (SELECT MaLoai FROM dbo.LOAIDIENTHOAI WHERE TenLoai = N'Điện thoại pin trâu')),
(N'Nothing Phone (2a)', N'Android', 2024, 8, 128, N'Trắng', 8490000, 0, N'Đèn Glyph Interface độc đáo', (SELECT MaHang FROM dbo.HANGDT WHERE TenHang = N'Nothing'), (SELECT MaLoai FROM dbo.LOAIDIENTHOAI WHERE TenLoai = N'Điện thoại tầm trung')),
(N'Nubia Red Magic 9 Pro', N'Android', 2023, 16, 512, N'Đen', 24990000, 0, N'Camera ẩn dưới màn hình, làm mát chủ động', (SELECT MaHang FROM dbo.HANGDT WHERE TenHang = N'Nubia'), (SELECT MaLoai FROM dbo.LOAIDIENTHOAI WHERE TenLoai = N'Điện thoại gaming cao cấp'));
SELECT @dt1 = MaDT FROM dbo.DIENTHOAI WHERE TenDT = N'Realme 12 Pro+ 5G' AND RAM = 12 AND BoNho = 256 AND MauSac = N'Xanh Navy';
SELECT @dt2 = MaDT FROM dbo.DIENTHOAI WHERE TenDT = N'Realme C67' AND RAM = 8 AND BoNho = 128 AND MauSac = N'Xanh Lá';
SELECT @dt3 = MaDT FROM dbo.DIENTHOAI WHERE TenDT = N'Vivo V30' AND RAM = 12 AND BoNho = 256 AND MauSac = N'Tím Lavender';
SELECT @dt4 = MaDT FROM dbo.DIENTHOAI WHERE TenDT = N'Vivo Y36' AND RAM = 8 AND BoNho = 128 AND MauSac = N'Đen';
SELECT @dt5 = MaDT FROM dbo.DIENTHOAI WHERE TenDT = N'Nokia G42 5G' AND RAM = 6 AND BoNho = 128 AND MauSac = N'Hồng';
SELECT @dt6 = MaDT FROM dbo.DIENTHOAI WHERE TenDT = N'Nokia XR21' AND RAM = 6 AND BoNho = 128 AND MauSac = N'Đen';
SELECT @dt7 = MaDT FROM dbo.DIENTHOAI WHERE TenDT = N'Sony Xperia 1 V' AND RAM = 12 AND BoNho = 256 AND MauSac = N'Đen';
SELECT @dt8 = MaDT FROM dbo.DIENTHOAI WHERE TenDT = N'Sony Xperia 10 V' AND RAM = 8 AND BoNho = 128 AND MauSac = N'Xanh Lá';
SELECT @dt9 = MaDT FROM dbo.DIENTHOAI WHERE TenDT = N'Google Pixel 8' AND RAM = 8 AND BoNho = 128 AND MauSac = N'Đen Obsidian';
SELECT @dt10 = MaDT FROM dbo.DIENTHOAI WHERE TenDT = N'Google Pixel 8 Pro' AND RAM = 12 AND BoNho = 256 AND MauSac = N'Xanh Bay';
SELECT @dt11 = MaDT FROM dbo.DIENTHOAI WHERE TenDT = N'Motorola Edge 40 Neo' AND RAM = 12 AND BoNho = 256 AND MauSac = N'Xanh Dương';
SELECT @dt12 = MaDT FROM dbo.DIENTHOAI WHERE TenDT = N'Huawei Pura 70 Pro' AND RAM = 12 AND BoNho = 512 AND MauSac = N'Trắng';
SELECT @dt13 = MaDT FROM dbo.DIENTHOAI WHERE TenDT = N'Honor Magic6 Pro' AND RAM = 12 AND BoNho = 512 AND MauSac = N'Đen';
SELECT @dt14 = MaDT FROM dbo.DIENTHOAI WHERE TenDT = N'Honor X8b' AND RAM = 8 AND BoNho = 256 AND MauSac = N'Bạc';
SELECT @dt15 = MaDT FROM dbo.DIENTHOAI WHERE TenDT = N'OnePlus 12' AND RAM = 16 AND BoNho = 512 AND MauSac = N'Xanh Lục';
SELECT @dt16 = MaDT FROM dbo.DIENTHOAI WHERE TenDT = N'ASUS ROG Phone 8' AND RAM = 16 AND BoNho = 512 AND MauSac = N'Đen';
SELECT @dt17 = MaDT FROM dbo.DIENTHOAI WHERE TenDT = N'Tecno Camon 30' AND RAM = 8 AND BoNho = 256 AND MauSac = N'Xanh Lam';
SELECT @dt18 = MaDT FROM dbo.DIENTHOAI WHERE TenDT = N'Infinix Note 40 Pro' AND RAM = 8 AND BoNho = 256 AND MauSac = N'Vàng';
SELECT @dt19 = MaDT FROM dbo.DIENTHOAI WHERE TenDT = N'Nothing Phone (2a)' AND RAM = 8 AND BoNho = 128 AND MauSac = N'Trắng';
SELECT @dt20 = MaDT FROM dbo.DIENTHOAI WHERE TenDT = N'Nubia Red Magic 9 Pro' AND RAM = 16 AND BoNho = 512 AND MauSac = N'Đen';

-- 5. NHANVIEN (20)
INSERT dbo.NHANVIEN(Ho, Ten, GioiTinh, NgaySinh, SDT, Email, ChucVu, NgayVaoLam) VALUES
(N'Nguyễn Thị', N'Hoa', N'Nữ', '19970214', '0382001001', 'hoan01@phonestore.vn', N'Nhân viên bán hàng', '20230301'),
(N'Trần Minh', N'Quân', N'Nam', '19950709', '0382001002', 'quant02@phonestore.vn', N'Nhân viên bán hàng', '20221115'),
(N'Lê Thị', N'Thảo', N'Nữ', '19991201', '0382001003', 'thaol03@phonestore.vn', N'Thu ngân', '20240108'),
(N'Phạm Quốc', N'Huy', N'Nam', '19940521', '0382001004', 'huyp04@phonestore.vn', N'Nhân viên kho', '20220620'),
(N'Võ Thanh', N'Tùng', N'Nam', '19980930', '0382001005', 'tungv05@phonestore.vn', N'Nhân viên bán hàng', '20230814'),
(N'Đặng Ngọc', N'Lan', N'Nữ', '20000317', '0382001006', 'land06@phonestore.vn', N'Nhân viên bán hàng', '20240219'),
(N'Bùi Văn', N'Khoa', N'Nam', '19961105', '0382001007', 'khoab07@phonestore.vn', N'Kỹ thuật viên bảo hành', '20230502'),
(N'Hoàng Thị', N'Yến', N'Nữ', '20010625', '0382001008', 'yenh08@phonestore.vn', N'Thu ngân', '20250106'),
(N'Ngô Đức', N'Anh', N'Nam', '19930112', '0382001009', 'anhn09@phonestore.vn', N'Quản lý kho', '20210913'),
(N'Dương Thị', N'Ngọc', N'Nữ', '19980408', '0382001010', 'ngocd10@phonestore.vn', N'Chăm sóc khách hàng', '20240422'),
(N'Lý Gia', N'Bảo', N'Nam', '20020819', '0382001011', 'baol11@phonestore.vn', N'Nhân viên bán hàng', '20250310'),
(N'Đỗ Thị', N'Phương', N'Nữ', '19971027', '0382001012', 'phuongd12@phonestore.vn', N'Kế toán', '20221205'),
(N'Huỳnh Minh', N'Trí', N'Nam', '19990202', '0382001013', 'trih13@phonestore.vn', N'Nhân viên bán hàng', '20240617'),
(N'Phan Thị', N'Kim', N'Nữ', '20001212', '0382001014', 'kimp14@phonestore.vn', N'Nhân viên bán hàng', '20250526'),
(N'Vũ Hoàng', N'Nam', N'Nam', '19920707', '0382001015', 'namv15@phonestore.vn', N'Trưởng ca', '20210215'),
(N'Tạ Thu', N'Hà', N'Nữ', '20010130', '0382001016', 'hat16@phonestore.vn', N'Chăm sóc khách hàng', '20250714'),
(N'Mai Xuân', N'Sơn', N'Nam', '19960303', '0382001017', 'sonm17@phonestore.vn', N'Kỹ thuật viên bảo hành', '20231009'),
(N'Trương Thị', N'Linh', N'Nữ', '19980815', '0382001018', 'linht18@phonestore.vn', N'Thu ngân', '20240902'),
(N'Cao Văn', N'Đạt', N'Nam', '20030511', '0382001019', 'datc19@phonestore.vn', N'Nhân viên kho', '20250901'),
(N'Lâm Bảo', N'Châu', N'Nữ', '19990909', '0382001020', 'chaul20@phonestore.vn', N'Nhân viên bán hàng', '20251103');
SELECT @nv1 = MaNV FROM dbo.NHANVIEN WHERE SDT = '0382001001';
SELECT @nv2 = MaNV FROM dbo.NHANVIEN WHERE SDT = '0382001002';
SELECT @nv3 = MaNV FROM dbo.NHANVIEN WHERE SDT = '0382001003';
SELECT @nv4 = MaNV FROM dbo.NHANVIEN WHERE SDT = '0382001004';
SELECT @nv5 = MaNV FROM dbo.NHANVIEN WHERE SDT = '0382001005';
SELECT @nv6 = MaNV FROM dbo.NHANVIEN WHERE SDT = '0382001006';
SELECT @nv7 = MaNV FROM dbo.NHANVIEN WHERE SDT = '0382001007';
SELECT @nv8 = MaNV FROM dbo.NHANVIEN WHERE SDT = '0382001008';
SELECT @nv9 = MaNV FROM dbo.NHANVIEN WHERE SDT = '0382001009';
SELECT @nv10 = MaNV FROM dbo.NHANVIEN WHERE SDT = '0382001010';
SELECT @nv11 = MaNV FROM dbo.NHANVIEN WHERE SDT = '0382001011';
SELECT @nv12 = MaNV FROM dbo.NHANVIEN WHERE SDT = '0382001012';
SELECT @nv13 = MaNV FROM dbo.NHANVIEN WHERE SDT = '0382001013';
SELECT @nv14 = MaNV FROM dbo.NHANVIEN WHERE SDT = '0382001014';
SELECT @nv15 = MaNV FROM dbo.NHANVIEN WHERE SDT = '0382001015';
SELECT @nv16 = MaNV FROM dbo.NHANVIEN WHERE SDT = '0382001016';
SELECT @nv17 = MaNV FROM dbo.NHANVIEN WHERE SDT = '0382001017';
SELECT @nv18 = MaNV FROM dbo.NHANVIEN WHERE SDT = '0382001018';
SELECT @nv19 = MaNV FROM dbo.NHANVIEN WHERE SDT = '0382001019';
SELECT @nv20 = MaNV FROM dbo.NHANVIEN WHERE SDT = '0382001020';

-- 6. KHACHHANG (20) - DiemTichLuy được tính lại ở cuối script
INSERT dbo.KHACHHANG(Ho, Ten, SDT, Email, DiaChi, NgaySinh, DiemTichLuy) VALUES
(N'Nguyễn Hoàng', N'Phúc', '0765001001', 'phucn01@gmail.com', N'15 Nguyễn Trãi, Quận 5, TP.HCM', '19900418', 0),
(N'Trần Thị', N'Hạnh', '0765001002', 'hanht02@gmail.com', N'72 Lê Văn Sỹ, Quận 3, TP.HCM', '19940902', 0),
(N'Lê Quang', N'Vinh', '0765001003', 'vinhl03@gmail.com', N'8 Hoàng Hoa Thám, Quận Tân Bình, TP.HCM', '19881225', 0),
(N'Phạm Thị', N'Tuyết', '0765001004', 'tuyetp04@gmail.com', N'31 Phan Xích Long, Quận Phú Nhuận, TP.HCM', '19960614', 0),
(N'Võ Văn', N'Hùng', '0765001005', 'hungv05@gmail.com', N'120 Quang Trung, Quận Gò Vấp, TP.HCM', '19850330', 0),
(N'Đinh Thị', N'Mỹ', '0765001006', 'myd06@gmail.com', N'54 Nguyễn Thị Thập, Quận 7, TP.HCM', '19991111', 0),
(N'Bùi Anh', N'Tuấn', '0765001007', 'tuanb07@gmail.com', N'9 Xô Viết Nghệ Tĩnh, Quận Bình Thạnh, TP.HCM', '19920107', 0),
(N'Hồ Thị', N'Diễm', '0765001008', 'diemh08@gmail.com', N'200 Lạc Long Quân, Quận 11, TP.HCM', '19970823', 0),
(N'Phan Đức', N'Thịnh', '0765001009', 'thinhp09@gmail.com', N'17 Nguyễn Văn Cừ, Quận 5, TP.HCM', '19910505', 0),
(N'Lương Thị', N'Bích', '0765001010', 'bichl10@gmail.com', N'63 Tô Ký, Quận 12, TP.HCM', '20000228', 0),
(N'Châu Minh', N'Khang', '0765001011', 'khangc11@gmail.com', N'5 Đường 3 Tháng 2, Quận 10, TP.HCM', '19931016', 0),
(N'Đoàn Thị', N'Nhung', '0765001012', 'nhungd12@gmail.com', N'28 Kha Vạn Cân, TP. Thủ Đức, TP.HCM', '19980719', 0),
(N'Trịnh Văn', N'Long', '0765001013', 'longt13@gmail.com', N'91 Tân Kỳ Tân Quý, Quận Tân Phú, TP.HCM', '19871203', 0),
(N'Lại Thị', N'Trang', '0765001014', 'trangl14@gmail.com', N'45 Lũy Bán Bích, Quận Tân Phú, TP.HCM', '20010409', 0),
(N'Mạc Quốc', N'Bình', '0765001015', 'binhm15@gmail.com', N'12 Huỳnh Tấn Phát, Quận 7, TP.HCM', '19950927', 0),
(N'Kiều Thị', N'Oanh', '0765001016', 'oanhk16@gmail.com', N'76 Nơ Trang Long, Quận Bình Thạnh, TP.HCM', '19960121', 0),
(N'Từ Gia', N'Hưng', '0765001017', 'hungt17@gmail.com', N'3 Võ Thị Sáu, Quận 3, TP.HCM', '19890606', 0),
(N'Âu Thị', N'Thu', '0765001018', 'thua18@gmail.com', N'150 Bà Hạt, Quận 10, TP.HCM', '20020315', 0),
(N'Thái Văn', N'Lộc', '0765001019', 'loct19@gmail.com', N'22 Thống Nhất, TP. Thủ Đức, TP.HCM', '19941130', 0),
(N'Quách Thị', N'Vân', '0765001020', 'vanq20@gmail.com', N'68 Cây Trâm, Quận Gò Vấp, TP.HCM', '19990522', 0);
SELECT @kh1 = MaKH FROM dbo.KHACHHANG WHERE SDT = '0765001001';
SELECT @kh2 = MaKH FROM dbo.KHACHHANG WHERE SDT = '0765001002';
SELECT @kh3 = MaKH FROM dbo.KHACHHANG WHERE SDT = '0765001003';
SELECT @kh4 = MaKH FROM dbo.KHACHHANG WHERE SDT = '0765001004';
SELECT @kh5 = MaKH FROM dbo.KHACHHANG WHERE SDT = '0765001005';
SELECT @kh6 = MaKH FROM dbo.KHACHHANG WHERE SDT = '0765001006';
SELECT @kh7 = MaKH FROM dbo.KHACHHANG WHERE SDT = '0765001007';
SELECT @kh8 = MaKH FROM dbo.KHACHHANG WHERE SDT = '0765001008';
SELECT @kh9 = MaKH FROM dbo.KHACHHANG WHERE SDT = '0765001009';
SELECT @kh10 = MaKH FROM dbo.KHACHHANG WHERE SDT = '0765001010';
SELECT @kh11 = MaKH FROM dbo.KHACHHANG WHERE SDT = '0765001011';
SELECT @kh12 = MaKH FROM dbo.KHACHHANG WHERE SDT = '0765001012';
SELECT @kh13 = MaKH FROM dbo.KHACHHANG WHERE SDT = '0765001013';
SELECT @kh14 = MaKH FROM dbo.KHACHHANG WHERE SDT = '0765001014';
SELECT @kh15 = MaKH FROM dbo.KHACHHANG WHERE SDT = '0765001015';
SELECT @kh16 = MaKH FROM dbo.KHACHHANG WHERE SDT = '0765001016';
SELECT @kh17 = MaKH FROM dbo.KHACHHANG WHERE SDT = '0765001017';
SELECT @kh18 = MaKH FROM dbo.KHACHHANG WHERE SDT = '0765001018';
SELECT @kh19 = MaKH FROM dbo.KHACHHANG WHERE SDT = '0765001019';
SELECT @kh20 = MaKH FROM dbo.KHACHHANG WHERE SDT = '0765001020';

-- 7. TAIKHOAN (20: 10 nhân viên + 10 khách hàng)
INSERT dbo.TAIKHOAN(TenDangNhap, MatKhau, VaiTro, MaNV, MaKH, TrangThai) VALUES
('nv_hoa01', 'a5d21a2fa99d15d6c13d848008559574fd222b2a51415c72abf3bae221c41f71', N'NhanVien', @nv1, NULL, 1),
('nv_quan02', 'a5d21a2fa99d15d6c13d848008559574fd222b2a51415c72abf3bae221c41f71', N'NhanVien', @nv2, NULL, 1),
('nv_thao03', 'a5d21a2fa99d15d6c13d848008559574fd222b2a51415c72abf3bae221c41f71', N'NhanVien', @nv3, NULL, 1),
('nv_huy04', 'a5d21a2fa99d15d6c13d848008559574fd222b2a51415c72abf3bae221c41f71', N'NhanVien', @nv4, NULL, 1),
('nv_tung05', 'a5d21a2fa99d15d6c13d848008559574fd222b2a51415c72abf3bae221c41f71', N'NhanVien', @nv5, NULL, 1),
('nv_lan06', 'a5d21a2fa99d15d6c13d848008559574fd222b2a51415c72abf3bae221c41f71', N'NhanVien', @nv6, NULL, 1),
('nv_khoa07', 'a5d21a2fa99d15d6c13d848008559574fd222b2a51415c72abf3bae221c41f71', N'NhanVien', @nv7, NULL, 1),
('nv_yen08', 'a5d21a2fa99d15d6c13d848008559574fd222b2a51415c72abf3bae221c41f71', N'NhanVien', @nv8, NULL, 1),
('nv_anh09', 'a5d21a2fa99d15d6c13d848008559574fd222b2a51415c72abf3bae221c41f71', N'NhanVien', @nv9, NULL, 1),
('nv_ngoc10', 'a5d21a2fa99d15d6c13d848008559574fd222b2a51415c72abf3bae221c41f71', N'NhanVien', @nv10, NULL, 1),
('kh_phuc01', 'b9edb8982854b5e79a890577927de0c29af629a14dd8c1d326bab0056ad0a313', N'KhachHang', NULL, @kh1, 1),
('kh_hanh02', 'b9edb8982854b5e79a890577927de0c29af629a14dd8c1d326bab0056ad0a313', N'KhachHang', NULL, @kh2, 1),
('kh_vinh03', 'b9edb8982854b5e79a890577927de0c29af629a14dd8c1d326bab0056ad0a313', N'KhachHang', NULL, @kh3, 1),
('kh_tuyet04', 'b9edb8982854b5e79a890577927de0c29af629a14dd8c1d326bab0056ad0a313', N'KhachHang', NULL, @kh4, 1),
('kh_hung05', 'b9edb8982854b5e79a890577927de0c29af629a14dd8c1d326bab0056ad0a313', N'KhachHang', NULL, @kh5, 1),
('kh_my06', 'b9edb8982854b5e79a890577927de0c29af629a14dd8c1d326bab0056ad0a313', N'KhachHang', NULL, @kh6, 1),
('kh_tuan07', 'b9edb8982854b5e79a890577927de0c29af629a14dd8c1d326bab0056ad0a313', N'KhachHang', NULL, @kh7, 1),
('kh_diem08', 'b9edb8982854b5e79a890577927de0c29af629a14dd8c1d326bab0056ad0a313', N'KhachHang', NULL, @kh8, 1),
('kh_thinh09', 'b9edb8982854b5e79a890577927de0c29af629a14dd8c1d326bab0056ad0a313', N'KhachHang', NULL, @kh9, 1),
('kh_bich10', 'b9edb8982854b5e79a890577927de0c29af629a14dd8c1d326bab0056ad0a313', N'KhachHang', NULL, @kh10, 1);

-- 8. KHUYENMAI (20)
INSERT dbo.KHUYENMAI(TenKM, NgayBatDau, NgayKetThuc, MoTa) VALUES
(N'Tết Dương lịch 2026', '20260101', '20260110', N'Mừng năm mới 2026'),
(N'Chào Xuân Bính Ngọ', '20260201', '20260228', N'Ưu đãi dịp Tết Nguyên Đán'),
(N'Valentine 14/2', '20260210', '20260216', N'Quà tặng người thương'),
(N'Quốc tế Phụ nữ 8/3', '20260301', '20260310', N'Ưu đãi chị em'),
(N'Mừng sinh nhật cửa hàng', '20260315', '20260331', N'Sinh nhật PhoneStore'),
(N'Lễ 30/4 - 1/5', '20260420', '20260505', N'Nghỉ lễ Thống nhất và Quốc tế Lao động'),
(N'Chào hè sôi động', '20260515', '20260630', N'Giảm giá mùa hè'),
(N'Ngày Gia đình Việt Nam 28/6', '20260625', '20260630', N'Ưu đãi gia đình'),
(N'Tựu trường 2026', '20260815', '20260910', N'Giảm cho học sinh sinh viên'),
(N'Quốc khánh 2/9', '20260828', '20260905', N'Mừng Quốc khánh'),
(N'Rằm Trung thu', '20260920', '20260927', N'Quà Trung thu'),
(N'Ngày Phụ nữ Việt Nam 20/10', '20261015', '20261022', N'Tri ân phái đẹp'),
(N'Halloween 2026', '20261025', '20261101', N'Ưu đãi Halloween'),
(N'Ngày độc thân 11/11', '20261105', '20261112', N'Siêu sale 11/11'),
(N'Black Friday 2026', '20261120', '20261130', N'Giảm sốc cuối tháng 11'),
(N'Giáng sinh 2026', '20261215', '20261226', N'Noel an lành'),
(N'Chào năm mới 2027', '20261228', '20270105', N'Đón năm mới 2027'),
(N'Khách hàng thân thiết 2026', '20260101', '20261231', N'Ưu đãi cả năm cho thành viên'),
(N'Xả kho cuối năm', '20261101', '20261231', N'Giảm giá thanh lý tồn kho'),
(N'Mua sớm Tết 2027', '20261201', '20270115', N'Đặt mua trước Tết');
SELECT @km1 = MaKM FROM dbo.KHUYENMAI WHERE TenKM = N'Tết Dương lịch 2026';
SELECT @km2 = MaKM FROM dbo.KHUYENMAI WHERE TenKM = N'Chào Xuân Bính Ngọ';
SELECT @km3 = MaKM FROM dbo.KHUYENMAI WHERE TenKM = N'Valentine 14/2';
SELECT @km4 = MaKM FROM dbo.KHUYENMAI WHERE TenKM = N'Quốc tế Phụ nữ 8/3';
SELECT @km5 = MaKM FROM dbo.KHUYENMAI WHERE TenKM = N'Mừng sinh nhật cửa hàng';
SELECT @km6 = MaKM FROM dbo.KHUYENMAI WHERE TenKM = N'Lễ 30/4 - 1/5';
SELECT @km7 = MaKM FROM dbo.KHUYENMAI WHERE TenKM = N'Chào hè sôi động';
SELECT @km8 = MaKM FROM dbo.KHUYENMAI WHERE TenKM = N'Ngày Gia đình Việt Nam 28/6';
SELECT @km9 = MaKM FROM dbo.KHUYENMAI WHERE TenKM = N'Tựu trường 2026';
SELECT @km10 = MaKM FROM dbo.KHUYENMAI WHERE TenKM = N'Quốc khánh 2/9';
SELECT @km11 = MaKM FROM dbo.KHUYENMAI WHERE TenKM = N'Rằm Trung thu';
SELECT @km12 = MaKM FROM dbo.KHUYENMAI WHERE TenKM = N'Ngày Phụ nữ Việt Nam 20/10';
SELECT @km13 = MaKM FROM dbo.KHUYENMAI WHERE TenKM = N'Halloween 2026';
SELECT @km14 = MaKM FROM dbo.KHUYENMAI WHERE TenKM = N'Ngày độc thân 11/11';
SELECT @km15 = MaKM FROM dbo.KHUYENMAI WHERE TenKM = N'Black Friday 2026';
SELECT @km16 = MaKM FROM dbo.KHUYENMAI WHERE TenKM = N'Giáng sinh 2026';
SELECT @km17 = MaKM FROM dbo.KHUYENMAI WHERE TenKM = N'Chào năm mới 2027';
SELECT @km18 = MaKM FROM dbo.KHUYENMAI WHERE TenKM = N'Khách hàng thân thiết 2026';
SELECT @km19 = MaKM FROM dbo.KHUYENMAI WHERE TenKM = N'Xả kho cuối năm';
SELECT @km20 = MaKM FROM dbo.KHUYENMAI WHERE TenKM = N'Mua sớm Tết 2027';

-- 9. CHITIETKHUYENMAI (26 dòng)
INSERT dbo.CHITIETKHUYENMAI(MaKM, MaDT, PhanTramGiam) VALUES
(@km1, @dt3, 5.00),
(@km2, @dt10, 8.00),
(@km3, @dt17, 10.00),
(@km4, @dt4, 12.00),
(@km5, @dt11, 15.00),
(@km6, @dt18, 7.00),
(@km7, @dt5, 6.00),
(@km8, @dt12, 9.00),
(@km9, @dt19, 10.00),
(@km10, @dt6, 5.00),
(@km11, @dt13, 8.00),
(@km12, @dt20, 12.00),
(@km13, @dt7, 10.00),
(@km14, @dt14, 15.00),
(@km15, @dt1, 20.00),
(@km16, @dt8, 7.00),
(@km17, @dt15, 5.00),
(@km18, @dt2, 5.00),
(@km18, @dt1, 5.00),
(@km18, @dt3, 5.00),
(@km18, @dt9, 5.00),
(@km18, @dt12, 5.00),
(@km19, @dt2, 8.00),
(@km19, @dt7, 8.00),
(@km19, @dt14, 8.00),
(@km19, @dt17, 8.00);

-- 10-11. PHIEUNHAPHANG (20) và CHITIETPHIEUNHAPHANG
INSERT dbo.PHIEUNHAPHANG(NgayNhap, TongTien, MaNCC, MaNV) VALUES ('20260105 09:00:00', 0, @ncc1, @nv4);
SET @pn = SCOPE_IDENTITY();
INSERT dbo.CHITIETPHIEUNHAPHANG(MaPN, MaDT, SoLuong, DonGiaNhap, ThanhTien) VALUES
(@pn, @dt1, 7, 10132000, 70924000);
INSERT dbo.PHIEUNHAPHANG(NgayNhap, TongTien, MaNCC, MaNV) VALUES ('20260112 11:00:00', 0, @ncc2, @nv9);
SET @pn = SCOPE_IDENTITY();
INSERT dbo.CHITIETPHIEUNHAPHANG(MaPN, MaDT, SoLuong, DonGiaNhap, ThanhTien) VALUES
(@pn, @dt2, 9, 4242000, 38178000);
INSERT dbo.PHIEUNHAPHANG(NgayNhap, TongTien, MaNCC, MaNV) VALUES ('20260118 08:00:00', 0, @ncc3, @nv19);
SET @pn = SCOPE_IDENTITY();
INSERT dbo.CHITIETPHIEUNHAPHANG(MaPN, MaDT, SoLuong, DonGiaNhap, ThanhTien) VALUES
(@pn, @dt3, 5, 9352000, 46760000),
(@pn, @dt2, 3, 4092000, 12276000);
INSERT dbo.PHIEUNHAPHANG(NgayNhap, TongTien, MaNCC, MaNV) VALUES ('20260203 10:00:00', 0, @ncc4, @nv4);
SET @pn = SCOPE_IDENTITY();
INSERT dbo.CHITIETPHIEUNHAPHANG(MaPN, MaDT, SoLuong, DonGiaNhap, ThanhTien) VALUES
(@pn, @dt4, 5, 4666000, 23330000);
INSERT dbo.PHIEUNHAPHANG(NgayNhap, TongTien, MaNCC, MaNV) VALUES ('20260220 11:00:00', 0, @ncc5, @nv9);
SET @pn = SCOPE_IDENTITY();
INSERT dbo.CHITIETPHIEUNHAPHANG(MaPN, MaDT, SoLuong, DonGiaNhap, ThanhTien) VALUES
(@pn, @dt5, 8, 4338000, 34704000),
(@pn, @dt4, 5, 4502000, 22510000);
INSERT dbo.PHIEUNHAPHANG(NgayNhap, TongTien, MaNCC, MaNV) VALUES ('20260308 10:00:00', 0, @ncc6, @nv19);
SET @pn = SCOPE_IDENTITY();
INSERT dbo.CHITIETPHIEUNHAPHANG(MaPN, MaDT, SoLuong, DonGiaNhap, ThanhTien) VALUES
(@pn, @dt6, 7, 7792000, 54544000);
INSERT dbo.PHIEUNHAPHANG(NgayNhap, TongTien, MaNCC, MaNV) VALUES ('20260325 10:00:00', 0, @ncc7, @nv1);
SET @pn = SCOPE_IDENTITY();
INSERT dbo.CHITIETPHIEUNHAPHANG(MaPN, MaDT, SoLuong, DonGiaNhap, ThanhTien) VALUES
(@pn, @dt7, 8, 26232000, 209856000),
(@pn, @dt4, 4, 4282000, 17128000);
INSERT dbo.PHIEUNHAPHANG(NgayNhap, TongTien, MaNCC, MaNV) VALUES ('20260410 08:00:00', 0, @ncc8, @nv4);
SET @pn = SCOPE_IDENTITY();
INSERT dbo.CHITIETPHIEUNHAPHANG(MaPN, MaDT, SoLuong, DonGiaNhap, ThanhTien) VALUES
(@pn, @dt8, 8, 9342000, 74736000);
INSERT dbo.PHIEUNHAPHANG(NgayNhap, TongTien, MaNCC, MaNV) VALUES ('20260422 11:00:00', 0, @ncc9, @nv9);
SET @pn = SCOPE_IDENTITY();
INSERT dbo.CHITIETPHIEUNHAPHANG(MaPN, MaDT, SoLuong, DonGiaNhap, ThanhTien) VALUES
(@pn, @dt9, 9, 14032000, 126288000),
(@pn, @dt4, 6, 4666000, 27996000);
INSERT dbo.PHIEUNHAPHANG(NgayNhap, TongTien, MaNCC, MaNV) VALUES ('20260506 11:00:00', 0, @ncc10, @nv19);
SET @pn = SCOPE_IDENTITY();
INSERT dbo.CHITIETPHIEUNHAPHANG(MaPN, MaDT, SoLuong, DonGiaNhap, ThanhTien) VALUES
(@pn, @dt10, 9, 20492000, 184428000);
INSERT dbo.PHIEUNHAPHANG(NgayNhap, TongTien, MaNCC, MaNV) VALUES ('20260520 08:00:00', 0, @ncc11, @nv4);
SET @pn = SCOPE_IDENTITY();
INSERT dbo.CHITIETPHIEUNHAPHANG(MaPN, MaDT, SoLuong, DonGiaNhap, ThanhTien) VALUES
(@pn, @dt11, 9, 7192000, 64728000),
(@pn, @dt4, 3, 4392000, 13176000);
INSERT dbo.PHIEUNHAPHANG(NgayNhap, TongTien, MaNCC, MaNV) VALUES ('20260608 11:00:00', 0, @ncc12, @nv9);
SET @pn = SCOPE_IDENTITY();
INSERT dbo.CHITIETPHIEUNHAPHANG(MaPN, MaDT, SoLuong, DonGiaNhap, ThanhTien) VALUES
(@pn, @dt12, 9, 22942000, 206478000);
INSERT dbo.PHIEUNHAPHANG(NgayNhap, TongTien, MaNCC, MaNV) VALUES ('20260625 11:00:00', 0, @ncc13, @nv19);
SET @pn = SCOPE_IDENTITY();
INSERT dbo.CHITIETPHIEUNHAPHANG(MaPN, MaDT, SoLuong, DonGiaNhap, ThanhTien) VALUES
(@pn, @dt13, 8, 22612000, 180896000),
(@pn, @dt4, 4, 4666000, 18664000);
INSERT dbo.PHIEUNHAPHANG(NgayNhap, TongTien, MaNCC, MaNV) VALUES ('20260710 10:00:00', 0, @ncc14, @nv4);
SET @pn = SCOPE_IDENTITY();
INSERT dbo.CHITIETPHIEUNHAPHANG(MaPN, MaDT, SoLuong, DonGiaNhap, ThanhTien) VALUES
(@pn, @dt14, 9, 5942000, 53478000);
INSERT dbo.PHIEUNHAPHANG(NgayNhap, TongTien, MaNCC, MaNV) VALUES ('20260728 10:00:00', 0, @ncc15, @nv9);
SET @pn = SCOPE_IDENTITY();
INSERT dbo.CHITIETPHIEUNHAPHANG(MaPN, MaDT, SoLuong, DonGiaNhap, ThanhTien) VALUES
(@pn, @dt15, 8, 18392000, 147136000),
(@pn, @dt4, 6, 4502000, 27012000);
INSERT dbo.PHIEUNHAPHANG(NgayNhap, TongTien, MaNCC, MaNV) VALUES ('20260812 11:00:00', 0, @ncc16, @nv19);
SET @pn = SCOPE_IDENTITY();
INSERT dbo.CHITIETPHIEUNHAPHANG(MaPN, MaDT, SoLuong, DonGiaNhap, ThanhTien) VALUES
(@pn, @dt16, 5, 21592000, 107960000);
INSERT dbo.PHIEUNHAPHANG(NgayNhap, TongTien, MaNCC, MaNV) VALUES ('20260830 11:00:00', 0, @ncc17, @nv1);
SET @pn = SCOPE_IDENTITY();
INSERT dbo.CHITIETPHIEUNHAPHANG(MaPN, MaDT, SoLuong, DonGiaNhap, ThanhTien) VALUES
(@pn, @dt17, 7, 5322000, 37254000),
(@pn, @dt4, 4, 4666000, 18664000);
INSERT dbo.PHIEUNHAPHANG(NgayNhap, TongTien, MaNCC, MaNV) VALUES ('20260915 10:00:00', 0, @ncc18, @nv4);
SET @pn = SCOPE_IDENTITY();
INSERT dbo.CHITIETPHIEUNHAPHANG(MaPN, MaDT, SoLuong, DonGiaNhap, ThanhTien) VALUES
(@pn, @dt18, 8, 4672000, 37376000);
INSERT dbo.PHIEUNHAPHANG(NgayNhap, TongTien, MaNCC, MaNV) VALUES ('20261002 11:00:00', 0, @ncc19, @nv9);
SET @pn = SCOPE_IDENTITY();
INSERT dbo.CHITIETPHIEUNHAPHANG(MaPN, MaDT, SoLuong, DonGiaNhap, ThanhTien) VALUES
(@pn, @dt19, 8, 7216000, 57728000),
(@pn, @dt4, 3, 4502000, 13506000);
INSERT dbo.PHIEUNHAPHANG(NgayNhap, TongTien, MaNCC, MaNV) VALUES ('20261020 08:00:00', 0, @ncc20, @nv19);
SET @pn = SCOPE_IDENTITY();
INSERT dbo.CHITIETPHIEUNHAPHANG(MaPN, MaDT, SoLuong, DonGiaNhap, ThanhTien) VALUES
(@pn, @dt20, 8, 21242000, 169936000);

-- 12-14. HOADON (20), CHITIETHOADON và BAOHANH (20)
INSERT dbo.HOADON(NgayLap, PhuongThucTT, TongTien, MaKH, MaNV) VALUES ('20260122 17:49:59', N'Tiền mặt', 0, @kh20, @nv20);
SET @hd = SCOPE_IDENTITY();
INSERT dbo.CHITIETHOADON(MaHD, MaDT, SoLuong, DonGia, ThanhTien, MaKM) VALUES
(@hd, @dt2, 2, 4990000, 9481000, @km18),
(@hd, @dt3, 2, 11990000, 22781000, @km18),
(@hd, @dt1, 2, 12990000, 24681000, @km18);
INSERT dbo.BAOHANH(SoIMEI, NgayBatDau, NgayKetThuc, TrangThai, GhiChu, MaHD, MaDT) VALUES
('354500000000007', '20260122', '20270122', N'Đang bảo hành', N'Kèm bảo hành rơi vỡ 6 tháng', @hd, @dt2),
('354500000000014', '20260122', '20270122', N'Đang bảo hành', N'Bảo hành chính hãng 12 tháng', @hd, @dt3),
('354500000000021', '20260122', '20270122', N'Đang bảo hành', N'Kèm bảo hành rơi vỡ 6 tháng', @hd, @dt1);
INSERT dbo.HOADON(NgayLap, PhuongThucTT, TongTien, MaKH, MaNV) VALUES ('20260206 18:10:59', N'Chuyển khoản', 0, @kh13, @nv5);
SET @hd = SCOPE_IDENTITY();
INSERT dbo.CHITIETHOADON(MaHD, MaDT, SoLuong, DonGia, ThanhTien, MaKM) VALUES
(@hd, @dt3, 2, 11990000, 22781000, @km18);
INSERT dbo.BAOHANH(SoIMEI, NgayBatDau, NgayKetThuc, TrangThai, GhiChu, MaHD, MaDT) VALUES
('354500000000028', '20260206', '20270206', N'Đang bảo hành', N'Kèm bảo hành rơi vỡ 6 tháng', @hd, @dt3);
INSERT dbo.HOADON(NgayLap, PhuongThucTT, TongTien, MaKH, MaNV) VALUES ('20260214 09:03:53', N'Thẻ', 0, @kh15, @nv13);
SET @hd = SCOPE_IDENTITY();
INSERT dbo.CHITIETHOADON(MaHD, MaDT, SoLuong, DonGia, ThanhTien, MaKM) VALUES
(@hd, @dt3, 1, 11990000, 11390500, @km18),
(@hd, @dt1, 1, 12990000, 12340500, @km18);
INSERT dbo.BAOHANH(SoIMEI, NgayBatDau, NgayKetThuc, TrangThai, GhiChu, MaHD, MaDT) VALUES
('354500000000035', '20260214', '20270214', N'Đã hoàn thành', N'Đã cập nhật phần mềm và trả máy', @hd, @dt3),
('354500000000042', '20260214', '20270214', N'Đang bảo hành', N'Kèm bảo hành rơi vỡ 6 tháng', @hd, @dt1);
INSERT dbo.HOADON(NgayLap, PhuongThucTT, TongTien, MaKH, MaNV) VALUES ('20260309 18:19:28', N'Tiền mặt', 0, @kh11, @nv13);
SET @hd = SCOPE_IDENTITY();
INSERT dbo.CHITIETHOADON(MaHD, MaDT, SoLuong, DonGia, ThanhTien, MaKM) VALUES
(@hd, @dt5, 2, 5290000, 10580000, NULL);
INSERT dbo.BAOHANH(SoIMEI, NgayBatDau, NgayKetThuc, TrangThai, GhiChu, MaHD, MaDT) VALUES
('354500000000049', '20260309', '20270309', N'Đang bảo hành', N'Bảo hành chính hãng 12 tháng', @hd, @dt5);
INSERT dbo.HOADON(NgayLap, PhuongThucTT, TongTien, MaKH, MaNV) VALUES ('20260402 15:38:14', N'Chuyển khoản', 0, @kh6, @nv2);
SET @hd = SCOPE_IDENTITY();
INSERT dbo.CHITIETHOADON(MaHD, MaDT, SoLuong, DonGia, ThanhTien, MaKM) VALUES
(@hd, @dt6, 3, 9990000, 29970000, NULL),
(@hd, @dt4, 2, 5490000, 10980000, NULL);
INSERT dbo.BAOHANH(SoIMEI, NgayBatDau, NgayKetThuc, TrangThai, GhiChu, MaHD, MaDT) VALUES
('354500000000056', '20260402', '20270402', N'Đang bảo hành', N'Bảo hành 12 tháng tại cửa hàng', @hd, @dt6),
('354500000000063', '20260402', '20270402', N'Đã từ chối', N'Rơi vỡ do người dùng, từ chối bảo hành', @hd, @dt4);
INSERT dbo.HOADON(NgayLap, PhuongThucTT, TongTien, MaKH, MaNV) VALUES ('20260429 15:45:43', N'Thẻ', 0, @kh5, @nv1);
SET @hd = SCOPE_IDENTITY();
INSERT dbo.CHITIETHOADON(MaHD, MaDT, SoLuong, DonGia, ThanhTien, MaKM) VALUES
(@hd, @dt6, 3, 9990000, 29970000, NULL),
(@hd, @dt5, 2, 5290000, 10580000, NULL);
INSERT dbo.BAOHANH(SoIMEI, NgayBatDau, NgayKetThuc, TrangThai, GhiChu, MaHD, MaDT) VALUES
('354500000000070', '20260429', '20270428', N'Đã hoàn thành', N'Đã thay pin, trả máy cho khách', @hd, @dt6),
('354500000000077', '20260429', '20270428', N'Đã hoàn thành', N'Đã cập nhật phần mềm và trả máy', @hd, @dt5);
INSERT dbo.HOADON(NgayLap, PhuongThucTT, TongTien, MaKH, MaNV) VALUES ('20260518 16:26:55', N'Tiền mặt', 0, @kh17, @nv2);
SET @hd = SCOPE_IDENTITY();
INSERT dbo.CHITIETHOADON(MaHD, MaDT, SoLuong, DonGia, ThanhTien, MaKM) VALUES
(@hd, @dt9, 3, 17990000, 51271500, @km18);
INSERT dbo.BAOHANH(SoIMEI, NgayBatDau, NgayKetThuc, TrangThai, GhiChu, MaHD, MaDT) VALUES
('354500000000084', '20260518', '20270518', N'Đang bảo hành', N'Bảo hành chính hãng 12 tháng', @hd, @dt9);
INSERT dbo.HOADON(NgayLap, PhuongThucTT, TongTien, MaKH, MaNV) VALUES ('20260605 16:50:33', N'Chuyển khoản', 0, @kh10, @nv11);
SET @hd = SCOPE_IDENTITY();
INSERT dbo.CHITIETHOADON(MaHD, MaDT, SoLuong, DonGia, ThanhTien, MaKM) VALUES
(@hd, @dt1, 3, 12990000, 37021500, @km18),
(@hd, @dt9, 3, 17990000, 51271500, @km18);
INSERT dbo.BAOHANH(SoIMEI, NgayBatDau, NgayKetThuc, TrangThai, GhiChu, MaHD, MaDT) VALUES
('354500000000091', '20260605', '20270605', N'Đang bảo hành', N'Bảo hành 12 tháng tại cửa hàng', @hd, @dt1),
('354500000000098', '20260605', '20270605', N'Đang bảo hành', N'Bảo hành chính hãng 12 tháng', @hd, @dt9);
INSERT dbo.HOADON(NgayLap, PhuongThucTT, TongTien, MaKH, MaNV) VALUES ('20260627 16:43:36', N'Thẻ', 0, @kh2, @nv6);
SET @hd = SCOPE_IDENTITY();
INSERT dbo.CHITIETHOADON(MaHD, MaDT, SoLuong, DonGia, ThanhTien, MaKM) VALUES
(@hd, @dt8, 2, 10990000, 21980000, NULL),
(@hd, @dt6, 1, 9990000, 9990000, NULL);
INSERT dbo.BAOHANH(SoIMEI, NgayBatDau, NgayKetThuc, TrangThai, GhiChu, MaHD, MaDT) VALUES
('354500000000105', '20260627', '20270627', N'Đã từ chối', N'Rơi vỡ do người dùng, từ chối bảo hành', @hd, @dt8),
('354500000000112', '20260627', '20270627', N'Đang bảo hành', N'Kèm bảo hành rơi vỡ 6 tháng', @hd, @dt6);
INSERT dbo.HOADON(NgayLap, PhuongThucTT, TongTien, MaKH, MaNV) VALUES ('20260714 10:06:43', N'Tiền mặt', 0, @kh1, @nv1);
SET @hd = SCOPE_IDENTITY();
INSERT dbo.CHITIETHOADON(MaHD, MaDT, SoLuong, DonGia, ThanhTien, MaKM) VALUES
(@hd, @dt8, 1, 10990000, 10990000, NULL),
(@hd, @dt14, 3, 6990000, 20970000, NULL);
INSERT dbo.BAOHANH(SoIMEI, NgayBatDau, NgayKetThuc, TrangThai, GhiChu, MaHD, MaDT) VALUES
('354500000000119', '20260714', '20270714', N'Đang bảo hành', N'Bảo hành 12 tháng tại cửa hàng', @hd, @dt8),
('354500000000126', '20260714', '20270714', N'Đang bảo hành', N'Kèm bảo hành rơi vỡ 6 tháng', @hd, @dt14);
INSERT dbo.HOADON(NgayLap, PhuongThucTT, TongTien, MaKH, MaNV) VALUES ('20260803 12:04:00', N'Chuyển khoản', 0, @kh8, @nv14);
SET @hd = SCOPE_IDENTITY();
INSERT dbo.CHITIETHOADON(MaHD, MaDT, SoLuong, DonGia, ThanhTien, MaKM) VALUES
(@hd, @dt5, 2, 5290000, 10580000, NULL);
INSERT dbo.BAOHANH(SoIMEI, NgayBatDau, NgayKetThuc, TrangThai, GhiChu, MaHD, MaDT) VALUES
('354500000000133', '20260803', '20270803', N'Đang bảo hành', N'Bảo hành 12 tháng tại cửa hàng', @hd, @dt5);
INSERT dbo.HOADON(NgayLap, PhuongThucTT, TongTien, MaKH, MaNV) VALUES ('20260830 13:38:13', N'Thẻ', 0, @kh3, @nv20);
SET @hd = SCOPE_IDENTITY();
INSERT dbo.CHITIETHOADON(MaHD, MaDT, SoLuong, DonGia, ThanhTien, MaKM) VALUES
(@hd, @dt17, 1, 6490000, 6490000, NULL),
(@hd, @dt13, 3, 28990000, 86970000, NULL);
INSERT dbo.BAOHANH(SoIMEI, NgayBatDau, NgayKetThuc, TrangThai, GhiChu, MaHD, MaDT) VALUES
('354500000000140', '20260830', '20270828', N'Đã hoàn thành', N'Đã cập nhật phần mềm và trả máy', @hd, @dt17);
INSERT dbo.HOADON(NgayLap, PhuongThucTT, TongTien, MaKH, MaNV) VALUES ('20260912 19:49:51', N'Tiền mặt', 0, @kh19, @nv2);
SET @hd = SCOPE_IDENTITY();
INSERT dbo.CHITIETHOADON(MaHD, MaDT, SoLuong, DonGia, ThanhTien, MaKM) VALUES
(@hd, @dt14, 2, 6990000, 13980000, NULL),
(@hd, @dt16, 2, 26990000, 53980000, NULL);
INSERT dbo.HOADON(NgayLap, PhuongThucTT, TongTien, MaKH, MaNV) VALUES ('20260924 17:41:41', N'Chuyển khoản', 0, @kh14, @nv11);
SET @hd = SCOPE_IDENTITY();
INSERT dbo.CHITIETHOADON(MaHD, MaDT, SoLuong, DonGia, ThanhTien, MaKM) VALUES
(@hd, @dt7, 2, 31990000, 63980000, NULL),
(@hd, @dt4, 2, 5490000, 10980000, NULL);
INSERT dbo.HOADON(NgayLap, PhuongThucTT, TongTien, MaKH, MaNV) VALUES ('20261008 08:01:17', N'Thẻ', 0, @kh18, @nv20);
SET @hd = SCOPE_IDENTITY();
INSERT dbo.CHITIETHOADON(MaHD, MaDT, SoLuong, DonGia, ThanhTien, MaKM) VALUES
(@hd, @dt10, 2, 24990000, 49980000, NULL);
INSERT dbo.HOADON(NgayLap, PhuongThucTT, TongTien, MaKH, MaNV) VALUES ('20261017 08:14:43', N'Tiền mặt', 0, @kh7, @nv5);
SET @hd = SCOPE_IDENTITY();
INSERT dbo.CHITIETHOADON(MaHD, MaDT, SoLuong, DonGia, ThanhTien, MaKM) VALUES
(@hd, @dt14, 2, 6990000, 13980000, NULL),
(@hd, @dt5, 2, 5290000, 10580000, NULL);
INSERT dbo.HOADON(NgayLap, PhuongThucTT, TongTien, MaKH, MaNV) VALUES ('20261030 08:22:52', N'Chuyển khoản', 0, @kh4, @nv14);
SET @hd = SCOPE_IDENTITY();
INSERT dbo.CHITIETHOADON(MaHD, MaDT, SoLuong, DonGia, ThanhTien, MaKM) VALUES
(@hd, @dt1, 1, 12990000, 12340500, @km18),
(@hd, @dt2, 3, 4990000, 14221500, @km18);
INSERT dbo.HOADON(NgayLap, PhuongThucTT, TongTien, MaKH, MaNV) VALUES ('20261106 16:51:41', N'Thẻ', 0, @kh12, @nv5);
SET @hd = SCOPE_IDENTITY();
INSERT dbo.CHITIETHOADON(MaHD, MaDT, SoLuong, DonGia, ThanhTien, MaKM) VALUES
(@hd, @dt17, 3, 6490000, 17912400, @km19);
INSERT dbo.HOADON(NgayLap, PhuongThucTT, TongTien, MaKH, MaNV) VALUES ('20261110 11:47:40', N'Tiền mặt', 0, @kh16, @nv1);
SET @hd = SCOPE_IDENTITY();
INSERT dbo.CHITIETHOADON(MaHD, MaDT, SoLuong, DonGia, ThanhTien, MaKM) VALUES
(@hd, @dt12, 2, 26990000, 51281000, @km18),
(@hd, @dt19, 3, 8490000, 25470000, NULL);
INSERT dbo.HOADON(NgayLap, PhuongThucTT, TongTien, MaKH, MaNV) VALUES ('20261112 11:34:09', N'Chuyển khoản', 0, @kh9, @nv6);
SET @hd = SCOPE_IDENTITY();
INSERT dbo.CHITIETHOADON(MaHD, MaDT, SoLuong, DonGia, ThanhTien, MaKM) VALUES
(@hd, @dt8, 1, 10990000, 10990000, NULL),
(@hd, @dt11, 2, 8990000, 17980000, NULL),
(@hd, @dt14, 1, 6990000, 5941500, @km14);

-- 15. GIOHANG (20)
INSERT dbo.GIOHANG(MaKH, MaDT, SoLuong, NgayThem) VALUES
(@kh1, @dt7, 1, '20261102'),
(@kh2, @dt8, 1, '20261108'),
(@kh3, @dt11, 1, '20261112'),
(@kh4, @dt15, 1, '20261107'),
(@kh5, @dt17, 1, '20261110'),
(@kh6, @dt20, 1, '20261111'),
(@kh7, @dt4, 1, '20261106'),
(@kh8, @dt7, 1, '20261108'),
(@kh9, @dt9, 1, '20261109'),
(@kh10, @dt12, 2, '20261104'),
(@kh11, @dt15, 1, '20261111'),
(@kh12, @dt18, 1, '20261110'),
(@kh13, @dt2, 1, '20261102'),
(@kh14, @dt4, 1, '20261102'),
(@kh15, @dt7, 1, '20261105'),
(@kh16, @dt10, 1, '20261111'),
(@kh17, @dt13, 1, '20261103'),
(@kh18, @dt16, 1, '20261107'),
(@kh19, @dt19, 2, '20261109'),
(@kh20, @dt2, 1, '20261106');

-- Tính lại tồn kho = tổng nhập - tổng bán (chỉ 20 điện thoại mới)
UPDATE d SET SoLuongTon =
    ISNULL((SELECT SUM(c.SoLuong) FROM dbo.CHITIETPHIEUNHAPHANG c WHERE c.MaDT = d.MaDT), 0)
  - ISNULL((SELECT SUM(c.SoLuong) FROM dbo.CHITIETHOADON c WHERE c.MaDT = d.MaDT), 0)
FROM dbo.DIENTHOAI d
WHERE d.MaDT IN (@dt1, @dt2, @dt3, @dt4, @dt5, @dt6, @dt7, @dt8, @dt9, @dt10, @dt11, @dt12, @dt13, @dt14, @dt15, @dt16, @dt17, @dt18, @dt19, @dt20);

-- Tính lại điểm tích lũy = tổng (tổng tiền hóa đơn / SoTienMotDiem, làm tròn xuống) của khách mới
UPDATE k SET DiemTichLuy = x.Diem
FROM dbo.KHACHHANG k
CROSS APPLY (SELECT CONVERT(int, ISNULL(SUM(CASE WHEN FLOOR(h.TongTien / @soTien) > 1000000 THEN 1000000 ELSE FLOOR(h.TongTien / @soTien) END), 0)) AS Diem
             FROM dbo.HOADON h WHERE h.MaKH = k.MaKH) x
WHERE k.MaKH IN (@kh1, @kh2, @kh3, @kh4, @kh5, @kh6, @kh7, @kh8, @kh9, @kh10, @kh11, @kh12, @kh13, @kh14, @kh15, @kh16, @kh17, @kh18, @kh19, @kh20);

COMMIT TRANSACTION;
PRINT N'Đã thêm xong dữ liệu mẫu.';
SELECT N'LOAIDIENTHOAI' AS Bang, COUNT(*) AS TongSoDong FROM dbo.LOAIDIENTHOAI
UNION ALL SELECT N'HANGDT', COUNT(*) FROM dbo.HANGDT
UNION ALL SELECT N'NHACUNGCAP', COUNT(*) FROM dbo.NHACUNGCAP
UNION ALL SELECT N'DIENTHOAI', COUNT(*) FROM dbo.DIENTHOAI
UNION ALL SELECT N'NHANVIEN', COUNT(*) FROM dbo.NHANVIEN
UNION ALL SELECT N'KHACHHANG', COUNT(*) FROM dbo.KHACHHANG
UNION ALL SELECT N'TAIKHOAN', COUNT(*) FROM dbo.TAIKHOAN
UNION ALL SELECT N'KHUYENMAI', COUNT(*) FROM dbo.KHUYENMAI
UNION ALL SELECT N'CHITIETKHUYENMAI', COUNT(*) FROM dbo.CHITIETKHUYENMAI
UNION ALL SELECT N'PHIEUNHAPHANG', COUNT(*) FROM dbo.PHIEUNHAPHANG
UNION ALL SELECT N'CHITIETPHIEUNHAPHANG', COUNT(*) FROM dbo.CHITIETPHIEUNHAPHANG
UNION ALL SELECT N'HOADON', COUNT(*) FROM dbo.HOADON
UNION ALL SELECT N'CHITIETHOADON', COUNT(*) FROM dbo.CHITIETHOADON
UNION ALL SELECT N'GIOHANG', COUNT(*) FROM dbo.GIOHANG
UNION ALL SELECT N'BAOHANH', COUNT(*) FROM dbo.BAOHANH;
END TRY
BEGIN CATCH
    IF XACT_STATE() <> 0 ROLLBACK TRANSACTION;
    THROW;
END CATCH;