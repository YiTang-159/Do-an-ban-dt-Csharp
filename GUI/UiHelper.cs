using System;
using System.Drawing;
using System.Windows.Forms;

namespace QuanLyBanDienThoai.GUI
{
    // Hàm dựng giao diện dùng chung cho các form đăng nhập, khách hàng, bán hàng, tài khoản
    public static class UiHelper
    {
        public static readonly Color MauXanh = Color.FromArgb(0, 166, 90);
        public static readonly Color MauXanhDam = Color.FromArgb(0, 141, 76);

        public static Label Nhan(string text, int x, int y)
        {
            Label l = new Label();
            l.Text = text;
            l.Location = new Point(x, y + 4);
            l.AutoSize = true;
            return l;
        }

        public static Button Nut(string text, int x, int y, int w)
        {
            Button b = new Button();
            b.Text = text;
            b.Location = new Point(x, y);
            b.Size = new Size(w, 30);
            return b;
        }

        public static TextBox O(int x, int y, int w)
        {
            TextBox t = new TextBox();
            t.Location = new Point(x, y);
            t.Width = w;
            return t;
        }

        public static void Loi(string noiDung)
        {
            MessageBox.Show(noiDung, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        public static string Tien(decimal so)
        {
            return so.ToString("#,##0") + " đ";
        }

        // Đổi tiêu đề cột của lưới sang tiếng Việt (bỏ qua nếu lưới không có cột đó)
        public static void TieuDe(DataGridView dgv, string cot, string tieuDe)
        {
            if (dgv.Columns.Contains(cot)) dgv.Columns[cot].HeaderText = tieuDe;
        }

        public static void Giau(DataGridView dgv, params string[] cacCot)
        {
            foreach (string c in cacCot)
            {
                if (dgv.Columns.Contains(c)) dgv.Columns[c].Visible = false;
            }
        }

        // Cột phần trăm hiển thị 10 hoặc 7,5 thay vì làm tròn thành số nguyên
        public static void DinhDangPhanTram(DataGridView dgv, string cot)
        {
            if (dgv.Columns.Contains(cot))
            {
                dgv.Columns[cot].DefaultCellStyle.Format = "0.##";
                dgv.Columns[cot].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            }
        }

        // Dòng đang chọn trên lưới, null nếu chưa chọn
        public static T DongDangChon<T>(DataGridView dgv) where T : class
        {
            if (dgv.CurrentRow == null) return null;
            return dgv.CurrentRow.DataBoundItem as T;
        }
    }
}
