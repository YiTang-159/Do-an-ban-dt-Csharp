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
            Application.Run(new MainForm());
        }
    }
}
