namespace QuanLyBanDienThoai.DAO
{
    // Đoạn SQL dùng chung cho nhiều DAO
    public static class BieuThucSql
    {
        // TenDT + RAM + BoNho + MauSac, giống DienThoaiDAO.GetAll. Bảng DIENTHOAI phải có bí danh "d".
        public const string TenHienThiDT =
            "d.TenDT + N' ' + CAST(d.RAM AS NVARCHAR(10)) + N'GB/' + CAST(d.BoNho AS NVARCHAR(10)) + N'GB ' + ISNULL(d.MauSac, N'')";
    }
}
