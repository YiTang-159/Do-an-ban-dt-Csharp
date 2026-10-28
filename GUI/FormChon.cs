using System;
using System.Collections;
using System.Drawing;
using System.Windows.Forms;
using QuanLyBanDienThoai.BUS;

namespace QuanLyBanDienThoai.GUI
{
    // Form tìm và chọn dùng chung cho nút "...".
    // Cách dùng ở form khác:
    //     FormChon f = new FormChon(dsDienThoai, new string[] { "MaDT", "TenHienThi", "GiaNiemYet" }, "Chọn điện thoại");
    //     if (f.ShowDialog() == DialogResult.OK)
    //     {
    //         DienThoai d = (DienThoai)FormChon.KetQua;   // đọc KetQua rồi điền các ô
    //     }
    // hoặc ngắn gọn:  object x = FormChon.Chon(ds, cot, "Chọn khách hàng");  // null nếu hủy
    public class FormChon : Form
    {
        // Đối tượng DTO của dòng được chọn; null nếu chưa chọn hoặc bấm Hủy
        public static object KetQua;

        private ArrayList dsGoc;
        private string[] cacCot;
        private TextBox txtTim;
        private DataGridView dgv;

        public FormChon(ArrayList ds, string[] cacCot, string tieuDe)
        {
            KetQua = null;
            dsGoc = ds ?? new ArrayList();
            this.cacCot = cacCot;

            Text = string.IsNullOrEmpty(tieuDe) ? "Chọn dữ liệu" : tieuDe;
            Size = new Size(720, 460);
            StartPosition = FormStartPosition.CenterParent;
            MinimizeBox = false;
            MaximizeBox = false;
            ShowInTaskbar = false;

            // lưới chỉ hiện các cột được yêu cầu
            dgv = new DataGridView();
            dgv.Dock = DockStyle.Fill;
            GridHelper.CauHinh(dgv);
            dgv.AutoGenerateColumns = false;
            foreach (string c in cacCot)
            {
                DataGridViewTextBoxColumn col = new DataGridViewTextBoxColumn();
                col.DataPropertyName = c;
                col.HeaderText = c;
                col.Name = "col" + c;
                dgv.Columns.Add(col);
            }
            dgv.CellDoubleClick += (s, e) => { if (e.RowIndex >= 0) Chon(); };

            Panel pnlTim = new Panel();
            pnlTim.Dock = DockStyle.Top;
            pnlTim.Height = 44;
            Label lblTim = new Label();
            lblTim.Text = "Tìm:";
            lblTim.Location = new Point(10, 14);
            lblTim.AutoSize = true;
            txtTim = new TextBox();
            txtTim.Location = new Point(50, 10);
            txtTim.Width = 300;
            txtTim.TextChanged += (s, e) => HienDanhSach();
            pnlTim.Controls.Add(lblTim);
            pnlTim.Controls.Add(txtTim);

            Panel pnlNut = new Panel();
            pnlNut.Dock = DockStyle.Bottom;
            pnlNut.Height = 46;
            Button btnChon = new Button();
            btnChon.Text = "Chọn";
            btnChon.Size = new Size(90, 30);
            btnChon.Location = new Point(10, 8);
            btnChon.Click += (s, e) => Chon();
            Button btnHuy = new Button();
            btnHuy.Text = "Hủy";
            btnHuy.Size = new Size(90, 30);
            btnHuy.Location = new Point(110, 8);
            btnHuy.Click += (s, e) =>
            {
                KetQua = null;
                DialogResult = DialogResult.Cancel;
            };
            pnlNut.Controls.Add(btnChon);
            pnlNut.Controls.Add(btnHuy);

            // Fill thêm vào trước, các thanh Top/Bottom thêm sau
            Controls.Add(dgv);
            Controls.Add(pnlNut);
            Controls.Add(pnlTim);

            HienDanhSach();
        }

        // Dùng ngắn gọn: trả về đối tượng được chọn, null nếu hủy
        public static object Chon(ArrayList ds, string[] cacCot, string tieuDe)
        {
            using (FormChon f = new FormChon(ds, cacCot, tieuDe))
            {
                if (f.ShowDialog() == DialogResult.OK) return KetQua;
                return null;
            }
        }

        private void HienDanhSach()
        {
            // tìm trên ArrayList bằng TimKiemHelper của BUS
            ArrayList ds = TimKiemHelper.TimTuKhoa(dsGoc, txtTim.Text, cacCot);
            dgv.DataSource = ds;
            GridHelper.DinhDang(dgv);
        }

        private void Chon()
        {
            if (dgv.CurrentRow == null || dgv.CurrentRow.DataBoundItem == null)
            {
                MessageBox.Show("Hãy chọn một dòng", "Thông báo");
                return;
            }
            KetQua = dgv.CurrentRow.DataBoundItem;
            DialogResult = DialogResult.OK;
        }
    }
}
