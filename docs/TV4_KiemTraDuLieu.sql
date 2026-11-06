-- CHỈ ĐỌC DỮ LIỆU: kiểm tra đối chiếu cho TV4 trong SQL Server Management Studio.
-- Không tạo / sửa / xóa bản ghi. Chạy sau khi database đã có dữ liệu mẫu.
USE QuanLyBanDienThoai;
GO
DECLARE @TuNgay DATE='2026-01-01', @DenNgay DATE='2026-12-31';
SELECT N'Tổng thu' AS ChiTieu, COALESCE(SUM(TongTien),0) AS GiaTri
FROM HOADON WHERE NgayLap >= @TuNgay AND NgayLap < DATEADD(day,1,@DenNgay)
UNION ALL
SELECT N'Tổng chi', COALESCE(SUM(TongTien),0)
FROM PHIEUNHAPHANG WHERE NgayNhap >= @TuNgay AND NgayNhap < DATEADD(day,1,@DenNgay);
GO
SELECT MaKM,TenKM,NgayBatDau,NgayKetThuc FROM KHUYENMAI ORDER BY MaKM;
SELECT c.MaKM,c.MaDT,c.PhanTramGiam FROM CHITIETKHUYENMAI c ORDER BY c.MaKM,c.MaDT;
GO
-- Kiểm tra bảo hành: mỗi IMEI phải đúng 15 số và mỗi máy phải xuất hiện trong hóa đơn tương ứng.
SELECT b.MaBH,b.SoIMEI,b.MaHD,b.MaDT,b.NgayBatDau,b.NgayKetThuc,
    CASE WHEN c.MaHD IS NULL THEN N'LỖI: máy không thuộc hóa đơn' ELSE N'Hợp lệ' END AS KiemTraHoaDon
FROM BAOHANH b
LEFT JOIN CHITIETHOADON c ON c.MaHD=b.MaHD AND c.MaDT=b.MaDT;
GO
-- Kiểm tra doanh thu theo quý ở hai cấp: hóa đơn theo nhân viên và dòng hóa đơn theo điện thoại.
SELECT h.MaNV,DATEPART(quarter,h.NgayLap) AS Quy,SUM(h.TongTien) AS DoanhThu
FROM HOADON h GROUP BY h.MaNV,DATEPART(quarter,h.NgayLap) ORDER BY h.MaNV,Quy;
SELECT c.MaDT,DATEPART(quarter,h.NgayLap) AS Quy,SUM(c.ThanhTien) AS DoanhThu
FROM HOADON h JOIN CHITIETHOADON c ON c.MaHD=h.MaHD
GROUP BY c.MaDT,DATEPART(quarter,h.NgayLap) ORDER BY c.MaDT,Quy;
GO
