using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using QuanLyBanDienThoai.BUS;
using QuanLyBanDienThoai.DTO;

namespace QuanLyBanDienThoai.GUI
{
    // Trang dành cho khách hàng sau khi đăng nhập. Giao diện giống MainForm của Admin và Nhân viên:
    // menu tối bên trái, thanh xanh phía trên, vùng nội dung bên phải có tiêu đề ở giữa và chân trang.
    // Chỉ gọi BUS, không viết SQL. Mọi dữ liệu đều theo MaKH của tài khoản đang đăng nhập (BUS lấy từ PhienDangNhap):
    //   Cửa hàng (DIENTHOAI + khuyến mãi) | Giỏ hàng (GIOHANG) | Đơn mua (HOADON, CHITIETHOADON) | Bảo hành (BAOHANH) | Thông tin (KHACHHANG, TAIKHOAN)
    public class FormTrangKhachHang : Form
    {
        // Màu theo giao diện MainForm
        private static readonly Color MauXanh = Color.FromArgb(0, 166, 90);
        private static readonly Color MauXanhDam = Color.FromArgb(0, 141, 76);
        private static readonly Color MauMenu = Color.FromArgb(34, 45, 50);
        private static readonly Color MauMenuToi = Color.FromArgb(30, 40, 44);
        private static readonly Color MauMenuCon = Color.FromArgb(44, 59, 65);
        private static readonly Color MauMenuConHover = Color.FromArgb(52, 70, 77);
        private static readonly Color MauChuMenu = Color.FromArgb(184, 199, 206);
        private static readonly Color MauChuMenuCon = Color.FromArgb(139, 164, 175);
        private static readonly Color MauNen = Color.FromArgb(236, 240, 245);

        private SanPhamBanBUS spBus = new SanPhamBanBUS();
        private GioHangBUS gioBus = new GioHangBUS();
        private HoaDonBUS hdBus = new HoaDonBUS();
        private BaoHanhBUS bhBus = new BaoHanhBUS();
        private KhachHangBUS khBus = new KhachHangBUS();
        private TaiKhoanBUS tkBus = new TaiKhoanBUS();

        // Khung giao diện
        private Panel pnlSidebar;
        private FlowLayoutPanel pnlMenu;
        private Panel pnlForm;                 // vùng chứa các trang
        private Label lblTieuDe;               // tiêu đề ở giữa phía trên
        private Label lblNguoiDung, lblTenMenu, lblDiem;
        private Panel avatarTren, avatarMenu;
        private Panel[] trang = new Panel[5];  // 0 Cửa hàng, 1 Giỏ hàng, 2 Đơn mua, 3 Bảo hành, 4 Thông tin
        private Button btnDangChon;            // mục con đang được chọn
        private List<FlowLayoutPanel> cacNhom = new List<FlowLayoutPanel>();
        private List<Button> cacNutNhom = new List<Button>();
        private List<Button> cacNutCon = new List<Button>();

        private DataGridView dgvSP;
        private TextBox txtTim;
        private NumericUpDown numSL;

        private DataGridView dgvGio;
        private NumericUpDown numSLGio;
        private Label lblTongGio;

        private DataGridView dgvHD, dgvCT;
        private Label lblTongDon;

        private DataGridView dgvBH;

        private TextBox txtHo, txtTen, txtSDT, txtEmail, txtDiaChi, txtDiem, txtMkCu, txtMkMoi, txtNhapLai;
        private DateTimePicker dtpNgaySinh;

        public FormTrangKhachHang()
        {
            Text = "Quản lý cửa hàng bán điện thoại";
            Size = new Size(1280, 760);
            MinimumSize = new Size(1000, 600);
            StartPosition = FormStartPosition.CenterScreen;

            // ===================== Các trang nội dung (chỉ hiện một trang tại một thời điểm) =====================
            trang[0] = TaoTabCuaHang();
            trang[1] = TaoTabGioHang();
            trang[2] = TaoTabDonMua();
            trang[3] = TaoTabBaoHanh();
            trang[4] = TaoTabThongTin();

            // ===================== Vùng bên phải =====================
            Panel pnlRight = new Panel();
            pnlRight.Dock = DockStyle.Fill;
            pnlRight.BackColor = MauNen;

            pnlForm = new Panel();
            pnlForm.Dock = DockStyle.Fill;
            pnlForm.BackColor = Color.White;
            foreach (Panel p in trang)
            {
                p.Visible = false;
                pnlForm.Controls.Add(p);
            }

            lblTieuDe = new Label();
            lblTieuDe.Dock = DockStyle.Top;
            lblTieuDe.Height = 52;
            lblTieuDe.Font = new Font("Segoe UI", 14, FontStyle.Bold);
            lblTieuDe.ForeColor = Color.FromArgb(60, 72, 80);
            lblTieuDe.BackColor = Color.White;
            lblTieuDe.TextAlign = ContentAlignment.MiddleCenter;
            lblTieuDe.Text = "Cửa hàng";
            lblTieuDe.Paint += (s, e) =>
            {
                using (Pen pen = new Pen(Color.FromArgb(225, 229, 233)))
                    e.Graphics.DrawLine(pen, 0, lblTieuDe.Height - 1, lblTieuDe.Width, lblTieuDe.Height - 1);
            };
            lblTieuDe.Resize += (s, e) => lblTieuDe.Invalidate();

            Panel pnlCard = new Panel();
            pnlCard.Dock = DockStyle.Fill;
            pnlCard.BackColor = Color.White;
            pnlCard.Padding = new Padding(0, 3, 0, 0);
            pnlCard.Paint += (s, e) =>
            {
                using (SolidBrush b = new SolidBrush(MauXanh))
                    e.Graphics.FillRectangle(b, 0, 0, pnlCard.Width, 3);
            };
            pnlCard.Resize += (s, e) => pnlCard.Invalidate();
            // Fill thêm vào trước, tiêu đề (Top) thêm sau
            pnlCard.Controls.Add(pnlForm);
            pnlCard.Controls.Add(lblTieuDe);

            Panel pnlNen = new Panel();
            pnlNen.Dock = DockStyle.Fill;
            pnlNen.BackColor = MauNen;
            pnlNen.Padding = new Padding(14);
            pnlNen.Controls.Add(pnlCard);

            Label lblChan = new Label();
            lblChan.Dock = DockStyle.Bottom;
            lblChan.Height = 34;
            lblChan.BackColor = Color.White;
            lblChan.ForeColor = Color.FromArgb(115, 135, 145);
            lblChan.Font = new Font("Segoe UI", 9);
            lblChan.Padding = new Padding(16, 0, 0, 0);
            lblChan.TextAlign = ContentAlignment.MiddleLeft;
            lblChan.Text = "Copyright © 2026 Quản lý cửa hàng bán điện thoại. Đồ án học phần Lập trình C#.";

            // Thứ tự thêm: Fill trước, Bottom, rồi Top sau cùng
            pnlRight.Controls.Add(pnlNen);
            pnlRight.Controls.Add(lblChan);
            pnlRight.Controls.Add(TaoThanhTren());

            // ===================== Menu dọc bên trái =====================
            pnlSidebar = new Panel();
            pnlSidebar.Dock = DockStyle.Left;
            pnlSidebar.Width = 230;
            pnlSidebar.BackColor = MauMenu;

            pnlMenu = new FlowLayoutPanel();
            pnlMenu.Dock = DockStyle.Fill;
            pnlMenu.FlowDirection = FlowDirection.TopDown;
            pnlMenu.WrapContents = false;
            pnlMenu.AutoScroll = true;
            pnlMenu.BackColor = MauMenu;
            pnlMenu.Padding = new Padding(0);
            pnlMenu.ClientSizeChanged += (s, e) => CanChinhDoRong();

            Label lblNav = new Label();
            lblNav.Dock = DockStyle.Top;
            lblNav.Height = 30;
            lblNav.Text = "CHỨC NĂNG";
            lblNav.Font = new Font("Segoe UI", 8.5f, FontStyle.Bold);
            lblNav.ForeColor = Color.FromArgb(75, 107, 120);
            lblNav.BackColor = MauMenuToi;
            lblNav.Padding = new Padding(14, 0, 0, 0);
            lblNav.TextAlign = ContentAlignment.MiddleLeft;

            // Thứ tự thêm: Fill trước, các khung Top từ dưới lên trên (khung thêm sau cùng nằm trên cùng)
            pnlSidebar.Controls.Add(pnlMenu);
            pnlSidebar.Controls.Add(lblNav);
            pnlSidebar.Controls.Add(TaoKhuNguoiDung());
            pnlSidebar.Controls.Add(TaoThuongHieu());

            // Fill thêm trước, menu trái (Left) thêm sau
            Controls.Add(pnlRight);
            Controls.Add(pnlSidebar);

            Button nutDau = TaoMenu();
            CanChinhDoRong();
            Load += (s, e) => CanChinhDoRong();

            // Mở sẵn trang Cửa hàng
            MoTrang(0, "Cửa hàng", nutDau);
        }

        // ===================== Khung giao diện (giống MainForm) =====================

        // Thanh xanh phía trên: nút ẩn/hiện menu, tên hệ thống, điểm tích lũy, người dùng, đăng xuất
        private Panel TaoThanhTren()
        {
            Panel p = new Panel();
            p.Dock = DockStyle.Top;
            p.Height = 50;
            p.BackColor = MauXanh;

            Label lblHeThong = new Label();
            lblHeThong.Dock = DockStyle.Fill;
            lblHeThong.Text = "Hệ thống quản lý cửa hàng bán điện thoại";
            lblHeThong.ForeColor = Color.White;
            lblHeThong.Font = new Font("Segoe UI", 10.5f);
            lblHeThong.TextAlign = ContentAlignment.MiddleLeft;

            lblDiem = new Label();
            lblDiem.Dock = DockStyle.Right;
            lblDiem.Width = 190;
            lblDiem.ForeColor = Color.White;
            lblDiem.Font = new Font("Segoe UI", 10);
            lblDiem.TextAlign = ContentAlignment.MiddleRight;

            avatarTren = TaoAvatar(34, MauXanhDam, 50);
            avatarTren.Dock = DockStyle.Right;
            avatarTren.BackColor = MauXanh;

            lblNguoiDung = new Label();
            lblNguoiDung.Dock = DockStyle.Right;
            lblNguoiDung.Width = 250;
            lblNguoiDung.ForeColor = Color.White;
            lblNguoiDung.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            lblNguoiDung.TextAlign = ContentAlignment.MiddleLeft;

            Button btnAn = new Button();
            btnAn.Dock = DockStyle.Left;
            btnAn.Width = 56;
            btnAn.Text = "☰";
            btnAn.Font = new Font("Segoe UI Symbol", 14);
            btnAn.ForeColor = Color.White;
            btnAn.BackColor = MauXanh;
            btnAn.FlatStyle = FlatStyle.Flat;
            btnAn.FlatAppearance.BorderSize = 0;
            btnAn.FlatAppearance.MouseOverBackColor = MauXanhDam;
            btnAn.FlatAppearance.MouseDownBackColor = MauXanhDam;
            btnAn.UseVisualStyleBackColor = false;
            btnAn.Cursor = Cursors.Hand;
            btnAn.Click += (s, e) => pnlSidebar.Visible = !pnlSidebar.Visible;

            Button btnDangXuat = new Button();
            btnDangXuat.Dock = DockStyle.Right;
            btnDangXuat.Width = 110;
            btnDangXuat.Text = "Đăng xuất";
            btnDangXuat.Font = new Font("Segoe UI", 10);
            btnDangXuat.ForeColor = Color.White;
            btnDangXuat.BackColor = MauXanhDam;
            btnDangXuat.FlatStyle = FlatStyle.Flat;
            btnDangXuat.FlatAppearance.BorderSize = 0;
            btnDangXuat.UseVisualStyleBackColor = false;
            btnDangXuat.Cursor = Cursors.Hand;
            btnDangXuat.Click += (s, e) =>
            {
                if (MessageBox.Show("Đăng xuất khỏi tài khoản này?", "Xác nhận",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;
                PhienDangNhap.YeuCauDangXuat = true;     // Program quay lại màn hình đăng nhập
                Close();
            };

            // Fill trước, rồi Right, Left thêm sau (nút Đăng xuất thêm sau cùng nên nằm sát mép phải)
            p.Controls.Add(lblHeThong);
            p.Controls.Add(lblDiem);
            p.Controls.Add(avatarTren);
            p.Controls.Add(lblNguoiDung);
            p.Controls.Add(btnDangXuat);
            p.Controls.Add(btnAn);
            return p;
        }

        // Khung thương hiệu trên cùng của menu trái
        private Panel TaoThuongHieu()
        {
            Panel p = new Panel();
            p.Dock = DockStyle.Top;
            p.Height = 50;
            p.BackColor = MauXanhDam;

            Label lbl = new Label();
            lbl.Dock = DockStyle.Fill;
            lbl.Text = "☎  PhoneStore";
            lbl.ForeColor = Color.White;
            lbl.Font = new Font("Segoe UI", 15, FontStyle.Bold);
            lbl.TextAlign = ContentAlignment.MiddleCenter;
            p.Controls.Add(lbl);
            return p;
        }

        // Khung người dùng dưới thương hiệu: ảnh tròn, tên, vai trò
        private Panel TaoKhuNguoiDung()
        {
            Panel p = new Panel();
            p.Dock = DockStyle.Top;
            p.Height = 66;
            p.BackColor = MauMenu;

            lblTenMenu = new Label();
            lblTenMenu.Dock = DockStyle.Fill;
            lblTenMenu.ForeColor = Color.White;
            lblTenMenu.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            lblTenMenu.TextAlign = ContentAlignment.BottomLeft;

            Label lblTrangThai = new Label();
            lblTrangThai.Dock = DockStyle.Bottom;
            lblTrangThai.Height = 26;
            lblTrangThai.Text = "●  " + PhienDangNhap.TenVaiTroHienTai;
            lblTrangThai.ForeColor = MauXanh;
            lblTrangThai.Font = new Font("Segoe UI", 8.5f);
            lblTrangThai.TextAlign = ContentAlignment.TopLeft;

            Panel pnlChu = new Panel();
            pnlChu.Dock = DockStyle.Fill;
            pnlChu.BackColor = MauMenu;
            pnlChu.Controls.Add(lblTenMenu);
            pnlChu.Controls.Add(lblTrangThai);

            avatarMenu = TaoAvatar(40, Color.FromArgb(96, 125, 139), 64);
            avatarMenu.Dock = DockStyle.Left;
            avatarMenu.BackColor = MauMenu;

            p.Controls.Add(pnlChu);
            p.Controls.Add(avatarMenu);
            return p;
        }

        // Ảnh đại diện hình tròn có hai chữ cái (lấy từ tên người đang đăng nhập, đổi tên thì vẽ lại)
        private Panel TaoAvatar(int kichThuoc, Color mauNen, int rong)
        {
            Panel p = new Panel();
            p.Width = rong;
            Font f = new Font("Segoe UI", kichThuoc / 3.2f, FontStyle.Bold);
            p.Paint += (s, e) =>
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                int x = (p.Width - kichThuoc) / 2;
                int y = (p.Height - kichThuoc) / 2;
                using (SolidBrush b = new SolidBrush(mauNen))
                    e.Graphics.FillEllipse(b, x, y, kichThuoc, kichThuoc);
                TextRenderer.DrawText(e.Graphics, PhienDangNhap.VietTat, f, new Rectangle(x, y, kichThuoc, kichThuoc), Color.White,
                    TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
            };
            p.Resize += (s, e) => p.Invalidate();
            return p;
        }

        // Menu của khách hàng. Trả về nút của trang đầu tiên (Cửa hàng) để mở sẵn.
        private Button TaoMenu()
        {
            FlowLayoutPanel nMuaSam = TaoNhom("Mua sắm");
            Button nutCuaHang = ThemMuc(nMuaSam, "Cửa hàng", 0);
            ThemMuc(nMuaSam, "Giỏ hàng", 1);

            FlowLayoutPanel nDonHang = TaoNhom("Đơn hàng");
            ThemMuc(nDonHang, "Đơn mua của tôi", 2);
            ThemMuc(nDonHang, "Bảo hành", 3);

            FlowLayoutPanel nTaiKhoan = TaoNhom("Tài khoản");
            ThemMuc(nTaiKhoan, "Thông tin cá nhân", 4);

            // Mở sẵn nhóm đầu tiên
            cacNhom[0].Visible = true;
            return nutCuaHang;
        }

        private static string LayBieuTuong(string tenNhom)
        {
            switch (tenNhom)
            {
                case "Mua sắm": return "¤";
                case "Đơn hàng": return "▦";
                case "Tài khoản": return "♟";
                default: return "■";
            }
        }

        // Tạo nút tiêu đề nhóm và khung chứa các mục con (ẩn sẵn). Bấm tiêu đề thì mở/đóng nhóm.
        private FlowLayoutPanel TaoNhom(string tieuDe)
        {
            FlowLayoutPanel nhom = new FlowLayoutPanel();
            nhom.FlowDirection = FlowDirection.TopDown;
            nhom.WrapContents = false;
            nhom.AutoSize = true;
            nhom.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            nhom.Margin = new Padding(0);
            nhom.Padding = new Padding(0);
            nhom.BackColor = MauMenuCon;
            nhom.Visible = false;

            Button btn = new Button();
            btn.Text = LayBieuTuong(tieuDe) + "   " + tieuDe;
            btn.Width = 228;
            btn.Height = 44;
            btn.Margin = new Padding(0);
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 0;
            btn.FlatAppearance.MouseOverBackColor = MauMenuToi;
            btn.FlatAppearance.MouseDownBackColor = MauMenuToi;
            btn.UseVisualStyleBackColor = false;
            btn.BackColor = MauMenu;
            btn.ForeColor = MauChuMenu;
            btn.Font = new Font("Segoe UI", 10);
            btn.TextAlign = ContentAlignment.MiddleLeft;
            btn.Padding = new Padding(14, 0, 0, 0);
            btn.Cursor = Cursors.Hand;

            // Mũi tên bên phải; vạch xanh bên trái khi nhóm đang mở
            btn.Paint += (s, e) =>
            {
                TextRenderer.DrawText(e.Graphics, nhom.Visible ? "▾" : "‹", btn.Font,
                    new Rectangle(btn.Width - 36, 0, 28, btn.Height), MauChuMenu,
                    TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
                if (nhom.Visible)
                {
                    using (SolidBrush b = new SolidBrush(MauXanh))
                        e.Graphics.FillRectangle(b, 0, 0, 4, btn.Height);
                }
            };

            nhom.VisibleChanged += (s, e) =>
            {
                btn.BackColor = nhom.Visible ? MauMenuToi : MauMenu;
                btn.ForeColor = nhom.Visible ? Color.White : MauChuMenu;
                btn.Invalidate();
            };

            btn.Click += (s, e) =>
            {
                bool hien = !nhom.Visible;
                foreach (FlowLayoutPanel n in cacNhom) n.Visible = false;
                nhom.Visible = hien;
            };

            cacNhom.Add(nhom);
            cacNutNhom.Add(btn);
            pnlMenu.Controls.Add(btn);
            pnlMenu.Controls.Add(nhom);
            return nhom;
        }

        // Thêm một mục con vào nhóm; bấm vào thì hiện trang có số thứ tự chiSo
        private Button ThemMuc(FlowLayoutPanel nhom, string ten, int chiSo)
        {
            Button btn = new Button();
            btn.Text = "○   " + ten;
            btn.Width = 228;
            btn.Height = 38;
            btn.Margin = new Padding(0);
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 0;
            btn.FlatAppearance.MouseOverBackColor = MauMenuConHover;
            btn.FlatAppearance.MouseDownBackColor = MauMenuConHover;
            btn.UseVisualStyleBackColor = false;
            btn.BackColor = MauMenuCon;
            btn.ForeColor = MauChuMenuCon;
            btn.Font = new Font("Segoe UI", 9.5f);
            btn.TextAlign = ContentAlignment.MiddleLeft;
            btn.Padding = new Padding(30, 0, 0, 0);
            btn.Cursor = Cursors.Hand;

            btn.MouseEnter += (s, e) => btn.ForeColor = Color.White;
            btn.MouseLeave += (s, e) => { if (btn != btnDangChon) btn.ForeColor = MauChuMenuCon; };
            btn.Click += (s, e) => MoTrang(chiSo, ten, btn);

            nhom.Controls.Add(btn);
            cacNutCon.Add(btn);
            return btn;
        }

        // Đánh dấu mục con đang chọn (chấm đặc, chữ trắng); bỏ đánh dấu mục trước
        private void DanhDauMuc(Button btn)
        {
            if (btnDangChon != null && btnDangChon != btn)
            {
                btnDangChon.Text = "○" + btnDangChon.Text.Substring(1);
                btnDangChon.ForeColor = MauChuMenuCon;
            }
            btnDangChon = btn;
            btn.Text = "●" + btn.Text.Substring(1);
            btn.ForeColor = Color.White;
        }

        // Cho các nút menu rộng đúng bằng vùng menu (kể cả khi có thanh cuộn)
        private void CanChinhDoRong()
        {
            int w = pnlMenu.ClientSize.Width;
            if (w <= 0) return;
            foreach (Button b in cacNutNhom) b.Width = w;
            foreach (Button b in cacNutCon) b.Width = w;
        }

        // Hiện trang số chiSo, đổi tiêu đề, đánh dấu mục menu và nạp lại dữ liệu của trang đó
        private void MoTrang(int chiSo, string tieuDe, Button btn)
        {
            for (int i = 0; i < trang.Length; i++) trang[i].Visible = (i == chiSo);
            trang[chiSo].BringToFront();
            lblTieuDe.Text = tieuDe;
            DanhDauMuc(btn);
            CapNhatThanhTren();
            NapTrang(chiSo);
        }

        // Tạo một trang nội dung rỗng (thay cho tab cũ)
        private Panel TaoTrang()
        {
            Panel p = new Panel();
            p.Dock = DockStyle.Fill;
            p.BackColor = Color.White;
            return p;
        }

        private void CapNhatThanhTren()
        {
            lblNguoiDung.Text = PhienDangNhap.HoTen + " (" + PhienDangNhap.TenVaiTroHienTai + ")";
            lblTenMenu.Text = PhienDangNhap.HoTen;
            avatarTren.Invalidate();
            avatarMenu.Invalidate();
            KhachHang kh = khBus.GetById(PhienDangNhap.MaKH);
            lblDiem.Text = kh == null ? "" : "Điểm tích lũy: " + kh.DiemTichLuy.ToString("#,##0");
        }

        private void NapTrang(int chiSo)
        {
            switch (chiSo)
            {
                case 0: NapSanPham(spBus.TimKiem(txtTim.Text)); break;
                case 1: NapGio(); break;
                case 2: NapDonMua(); break;
                case 3: NapBaoHanh(); break;
                case 4: NapThongTin(); break;
            }
        }

        // ===================== Tab 1: Cửa hàng =====================
        private Panel TaoTabCuaHang()
        {
            Panel t = TaoTrang();

            dgvSP = new DataGridView();
            dgvSP.Dock = DockStyle.Fill;
            GridHelper.CauHinh(dgvSP);

            Panel pnl = new Panel();
            pnl.Dock = DockStyle.Top;
            pnl.Height = 48;
            txtTim = UiHelper.O(75, 10, 220);
            txtTim.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Enter)
                {
                    e.SuppressKeyPress = true;
                    NapSanPham(spBus.TimKiem(txtTim.Text));
                }
            };
            Button btnTim = UiHelper.Nut("Tìm", 305, 8, 70);
            btnTim.Click += (s, e) => NapSanPham(spBus.TimKiem(txtTim.Text));
            Button btnTatCa = UiHelper.Nut("Hiện tất cả", 380, 8, 100);
            btnTatCa.Click += (s, e) => { txtTim.Clear(); NapSanPham(spBus.GetAll()); };
            numSL = new NumericUpDown();
            numSL.Location = new Point(640, 11);
            numSL.Width = 60;
            numSL.Minimum = 1;
            numSL.Maximum = 99;
            Button btnThem = UiHelper.Nut("Thêm vào giỏ", 710, 8, 130);
            btnThem.Click += btnThemGio_Click;
            pnl.Controls.Add(UiHelper.Nhan("Tìm kiếm:", 8, 8));
            pnl.Controls.Add(txtTim);
            pnl.Controls.AddRange(new Control[] { btnTim, btnTatCa });
            pnl.Controls.Add(UiHelper.Nhan("Số lượng:", 570, 8));
            pnl.Controls.Add(numSL);
            pnl.Controls.Add(btnThem);

            Label lblGhiChu = new Label();
            lblGhiChu.Dock = DockStyle.Bottom;
            lblGhiChu.Height = 30;
            lblGhiChu.ForeColor = Color.Gray;
            lblGhiChu.TextAlign = ContentAlignment.MiddleLeft;
            lblGhiChu.Text = "  Giá sau giảm đã áp dụng khuyến mãi đang có hiệu lực hôm nay. Mang giỏ hàng đến quầy, nhân viên sẽ lập hóa đơn cho bạn.";

            t.Controls.Add(dgvSP);
            t.Controls.Add(lblGhiChu);
            t.Controls.Add(pnl);
            return t;
        }

        private void NapSanPham(ArrayList ds)
        {
            GridHelper.GanDuLieu(dgvSP, ds, typeof(SanPhamBan));
            UiHelper.Giau(dgvSP, "MaKM");
            UiHelper.TieuDe(dgvSP, "MaDT", "Mã");
            UiHelper.TieuDe(dgvSP, "TenHienThi", "Điện thoại");
            UiHelper.TieuDe(dgvSP, "TenHang", "Hãng");
            UiHelper.TieuDe(dgvSP, "GiaNiemYet", "Giá niêm yết");
            UiHelper.TieuDe(dgvSP, "PhanTramGiam", "Giảm (%)");
            UiHelper.TieuDe(dgvSP, "GiaSauGiam", "Giá sau giảm");
            UiHelper.TieuDe(dgvSP, "SoLuongTon", "Còn lại");
            UiHelper.TieuDe(dgvSP, "TenKM", "Khuyến mãi");
            UiHelper.DinhDangPhanTram(dgvSP, "PhanTramGiam");
            GridHelper.BoChon(dgvSP);
        }

        private void btnThemGio_Click(object sender, EventArgs e)
        {
            SanPhamBan sp = UiHelper.DongDangChon<SanPhamBan>(dgvSP);
            if (sp == null) { MessageBox.Show("Hãy chọn một điện thoại"); return; }
            string loi = gioBus.ThemVaoGio(sp.MaDT, (int)numSL.Value);
            if (loi != "") { UiHelper.Loi(loi); return; }
            MessageBox.Show("Đã thêm \"" + sp.TenHienThi + "\" vào giỏ hàng");
        }

        // ===================== Tab 2: Giỏ hàng =====================
        private Panel TaoTabGioHang()
        {
            Panel t = TaoTrang();

            dgvGio = new DataGridView();
            dgvGio.Dock = DockStyle.Fill;
            GridHelper.CauHinh(dgvGio);
            dgvGio.SelectionChanged += (s, e) =>
            {
                GioHang g = UiHelper.DongDangChon<GioHang>(dgvGio);
                if (g != null) numSLGio.Value = Math.Max(numSLGio.Minimum, Math.Min(numSLGio.Maximum, g.SoLuong));
            };

            Panel pnl = new Panel();
            pnl.Dock = DockStyle.Bottom;
            pnl.Height = 80;
            numSLGio = new NumericUpDown();
            numSLGio.Location = new Point(80, 13);
            numSLGio.Width = 60;
            numSLGio.Minimum = 1;
            numSLGio.Maximum = 99;
            Button btnCapNhat = UiHelper.Nut("Cập nhật số lượng", 155, 10, 150);
            btnCapNhat.Click += btnCapNhatGio_Click;
            Button btnXoa = UiHelper.Nut("Xóa khỏi giỏ", 315, 10, 120);
            btnXoa.Click += btnXoaGio_Click;
            Button btnLamMoi = UiHelper.Nut("Làm mới", 445, 10, 100);
            btnLamMoi.Click += (s, e) => NapGio();
            lblTongGio = new Label();
            lblTongGio.AutoSize = true;
            lblTongGio.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            lblTongGio.ForeColor = Color.FromArgb(200, 40, 40);
            lblTongGio.Location = new Point(10, 48);
            pnl.Controls.Add(UiHelper.Nhan("Số lượng:", 8, 10));
            pnl.Controls.AddRange(new Control[] { numSLGio, btnCapNhat, btnXoa, btnLamMoi, lblTongGio });

            t.Controls.Add(dgvGio);
            t.Controls.Add(pnl);
            return t;
        }

        private void NapGio()
        {
            ArrayList ds = gioBus.GetCuaToi();
            GridHelper.GanDuLieu(dgvGio, ds, typeof(GioHang));
            UiHelper.Giau(dgvGio, "MaKH", "MaKM");
            UiHelper.TieuDe(dgvGio, "MaDT", "Mã");
            UiHelper.TieuDe(dgvGio, "SoLuong", "Số lượng");
            UiHelper.TieuDe(dgvGio, "NgayThem", "Ngày thêm");
            UiHelper.TieuDe(dgvGio, "TenHienThi", "Điện thoại");
            UiHelper.TieuDe(dgvGio, "GiaNiemYet", "Giá niêm yết");
            UiHelper.TieuDe(dgvGio, "SoLuongTon", "Còn lại");
            UiHelper.TieuDe(dgvGio, "PhanTramGiam", "Giảm (%)");
            UiHelper.TieuDe(dgvGio, "GiaSauGiam", "Giá sau giảm");
            UiHelper.TieuDe(dgvGio, "ThanhTien", "Thành tiền");
            UiHelper.DinhDangPhanTram(dgvGio, "PhanTramGiam");
            GridHelper.BoChon(dgvGio);
            lblTongGio.Text = "Tạm tính: " + UiHelper.Tien(gioBus.TinhTong(ds));
        }

        private void btnCapNhatGio_Click(object sender, EventArgs e)
        {
            GioHang g = UiHelper.DongDangChon<GioHang>(dgvGio);
            if (g == null) { MessageBox.Show("Hãy chọn một dòng trong giỏ"); return; }
            string loi = gioBus.DatSoLuong(g.MaDT, (int)numSLGio.Value);
            if (loi != "") { UiHelper.Loi(loi); return; }
            NapGio();
        }

        private void btnXoaGio_Click(object sender, EventArgs e)
        {
            GioHang g = UiHelper.DongDangChon<GioHang>(dgvGio);
            if (g == null) { MessageBox.Show("Hãy chọn một dòng trong giỏ"); return; }
            string loi = gioBus.Xoa(g.MaDT);
            if (loi != "") { UiHelper.Loi(loi); return; }
            NapGio();
        }

        // ===================== Tab 3: Đơn mua (main-sub: 1 hóa đơn - n chi tiết) =====================
        private Panel TaoTabDonMua()
        {
            Panel t = TaoTrang();

            dgvHD = new DataGridView();
            dgvHD.Dock = DockStyle.Fill;
            GridHelper.CauHinh(dgvHD);
            dgvHD.SelectionChanged += (s, e) => NapChiTietDon();

            dgvCT = new DataGridView();
            dgvCT.Dock = DockStyle.Fill;
            GridHelper.CauHinh(dgvCT);

            SplitContainer sc = new SplitContainer();
            sc.Dock = DockStyle.Fill;
            sc.Orientation = Orientation.Horizontal;
            sc.Panel1.Controls.Add(dgvHD);
            sc.Panel2.Controls.Add(dgvCT);

            lblTongDon = new Label();
            lblTongDon.Dock = DockStyle.Bottom;
            lblTongDon.Height = 32;
            lblTongDon.Font = new Font("Segoe UI", 10.5f, FontStyle.Bold);
            lblTongDon.TextAlign = ContentAlignment.MiddleLeft;

            t.Controls.Add(sc);
            t.Controls.Add(lblTongDon);
            return t;
        }

        private void NapDonMua()
        {
            ArrayList ds = hdBus.GetCuaToi();
            GridHelper.GanDuLieu(dgvHD, ds, typeof(HoaDon));
            UiHelper.Giau(dgvHD, "MaKH", "MaNV", "TenKhachHang");
            UiHelper.TieuDe(dgvHD, "MaHD", "Mã hóa đơn");
            UiHelper.TieuDe(dgvHD, "NgayLap", "Ngày lập");
            UiHelper.TieuDe(dgvHD, "PhuongThucTT", "Thanh toán");
            UiHelper.TieuDe(dgvHD, "TongTien", "Tổng tiền");
            UiHelper.TieuDe(dgvHD, "TenNhanVien", "Nhân viên phục vụ");
            lblTongDon.Text = "  Tổng " + ds.Count + " đơn, tổng chi tiêu: " + UiHelper.Tien(hdBus.TinhTongTien(ds));
            NapChiTietDon();
        }

        private void NapChiTietDon()
        {
            HoaDon hd = UiHelper.DongDangChon<HoaDon>(dgvHD);
            ArrayList ct = hd == null ? new ArrayList() : hdBus.GetChiTietCuaToi(hd.MaHD);
            GridHelper.GanDuLieu(dgvCT, ct, typeof(ChiTietHoaDon));
            UiHelper.Giau(dgvCT, "MaHD", "MaKM");
            UiHelper.TieuDe(dgvCT, "MaDT", "Mã");
            UiHelper.TieuDe(dgvCT, "TenHienThi", "Điện thoại");
            UiHelper.TieuDe(dgvCT, "SoLuong", "Số lượng");
            UiHelper.TieuDe(dgvCT, "DonGia", "Đơn giá");
            UiHelper.TieuDe(dgvCT, "TenKM", "Khuyến mãi");
            UiHelper.TieuDe(dgvCT, "ThanhTien", "Thành tiền");
        }

        // ===================== Tab 4: Bảo hành =====================
        private Panel TaoTabBaoHanh()
        {
            Panel t = TaoTrang();
            dgvBH = new DataGridView();
            dgvBH.Dock = DockStyle.Fill;
            GridHelper.CauHinh(dgvBH);
            t.Controls.Add(dgvBH);
            return t;
        }

        private void NapBaoHanh()
        {
            GridHelper.GanDuLieu(dgvBH, bhBus.GetCuaToi(), typeof(BaoHanh));
            UiHelper.Giau(dgvBH, "MaDT");
            UiHelper.TieuDe(dgvBH, "MaBH", "Mã BH");
            UiHelper.TieuDe(dgvBH, "SoIMEI", "Số IMEI");
            UiHelper.TieuDe(dgvBH, "NgayBatDau", "Bắt đầu");
            UiHelper.TieuDe(dgvBH, "NgayKetThuc", "Kết thúc");
            UiHelper.TieuDe(dgvBH, "TrangThai", "Trạng thái");
            UiHelper.TieuDe(dgvBH, "GhiChu", "Ghi chú");
            UiHelper.TieuDe(dgvBH, "MaHD", "Hóa đơn");
            UiHelper.TieuDe(dgvBH, "TenHienThi", "Điện thoại");
        }

        // ===================== Tab 5: Thông tin cá nhân =====================
        private Panel TaoTabThongTin()
        {
            Panel t = TaoTrang();
            int x = 150, w = 280;

            txtHo = UiHelper.O(x, 20, w);
            txtTen = UiHelper.O(x, 54, w);
            txtSDT = UiHelper.O(x, 88, w);
            txtEmail = UiHelper.O(x, 122, w);
            txtDiaChi = UiHelper.O(x, 156, w);
            dtpNgaySinh = new DateTimePicker();
            dtpNgaySinh.Location = new Point(x, 190);
            dtpNgaySinh.Width = w;
            dtpNgaySinh.Format = DateTimePickerFormat.Short;
            dtpNgaySinh.MaxDate = DateTime.Today;
            dtpNgaySinh.ShowCheckBox = true;
            txtDiem = UiHelper.O(x, 224, 120);
            txtDiem.ReadOnly = true;                 // chỉ tăng khi mua hàng, khách không được sửa
            Button btnLuu = UiHelper.Nut("Lưu thông tin", x, 266, 130);
            btnLuu.Click += btnLuuThongTin_Click;

            txtMkCu = UiHelper.O(620, 20, 240);
            txtMkMoi = UiHelper.O(620, 54, 240);
            txtNhapLai = UiHelper.O(620, 88, 240);
            txtMkCu.UseSystemPasswordChar = true;
            txtMkMoi.UseSystemPasswordChar = true;
            txtNhapLai.UseSystemPasswordChar = true;
            Button btnDoiMk = UiHelper.Nut("Đổi mật khẩu", 620, 130, 130);
            btnDoiMk.Click += btnDoiMatKhau_Click;

            t.Controls.Add(UiHelper.Nhan("Họ:", 20, 20));
            t.Controls.Add(txtHo);
            t.Controls.Add(UiHelper.Nhan("Tên:", 20, 54));
            t.Controls.Add(txtTen);
            t.Controls.Add(UiHelper.Nhan("Số điện thoại:", 20, 88));
            t.Controls.Add(txtSDT);
            t.Controls.Add(UiHelper.Nhan("Email:", 20, 122));
            t.Controls.Add(txtEmail);
            t.Controls.Add(UiHelper.Nhan("Địa chỉ:", 20, 156));
            t.Controls.Add(txtDiaChi);
            t.Controls.Add(UiHelper.Nhan("Ngày sinh:", 20, 190));
            t.Controls.Add(dtpNgaySinh);
            t.Controls.Add(UiHelper.Nhan("Điểm tích lũy:", 20, 224));
            t.Controls.Add(txtDiem);
            t.Controls.Add(btnLuu);

            Label lblDoiMk = new Label();
            lblDoiMk.Text = "Đổi mật khẩu";
            lblDoiMk.Font = new Font("Segoe UI", 10.5f, FontStyle.Bold);
            lblDoiMk.AutoSize = true;
            lblDoiMk.Location = new Point(490, 0);
            t.Controls.Add(lblDoiMk);
            t.Controls.Add(UiHelper.Nhan("Mật khẩu hiện tại:", 490, 20));
            t.Controls.Add(txtMkCu);
            t.Controls.Add(UiHelper.Nhan("Mật khẩu mới:", 490, 54));
            t.Controls.Add(txtMkMoi);
            t.Controls.Add(UiHelper.Nhan("Nhập lại:", 490, 88));
            t.Controls.Add(txtNhapLai);
            t.Controls.Add(btnDoiMk);
            return t;
        }

        private void NapThongTin()
        {
            KhachHang kh = khBus.GetById(PhienDangNhap.MaKH);
            if (kh == null) return;
            txtHo.Text = kh.Ho;
            txtTen.Text = kh.Ten;
            txtSDT.Text = kh.SDT;
            txtEmail.Text = kh.Email;
            txtDiaChi.Text = kh.DiaChi;
            if (kh.NgaySinh.HasValue)
            {
                dtpNgaySinh.Value = kh.NgaySinh.Value;
                dtpNgaySinh.Checked = true;
            }
            else
            {
                dtpNgaySinh.Checked = false;
            }
            txtDiem.Text = kh.DiemTichLuy.ToString("#,##0");
            txtMkCu.Clear();
            txtMkMoi.Clear();
            txtNhapLai.Clear();
        }

        private void btnLuuThongTin_Click(object sender, EventArgs e)
        {
            KhachHang kh = new KhachHang();
            kh.MaKH = PhienDangNhap.MaKH;
            kh.Ho = txtHo.Text;
            kh.Ten = txtTen.Text;
            kh.SDT = txtSDT.Text;
            kh.Email = txtEmail.Text;
            kh.DiaChi = txtDiaChi.Text;
            kh.NgaySinh = dtpNgaySinh.Checked ? (DateTime?)dtpNgaySinh.Value.Date : null;

            string loi = khBus.CapNhatThongTin(kh);
            if (loi != "") { UiHelper.Loi(loi); return; }
            PhienDangNhap.CapNhatHoTen(kh.Ho + " " + kh.Ten);
            CapNhatThanhTren();
            MessageBox.Show("Đã lưu thông tin");
        }

        private void btnDoiMatKhau_Click(object sender, EventArgs e)
        {
            string loi = tkBus.DoiMatKhau(PhienDangNhap.TaiKhoanHienTai.TenDangNhap, txtMkCu.Text, txtMkMoi.Text, txtNhapLai.Text);
            if (loi != "") { UiHelper.Loi(loi); return; }
            txtMkCu.Clear();
            txtMkMoi.Clear();
            txtNhapLai.Clear();
            MessageBox.Show("Đã đổi mật khẩu");
        }
    }
}
