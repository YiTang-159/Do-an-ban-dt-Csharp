-- SQL Server | Dữ liệu giả lập cho đồ án quản lý cửa hàng bán điện thoại.
-- Chạy toàn bộ tệp trong SSMS sau khi đã tạo database từ script gốc.
-- Tài khoản demo01 đến demo10 là khách hàng, demo11 đến demo20 là nhân viên.
-- Mật khẩu demo: QldtDemo2026!  |  Chỉ sử dụng trên cơ sở dữ liệu học tập.
USE [QuanLyBanDienThoai];
GO
SET XACT_ABORT ON;
SET NOCOUNT ON;
SET ANSI_NULLS ON;
SET ANSI_PADDING ON;
SET ANSI_WARNINGS ON;
SET ARITHABORT ON;
SET CONCAT_NULL_YIELDS_NULL ON;
SET QUOTED_IDENTIFIER ON;
SET NUMERIC_ROUNDABORT OFF;

BEGIN TRY
    BEGIN TRANSACTION;

    -- Giữ khóa kiểm tra trong suốt giao dịch để hai phiên không nạp trùng.
    IF EXISTS (SELECT 1 FROM dbo.LOAIDIENTHOAI WITH (UPDLOCK, HOLDLOCK)
               WHERE TenLoai = N'[Demo20] Phổ thông')
    BEGIN
        ROLLBACK TRANSACTION;
        PRINT N'Bộ Demo20 đã được nạp.';
        RETURN;
    END;

    DECLARE @Mau TABLE (
        STT int PRIMARY KEY, Loai nvarchar(100), Hang nvarchar(50),
        Tinh nvarchar(60), Ten nvarchar(50), Ho nvarchar(50), ChuDe nvarchar(80)
    );
    INSERT @Mau VALUES
(1, N'Phổ thông', N'Aster', N'TP.HCM', N'Anh', N'Nguyễn Văn', N'Khởi đầu'),
(2, N'Gập ngang', N'Boreal', N'Hà Nội', N'Bảo', N'Trần Thị', N'Đổi máy'),
(3, N'Gập dọc', N'Cobalt', N'Đà Nẵng', N'Chi', N'Lê Minh', N'Kết nối'),
(4, N'Pin lớn', N'Dalia', N'Cần Thơ', N'Duy', N'Phạm Hoàng', N'Ngày hội'),
(5, N'Chơi game', N'Eon', N'Hải Phòng', N'Giang', N'Võ Thu', N'Đi học'),
(6, N'Chụp ảnh', N'Fable', N'Huế', N'Hà', N'Đặng Ngọc', N'Săn ưu đãi'),
(7, N'Màn hình nhỏ', N'Gala', N'Đồng Nai', N'Hân', N'Bùi Thanh', N'Ảnh đẹp'),
(8, N'Màn hình lớn', N'Halo', N'Đồng Tháp', N'Hải', N'Hồ Quang', N'Pin khỏe'),
(9, N'Bền bỉ', N'Indigo', N'An Giang', N'Huy', N'Đỗ Gia', N'Trải nghiệm'),
(10, N'Doanh nghiệp', N'Jade', N'Cà Mau', N'Khánh', N'Vũ Bảo', N'Năng động'),
(11, N'Tầm trung', N'Kite', N'Tây Ninh', N'Lan', N'Nguyễn Văn', N'Cuối tuần'),
(12, N'Cao cấp', N'Luma', N'Khánh Hòa', N'Linh', N'Trần Thị', N'Khách thân thiết'),
(13, N'Học tập', N'Mica', N'Lâm Đồng', N'Mai', N'Lê Minh', N'Mở bán'),
(14, N'Sáng tạo nội dung', N'Nova', N'Đắk Lắk', N'Minh', N'Phạm Hoàng', N'Mùa hè'),
(15, N'Sạc nhanh', N'Orchid', N'Gia Lai', N'Nga', N'Võ Thu', N'Tựu trường'),
(16, N'Hai SIM', N'Pavo', N'Quảng Ngãi', N'Ngọc', N'Đặng Ngọc', N'Tri ân'),
(17, N'Hỗ trợ bút', N'Quartz', N'Quảng Trị', N'Phúc', N'Bùi Thanh', N'Ngày công nghệ'),
(18, N'Chuyên nghe nhạc', N'Riva', N'Hà Tĩnh', N'Quân', N'Hồ Quang', N'Siêu tốc'),
(19, N'Màn hình 120 Hz', N'Sable', N'Nghệ An', N'Thảo', N'Đỗ Gia', N'Phong cách'),
(20, N'Thiết kế mỏng', N'Tera', N'Thanh Hóa', N'Vy', N'Vũ Bảo', N'Đón năm mới');

    DECLARE @LienKet TABLE (STT int PRIMARY KEY, MaDT int, Gia decimal(18,0));
    DECLARE @DiemQuyDoi decimal(18,2) = (
        SELECT GiaTri FROM dbo.THAMSO WHERE TenThamSo = 'SoTienMotDiem'
    );
    IF @DiemQuyDoi IS NULL OR @DiemQuyDoi < 1 OR @DiemQuyDoi <> FLOOR(@DiemQuyDoi)
        SET @DiemQuyDoi = 100000;
    DECLARE @ThangBH int = 12;
    SELECT @ThangBH = CONVERT(int,GiaTri)
    FROM dbo.THAMSO
    WHERE TenThamSo = 'ThangBaoHanhMacDinh'
      AND GiaTri BETWEEN 1 AND 120 AND GiaTri = FLOOR(GiaTri);

    DECLARE @HomNay date = CONVERT(date,GETDATE());
    DECLARE @MatKhau char(64) = LOWER(CONVERT(varchar(64),HASHBYTES('SHA2_256','QldtDemo2026!'),2));
    DECLARE @i int = 1;
    WHILE @i <= 20
    BEGIN
        DECLARE @So char(2) = RIGHT('0' + CONVERT(varchar(2),@i),2);
        DECLARE @Loai nvarchar(100), @Hang nvarchar(50), @Tinh nvarchar(60),
                @Ten nvarchar(50), @Ho nvarchar(50), @ChuDe nvarchar(80);
        SELECT @Loai=Loai,@Hang=Hang,@Tinh= Tinh,@Ten=Ten,@Ho=Ho,@ChuDe=ChuDe
        FROM @Mau WHERE STT=@i;
        DECLARE @MaLoai int,@MaHang int,@MaNCC int,@MaDT int,@MaNV int,
                @MaKH int,@MaKM int,@MaPN int,@MaHD int;

        INSERT dbo.LOAIDIENTHOAI (TenLoai,MoTa) VALUES (N'[Demo20] '+@Loai,N'Phân loại mẫu');
        SET @MaLoai = CONVERT(int,SCOPE_IDENTITY());
        INSERT dbo.HANGDT (TenHang,QuocGia) VALUES (N'[Demo20] '+@Hang,
            CHOOSE(1+(@i-1)%5,N'Việt Nam',N'Hàn Quốc',N'Nhật Bản',N'Hoa Kỳ',N'Đài Loan'));
        SET @MaHang = CONVERT(int,SCOPE_IDENTITY());
        INSERT dbo.NHACUNGCAP (TenNCC,DiaChi,SDT,Email) VALUES
            (N'[Demo20] Phân phối '+@Hang,@Tinh,'09070000'+@So,'ncc'+@So+'@demo20.example');
        SET @MaNCC = CONVERT(int,SCOPE_IDENTITY());

        DECLARE @Gia decimal(18,0) = 3500000 + @i*385000;
        INSERT dbo.DIENTHOAI (TenDT,HeDieuHanh,NamRaMat,RAM,BoNho,MauSac,GiaNiemYet,MoTa,MaHang,MaLoai)
        VALUES (N'[Demo20] '+@Hang+N' '+CHOOSE(1+(@i-1)%4,N'Lite',N'Plus',N'Pro',N'Max'),
            N'Android',YEAR(@HomNay)-@i%4,CHOOSE(1+(@i-1)%4,4,6,8,12),
            CHOOSE(1+(@i-1)%4,64,128,256,512),CHOOSE(1+(@i-1)%5,N'Đen',N'Trắng',N'Xanh',N'Bạc',N'Tím'),
            @Gia,N'Sản phẩm giả lập',@MaHang,@MaLoai);
        SET @MaDT = CONVERT(int,SCOPE_IDENTITY());
        INSERT @LienKet VALUES (@i,@MaDT,@Gia);

        INSERT dbo.NHANVIEN (Ho,Ten,GioiTinh,NgaySinh,SDT,Email,ChucVu,NgayVaoLam)
        VALUES (@Ho,@Ten,CHOOSE(1+(@i-1)%2,N'Nam',N'Nữ'),DATEADD(YEAR,-(25+@i%10),@HomNay),
                '09071000'+@So,'nv'+@So+'@demo20.example',
                CHOOSE(1+(@i-1)%3,N'Bán hàng',N'Thủ kho',N'Thu ngân'),DATEADD(YEAR,-2,@HomNay));
        SET @MaNV = CONVERT(int,SCOPE_IDENTITY());
        INSERT dbo.KHACHHANG (Ho,Ten,SDT,Email,DiaChi,NgaySinh)
        VALUES (CHOOSE(1+(@i-1)%4,N'Lý',N'Phan',N'Trương',N'Cao'),@Ten,
                '09072000'+@So,'kh'+@So+'@demo20.example',@Tinh,DATEADD(YEAR,-(20+@i%15),@HomNay));
        SET @MaKH = CONVERT(int,SCOPE_IDENTITY());

        INSERT dbo.TAIKHOAN (TenDangNhap,MatKhau,VaiTro,MaNV,MaKH,TrangThai)
        VALUES ('demo'+@So,@MatKhau,CASE WHEN @i<=10 THEN N'KhachHang' ELSE N'NhanVien' END,
                CASE WHEN @i>10 THEN @MaNV END, CASE WHEN @i<=10 THEN @MaKH END,1);

        -- Lùi tối đa 285 ngày để có giao dịch ở nhiều quý. Phiếu nhập đi trước hóa đơn.
        DECLARE @NgayBan date = DATEADD(DAY,-(20-@i)*15,@HomNay);
        DECLARE @NgayNhap date = DATEADD(DAY,-3,@NgayBan);
        DECLARE @Giam int = 5+@i%16;
        DECLARE @SoNhap int = 10+@i%8, @SoBan int = 1+@i%3;
        DECLARE @GiaNhap decimal(18,0) = ROUND(@Gia*0.78,0);
        DECLARE @TienBan decimal(18,0) = ROUND(@Gia*(100-@Giam)*@SoBan/100,0);
        DECLARE @MaDTTruoc int = NULL, @GiaTruoc decimal(18,0) = NULL;
        -- Bốn hóa đơn và bốn phiếu nhập có thêm dòng để demo main–sub.
        IF @i%5=0
            SELECT @MaDTTruoc=MaDT,@GiaTruoc=Gia FROM @LienKet WHERE STT=@i-1;
        DECLARE @GiaNhapTruoc decimal(18,0) = ROUND(@GiaTruoc*0.78,0);
        DECLARE @TongNhap decimal(18,0) = @SoNhap*@GiaNhap + COALESCE(2*@GiaNhapTruoc,0);
        DECLARE @TongBan decimal(18,0) = @TienBan + COALESCE(@GiaTruoc,0);

        INSERT dbo.KHUYENMAI (TenKM,NgayBatDau,NgayKetThuc,MoTa)
        VALUES (N'[Demo20] '+@ChuDe,DATEADD(DAY,-2,@NgayBan),
                CASE WHEN @i%4=0 THEN DATEADD(DAY,30,@HomNay) ELSE DATEADD(DAY,2,@NgayBan) END,
                N'Khuyến mãi mẫu theo sản phẩm');
        SET @MaKM = CONVERT(int,SCOPE_IDENTITY());
        INSERT dbo.CHITIETKHUYENMAI VALUES (@MaKM,@MaDT,@Giam);

        INSERT dbo.PHIEUNHAPHANG (NgayNhap,TongTien,MaNCC,MaNV)
        VALUES (@NgayNhap,@TongNhap,@MaNCC,@MaNV);
        SET @MaPN = CONVERT(int,SCOPE_IDENTITY());
        INSERT dbo.CHITIETPHIEUNHAPHANG (MaPN,MaDT,SoLuong,DonGiaNhap,ThanhTien)
        VALUES (@MaPN,@MaDT,@SoNhap,@GiaNhap,@SoNhap*@GiaNhap);

        INSERT dbo.HOADON (NgayLap,PhuongThucTT,TongTien,MaKH,MaNV)
        VALUES (@NgayBan,CHOOSE(1+(@i-1)%3,N'Tiền mặt',N'Chuyển khoản',N'Thẻ'),@TongBan,@MaKH,@MaNV);
        SET @MaHD = CONVERT(int,SCOPE_IDENTITY());
        INSERT dbo.CHITIETHOADON (MaHD,MaDT,SoLuong,DonGia,ThanhTien,MaKM)
        VALUES (@MaHD,@MaDT,@SoBan,@Gia,@TienBan,@MaKM);

        UPDATE dbo.DIENTHOAI SET SoLuongTon=@SoNhap-@SoBan WHERE MaDT=@MaDT;
        IF @MaDTTruoc IS NOT NULL
        BEGIN
            INSERT dbo.CHITIETPHIEUNHAPHANG (MaPN,MaDT,SoLuong,DonGiaNhap,ThanhTien)
            VALUES (@MaPN,@MaDTTruoc,2,@GiaNhapTruoc,2*@GiaNhapTruoc);
            INSERT dbo.CHITIETHOADON (MaHD,MaDT,SoLuong,DonGia,ThanhTien,MaKM)
            VALUES (@MaHD,@MaDTTruoc,1,@GiaTruoc,@GiaTruoc,NULL);
            UPDATE dbo.DIENTHOAI SET SoLuongTon=SoLuongTon+1 WHERE MaDT=@MaDTTruoc;
        END;
        DECLARE @Diem decimal(38,0) = FLOOR(@TongBan/@DiemQuyDoi);
        UPDATE dbo.KHACHHANG SET DiemTichLuy=CONVERT(int,
            CASE WHEN @Diem>1000000 THEN 1000000 ELSE @Diem END) WHERE MaKH=@MaKH;
        INSERT dbo.GIOHANG (MaKH,MaDT,SoLuong,NgayThem)
        VALUES (@MaKH,@MaDT,1+@i%2,@HomNay);

        DECLARE @HetBaoHanh date = DATEADD(MONTH,@ThangBH,@NgayBan);
        INSERT dbo.BAOHANH (SoIMEI,NgayBatDau,NgayKetThuc,TrangThai,GhiChu,MaHD,MaDT)
        VALUES ('9902600000000'+@So,@NgayBan,@HetBaoHanh,
                CASE WHEN @HetBaoHanh<@HomNay THEN N'Hết hạn' ELSE N'Đang bảo hành' END,
                N'IMEI giả lập cho đồ án',@MaHD,@MaDT);
        SET @i += 1;
    END;

    COMMIT TRANSACTION;
    SELECT N'Đã thêm bộ Demo20' AS KetQua, 20 AS DongMoiMoiBangNghiepVu,
           24 AS ChiTietPhieuNhap, 24 AS ChiTietHoaDon;
END TRY
BEGIN CATCH
    IF XACT_STATE() <> 0 ROLLBACK TRANSACTION;
    THROW;
END CATCH;
