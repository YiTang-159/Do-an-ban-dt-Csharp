using System;

namespace QuanLyBanDienThoai.DTO
{
    // Bảng GIOHANG, khóa chính kép (MaKH, MaDT)
    public class GioHang
    {
        public int MaKH { get; set; }
        public int MaDT { get; set; }
        public int SoLuong { get; set; }
        public DateTime NgayThem { get; set; }

        // Cột nối thêm / tính thêm khi đọc (không lưu vào bảng)
        public string TenHienThi { get; set; }
        public decimal GiaNiemYet { get; set; }
        public int SoLuongTon { get; set; }
        public decimal PhanTramGiam { get; set; }
        public decimal GiaSauGiam { get; set; }
        public decimal ThanhTien { get; set; }
        public int? MaKM { get; set; }
    }
}
