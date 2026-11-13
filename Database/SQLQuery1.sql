USE [QuanLyBanDienThoai];
GO

SET XACT_ABORT ON;

BEGIN TRY
    BEGIN TRANSACTION;

    UPDATE dbo.LOAIDIENTHOAI
    SET TenLoai = STUFF(TenLoai, 1, 9, N'')
    WHERE LEFT(TenLoai, 9) = N'[Demo20] ';

    UPDATE dbo.HANGDT
    SET TenHang = STUFF(TenHang, 1, 9, N'')
    WHERE LEFT(TenHang, 9) = N'[Demo20] ';

    UPDATE dbo.NHACUNGCAP
    SET TenNCC = STUFF(TenNCC, 1, 9, N'')
    WHERE LEFT(TenNCC, 9) = N'[Demo20] ';

    UPDATE dbo.DIENTHOAI
    SET TenDT = STUFF(TenDT, 1, 9, N'')
    WHERE LEFT(TenDT, 9) = N'[Demo20] ';

    UPDATE dbo.KHUYENMAI
    SET TenKM = STUFF(TenKM, 1, 9, N'')
    WHERE LEFT(TenKM, 9) = N'[Demo20] ';

    COMMIT TRANSACTION;
END TRY
BEGIN CATCH
    IF XACT_STATE() <> 0 ROLLBACK TRANSACTION;
    THROW;
END CATCH;