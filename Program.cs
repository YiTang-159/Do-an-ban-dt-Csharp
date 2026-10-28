using System;
using System.Windows.Forms;
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

            Application.Run(new MainForm());
        }
    }
}
