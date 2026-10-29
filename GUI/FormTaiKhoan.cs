using System;
using System.Collections;
using System.Drawing;
using System.Windows.Forms;
using QuanLyBanDienThoai.BUS;
using QuanLyBanDienThoai.DTO;

namespace QuanLyBanDienThoai.GUI
{
    // Quản lý tài khoản đăng nhập (chỉ Admin thấy trong menu). Chỉ gọi BUS, không viết SQL.
    // Tài khoản luôn gắn với một nhân viên hoặc khách hàng CÓ SẴN (chọn bằng nút "..."), nên không lệch dữ liệu với ERD.
    public class FormTaiKhoan : Form
    {
        private TaiKhoanBUS bus = new TaiKhoanBUS();
        private ArrayList dsHienTai = new ArrayList();
        private DataGridView dgv;
        private TextBox txtTimKiem, txtDoiTuong, txtTenDN, txtMk;
        private ComboBox cboVaiTro;
        private int maDoiTuong = 0;

        public FormTaiKhoan()
        {
            Text = "Quản lý tài khoản";
            TaoGiaoDien();
            TaiDuLieu();
        }

        private void TaoGiaoDien()
        {
            dgv = new DataGridView();
            dgv.Dock = DockStyle.Fill;
            GridHelper.CauHinh(dgv);

            Panel pnlTim = new Panel();
            pnlTim.Dock = DockStyle.Top;
            pnlTim.Height = 46;
            txtTimKiem = UiHelper.O(75, 11, 200);
            txtTimKiem.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Enter)
                {
                    e.SuppressKeyPress = true;
                    HienDanhSach(bus.TimKiem(txtTimKiem.Text));
                }
            };
            Button btnTim = UiHelper.Nut("Tìm", 285, 8, 65);
            btnTim.Click += (s, e) => HienDanhSach(bus.TimKiem(txtTimKiem.Text));
            Button btnTatCa = UiHelper.Nut("Hiện tất cả", 355, 8, 95);
            btnTatCa.Click += (s, e) => TaiDuLieu();
            pnlTim.Controls.Add(UiHelper.Nhan("Tìm kiếm:", 10, 8));
            pnlTim.Controls.AddRange(new Control[] { txtTimKiem, btnTim, btnTatCa });

            Panel pnlNhap = new Panel();
            pnlNhap.Dock = DockStyle.Bottom;
            pnlNhap.Height = 170;

            cboVaiTro = new ComboBox();
            cboVaiTro.DropDownStyle = ComboBoxStyle.DropDownList;
            cboVaiTro.Items.AddRange(new object[] { "Khách hàng", "Nhân viên", "Quản trị viên (Admin)" });
            cboVaiTro.SelectedIndex = 1;
            cboVaiTro.Location = new Point(120, 12);
            cboVaiTro.Width = 200;
            cboVaiTro.SelectedIndexChanged += (s, e) =>
            {
                maDoiTuong = 0;          // đổi vai trò thì phải chọn lại người
                txtDoiTuong.Clear();
            };

            txtDoiTuong = UiHelper.O(120, 46, 200);
            txtDoiTuong.ReadOnly = true;
            Button btnChon = UiHelper.Nut("…", 325, 44, 36);
            btnChon.Click += btnChonDoiTuong_Click;
            txtTenDN = UiHelper.O(120, 80, 200);
            txtMk = UiHelper.O(120, 114, 200);
            txtMk.UseSystemPasswordChar = true;

            Button btnTao = UiHelper.Nut("Tạo tài khoản", 400, 10, 140);
            btnTao.Click += btnTao_Click;
            Button btnDatLai = UiHelper.Nut("Đặt lại mật khẩu", 400, 46, 140);
            btnDatLai.Click += btnDatLai_Click;
            Button btnKhoa = UiHelper.Nut("Khóa / Mở khóa", 400, 82, 140);
            btnKhoa.Click += btnKhoa_Click;
            Button btnXoa = UiHelper.Nut("Xóa", 400, 118, 140);
            btnXoa.Click += btnXoa_Click;

            Label lblHuongDan = new Label();
            lblHuongDan.Location = new Point(560, 12);
            lblHuongDan.Size = new Size(330, 120);
            lblHuongDan.ForeColor = Color.Gray;
            lblHuongDan.Text =
                "Tạo tài khoản: chọn vai trò, bấm … để chọn nhân viên/khách hàng đã có, nhập tên đăng nhập và mật khẩu.\r\n\r\n" +
                "Đặt lại mật khẩu, Khóa/Mở khóa, Xóa: chọn một tài khoản trên lưới. " +
                "Đặt lại mật khẩu dùng ô Mật khẩu ở bên trái.";

            pnlNhap.Controls.Add(UiHelper.Nhan("Vai trò:", 15, 12));
            pnlNhap.Controls.Add(cboVaiTro);
            pnlNhap.Controls.Add(UiHelper.Nhan("Người dùng:", 15, 46));
            pnlNhap.Controls.AddRange(new Control[] { txtDoiTuong, btnChon });
            pnlNhap.Controls.Add(UiHelper.Nhan("Tên đăng nhập:", 15, 80));
            pnlNhap.Controls.Add(txtTenDN);
            pnlNhap.Controls.Add(UiHelper.Nhan("Mật khẩu:", 15, 114));
            pnlNhap.Controls.Add(txtMk);
            pnlNhap.Controls.AddRange(new Control[] { btnTao, btnDatLai, btnKhoa, btnXoa, lblHuongDan });

            // Fill thêm vào trước, các thanh Top/Bottom thêm sau
            Controls.Add(dgv);
            Controls.Add(pnlNhap);
            Controls.Add(pnlTim);
        }

        private string VaiTroDaChon()
        {
            switch (cboVaiTro.SelectedIndex)
            {
                case 0: return VaiTroTK.KhachHang;
                case 2: return VaiTroTK.Admin;
                default: return VaiTroTK.NhanVien;
            }
        }

        private void TaiDuLieu()
        {
            txtTimKiem.Clear();
            HienDanhSach(bus.GetAll());
        }

        private void HienDanhSach(ArrayList ds)
        {
            dsHienTai = ds;
            GridHelper.GanDuLieu(dgv, ds, typeof(TaiKhoan));
            UiHelper.Giau(dgv, "MatKhau");        // không hiện mã băm mật khẩu
            UiHelper.TieuDe(dgv, "TenDangNhap", "Tên đăng nhập");
            UiHelper.TieuDe(dgv, "VaiTro", "Vai trò");
            UiHelper.TieuDe(dgv, "MaNV", "Mã NV");
            UiHelper.TieuDe(dgv, "MaKH", "Mã KH");
            UiHelper.TieuDe(dgv, "TrangThai", "Hoạt động");
            UiHelper.TieuDe(dgv, "HoTen", "Họ tên");
            GridHelper.BoChon(dgv);
        }

        private void btnChonDoiTuong_Click(object sender, EventArgs e)
        {
            if (VaiTroDaChon() == VaiTroTK.KhachHang)
            {
                KhachHang kh = FormChon.Chon(new KhachHangBUS().GetAll(),
                    new string[] { "MaKH", "HoTen", "SDT", "Email" }, "Chọn khách hàng") as KhachHang;
                if (kh == null) return;
                maDoiTuong = kh.MaKH;
                txtDoiTuong.Text = kh.HoTen;
            }
            else
            {
                NhanVien nv = FormChon.Chon(new NhanVienBUS().GetAll(),
                    new string[] { "MaNV", "HoTen", "ChucVu", "SDT" }, "Chọn nhân viên") as NhanVien;
                if (nv == null) return;
                maDoiTuong = nv.MaNV;
                txtDoiTuong.Text = nv.HoTen;
            }
        }

        private void btnTao_Click(object sender, EventArgs e)
        {
            string loi = bus.TaoTaiKhoan(VaiTroDaChon(), maDoiTuong, txtTenDN.Text, txtMk.Text);
            if (loi != "") { UiHelper.Loi(loi); return; }
            MessageBox.Show("Đã tạo tài khoản " + txtTenDN.Text.Trim());
            txtTenDN.Clear();
            txtMk.Clear();
            maDoiTuong = 0;
            txtDoiTuong.Clear();
            TaiDuLieu();
        }

        private TaiKhoan DangChon()
        {
            TaiKhoan tk = UiHelper.DongDangChon<TaiKhoan>(dgv);
            if (tk == null) MessageBox.Show("Hãy chọn một tài khoản trên lưới");
            return tk;
        }

        private void btnDatLai_Click(object sender, EventArgs e)
        {
            TaiKhoan tk = DangChon();
            if (tk == null) return;
            string loi = bus.DatLaiMatKhau(tk.TenDangNhap, txtMk.Text);
            if (loi != "") { UiHelper.Loi(loi); return; }
            txtMk.Clear();
            MessageBox.Show("Đã đặt lại mật khẩu cho " + tk.TenDangNhap);
        }

        private void btnKhoa_Click(object sender, EventArgs e)
        {
            TaiKhoan tk = DangChon();
            if (tk == null) return;
            string loi = bus.KhoaMo(tk.TenDangNhap, tk.TrangThai);   // đang hoạt động thì khóa, đang khóa thì mở
            if (loi != "") { UiHelper.Loi(loi); return; }
            TaiDuLieu();
        }

        private void btnXoa_Click(object sender, EventArgs e)
        {
            TaiKhoan tk = DangChon();
            if (tk == null) return;
            if (MessageBox.Show("Xóa tài khoản \"" + tk.TenDangNhap + "\"? (Nhân viên/khách hàng vẫn được giữ lại)", "Xác nhận",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;
            string loi = bus.Xoa(tk.TenDangNhap);
            if (loi != "") { UiHelper.Loi(loi); return; }
            TaiDuLieu();
        }
    }
}
