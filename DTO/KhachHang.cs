using System;

namespace QuanLyBanDienThoai.DTO
{
    public class KhachHang
    {
        public int MaKH { get; set; }
        public string Ho { get; set; }
        public string Ten { get; set; }
        public string SDT { get; set; }
        public string Email { get; set; }
        public string DiaChi { get; set; }
        public DateTime? NgaySinh { get; set; }
        public int DiemTichLuy { get; set; }     // chỉ tăng khi lập hóa đơn

        // Ho + ' ' + Ten, dùng cho ComboBox và FormChon
        public string HoTen { get; set; }
    }
}
