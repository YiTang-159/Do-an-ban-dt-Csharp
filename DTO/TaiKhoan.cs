namespace QuanLyBanDienThoai.DTO
{
    // Bảng TAIKHOAN (ngoài ERD): mỗi tài khoản gắn với một nhân viên (Admin, NhanVien) hoặc một khách hàng
    public class TaiKhoan
    {
        public string TenDangNhap { get; set; }
        public string MatKhau { get; set; }      // mã băm SHA-256, không phải mật khẩu thật
        public string VaiTro { get; set; }       // xem VaiTroTK
        public int? MaNV { get; set; }
        public int? MaKH { get; set; }
        public bool TrangThai { get; set; }      // true = hoạt động

        // Cột nối thêm khi đọc (không lưu vào bảng)
        public string HoTen { get; set; }
    }
}
