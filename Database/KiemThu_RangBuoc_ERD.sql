-- Chạy sau khi cài bản v3, trên database demo có dữ liệu mẫu.
-- Mỗi ca thử dùng transaction riêng và hoàn tác.
USE [QuanLyBanDienThoai];
GO
SET NOCOUNT ON;

DECLARE @MaHD int, @MaDT int, @Ngay date, @MaKM int;
SELECT TOP (1) @MaHD = b.MaHD, @MaDT = b.MaDT, @Ngay = MAX(b.NgayBatDau)
FROM dbo.BAOHANH b
JOIN dbo.CHITIETHOADON c ON c.MaHD = b.MaHD AND c.MaDT = b.MaDT
GROUP BY b.MaHD, b.MaDT, c.SoLuong
HAVING COUNT_BIG(*) = c.SoLuong;

IF @MaHD IS NULL
    THROW 50200, N'Cần một dòng hóa đơn đã cấp đủ bảo hành để chạy ca kiểm thử.', 1;

DECLARE @IMEI varchar(15) = '990000000000001';
WHILE EXISTS (SELECT 1 FROM dbo.BAOHANH WHERE SoIMEI = @IMEI)
    SET @IMEI = CONVERT(varchar(15), CONVERT(bigint, @IMEI) + 1);

BEGIN TRY
    BEGIN TRANSACTION;
    INSERT dbo.BAOHANH (SoIMEI, NgayBatDau, NgayKetThuc, MaHD, MaDT)
    VALUES (@IMEI, @Ngay, DATEADD(month, 12, @Ngay), @MaHD, @MaDT);
    THROW 50201, N'FAIL: SQL đã cho cấp vượt số bảo hành.', 1;
END TRY
BEGIN CATCH
    IF XACT_STATE() <> 0 ROLLBACK;
    IF ERROR_NUMBER() <> 50105 THROW;
    PRINT N'PASS: Chặn bảo hành vượt số máy.';
END CATCH;

BEGIN TRY
    BEGIN TRANSACTION;
    UPDATE dbo.HOADON SET TongTien = TongTien + 1 WHERE MaHD = @MaHD;
    THROW 50202, N'FAIL: SQL đã cho lưu tổng tiền sai.', 1;
END TRY
BEGIN CATCH
    IF XACT_STATE() <> 0 ROLLBACK;
    IF ERROR_NUMBER() <> 50101 THROW;
    PRINT N'PASS: Chặn tổng tiền hóa đơn sai.';
END CATCH;

BEGIN TRY
    BEGIN TRANSACTION;
    UPDATE dbo.CHITIETHOADON SET ThanhTien = ThanhTien - 1
    WHERE MaHD = @MaHD AND MaDT = @MaDT AND ThanhTien > 0;
    THROW 50203, N'FAIL: SQL đã cho lưu thành tiền sai hoặc chưa có dòng tiền dương để thử.', 1;
END TRY
BEGIN CATCH
    IF XACT_STATE() <> 0 ROLLBACK;
    IF ERROR_NUMBER() <> 50104 THROW;
    PRINT N'PASS: Chặn thành tiền dòng hóa đơn sai.';
END CATCH;

SELECT TOP (1) @MaHD = MaHD, @MaKM = MaKM
FROM dbo.CHITIETHOADON WHERE MaKM IS NOT NULL;
IF @MaKM IS NULL
    THROW 50204, N'Cần một hóa đơn có khuyến mãi để chạy ca kiểm thử ngày.', 1;

BEGIN TRY
    BEGIN TRANSACTION;
    UPDATE dbo.HOADON SET NgayLap = '20000101' WHERE MaHD = @MaHD;
    THROW 50205, N'FAIL: SQL đã cho áp khuyến mãi ngoài thời hạn.', 1;
END TRY
BEGIN CATCH
    IF XACT_STATE() <> 0 ROLLBACK;
    IF ERROR_NUMBER() <> 50102 THROW;
    PRINT N'PASS: Chặn áp khuyến mãi ngoài thời hạn.';
END CATCH;
