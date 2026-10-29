using System;
using System.Collections;
using System.Text.RegularExpressions;
using QuanLyBanDienThoai.DAO;
using QuanLyBanDienThoai.DTO;

namespace QuanLyBanDienThoai.BUS
{
    // Đăng nhập, đăng ký khách hàng, đổi mật khẩu, quản lý tài khoản (Admin).
    // Mọi hàm trả về "" nếu thành công, ngược lại là thông báo lỗi để GUI hiển thị.
    public class TaiKhoanBUS
    {
        private TaiKhoanDAO dao = new TaiKhoanDAO();
        private KhachHangBUS khBus = new KhachHangBUS();
        private NhanVienBUS nvBus = new NhanVienBUS();

        public ArrayList GetAll()
        {
            return dao.GetAll();
        }

        public ArrayList TimKiem(string tuKhoa)
        {
            return TimKiemHelper.TimTuKhoa(dao.GetAll(), tuKhoa, "TenDangNhap", "HoTen", "VaiTro");
        }

        // Đăng nhập: vai trò (Admin / Nhân viên / Khách hàng) lấy từ chính tài khoản, không cần chọn trên màn hình.
        // Thành công thì ghi phiên đăng nhập (PhienDangNhap) để các form dùng MaNV hoặc MaKH của người này.
        public string DangNhap(string tenDangNhap, string matKhau)
        {
            tenDangNhap = (tenDangNhap ?? "").Trim();
            if (tenDangNhap == "" || string.IsNullOrEmpty(matKhau)) return "Hãy nhập tên đăng nhập và mật khẩu";
            try
            {
                TaiKhoan tk = dao.GetByTenDangNhap(tenDangNhap);
                if (tk == null || tk.MatKhau != MatKhauHelper.Bam(matKhau)) return "Sai tên đăng nhập hoặc mật khẩu";
                if (!tk.TrangThai) return "Tài khoản đã bị khóa, hãy liên hệ quản trị viên";
                PhienDangNhap.BatDau(tk);
                return "";
            }
            catch (Exception ex)
            {
                return "Không thể đăng nhập: " + ex.Message;
            }
        }

        private static string KiemTraTaiKhoan(string tenDangNhap, string matKhau, string nhapLai)
        {
            if (!Regex.IsMatch((tenDangNhap ?? "").Trim(), @"^[A-Za-z0-9_.]{4,50}$"))
                return "Tên đăng nhập gồm 4 đến 50 ký tự: chữ không dấu, số, dấu _ hoặc .";
            if ((matKhau ?? "").Length < 6) return "Mật khẩu phải có ít nhất 6 ký tự";
            if (matKhau != nhapLai) return "Nhập lại mật khẩu không khớp";
            return "";
        }

        // Khách hàng tự đăng ký: tạo KHACHHANG và TAIKHOAN trong một transaction nên hai bảng luôn khớp nhau.
        public string DangKyKhachHang(KhachHang kh, string tenDangNhap, string matKhau, string nhapLai, out int maKH)
        {
            maKH = 0;
            string loi = khBus.KiemTra(kh);
            if (loi != "") return loi;
            loi = KiemTraTaiKhoan(tenDangNhap, matKhau, nhapLai);
            if (loi != "") return loi;
            try
            {
                tenDangNhap = tenDangNhap.Trim();
                if (dao.GetByTenDangNhap(tenDangNhap) != null) return "Tên đăng nhập đã tồn tại";

                TaiKhoan tk = new TaiKhoan();
                tk.TenDangNhap = tenDangNhap;
                tk.MatKhau = MatKhauHelper.Bam(matKhau);
                tk.VaiTro = VaiTroTK.KhachHang;
                tk.TrangThai = true;
                maKH = dao.DangKyKhachHang(kh, tk);
                return "";
            }
            catch (Exception ex)
            {
                return "Không thể đăng ký: " + ex.Message;
            }
        }

        public string DoiMatKhau(string tenDangNhap, string matKhauCu, string matKhauMoi, string nhapLai)
        {
            try
            {
                TaiKhoan tk = dao.GetByTenDangNhap(tenDangNhap);
                if (tk == null) return "Không tìm thấy tài khoản";
                if (tk.MatKhau != MatKhauHelper.Bam(matKhauCu)) return "Mật khẩu hiện tại không đúng";
                if ((matKhauMoi ?? "").Length < 6) return "Mật khẩu mới phải có ít nhất 6 ký tự";
                if (matKhauMoi != nhapLai) return "Nhập lại mật khẩu mới không khớp";
                dao.UpdateMatKhau(tenDangNhap, MatKhauHelper.Bam(matKhauMoi));
                return "";
            }
            catch (Exception ex)
            {
                return "Không thể đổi mật khẩu: " + ex.Message;
            }
        }

        // ===================== Dành cho Admin =====================

        private static string KiemTraQuyen()
        {
            return PhienDangNhap.LaAdmin ? "" : "Chỉ quản trị viên mới được thực hiện thao tác này";
        }

        // Tạo tài khoản cho một nhân viên hoặc khách hàng ĐÃ CÓ trong CSDL (maDoiTuong = MaNV hoặc MaKH).
        // Mỗi người chỉ có một tài khoản.
        public string TaoTaiKhoan(string vaiTro, int maDoiTuong, string tenDangNhap, string matKhau)
        {
            string loi = KiemTraQuyen();
            if (loi != "") return loi;
            loi = KiemTraTaiKhoan(tenDangNhap, matKhau, matKhau);
            if (loi != "") return loi;
            if (maDoiTuong <= 0) return "Hãy chọn nhân viên hoặc khách hàng cho tài khoản";
            try
            {
                tenDangNhap = tenDangNhap.Trim();
                TaiKhoan tk = new TaiKhoan();
                tk.TenDangNhap = tenDangNhap;
                tk.MatKhau = MatKhauHelper.Bam(matKhau);
                tk.VaiTro = vaiTro;
                tk.TrangThai = true;

                if (vaiTro == VaiTroTK.KhachHang)
                {
                    if (khBus.GetById(maDoiTuong) == null) return "Không tìm thấy khách hàng";
                    tk.MaKH = maDoiTuong;
                }
                else if (vaiTro == VaiTroTK.Admin || vaiTro == VaiTroTK.NhanVien)
                {
                    if (nvBus.GetById(maDoiTuong) == null) return "Không tìm thấy nhân viên";
                    tk.MaNV = maDoiTuong;
                }
                else
                {
                    return "Vai trò không hợp lệ";
                }

                foreach (object o in dao.GetAll())
                {
                    TaiKhoan t = (TaiKhoan)o;
                    if (string.Equals(t.TenDangNhap, tenDangNhap, StringComparison.OrdinalIgnoreCase)) return "Tên đăng nhập đã tồn tại";
                    if (tk.MaNV.HasValue && t.MaNV == tk.MaNV) return "Nhân viên này đã có tài khoản (" + t.TenDangNhap + ")";
                    if (tk.MaKH.HasValue && t.MaKH == tk.MaKH) return "Khách hàng này đã có tài khoản (" + t.TenDangNhap + ")";
                }
                dao.Insert(tk);
                return "";
            }
            catch (Exception ex)
            {
                return "Không thể tạo tài khoản: " + ex.Message;
            }
        }

        public string DatLaiMatKhau(string tenDangNhap, string matKhauMoi)
        {
            string loi = KiemTraQuyen();
            if (loi != "") return loi;
            if ((matKhauMoi ?? "").Length < 6) return "Mật khẩu mới phải có ít nhất 6 ký tự";
            try
            {
                if (dao.UpdateMatKhau(tenDangNhap, MatKhauHelper.Bam(matKhauMoi)) == 0) return "Không tìm thấy tài khoản";
                return "";
            }
            catch (Exception ex)
            {
                return "Không thể đặt lại mật khẩu: " + ex.Message;
            }
        }

        // Không khóa/xóa tài khoản đang đăng nhập và luôn giữ lại ít nhất một Admin đang hoạt động
        private string KiemTraDuocVoHieu(TaiKhoan tk)
        {
            if (PhienDangNhap.TaiKhoanHienTai != null &&
                string.Equals(PhienDangNhap.TaiKhoanHienTai.TenDangNhap, tk.TenDangNhap, StringComparison.OrdinalIgnoreCase))
                return "Không thể khóa hoặc xóa tài khoản đang đăng nhập";
            if (tk.VaiTro == VaiTroTK.Admin)
            {
                int conLai = 0;
                foreach (object o in dao.GetAll())
                {
                    TaiKhoan t = (TaiKhoan)o;
                    if (t.VaiTro == VaiTroTK.Admin && t.TrangThai && t.TenDangNhap != tk.TenDangNhap) conLai++;
                }
                if (conLai == 0) return "Phải còn ít nhất một tài khoản Admin đang hoạt động";
            }
            return "";
        }

        public string KhoaMo(string tenDangNhap, bool khoa)
        {
            string loi = KiemTraQuyen();
            if (loi != "") return loi;
            try
            {
                TaiKhoan tk = dao.GetByTenDangNhap(tenDangNhap);
                if (tk == null) return "Không tìm thấy tài khoản";
                if (khoa)
                {
                    loi = KiemTraDuocVoHieu(tk);
                    if (loi != "") return loi;
                }
                dao.UpdateTrangThai(tenDangNhap, !khoa);
                return "";
            }
            catch (Exception ex)
            {
                return "Không thể đổi trạng thái: " + ex.Message;
            }
        }

        public string Xoa(string tenDangNhap)
        {
            string loi = KiemTraQuyen();
            if (loi != "") return loi;
            try
            {
                TaiKhoan tk = dao.GetByTenDangNhap(tenDangNhap);
                if (tk == null) return "Không tìm thấy tài khoản";
                loi = KiemTraDuocVoHieu(tk);
                if (loi != "") return loi;
                dao.Delete(tenDangNhap);
                return "";
            }
            catch (Exception ex)
            {
                return "Không thể xóa: " + ex.Message;
            }
        }
    }
}
