using System;

namespace QuanLyBanDienThoai.DTO
{
    public class BaoHanh
    {
        public int MaBH { get; set; }
        public string SoIMEI { get; set; }
        public DateTime NgayBatDau { get; set; }
        public DateTime NgayKetThuc { get; set; }
        public string TrangThai { get; set; }
        public string GhiChu { get; set; }
        public int MaHD { get; set; }
        public int MaDT { get; set; }

        // Cột nối thêm khi đọc (không lưu vào bảng)
        public string TenHienThi { get; set; }
    }
}
