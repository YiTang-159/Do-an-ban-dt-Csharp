using System;
using System.Collections;
using System.Drawing;
using System.Windows.Forms;
using QuanLyBanDienThoai.BUS;
using QuanLyBanDienThoai.DTO;

namespace QuanLyBanDienThoai.GUI
{
    // Trang dành cho khách hàng sau khi đăng nhập. Chỉ gọi BUS, không viết SQL.
    // Mọi dữ liệu đều theo MaKH của tài khoản đang đăng nhập (BUS lấy từ PhienDangNhap):
    //   Cửa hàng (DIENTHOAI + khuyến mãi) | Giỏ hàng (GIOHANG) | Đơn mua (HOADON, CHITIETHOADON) | Bảo hành (BAOHANH) | Thông tin (KHACHHANG, TAIKHOAN)
    public class FormTrangKhachHang : Form
    {
        private SanPhamBanBUS spBus = new SanPhamBanBUS();
        private GioHangBUS gioBus = new GioHangBUS();
        private HoaDonBUS hdBus = new HoaDonBUS();
        private BaoHanhBUS bhBus = new BaoHanhBUS();
        private KhachHangBUS khBus = new KhachHangBUS();
        private TaiKhoanBUS tkBus = new TaiKhoanBUS();

        private TabControl tab;
        private Label lblXinChao, lblDiem;

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
            Text = "PhoneStore - Khách hàng";
            Size = new Size(1180, 720);
            MinimumSize = new Size(960, 600);
            StartPosition = FormStartPosition.CenterScreen;

            tab = new TabControl();
            tab.Dock = DockStyle.Fill;
            tab.Font = new Font("Segoe UI", 10);
            tab.TabPages.Add(TaoTabCuaHang());
            tab.TabPages.Add(TaoTabGioHang());
            tab.TabPages.Add(TaoTabDonMua());
            tab.TabPages.Add(TaoTabBaoHanh());
            tab.TabPages.Add(TaoTabThongTin());
            tab.SelectedIndexChanged += (s, e) => NapTabHienTai();

            // Fill thêm vào trước, thanh Top thêm sau
            Controls.Add(tab);
            Controls.Add(TaoThanhTren());

            CapNhatThanhTren();
            NapTabHienTai();
        }

        // ===================== Thanh trên =====================
        private Panel TaoThanhTren()
        {
            Panel p = new Panel();
            p.Dock = DockStyle.Top;
            p.Height = 56;
            p.BackColor = UiHelper.MauXanh;

            Label lblTen = new Label();
            lblTen.Text = "☎  PhoneStore";
            lblTen.ForeColor = Color.White;
            lblTen.Font = new Font("Segoe UI", 15, FontStyle.Bold);
            lblTen.Dock = DockStyle.Left;
            lblTen.Width = 210;
            lblTen.TextAlign = ContentAlignment.MiddleCenter;

            lblXinChao = new Label();
            lblXinChao.ForeColor = Color.White;
            lblXinChao.Font = new Font("Segoe UI", 10.5f, FontStyle.Bold);
            lblXinChao.Dock = DockStyle.Fill;
            lblXinChao.TextAlign = ContentAlignment.MiddleLeft;

            lblDiem = new Label();
            lblDiem.ForeColor = Color.White;
            lblDiem.Font = new Font("Segoe UI", 10);
            lblDiem.Dock = DockStyle.Right;
            lblDiem.Width = 190;
            lblDiem.TextAlign = ContentAlignment.MiddleRight;

            Button btnDangXuat = new Button();
            btnDangXuat.Text = "Đăng xuất";
            btnDangXuat.Dock = DockStyle.Right;
            btnDangXuat.Width = 110;
            btnDangXuat.FlatStyle = FlatStyle.Flat;
            btnDangXuat.FlatAppearance.BorderSize = 0;
            btnDangXuat.BackColor = UiHelper.MauXanhDam;
            btnDangXuat.ForeColor = Color.White;
            btnDangXuat.Font = new Font("Segoe UI", 10);
            btnDangXuat.UseVisualStyleBackColor = false;
            btnDangXuat.Cursor = Cursors.Hand;
            btnDangXuat.Click += (s, e) =>
            {
                if (MessageBox.Show("Đăng xuất khỏi tài khoản này?", "Xác nhận",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;
                PhienDangNhap.YeuCauDangXuat = true;
                Close();
            };

            // Fill trước, các khung Left/Right thêm sau (khung thêm sau cùng nằm ngoài cùng)
            p.Controls.Add(lblXinChao);
            p.Controls.Add(lblDiem);
            p.Controls.Add(lblTen);
            p.Controls.Add(btnDangXuat);
            return p;
        }

        private void CapNhatThanhTren()
        {
            lblXinChao.Text = "Xin chào, " + PhienDangNhap.HoTen;
            KhachHang kh = khBus.GetById(PhienDangNhap.MaKH);
            lblDiem.Text = kh == null ? "" : "Điểm tích lũy: " + kh.DiemTichLuy.ToString("#,##0");
        }

        private void NapTabHienTai()
        {
            switch (tab.SelectedIndex)
            {
                case 0: NapSanPham(spBus.TimKiem(txtTim.Text)); break;
                case 1: NapGio(); break;
                case 2: NapDonMua(); break;
                case 3: NapBaoHanh(); break;
                case 4: NapThongTin(); break;
            }
        }

        // ===================== Tab 1: Cửa hàng =====================
        private TabPage TaoTabCuaHang()
        {
            TabPage t = new TabPage("Cửa hàng");

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
        private TabPage TaoTabGioHang()
        {
            TabPage t = new TabPage("Giỏ hàng");

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
        private TabPage TaoTabDonMua()
        {
            TabPage t = new TabPage("Đơn mua của tôi");

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
        private TabPage TaoTabBaoHanh()
        {
            TabPage t = new TabPage("Bảo hành");
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
        private TabPage TaoTabThongTin()
        {
            TabPage t = new TabPage("Thông tin cá nhân");
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
