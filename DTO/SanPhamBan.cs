namespace QuanLyBanDienThoai.DTO
{
    // Điện thoại kèm giá sau khuyến mãi, dùng cho trang khách hàng và form bán hàng
    public class SanPhamBan
    {
        public int MaDT { get; set; }
        public string TenHienThi { get; set; }
        public string TenHang { get; set; }
        public decimal GiaNiemYet { get; set; }
        public decimal PhanTramGiam { get; set; }   // 0 nếu không có khuyến mãi
        public decimal GiaSauGiam { get; set; }
        public int SoLuongTon { get; set; }
        public int? MaKM { get; set; }
        public string TenKM { get; set; }
    }
}
