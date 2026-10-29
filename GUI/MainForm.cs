using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using QuanLyBanDienThoai.BUS;

namespace QuanLyBanDienThoai.GUI
{
    // Màn hình chính dạng desktop theo mẫu: menu tối bên trái (có thương hiệu, người dùng, nhóm mục con),
    // thanh xanh phía trên, vùng nội dung bên phải có tiêu đề ở giữa và form con nhúng bên trong, chân trang.
    // Bấm tên nhóm để mở/đóng các mục con; bấm mục con để mở form.
    // Ai làm xong form nào thì BỎ DẤU // ở đúng dòng ThemMuc của mình trong TaoMenu() (chỉ sửa đúng dòng của mình).
    // Menu theo vai trò đăng nhập: Admin và Nhân viên thấy cùng các chức năng quản lý.
    // Riêng quản lý nhân sự (mục Nhân viên) và nhóm Tài khoản chỉ Admin thấy (dùng biến laAdmin trong TaoMenu).
    // Khách hàng không dùng MainForm mà dùng FormTrangKhachHang (xem Program.cs).
    public class MainForm : Form
    {
        // Màu theo giao diện mẫu
        private static readonly Color MauXanh = Color.FromArgb(0, 166, 90);
        private static readonly Color MauXanhDam = Color.FromArgb(0, 141, 76);
        private static readonly Color MauMenu = Color.FromArgb(34, 45, 50);
        private static readonly Color MauMenuToi = Color.FromArgb(30, 40, 44);
        private static readonly Color MauMenuCon = Color.FromArgb(44, 59, 65);
        private static readonly Color MauMenuConHover = Color.FromArgb(52, 70, 77);
        private static readonly Color MauChuMenu = Color.FromArgb(184, 199, 206);
        private static readonly Color MauChuMenuCon = Color.FromArgb(139, 164, 175);
        private static readonly Color MauNen = Color.FromArgb(236, 240, 245);

        private Panel pnlSidebar;
        private FlowLayoutPanel pnlMenu;
        private Panel pnlForm;         // vùng chứa form con
        private Label lblTieuDe;       // tiêu đề ở giữa phía trên form con
        private Label lblChao;
        private Form formHienTai;
        private Button btnDangChon;    // mục con đang được chọn
        private List<FlowLayoutPanel> cacNhom = new List<FlowLayoutPanel>();
        private List<Button> cacNutNhom = new List<Button>();
        private List<Button> cacNutCon = new List<Button>();

        public MainForm()
        {
            Text = "Quản lý cửa hàng bán điện thoại";
            Size = new Size(1280, 760);
            MinimumSize = new Size(1000, 600);
            StartPosition = FormStartPosition.CenterScreen;

            // ===================== Vùng bên phải =====================
            Panel pnlRight = new Panel();
            pnlRight.Dock = DockStyle.Fill;
            pnlRight.BackColor = MauNen;

            // Thẻ trắng chứa tiêu đề và form con (viền xanh phía trên)
            pnlForm = new Panel();
            pnlForm.Dock = DockStyle.Fill;
            pnlForm.BackColor = Color.White;

            lblChao = new Label();
            lblChao.Text = "QUẢN LÝ CỬA HÀNG BÁN ĐIỆN THOẠI\r\n\r\nXin chào " + PhienDangNhap.HoTen + " (" + PhienDangNhap.TenVaiTroHienTai + ")\r\nChọn chức năng ở menu bên trái để bắt đầu";
            lblChao.Font = new Font("Segoe UI", 18, FontStyle.Bold);
            lblChao.ForeColor = Color.FromArgb(90, 110, 120);
            lblChao.Dock = DockStyle.Fill;
            lblChao.TextAlign = ContentAlignment.MiddleCenter;
            pnlForm.Controls.Add(lblChao);

            lblTieuDe = new Label();
            lblTieuDe.Dock = DockStyle.Top;
            lblTieuDe.Height = 52;
            lblTieuDe.Font = new Font("Segoe UI", 14, FontStyle.Bold);
            lblTieuDe.ForeColor = Color.FromArgb(60, 72, 80);
            lblTieuDe.BackColor = Color.White;
            lblTieuDe.TextAlign = ContentAlignment.MiddleCenter;
            lblTieuDe.Text = "Trang chủ";
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

            TaoMenu();
            CanChinhDoRong();
            Load += (s, e) => CanChinhDoRong();
        }

        // Thanh xanh phía trên: nút ẩn/hiện menu, tên hệ thống, người dùng
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

            Panel avatar = TaoAvatar(34, PhienDangNhap.VietTat, MauXanhDam, 50);
            avatar.Dock = DockStyle.Right;
            avatar.BackColor = MauXanh;

            Label lblNguoiDung = new Label();
            lblNguoiDung.Dock = DockStyle.Right;
            lblNguoiDung.Width = 230;
            lblNguoiDung.Text = PhienDangNhap.HoTen + " (" + PhienDangNhap.TenVaiTroHienTai + ")";
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
            p.Controls.Add(avatar);
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

        // Khung người dùng dưới thương hiệu: ảnh tròn, tên, trạng thái
        private Panel TaoKhuNguoiDung()
        {
            Panel p = new Panel();
            p.Dock = DockStyle.Top;
            p.Height = 66;
            p.BackColor = MauMenu;

            Label lblTen = new Label();
            lblTen.Dock = DockStyle.Fill;
            lblTen.Text = PhienDangNhap.HoTen;
            lblTen.ForeColor = Color.White;
            lblTen.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            lblTen.TextAlign = ContentAlignment.BottomLeft;

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
            pnlChu.Controls.Add(lblTen);
            pnlChu.Controls.Add(lblTrangThai);

            Panel avatar = TaoAvatar(40, PhienDangNhap.VietTat, Color.FromArgb(96, 125, 139), 64);
            avatar.Dock = DockStyle.Left;
            avatar.BackColor = MauMenu;

            p.Controls.Add(pnlChu);
            p.Controls.Add(avatar);
            return p;
        }

        // Ảnh đại diện hình tròn có hai chữ cái
        private Panel TaoAvatar(int kichThuoc, string chu, Color mauNen, int rong)
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
                TextRenderer.DrawText(e.Graphics, chu, f, new Rectangle(x, y, kichThuoc, kichThuoc), Color.White,
                    TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
            };
            p.Resize += (s, e) => p.Invalidate();
            return p;
        }

        private void TaoMenu()
        {
            // Admin và Nhân viên có đủ các nhóm chức năng như nhau.
            // Chỉ Admin có: mục Nhân viên (quản lý nhân sự) và nhóm Tài khoản.
            bool laAdmin = PhienDangNhap.LaAdmin;

            FlowLayoutPanel nDanhMuc = TaoNhom("Danh mục");
            // TV1
            ThemMuc(nDanhMuc, "Hãng điện thoại", () => new FormHangDT());
            ThemMuc(nDanhMuc, "Loại điện thoại", () => new FormLoaiDienThoai());
            ThemMuc(nDanhMuc, "Nhà cung cấp", () => new FormNhaCungCap());
            ThemMuc(nDanhMuc, "Điện thoại", () => new FormDienThoai());

            FlowLayoutPanel nNhapHang = TaoNhom("Nhập hàng");
            // TV2
            // ThemMuc(nNhapHang, "Lập phiếu nhập hàng", () => new FormNhapHang());
            // ThemMuc(nNhapHang, "Danh sách phiếu nhập", () => new FormPhieuNhap());

            FlowLayoutPanel nNhanSu = TaoNhom("Nhân sự - Khách hàng");
            // TV2
            // Mục "Nhân viên" là quản lý nhân sự, CHỈ Admin thấy: khi làm xong thì bỏ // và giữ điều kiện laAdmin
            // if (laAdmin) ThemMuc(nNhanSu, "Nhân viên", () => new FormNhanVien());
            // ThemMuc(nNhanSu, "Khách hàng", () => new FormKhachHang());

            FlowLayoutPanel nKhuyenMai = TaoNhom("Khuyến mãi - Bảo hành");
            // TV4
            // ThemMuc(nKhuyenMai, "Khuyến mãi", () => new FormKhuyenMai());
            // ThemMuc(nKhuyenMai, "Chi tiết khuyến mãi", () => new FormChiTietKhuyenMai());
            // ThemMuc(nKhuyenMai, "Bảo hành", () => new FormBaoHanh());

            FlowLayoutPanel nBanHang = TaoNhom("Bán hàng");
            // TV3
            ThemMuc(nBanHang, "Lập hóa đơn bán hàng", () => new FormBanHang());
            ThemMuc(nBanHang, "Danh sách hóa đơn", () => new FormHoaDon());
            // ThemMuc(nBanHang, "Giỏ hàng", () => new FormGioHang());

            FlowLayoutPanel nThongKe = TaoNhom("Thống kê");
            // TV4
            // ThemMuc(nThongKe, "Thống kê thu chi", () => new FormThongKe());

            FlowLayoutPanel nThamSo = TaoNhom("Tham số");
            // TV2
            // ThemMuc(nThamSo, "Bảng tham số", () => new FormThamSo());

            // ---- Chỉ Admin ----
            if (laAdmin)
            {
                FlowLayoutPanel nTaiKhoan = TaoNhom("Tài khoản");
                ThemMuc(nTaiKhoan, "Tài khoản đăng nhập", () => new FormTaiKhoan());
            }

            // Mở sẵn nhóm đầu tiên
            if (cacNhom.Count > 0) cacNhom[0].Visible = true;
        }

        private static string LayBieuTuong(string tenNhom)
        {
            switch (tenNhom)
            {
                case "Danh mục": return "▦";
                case "Nhập hàng": return "⇩";
                case "Nhân sự - Khách hàng": return "☺";
                case "Khuyến mãi - Bảo hành": return "★";
                case "Bán hàng": return "¤";
                case "Thống kê": return "▥";
                case "Tham số": return "⚙";
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

        // Thêm một mục con vào nhóm; bấm vào thì tạo form bằng taoForm rồi nhúng vào vùng bên phải
        private void ThemMuc(FlowLayoutPanel nhom, string ten, Func<Form> taoForm)
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
            btn.Click += (s, e) =>
            {
                Form f = taoForm();
                lblTieuDe.Text = ten;
                DanhDauMuc(btn);
                MoFormCon(f);
            };

            nhom.Controls.Add(btn);
            cacNutCon.Add(btn);
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

        // Nhúng form con (TopLevel = false) vào vùng bên phải, thay form đang mở
        private void MoFormCon(Form f)
        {
            if (formHienTai != null)
            {
                pnlForm.Controls.Remove(formHienTai);
                formHienTai.Dispose();
                formHienTai = null;
            }
            lblChao.Visible = false;

            f.TopLevel = false;
            f.FormBorderStyle = FormBorderStyle.None;
            f.Dock = DockStyle.Fill;
            pnlForm.Controls.Add(f);
            formHienTai = f;
            f.Show();
            f.BringToFront();
        }
    }
}
