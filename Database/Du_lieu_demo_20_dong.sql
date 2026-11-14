-- Chạy trong SSMS sau khi database đã được tạo.
-- Tài khoản mới được tạo theo vai trò Khách hàng. Mật khẩu QldtDemo2026!
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
BEGIN TRY
    BEGIN TRANSACTION;
    EXEC sp_set_session_context N'cho_phep_sua_ton', 1;
    DECLARE @Lock int;
    EXEC @Lock = sys.sp_getapplock @Resource=N'BoSungDuLieuMau20', @LockMode='Exclusive',
        @LockOwner='Transaction', @LockTimeout=10000;
    IF @Lock < 0 THROW 50001, N'Không khóa được tác vụ bổ sung dữ liệu.', 1;
    DECLARE @MucTieu int=20, @i int, @Ngay date=CONVERT(date,GETDATE()),
        @MaDT int,@MaKH int,@MaNV int,@MaNCC int,@MaHD int,@MaPN int,
        @Gia decimal(18,0), @GiaNhap decimal(18,0), @Ban date,@Nhap date,
        @Diem int,@MaBH_HD int,@MaBH_DT int,@IMEI varchar(15),@TenTK varchar(50),
        @SoTien decimal(18,2),@Thang int;
    SELECT @SoTien=GiaTri FROM dbo.THAMSO WHERE TenThamSo='SoTienMotDiem'
        AND GiaTri>=1 AND GiaTri=FLOOR(GiaTri);
    SELECT @Thang=CONVERT(int,GiaTri) FROM dbo.THAMSO WHERE TenThamSo='ThangBaoHanhMacDinh'
        AND GiaTri BETWEEN 1 AND 120 AND GiaTri=FLOOR(GiaTri);
    SET @SoTien=COALESCE(@SoTien,100000);
    SET @Thang=COALESCE(@Thang,12);

    UPDATE s SET TenLoai=STUFF(s.TenLoai,1,9,N'')
    FROM dbo.LOAIDIENTHOAI s
    WHERE LEFT(s.TenLoai,9)=N'[Demo20] '
      AND NOT EXISTS (SELECT 1 FROM dbo.LOAIDIENTHOAI d WHERE d.TenLoai=STUFF(s.TenLoai,1,9,N''));
    UPDATE s SET TenHang=STUFF(s.TenHang,1,9,N'')
    FROM dbo.HANGDT s
    WHERE LEFT(s.TenHang,9)=N'[Demo20] '
      AND NOT EXISTS (SELECT 1 FROM dbo.HANGDT d WHERE d.TenHang=STUFF(s.TenHang,1,9,N''));
    UPDATE s SET TenNCC=STUFF(s.TenNCC,1,9,N'')
    FROM dbo.NHACUNGCAP s
    WHERE LEFT(s.TenNCC,9)=N'[Demo20] '
      AND NOT EXISTS (SELECT 1 FROM dbo.NHACUNGCAP d WHERE d.TenNCC=STUFF(s.TenNCC,1,9,N''));
    UPDATE s SET TenDT=STUFF(s.TenDT,1,9,N'')
    FROM dbo.DIENTHOAI s
    WHERE LEFT(s.TenDT,9)=N'[Demo20] '
      AND NOT EXISTS (SELECT 1 FROM dbo.DIENTHOAI d WHERE d.TenDT=STUFF(s.TenDT,1,9,N'') AND d.RAM=s.RAM AND d.BoNho=s.BoNho AND d.MauSac=s.MauSac);
    UPDATE s SET TenKM=STUFF(s.TenKM,1,9,N'')
    FROM dbo.KHUYENMAI s
    WHERE LEFT(s.TenKM,9)=N'[Demo20] '
      AND NOT EXISTS (SELECT 1 FROM dbo.KHUYENMAI d WHERE d.TenKM=STUFF(s.TenKM,1,9,N''));

    -- Mã hậu tố tự tìm chỗ trống để tránh đụng dữ liệu đã có.
    DECLARE @Ma varchar(7), @BoSungKhach int=0;

    SET @i=1;
    WHILE (SELECT COUNT(*) FROM dbo.LOAIDIENTHOAI) < @MucTieu
    BEGIN
        SET @Ma=RIGHT('0000000'+CONVERT(varchar(10),@i),7);
        IF NOT EXISTS (SELECT 1 FROM dbo.LOAIDIENTHOAI WHERE TenLoai=N'Nhóm điện thoại '+@Ma)
        BEGIN
            INSERT dbo.LOAIDIENTHOAI(TenLoai,MoTa) VALUES(N'Nhóm điện thoại '+@Ma,N'Phân loại điện thoại');
        END;
        SET @i+=1;
    END;

    SET @i=1;
    WHILE (SELECT COUNT(*) FROM dbo.HANGDT) < @MucTieu
    BEGIN
        SET @Ma=RIGHT('0000000'+CONVERT(varchar(10),@i),7);
        IF NOT EXISTS (SELECT 1 FROM dbo.HANGDT WHERE TenHang=N'Hãng '+@Ma)
        BEGIN
            INSERT dbo.HANGDT(TenHang,QuocGia) VALUES(N'Hãng '+@Ma,CHOOSE(1+@i%4,N'Việt Nam',N'Nhật Bản',N'Hàn Quốc',N'Hoa Kỳ'));
        END;
        SET @i+=1;
    END;

    SET @i=1;
    WHILE (SELECT COUNT(*) FROM dbo.NHACUNGCAP) < @MucTieu
    BEGIN
        SET @Ma=RIGHT('0000000'+CONVERT(varchar(10),@i),7);
        IF NOT EXISTS (SELECT 1 FROM dbo.NHACUNGCAP WHERE SDT='091'+@Ma OR Email='ncc'+@Ma+'@phonestore.example')
        BEGIN
            INSERT dbo.NHACUNGCAP(TenNCC,DiaChi,SDT,Email) VALUES(N'Nhà phân phối '+@Ma,N'TP.HCM','091'+@Ma,'ncc'+@Ma+'@phonestore.example');
        END;
        SET @i+=1;
    END;

    SET @i=1;
    WHILE (SELECT COUNT(*) FROM dbo.DIENTHOAI) < @MucTieu
    BEGIN
        SET @Ma=RIGHT('0000000'+CONVERT(varchar(10),@i),7);
        IF NOT EXISTS (SELECT 1 FROM dbo.DIENTHOAI WHERE TenDT=N'Điện thoại '+@Ma)
        BEGIN
            INSERT dbo.DIENTHOAI(TenDT,HeDieuHanh,NamRaMat,RAM,BoNho,MauSac,GiaNiemYet,MaHang,MaLoai)
            SELECT N'Điện thoại '+@Ma,N'Android',YEAR(@Ngay),4+2*(@i%5),128,
                CHOOSE(1+@i%3,N'Đen',N'Xanh',N'Trắng'),3000000+@i*150000,
                (SELECT MIN(MaHang) FROM dbo.HANGDT),(SELECT MIN(MaLoai) FROM dbo.LOAIDIENTHOAI);
        END;
        SET @i+=1;
    END;

    SET @i=1;
    WHILE (SELECT COUNT(*) FROM dbo.NHANVIEN) < @MucTieu
    BEGIN
        SET @Ma=RIGHT('0000000'+CONVERT(varchar(10),@i),7);
        IF NOT EXISTS (SELECT 1 FROM dbo.NHANVIEN WHERE SDT='092'+@Ma OR Email='nv'+@Ma+'@phonestore.example')
        BEGIN
            INSERT dbo.NHANVIEN(Ho,Ten,NgaySinh,SDT,Email,ChucVu,NgayVaoLam)
            VALUES(N'Nguyễn',N'Nhân viên '+@Ma,DATEADD(year,-30,@Ngay),'092'+@Ma,
                'nv'+@Ma+'@phonestore.example',N'Bán hàng',DATEADD(year,-2,@Ngay));
        END;
        SET @i+=1;
    END;

    SET @i=1;
    WHILE (SELECT COUNT(*) FROM dbo.KHACHHANG) < @MucTieu
    BEGIN
        SET @Ma=RIGHT('0000000'+CONVERT(varchar(10),@i),7);
        IF NOT EXISTS (SELECT 1 FROM dbo.KHACHHANG WHERE SDT='093'+@Ma OR Email='kh'+@Ma+'@phonestore.example')
        BEGIN
            INSERT dbo.KHACHHANG(Ho,Ten,SDT,Email,DiaChi,NgaySinh)
            VALUES(N'Trần',N'Khách '+@Ma,'093'+@Ma,'kh'+@Ma+'@phonestore.example',
                N'TP.HCM',DATEADD(year,-25,@Ngay));
        END;
        SET @i+=1;
    END;

    SET @i=1;
    WHILE (SELECT COUNT(*) FROM dbo.KHUYENMAI) < @MucTieu
    BEGIN
        SET @Ma=RIGHT('0000000'+CONVERT(varchar(10),@i),7);
        IF NOT EXISTS (SELECT 1 FROM dbo.KHUYENMAI WHERE TenKM=N'Ưu đãi '+@Ma)
        BEGIN
            INSERT dbo.KHUYENMAI(TenKM,NgayBatDau,NgayKetThuc,MoTa)
            VALUES(N'Ưu đãi '+@Ma,DATEADD(day,-@i,@Ngay),DATEADD(day,30+@i,@Ngay),N'Giảm giá theo sản phẩm');
        END;
        SET @i+=1;
    END;

    SET @i=1;
    WHILE (SELECT COUNT(*) FROM dbo.TAIKHOAN)<@MucTieu
    BEGIN
        SET @MaKH=NULL;
        SELECT TOP (1) @MaKH=k.MaKH FROM dbo.KHACHHANG k
        WHERE NOT EXISTS (SELECT 1 FROM dbo.TAIKHOAN t WHERE t.MaKH=k.MaKH)
        ORDER BY k.MaKH;
        SET @Ma=RIGHT('0000000'+CONVERT(varchar(10),@i),7);
        SET @TenTK='mau'+@Ma;
        IF NOT EXISTS (SELECT 1 FROM dbo.TAIKHOAN WHERE TenDangNhap=@TenTK)
        BEGIN
            IF @MaKH IS NULL AND NOT EXISTS (
                SELECT 1 FROM dbo.KHACHHANG WHERE SDT='094'+@Ma OR Email='tk'+@Ma+'@phonestore.example'
            )
            BEGIN
                INSERT dbo.KHACHHANG(Ho,Ten,SDT,Email,DiaChi,NgaySinh)
                VALUES(N'Lê',N'Khách '+@Ma,'094'+@Ma,'tk'+@Ma+'@phonestore.example',N'TP.HCM',DATEADD(year,-26,@Ngay));
                SET @MaKH=CONVERT(int,SCOPE_IDENTITY());
            END;
            IF @MaKH IS NOT NULL
                INSERT dbo.TAIKHOAN(TenDangNhap,MatKhau,VaiTro,MaKH,TrangThai)
                VALUES(@TenTK,LOWER(CONVERT(varchar(64),HASHBYTES('SHA2_256','QldtDemo2026!'),2)),N'KhachHang',@MaKH,1);
        END;
        SET @i+=1;
    END;

    SELECT @BoSungKhach=COUNT(*) FROM dbo.CHITIETKHUYENMAI;
    INSERT dbo.CHITIETKHUYENMAI(MaKM,MaDT,PhanTramGiam)
    SELECT TOP (CASE WHEN @MucTieu>@BoSungKhach THEN @MucTieu-@BoSungKhach ELSE 0 END)
        k.MaKM,d.MaDT,5+(k.MaKM%11)
    FROM dbo.KHUYENMAI k CROSS JOIN dbo.DIENTHOAI d
    WHERE NOT EXISTS (SELECT 1 FROM dbo.CHITIETKHUYENMAI x WHERE x.MaKM=k.MaKM AND x.MaDT=d.MaDT)
    ORDER BY k.MaKM,d.MaDT;

    SET @i=1;
    WHILE (SELECT COUNT(*) FROM dbo.PHIEUNHAPHANG)<@MucTieu
       OR (SELECT COUNT(*) FROM dbo.CHITIETPHIEUNHAPHANG)<@MucTieu
       OR (SELECT COUNT(*) FROM dbo.HOADON)<@MucTieu
       OR (SELECT COUNT(*) FROM dbo.CHITIETHOADON)<@MucTieu
       OR (SELECT COUNT(*) FROM dbo.BAOHANH)<@MucTieu
    BEGIN
        SELECT @MaDT=MaDT,@Gia=GiaNiemYet FROM dbo.DIENTHOAI
        ORDER BY MaDT OFFSET ((@i-1)%20) ROWS FETCH NEXT 1 ROWS ONLY;
        SELECT @MaNV=MIN(MaNV) FROM dbo.NHANVIEN;
        SELECT @MaNCC=MIN(MaNCC) FROM dbo.NHACUNGCAP;
        SELECT @MaKH=MIN(MaKH) FROM dbo.KHACHHANG
        WHERE CONVERT(bigint,DiemTichLuy)+1000000<=2147483647;
        SET @Ban=DATEADD(day,-((@i-1)*13%280),@Ngay);
        SET @Nhap=DATEADD(day,-1,@Ban);
        SET @GiaNhap=CASE WHEN @Gia<2 THEN 1 ELSE ROUND(@Gia*0.75,0) END;
        IF (SELECT COUNT(*) FROM dbo.PHIEUNHAPHANG)<@MucTieu
           OR (SELECT COUNT(*) FROM dbo.CHITIETPHIEUNHAPHANG)<@MucTieu
           OR ((SELECT COUNT(*) FROM dbo.HOADON)<@MucTieu AND NOT EXISTS (
               SELECT 1 FROM dbo.DIENTHOAI WHERE MaDT=@MaDT AND SoLuongTon>1))
        BEGIN
            INSERT dbo.PHIEUNHAPHANG(NgayNhap,TongTien,MaNCC,MaNV) VALUES(@Nhap,@GiaNhap*5,@MaNCC,@MaNV);
            SET @MaPN=CONVERT(int,SCOPE_IDENTITY());
            INSERT dbo.CHITIETPHIEUNHAPHANG(MaPN,MaDT,SoLuong,DonGiaNhap,ThanhTien)
            VALUES(@MaPN,@MaDT,5,@GiaNhap,@GiaNhap*5);
            UPDATE dbo.DIENTHOAI SET SoLuongTon=SoLuongTon+5 WHERE MaDT=@MaDT;
        END;

        SET @MaBH_HD=NULL;
        IF (SELECT COUNT(*) FROM dbo.BAOHANH)<@MucTieu
            SELECT TOP (1) @MaBH_HD=c.MaHD,@MaBH_DT=c.MaDT
            FROM dbo.CHITIETHOADON c
            JOIN dbo.HOADON h ON h.MaHD=c.MaHD AND h.NgayLap<DATEADD(day,1,@Ngay)
            WHERE (SELECT COUNT(*) FROM dbo.BAOHANH b WHERE b.MaHD=c.MaHD AND b.MaDT=c.MaDT)<c.SoLuong
            ORDER BY c.MaHD,c.MaDT;
        IF (SELECT COUNT(*) FROM dbo.HOADON)<@MucTieu
           OR (SELECT COUNT(*) FROM dbo.CHITIETHOADON)<@MucTieu
           OR ((SELECT COUNT(*) FROM dbo.BAOHANH)<@MucTieu AND @MaBH_HD IS NULL)
        BEGIN
            IF @MaKH IS NULL THROW 50002,N'Không có khách hàng còn đủ giới hạn điểm tích lũy.',1;
            UPDATE dbo.DIENTHOAI SET SoLuongTon=SoLuongTon-1 WHERE MaDT=@MaDT AND SoLuongTon>0;
            IF @@ROWCOUNT=0
            BEGIN
                INSERT dbo.PHIEUNHAPHANG(NgayNhap,TongTien,MaNCC,MaNV) VALUES(@Nhap,@GiaNhap*5,@MaNCC,@MaNV);
                SET @MaPN=CONVERT(int,SCOPE_IDENTITY());
                INSERT dbo.CHITIETPHIEUNHAPHANG(MaPN,MaDT,SoLuong,DonGiaNhap,ThanhTien)
                VALUES(@MaPN,@MaDT,5,@GiaNhap,@GiaNhap*5);
                UPDATE dbo.DIENTHOAI SET SoLuongTon=SoLuongTon+4 WHERE MaDT=@MaDT;
            END;
            INSERT dbo.HOADON(NgayLap,PhuongThucTT,TongTien,MaKH,MaNV)
            VALUES(@Ban,CHOOSE(1+@i%3,N'Tiền mặt',N'Chuyển khoản',N'Thẻ'),@Gia,@MaKH,@MaNV);
            SET @MaHD=CONVERT(int,SCOPE_IDENTITY());
            INSERT dbo.CHITIETHOADON(MaHD,MaDT,SoLuong,DonGia,ThanhTien) VALUES(@MaHD,@MaDT,1,@Gia,@Gia);
            SET @Diem=CONVERT(int,CASE WHEN FLOOR(@Gia/@SoTien)>1000000 THEN 1000000 ELSE FLOOR(@Gia/@SoTien) END);
            UPDATE dbo.KHACHHANG SET DiemTichLuy=DiemTichLuy+@Diem WHERE MaKH=@MaKH;
            IF @MaBH_HD IS NULL
                SELECT @MaBH_HD=@MaHD,@MaBH_DT=@MaDT;
        END;
        IF (SELECT COUNT(*) FROM dbo.BAOHANH)<@MucTieu
        BEGIN
            DECLARE @j int=1;
            SET @IMEI='99877'+RIGHT('0000000000'+CONVERT(varchar(10),@j),10);
            WHILE EXISTS (SELECT 1 FROM dbo.BAOHANH WHERE SoIMEI=@IMEI)
            BEGIN
                SET @j+=1;
                SET @IMEI='99877'+RIGHT('0000000000'+CONVERT(varchar(10),@j),10);
            END;
            SELECT @Ban=CONVERT(date,NgayLap) FROM dbo.HOADON WHERE MaHD=@MaBH_HD;
            INSERT dbo.BAOHANH(SoIMEI,NgayBatDau,NgayKetThuc,TrangThai,MaHD,MaDT)
            VALUES(@IMEI,@Ban,DATEADD(month,@Thang,@Ban),
                CASE WHEN DATEADD(month,@Thang,@Ban)<@Ngay THEN N'Hết hạn' ELSE N'Đang bảo hành' END,@MaBH_HD,@MaBH_DT);
        END;
        SET @i+=1;
    END;

    IF (SELECT COUNT(*) FROM dbo.GIOHANG)<@MucTieu
       AND NOT EXISTS (SELECT 1 FROM dbo.DIENTHOAI WHERE SoLuongTon>0)
    BEGIN
        SELECT TOP (1) @MaDT=MaDT,@Gia=GiaNiemYet FROM dbo.DIENTHOAI ORDER BY MaDT;
        SELECT @MaNV=MIN(MaNV) FROM dbo.NHANVIEN;
        SELECT @MaNCC=MIN(MaNCC) FROM dbo.NHACUNGCAP;
        INSERT dbo.PHIEUNHAPHANG(NgayNhap,TongTien,MaNCC,MaNV) VALUES(@Ngay,@Gia*5,@MaNCC,@MaNV);
        SET @MaPN=CONVERT(int,SCOPE_IDENTITY());
        INSERT dbo.CHITIETPHIEUNHAPHANG(MaPN,MaDT,SoLuong,DonGiaNhap,ThanhTien)
        VALUES(@MaPN,@MaDT,5,@Gia,@Gia*5);
        UPDATE dbo.DIENTHOAI SET SoLuongTon=SoLuongTon+5 WHERE MaDT=@MaDT;
    END;
    SELECT @BoSungKhach=COUNT(*) FROM dbo.GIOHANG;
    INSERT dbo.GIOHANG(MaKH,MaDT,SoLuong,NgayThem)
    SELECT TOP (CASE WHEN @MucTieu>@BoSungKhach THEN @MucTieu-@BoSungKhach ELSE 0 END)
        k.MaKH,d.MaDT,1,@Ngay
    FROM dbo.KHACHHANG k CROSS JOIN dbo.DIENTHOAI d
    WHERE d.SoLuongTon>0 AND NOT EXISTS (SELECT 1 FROM dbo.GIOHANG g WHERE g.MaKH=k.MaKH AND g.MaDT=d.MaDT)
    ORDER BY k.MaKH,d.MaDT;

    DECLARE @KetQua TABLE (Bang sysname, SoDong int);
    INSERT @KetQua
    SELECT N'LOAIDIENTHOAI',COUNT(*) FROM dbo.LOAIDIENTHOAI
    UNION ALL     SELECT N'HANGDT',COUNT(*) FROM dbo.HANGDT
    UNION ALL     SELECT N'NHACUNGCAP',COUNT(*) FROM dbo.NHACUNGCAP
    UNION ALL     SELECT N'DIENTHOAI',COUNT(*) FROM dbo.DIENTHOAI
    UNION ALL     SELECT N'NHANVIEN',COUNT(*) FROM dbo.NHANVIEN
    UNION ALL     SELECT N'KHACHHANG',COUNT(*) FROM dbo.KHACHHANG
    UNION ALL     SELECT N'TAIKHOAN',COUNT(*) FROM dbo.TAIKHOAN
    UNION ALL     SELECT N'KHUYENMAI',COUNT(*) FROM dbo.KHUYENMAI
    UNION ALL     SELECT N'CHITIETKHUYENMAI',COUNT(*) FROM dbo.CHITIETKHUYENMAI
    UNION ALL     SELECT N'PHIEUNHAPHANG',COUNT(*) FROM dbo.PHIEUNHAPHANG
    UNION ALL     SELECT N'CHITIETPHIEUNHAPHANG',COUNT(*) FROM dbo.CHITIETPHIEUNHAPHANG
    UNION ALL     SELECT N'HOADON',COUNT(*) FROM dbo.HOADON
    UNION ALL     SELECT N'CHITIETHOADON',COUNT(*) FROM dbo.CHITIETHOADON
    UNION ALL     SELECT N'BAOHANH',COUNT(*) FROM dbo.BAOHANH
    UNION ALL     SELECT N'GIOHANG',COUNT(*) FROM dbo.GIOHANG;
    IF EXISTS(SELECT 1 FROM @KetQua WHERE SoDong<@MucTieu)
        THROW 50003,N'Có bảng chưa đủ số dòng. Toàn bộ thay đổi đã được hoàn tác.',1;
    COMMIT TRANSACTION;
    EXEC sp_set_session_context N'cho_phep_sua_ton', 0;
    SELECT Bang,SoDong,CASE WHEN SoDong<@MucTieu THEN @MucTieu-SoDong ELSE 0 END AS CanThem
    FROM @KetQua ORDER BY Bang;
END TRY
BEGIN CATCH
    IF XACT_STATE()<>0 ROLLBACK TRANSACTION;
    THROW;
END CATCH;
