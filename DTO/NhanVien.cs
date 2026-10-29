using System;

namespace QuanLyBanDienThoai.DTO
{
    public class NhanVien
    {
        public int MaNV { get; set; }
        public string Ho { get; set; }
        public string Ten { get; set; }
        public string GioiTinh { get; set; }
        public DateTime? NgaySinh { get; set; }
        public string SDT { get; set; }
        public string Email { get; set; }
        public string ChucVu { get; set; }
        public DateTime? NgayVaoLam { get; set; }

        // Ho + ' ' + Ten, dùng cho ComboBox và FormChon
        public string HoTen { get; set; }
    }
}
