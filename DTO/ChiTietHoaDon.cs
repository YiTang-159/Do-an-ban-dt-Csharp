namespace QuanLyBanDienThoai.DTO
{
    // Bảng CHITIETHOADON, khóa chính kép (MaHD, MaDT)
    public class ChiTietHoaDon
    {
        public int MaHD { get; set; }
        public int MaDT { get; set; }
        public int SoLuong { get; set; }
        public decimal DonGia { get; set; }
        public decimal ThanhTien { get; set; }
        public int? MaKM { get; set; }           // null nếu không áp dụng khuyến mãi

        // Cột nối thêm khi đọc (không lưu vào bảng)
        public string TenHienThi { get; set; }
        public string TenKM { get; set; }
    }
}
