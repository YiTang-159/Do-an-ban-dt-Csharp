using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using QuanLyBanDienThoai.BUS;

namespace QuanLyBanDienThoai.GUI
{
    // Form tìm nâng cao dùng chung: chọn trường, phép toán, giá trị, thêm nhiều điều kiện nối AND/OR, có NOT.
    // Cách dùng ở form danh mục:
    //     using (FormTimNangCao f = new FormTimNangCao(typeof(HangDT)))
    //     {
    //         if (f.ShowDialog() == DialogResult.OK)
    //             hienLuoi(bus.TimNangCao(f.KetQua));
    //     }
    public class FormTimNangCao : Form
    {
        // Danh sách điều kiện người dùng đã lập (có giá trị khi ShowDialog trả về OK)
        public List<DieuKien> KetQua { get; private set; }

        private Type kieu;
        private List<DieuKien> dsDieuKien = new List<DieuKien>();
        private ComboBox cboNoi, cboTruong, cboPhepToan;
        private CheckBox chkNot;
        private TextBox txtGiaTri;
        private ListBox lstDieuKien;

        public FormTimNangCao(Type kieuDTO)
        {
            kieu = kieuDTO;
            Text = "Tìm kiếm nâng cao";
            FormBorderStyle = FormBorderStyle.FixedDialog;
            StartPosition = FormStartPosition.CenterParent;
            MinimizeBox = false;
            MaximizeBox = false;
            ShowInTaskbar = false;
            ClientSize = new Size(634, 395);

            Label lblNoi = new Label();
            lblNoi.Text = "Nối với điều kiện trước:";
            lblNoi.Location = new Point(15, 22);
            lblNoi.AutoSize = true;
            cboNoi = new ComboBox();
            cboNoi.DropDownStyle = ComboBoxStyle.DropDownList;
            cboNoi.Items.AddRange(new object[] { "AND", "OR" });
            cboNoi.SelectedIndex = 0;
            cboNoi.Location = new Point(165, 18);
            cboNoi.Width = 70;
            cboNoi.Enabled = false;
            chkNot = new CheckBox();
            chkNot.Text = "NOT (phủ định điều kiện)";
            chkNot.Location = new Point(260, 20);
            chkNot.AutoSize = true;

            Label lblTruong = new Label();
            lblTruong.Text = "Trường:";
            lblTruong.Location = new Point(15, 62);
            lblTruong.AutoSize = true;
            cboTruong = new ComboBox();
            cboTruong.DropDownStyle = ComboBoxStyle.DropDownList;
            cboTruong.Items.AddRange(TimKiemHelper.LayDanhSachTruong(kieu));
            if (cboTruong.Items.Count > 0) cboTruong.SelectedIndex = 0;
            cboTruong.Location = new Point(70, 58);
            cboTruong.Width = 160;

            Label lblPT = new Label();
            lblPT.Text = "Phép toán:";
            lblPT.Location = new Point(245, 62);
            lblPT.AutoSize = true;
            cboPhepToan = new ComboBox();
            cboPhepToan.DropDownStyle = ComboBoxStyle.DropDownList;
            cboPhepToan.Items.AddRange(TimKiemHelper.CacPhepToan);
            cboPhepToan.SelectedIndex = 0;
            cboPhepToan.Location = new Point(315, 58);
            cboPhepToan.Width = 80;

            Label lblGT = new Label();
            lblGT.Text = "Giá trị:";
            lblGT.Location = new Point(410, 62);
            lblGT.AutoSize = true;
            txtGiaTri = new TextBox();
            txtGiaTri.Location = new Point(460, 58);
            txtGiaTri.Width = 160;

            Button btnThem = new Button();
            btnThem.Text = "Thêm điều kiện";
            btnThem.Size = new Size(150, 30);
            btnThem.Location = new Point(15, 95);
            btnThem.Click += btnThem_Click;

            lstDieuKien = new ListBox();
            lstDieuKien.Location = new Point(15, 135);
            lstDieuKien.Size = new Size(605, 200);

            Button btnXoa = new Button();
            btnXoa.Text = "Xóa điều kiện chọn";
            btnXoa.Size = new Size(150, 30);
            btnXoa.Location = new Point(15, 348);
            btnXoa.Click += btnXoa_Click;

            Button btnXoaHet = new Button();
            btnXoaHet.Text = "Xóa hết";
            btnXoaHet.Size = new Size(90, 30);
            btnXoaHet.Location = new Point(175, 348);
            btnXoaHet.Click += (s, e) => { dsDieuKien.Clear(); CapNhatDanhSach(); };

            Button btnLoc = new Button();
            btnLoc.Text = "Lọc";
            btnLoc.Size = new Size(80, 30);
            btnLoc.Location = new Point(450, 348);
            btnLoc.Click += btnLoc_Click;

            Button btnHuy = new Button();
            btnHuy.Text = "Hủy";
            btnHuy.Size = new Size(80, 30);
            btnHuy.Location = new Point(540, 348);
            btnHuy.Click += (s, e) => { KetQua = null; DialogResult = DialogResult.Cancel; };

            Controls.AddRange(new Control[] {
                lblNoi, cboNoi, chkNot, lblTruong, cboTruong, lblPT, cboPhepToan, lblGT, txtGiaTri,
                btnThem, lstDieuKien, btnXoa, btnXoaHet, btnLoc, btnHuy });
        }

        private void btnThem_Click(object sender, EventArgs e)
        {
            DieuKien dk = new DieuKien();
            dk.Truong = cboTruong.SelectedItem == null ? "" : cboTruong.SelectedItem.ToString();
            dk.PhepToan = cboPhepToan.SelectedItem == null ? "" : cboPhepToan.SelectedItem.ToString();
            dk.GiaTri = txtGiaTri.Text.Trim();
            dk.Noi = cboNoi.SelectedIndex == 1 ? PhepNoi.Or : PhepNoi.And;
            dk.PhuDinh = chkNot.Checked;

            string loi = TimKiemHelper.KiemTraDieuKien(kieu, dk);
            if (loi != "")
            {
                MessageBox.Show(loi, "Điều kiện chưa hợp lệ");
                return;
            }
            dsDieuKien.Add(dk);
            txtGiaTri.Clear();
            chkNot.Checked = false;
            CapNhatDanhSach();
        }

        private void btnXoa_Click(object sender, EventArgs e)
        {
            int i = lstDieuKien.SelectedIndex;
            if (i < 0) return;
            dsDieuKien.RemoveAt(i);
            CapNhatDanhSach();
        }

        private void btnLoc_Click(object sender, EventArgs e)
        {
            if (dsDieuKien.Count == 0)
            {
                MessageBox.Show("Hãy thêm ít nhất một điều kiện", "Thông báo");
                return;
            }
            KetQua = new List<DieuKien>(dsDieuKien);
            DialogResult = DialogResult.OK;
        }

        private void CapNhatDanhSach()
        {
            lstDieuKien.Items.Clear();
            for (int i = 0; i < dsDieuKien.Count; i++)
            {
                lstDieuKien.Items.Add(dsDieuKien[i].MoTa(i == 0));
            }
            // điều kiện đầu tiên không cần nối
            cboNoi.Enabled = dsDieuKien.Count > 0;
        }
    }
}
