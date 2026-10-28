namespace QuanLyBanDienThoai.DTO
{
    // Mỗi dòng DIENTHOAI là một mẫu bán được (đã gộp RAM, BoNho, MauSac, GiaNiemYet, SoLuongTon)
    public class DienThoai
    {
        public int MaDT { get; set; }
        public string TenDT { get; set; }
        public string HeDieuHanh { get; set; }
        public int NamRaMat { get; set; }
        public int RAM { get; set; }
        public int BoNho { get; set; }
        public string MauSac { get; set; }
        public decimal GiaNiemYet { get; set; }
        public int SoLuongTon { get; set; }
        public string MoTa { get; set; }
        public int MaHang { get; set; }
        public int MaLoai { get; set; }

        // Các cột nối thêm khi đọc (không lưu vào bảng)
        public string TenHang { get; set; }
        public string TenLoai { get; set; }
        // TenDT + RAM + BoNho + MauSac, dùng cho ComboBox ở form của TV2, TV3, TV4
        public string TenHienThi { get; set; }
    }
}
