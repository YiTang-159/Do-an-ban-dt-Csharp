using System;
using QuanLyBanDienThoai.DTO;

namespace QuanLyBanDienThoai.BUS
{
    // Phiên đăng nhập hiện tại, dùng chung cho cả chương trình.
    // Mọi form lấy MaNV, MaKH, vai trò từ đây (không cho người dùng tự chọn), nhờ đó dữ liệu luôn khớp với tài khoản.
    public static class PhienDangNhap
    {
        public static TaiKhoan TaiKhoanHienTai { get; private set; }

        // true khi bấm "Đăng xuất" (khác với tắt cửa sổ): Program quay lại màn hình đăng nhập
        public static bool YeuCauDangXuat;

        public static void BatDau(TaiKhoan tk)
        {
            TaiKhoanHienTai = tk;
            YeuCauDangXuat = false;
        }

        public static void KetThuc()
        {
            TaiKhoanHienTai = null;
        }

        public static void CapNhatHoTen(string hoTen)
        {
            if (TaiKhoanHienTai != null) TaiKhoanHienTai.HoTen = hoTen;
        }

        public static bool DaDangNhap { get { return TaiKhoanHienTai != null; } }
        public static string VaiTro { get { return TaiKhoanHienTai == null ? "" : TaiKhoanHienTai.VaiTro; } }
        public static bool LaAdmin { get { return VaiTro == VaiTroTK.Admin; } }
        public static bool LaNhanVien { get { return VaiTro == VaiTroTK.NhanVien; } }
        public static bool LaKhachHang { get { return VaiTro == VaiTroTK.KhachHang; } }
        // Admin và nhân viên đều là nhân sự của cửa hàng (có MaNV)
        public static bool LaNhanSu { get { return LaAdmin || LaNhanVien; } }

        public static int MaNV
        {
            get { return TaiKhoanHienTai != null && TaiKhoanHienTai.MaNV.HasValue ? TaiKhoanHienTai.MaNV.Value : 0; }
        }

        public static int MaKH
        {
            get { return TaiKhoanHienTai != null && TaiKhoanHienTai.MaKH.HasValue ? TaiKhoanHienTai.MaKH.Value : 0; }
        }

        public static string HoTen
        {
            get { return TaiKhoanHienTai == null || TaiKhoanHienTai.HoTen == null ? "" : TaiKhoanHienTai.HoTen; }
        }

        public static string TenVaiTro(string vaiTro)
        {
            switch (vaiTro)
            {
                case VaiTroTK.Admin: return "Quản trị viên";
                case VaiTroTK.NhanVien: return "Nhân viên";
                case VaiTroTK.KhachHang: return "Khách hàng";
                default: return "";
            }
        }

        public static string TenVaiTroHienTai { get { return TenVaiTro(VaiTro); } }

        // Hai chữ cái đầu của hai từ cuối trong họ tên, ví dụ "Nguyễn Văn An" -> "VA"
        public static string VietTat
        {
            get
            {
                string[] p = HoTen.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                if (p.Length == 0) return "?";
                if (p.Length == 1) return p[0].Substring(0, 1).ToUpper();
                return (p[p.Length - 2].Substring(0, 1) + p[p.Length - 1].Substring(0, 1)).ToUpper();
            }
        }
    }
}
