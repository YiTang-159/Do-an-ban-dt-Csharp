using System;

namespace QuanLyBanDienThoai.DTO
{
    public class HoaDon
    {
        public int MaHD { get; set; }
        public DateTime NgayLap { get; set; }
        public string PhuongThucTT { get; set; }
        public decimal TongTien { get; set; }
        public int MaKH { get; set; }
        public int MaNV { get; set; }

        // Cột nối thêm khi đọc (không lưu vào bảng)
        public string TenKhachHang { get; set; }
        public string TenNhanVien { get; set; }
    }
}
