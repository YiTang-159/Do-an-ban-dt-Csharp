using System;
using System.Drawing;
using System.Windows.Forms;
using QuanLyBanDienThoai.BUS;
using QuanLyBanDienThoai.DTO;

namespace QuanLyBanDienThoai.GUI
{
    // Màn hình đăng nhập: chọn vai trò (Khách hàng / Nhân viên / Quản trị viên), nhập tài khoản và mật khẩu.
    // Đăng nhập đúng thì ShowDialog() trả về OK và PhienDangNhap đã có người dùng; Program mở form tương ứng.
    public class FormDangNhap : Form
    {
        private TaiKhoanBUS bus = new TaiKhoanBUS();
        private ComboBox cboVaiTro;
        private TextBox txtTen, txtMk;
        private CheckBox chkHien;

        public FormDangNhap()
        {
            Text = "Đăng nhập - PhoneStore";
            ClientSize = new Size(400, 390);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            StartPosition = FormStartPosition.CenterScreen;
            MaximizeBox = false;
            MinimizeBox = false;

            Label lblTieuDe = new Label();
            lblTieuDe.Dock = DockStyle.Top;
            lblTieuDe.Height = 90;
            lblTieuDe.Text = "☎  PhoneStore\r\nQuản lý cửa hàng bán điện thoại";
            lblTieuDe.BackColor = UiHelper.MauXanh;
            lblTieuDe.ForeColor = Color.White;
            lblTieuDe.Font = new Font("Segoe UI", 13, FontStyle.Bold);
            lblTieuDe.TextAlign = ContentAlignment.MiddleCenter;

            cboVaiTro = new ComboBox();
            cboVaiTro.DropDownStyle = ComboBoxStyle.DropDownList;
            cboVaiTro.Items.AddRange(new object[] { "Khách hàng", "Nhân viên", "Quản trị viên (Admin)" });
            cboVaiTro.SelectedIndex = 0;
            cboVaiTro.Location = new Point(130, 112);
            cboVaiTro.Width = 240;

            txtTen = UiHelper.O(130, 152, 240);
            txtMk = UiHelper.O(130, 192, 240);
            txtMk.UseSystemPasswordChar = true;

            chkHien = new CheckBox();
            chkHien.Text = "Hiện mật khẩu";
            chkHien.AutoSize = true;
            chkHien.Location = new Point(130, 224);
            chkHien.CheckedChanged += (s, e) => txtMk.UseSystemPasswordChar = !chkHien.Checked;

            Button btnDangNhap = UiHelper.Nut("Đăng nhập", 130, 258, 115);
            btnDangNhap.BackColor = UiHelper.MauXanh;
            btnDangNhap.ForeColor = Color.White;
            btnDangNhap.FlatStyle = FlatStyle.Flat;
            btnDangNhap.Click += btnDangNhap_Click;

            Button btnThoat = UiHelper.Nut("Thoát", 255, 258, 115);
            btnThoat.DialogResult = DialogResult.Cancel;

            LinkLabel lnkDangKy = new LinkLabel();
            lnkDangKy.Text = "Chưa có tài khoản? Đăng ký khách hàng";
            lnkDangKy.AutoSize = true;
            lnkDangKy.Location = new Point(130, 304);
            lnkDangKy.LinkClicked += lnkDangKy_LinkClicked;

            Label lblGoiY = new Label();
            lblGoiY.Text = "Tài khoản mẫu: admin / admin123, dung / nv123456, an / kh123456";
            lblGoiY.ForeColor = Color.Gray;
            lblGoiY.Font = new Font("Segoe UI", 8.5f);
            lblGoiY.Dock = DockStyle.Bottom;
            lblGoiY.Height = 30;
            lblGoiY.TextAlign = ContentAlignment.MiddleCenter;

            Controls.Add(UiHelper.Nhan("Đăng nhập với tư cách:", 12, 112));
            Controls.Add(cboVaiTro);
            Controls.Add(UiHelper.Nhan("Tên đăng nhập:", 12, 152));
            Controls.Add(txtTen);
            Controls.Add(UiHelper.Nhan("Mật khẩu:", 12, 192));
            Controls.Add(txtMk);
            Controls.Add(chkHien);
            Controls.Add(btnDangNhap);
            Controls.Add(btnThoat);
            Controls.Add(lnkDangKy);
            Controls.Add(lblGoiY);
            Controls.Add(lblTieuDe);

            AcceptButton = btnDangNhap;     // Enter = Đăng nhập
            CancelButton = btnThoat;        // Esc = Thoát
        }

        private string VaiTroDaChon()
        {
            switch (cboVaiTro.SelectedIndex)
            {
                case 1: return VaiTroTK.NhanVien;
                case 2: return VaiTroTK.Admin;
                default: return VaiTroTK.KhachHang;
            }
        }

        private void btnDangNhap_Click(object sender, EventArgs e)
        {
            string loi = bus.DangNhap(txtTen.Text, txtMk.Text, VaiTroDaChon());
            if (loi != "")
            {
                UiHelper.Loi(loi);
                txtMk.Clear();
                txtMk.Focus();
                return;
            }
            DialogResult = DialogResult.OK;
        }

        private void lnkDangKy_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            using (FormDangKy f = new FormDangKy())
            {
                if (f.ShowDialog(this) != DialogResult.OK) return;
                cboVaiTro.SelectedIndex = 0;
                txtTen.Text = f.TenDangNhapMoi;
                txtMk.Clear();
                txtMk.Focus();
            }
        }
    }
}
