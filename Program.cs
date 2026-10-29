using System;
using System.Windows.Forms;
using QuanLyBanDienThoai.BUS;
using QuanLyBanDienThoai.GUI;

namespace QuanLyBanDienThoai
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Lỗi chưa bắt (ví dụ mất kết nối SQL Server) chỉ hiện thông báo, chương trình không bị tắt
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
            Application.ThreadException += (s, e) =>
                MessageBox.Show(e.Exception.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);

            // Vòng lặp: Đăng nhập -> mở trang theo vai trò -> (Đăng xuất thì quay lại Đăng nhập, tắt cửa sổ thì thoát)
            while (true)
            {
                using (FormDangNhap dangNhap = new FormDangNhap())
                {
                    if (dangNhap.ShowDialog() != DialogResult.OK) break;
                }

                // Khách hàng: trang mua sắm. Admin và nhân viên: màn hình quản lý (menu khác nhau theo vai trò)
                Form trangChinh = PhienDangNhap.LaKhachHang ? (Form)new FormTrangKhachHang() : new MainForm();
                Application.Run(trangChinh);
                trangChinh.Dispose();

                bool dangXuat = PhienDangNhap.YeuCauDangXuat;
                PhienDangNhap.KetThuc();
                if (!dangXuat) break;
            }
        }
    }
}
