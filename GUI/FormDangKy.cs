using System;
using System.Drawing;
using System.Windows.Forms;
using QuanLyBanDienThoai.BUS;
using QuanLyBanDienThoai.DTO;

namespace QuanLyBanDienThoai.GUI
{
    // Đăng ký khách hàng mới: tạo một dòng KHACHHANG và một TAIKHOAN gắn với dòng đó (cùng một transaction).
    // Nhân viên cũng dùng form này (nút "Khách mới" ở FormBanHang) để lập tài khoản cho khách mua tại quầy.
    public class FormDangKy : Form
    {
        private TaiKhoanBUS bus = new TaiKhoanBUS();
        private TextBox txtHo, txtTen, txtSDT, txtEmail, txtDiaChi, txtTenDN, txtMk, txtNhapLai;
        private DateTimePicker dtpNgaySinh;

        public string TenDangNhapMoi { get; private set; }
        public int MaKHMoi { get; private set; }

        public FormDangKy()
        {
            Text = "Đăng ký khách hàng";
            ClientSize = new Size(440, 470);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            StartPosition = FormStartPosition.CenterParent;
            MaximizeBox = false;
            MinimizeBox = false;
            ShowInTaskbar = false;

            int x = 150, w = 260;
            txtHo = UiHelper.O(x, 14, w);
            txtTen = UiHelper.O(x, 48, w);
            txtSDT = UiHelper.O(x, 82, w);
            txtEmail = UiHelper.O(x, 116, w);
            txtDiaChi = UiHelper.O(x, 150, w);
            dtpNgaySinh = new DateTimePicker();
            dtpNgaySinh.Location = new Point(x, 184);
            dtpNgaySinh.Width = w;
            dtpNgaySinh.Format = DateTimePickerFormat.Short;
            dtpNgaySinh.MaxDate = DateTime.Today;
            dtpNgaySinh.ShowCheckBox = true;
            dtpNgaySinh.Checked = false;            // không tick = không khai ngày sinh
            txtTenDN = UiHelper.O(x, 240, w);
            txtMk = UiHelper.O(x, 274, w);
            txtMk.UseSystemPasswordChar = true;
            txtNhapLai = UiHelper.O(x, 308, w);
            txtNhapLai.UseSystemPasswordChar = true;

            Label lblGoiY = new Label();
            lblGoiY.Text = "Tên đăng nhập: 4-50 ký tự (chữ không dấu, số, _ hoặc .). Mật khẩu từ 6 ký tự.";
            lblGoiY.ForeColor = Color.Gray;
            lblGoiY.Font = new Font("Segoe UI", 8.5f);
            lblGoiY.Location = new Point(20, 346);
            lblGoiY.Size = new Size(400, 34);

            Button btnDangKy = UiHelper.Nut("Đăng ký", x, 392, 120);
            btnDangKy.Click += btnDangKy_Click;
            Button btnHuy = UiHelper.Nut("Hủy", x + 135, 392, 120);
            btnHuy.DialogResult = DialogResult.Cancel;

            Controls.Add(UiHelper.Nhan("Họ:", 20, 14));
            Controls.Add(txtHo);
            Controls.Add(UiHelper.Nhan("Tên:", 20, 48));
            Controls.Add(txtTen);
            Controls.Add(UiHelper.Nhan("Số điện thoại:", 20, 82));
            Controls.Add(txtSDT);
            Controls.Add(UiHelper.Nhan("Email:", 20, 116));
            Controls.Add(txtEmail);
            Controls.Add(UiHelper.Nhan("Địa chỉ:", 20, 150));
            Controls.Add(txtDiaChi);
            Controls.Add(UiHelper.Nhan("Ngày sinh (tùy chọn):", 20, 184));
            Controls.Add(dtpNgaySinh);
            Controls.Add(UiHelper.Nhan("Tên đăng nhập:", 20, 240));
            Controls.Add(txtTenDN);
            Controls.Add(UiHelper.Nhan("Mật khẩu:", 20, 274));
            Controls.Add(txtMk);
            Controls.Add(UiHelper.Nhan("Nhập lại mật khẩu:", 20, 308));
            Controls.Add(txtNhapLai);
            Controls.Add(lblGoiY);
            Controls.Add(btnDangKy);
            Controls.Add(btnHuy);
            AcceptButton = btnDangKy;
            CancelButton = btnHuy;
        }

        private void btnDangKy_Click(object sender, EventArgs e)
        {
            KhachHang kh = new KhachHang();
            kh.Ho = txtHo.Text;
            kh.Ten = txtTen.Text;
            kh.SDT = txtSDT.Text;
            kh.Email = txtEmail.Text;
            kh.DiaChi = txtDiaChi.Text;
            kh.NgaySinh = dtpNgaySinh.Checked ? (DateTime?)dtpNgaySinh.Value.Date : null;

            int maKH;
            string loi = bus.DangKyKhachHang(kh, txtTenDN.Text, txtMk.Text, txtNhapLai.Text, out maKH);
            if (loi != "")
            {
                UiHelper.Loi(loi);
                return;
            }
            TenDangNhapMoi = txtTenDN.Text.Trim();
            MaKHMoi = maKH;
            MessageBox.Show("Đăng ký thành công. Tên đăng nhập: " + TenDangNhapMoi, "Thông báo");
            DialogResult = DialogResult.OK;
        }
    }
}
