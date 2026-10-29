using System;
using System.Collections;
using System.Drawing;
using System.Windows.Forms;
using QuanLyBanDienThoai.BUS;
using QuanLyBanDienThoai.DTO;

namespace QuanLyBanDienThoai.GUI
{
    // Lập hóa đơn bán hàng (nhân viên, admin). Chỉ gọi BUS, không viết SQL.
    //  - Nhân viên lập hóa đơn là người đang đăng nhập (không cho chọn lại), nên hóa đơn luôn khớp với tài khoản.
    //  - Nút "..." mở FormChon để chọn khách hàng và điện thoại (kết quả nằm ở biến static FormChon.KetQua).
    //  - "Lấy từ giỏ hàng" nạp các dòng GIOHANG mà khách đã chọn ở trang khách hàng.
    //  - DonGia, ThanhTien, TongTien, NgayLap do chương trình tính, không cho nhập.
    public class FormBanHang : Form
    {
        private HoaDonBUS bus = new HoaDonBUS();
        private SanPhamBanBUS spBus = new SanPhamBanBUS();

        private ArrayList dong = new ArrayList();      // các ChiTietHoaDon đang lập
        private bool tuGioHang = false;                // có dòng nào nạp từ giỏ hàng không
        private int maKH = 0;
        private bool dangNap = false;

        private TextBox txtKhach;
        private ComboBox cboPTTT, cboDT, cboKM;
        private NumericUpDown numSL;
        private DataGridView dgv;
        private Label lblTong;

        public FormBanHang()
        {
            Text = "Lập hóa đơn bán hàng";
            TaoGiaoDien();
            NapSanPham();
            CapNhatLuoi();
        }

        private void TaoGiaoDien()
        {
            Panel pnlTren = new Panel();
            pnlTren.Dock = DockStyle.Top;
            pnlTren.Height = 112;

            // Hàng 1: khách hàng, nhân viên, phương thức thanh toán
            txtKhach = UiHelper.O(100, 10, 250);
            txtKhach.ReadOnly = true;
            Button btnChonKH = UiHelper.Nut("…", 355, 8, 36);
            btnChonKH.Click += btnChonKH_Click;
            Button btnKhachMoi = UiHelper.Nut("Khách mới", 396, 8, 90);
            btnKhachMoi.Click += btnKhachMoi_Click;

            TextBox txtNV = UiHelper.O(600, 10, 200);
            txtNV.ReadOnly = true;
            txtNV.Text = PhienDangNhap.HoTen;

            cboPTTT = new ComboBox();
            cboPTTT.DropDownStyle = ComboBoxStyle.DropDownList;
            cboPTTT.Items.AddRange(new object[] { "Tiền mặt", "Chuyển khoản", "Thẻ" });
            cboPTTT.SelectedIndex = 0;
            cboPTTT.Location = new Point(925, 10);
            cboPTTT.Width = 130;

            // Hàng 2: điện thoại, số lượng, khuyến mãi
            cboDT = new ComboBox();
            cboDT.DropDownStyle = ComboBoxStyle.DropDownList;
            cboDT.Location = new Point(100, 52);
            cboDT.Width = 250;
            cboDT.SelectedIndexChanged += (s, e) => { if (!dangNap) NapKhuyenMai(); };
            Button btnChonDT = UiHelper.Nut("…", 355, 50, 36);
            btnChonDT.Click += btnChonDT_Click;

            numSL = new NumericUpDown();
            numSL.Location = new Point(470, 53);
            numSL.Width = 60;
            numSL.Minimum = 1;
            numSL.Maximum = 99;

            cboKM = new ComboBox();
            cboKM.DropDownStyle = ComboBoxStyle.DropDownList;
            cboKM.Location = new Point(625, 52);
            cboKM.Width = 260;

            Button btnThem = UiHelper.Nut("Thêm vào hóa đơn", 895, 50, 150);
            btnThem.Click += btnThemDong_Click;

            // Hàng 3: các nút
            Button btnGio = UiHelper.Nut("Lấy từ giỏ hàng", 10, 76, 130);
            btnGio.Click += btnLayGio_Click;
            Button btnXoaDong = UiHelper.Nut("Xóa dòng", 150, 76, 100);
            btnXoaDong.Click += btnXoaDong_Click;
            Button btnLap = UiHelper.Nut("Lập hóa đơn", 260, 76, 120);
            btnLap.Click += btnLap_Click;
            Button btnLamMoi = UiHelper.Nut("Làm mới", 390, 76, 100);
            btnLamMoi.Click += (s, e) => LamMoi();

            pnlTren.Controls.Add(UiHelper.Nhan("Khách hàng:", 10, 10));
            pnlTren.Controls.AddRange(new Control[] { txtKhach, btnChonKH, btnKhachMoi });
            pnlTren.Controls.Add(UiHelper.Nhan("Nhân viên lập:", 505, 10));
            pnlTren.Controls.Add(txtNV);
            pnlTren.Controls.Add(UiHelper.Nhan("Thanh toán:", 835, 10));
            pnlTren.Controls.Add(cboPTTT);
            pnlTren.Controls.Add(UiHelper.Nhan("Điện thoại:", 10, 52));
            pnlTren.Controls.AddRange(new Control[] { cboDT, btnChonDT });
            pnlTren.Controls.Add(UiHelper.Nhan("Số lượng:", 405, 52));
            pnlTren.Controls.Add(numSL);
            pnlTren.Controls.Add(UiHelper.Nhan("Khuyến mãi:", 545, 52));
            pnlTren.Controls.AddRange(new Control[] { cboKM, btnThem, btnGio, btnXoaDong, btnLap, btnLamMoi });

            dgv = new DataGridView();
            dgv.Dock = DockStyle.Fill;
            GridHelper.CauHinh(dgv);

            Panel pnlDuoi = new Panel();
            pnlDuoi.Dock = DockStyle.Bottom;
            pnlDuoi.Height = 44;
            lblTong = new Label();
            lblTong.AutoSize = true;
            lblTong.Font = new Font("Segoe UI", 13, FontStyle.Bold);
            lblTong.ForeColor = Color.FromArgb(200, 40, 40);
            lblTong.Location = new Point(10, 8);
            pnlDuoi.Controls.Add(lblTong);

            // Fill thêm vào trước, các thanh Top/Bottom thêm sau
            Controls.Add(dgv);
            Controls.Add(pnlDuoi);
            Controls.Add(pnlTren);
        }

        // Nạp danh sách điện thoại (kèm tồn kho hiện tại) vào ComboBox
        private void NapSanPham()
        {
            dangNap = true;
            cboDT.DataSource = null;
            cboDT.DisplayMember = "TenHienThi";
            cboDT.ValueMember = "MaDT";
            cboDT.DataSource = spBus.GetAll();
            dangNap = false;
            NapKhuyenMai();
        }

        // ComboBox khuyến mãi: "Không áp dụng" + các khuyến mãi đang hiệu lực của điện thoại đang chọn
        // (mặc định chọn khuyến mãi cao nhất, giống giá khách thấy ở trang khách hàng)
        private void NapKhuyenMai()
        {
            ArrayList ds = new ArrayList();
            KhuyenMaiApDung khong = new KhuyenMaiApDung();
            khong.MaKM = 0;
            khong.TenKM = "Không áp dụng";
            ds.Add(khong);
            SanPhamBan sp = cboDT.SelectedItem as SanPhamBan;
            if (sp != null)
            {
                foreach (object o in bus.GetKhuyenMaiApDung(sp.MaDT)) ds.Add(o);
            }
            cboKM.DataSource = null;
            cboKM.DisplayMember = "TenHienThi";
            cboKM.ValueMember = "MaKM";
            cboKM.DataSource = ds;
            if (ds.Count > 1) cboKM.SelectedIndex = 1;
        }

        private void CapNhatLuoi()
        {
            GridHelper.GanDuLieu(dgv, dong, typeof(ChiTietHoaDon));
            UiHelper.Giau(dgv, "MaHD", "MaKM");
            UiHelper.TieuDe(dgv, "MaDT", "Mã");
            UiHelper.TieuDe(dgv, "TenHienThi", "Điện thoại");
            UiHelper.TieuDe(dgv, "SoLuong", "Số lượng");
            UiHelper.TieuDe(dgv, "DonGia", "Đơn giá");
            UiHelper.TieuDe(dgv, "TenKM", "Khuyến mãi");
            UiHelper.TieuDe(dgv, "ThanhTien", "Thành tiền");
            GridHelper.BoChon(dgv);
            lblTong.Text = "Tổng tiền: " + UiHelper.Tien(bus.TinhTongDong(dong));
        }

        private void DatKhach(KhachHang kh)
        {
            // Đổi khách thì bỏ các dòng đã nạp từ giỏ của khách trước
            if (maKH != kh.MaKH && tuGioHang)
            {
                dong.Clear();
                tuGioHang = false;
                CapNhatLuoi();
            }
            maKH = kh.MaKH;
            txtKhach.Text = kh.HoTen + " - " + kh.SDT;
        }

        private void btnChonKH_Click(object sender, EventArgs e)
        {
            KhachHang kh = FormChon.Chon(new KhachHangBUS().GetAll(),
                new string[] { "MaKH", "HoTen", "SDT", "DiaChi" }, "Chọn khách hàng") as KhachHang;
            if (kh != null) DatKhach(kh);
        }

        // Khách mua tại quầy chưa có tài khoản: đăng ký ngay rồi chọn luôn khách đó
        private void btnKhachMoi_Click(object sender, EventArgs e)
        {
            using (FormDangKy f = new FormDangKy())
            {
                if (f.ShowDialog() != DialogResult.OK) return;
                KhachHang kh = new KhachHangBUS().GetById(f.MaKHMoi);
                if (kh != null) DatKhach(kh);
            }
        }

        private void btnChonDT_Click(object sender, EventArgs e)
        {
            SanPhamBan sp = FormChon.Chon(spBus.GetAll(),
                new string[] { "MaDT", "TenHienThi", "GiaSauGiam", "SoLuongTon" }, "Chọn điện thoại") as SanPhamBan;
            if (sp != null) cboDT.SelectedValue = sp.MaDT;
        }

        private void btnThemDong_Click(object sender, EventArgs e)
        {
            SanPhamBan sp = cboDT.SelectedItem as SanPhamBan;
            if (sp == null) { MessageBox.Show("Hãy chọn điện thoại"); return; }
            KhuyenMaiApDung km = cboKM.SelectedItem as KhuyenMaiApDung;
            int? maKM = (km == null || km.MaKM == 0) ? (int?)null : km.MaKM;

            string loi = bus.TaoDong(dong, sp.MaDT, (int)numSL.Value, maKM);
            if (loi != "") { UiHelper.Loi(loi); return; }
            CapNhatLuoi();
        }

        private void btnLayGio_Click(object sender, EventArgs e)
        {
            if (maKH <= 0) { MessageBox.Show("Hãy chọn khách hàng trước"); return; }
            int soDongTruoc = dong.Count;
            string loi = bus.NapTuGioHang(maKH, dong);
            if (dong.Count > soDongTruoc) tuGioHang = true;
            CapNhatLuoi();
            if (loi != "") MessageBox.Show(loi, "Một số dòng chưa nạp được", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private void btnXoaDong_Click(object sender, EventArgs e)
        {
            ChiTietHoaDon ct = UiHelper.DongDangChon<ChiTietHoaDon>(dgv);
            if (ct == null) { MessageBox.Show("Hãy chọn một dòng để xóa"); return; }
            dong.Remove(ct);
            CapNhatLuoi();
        }

        private void btnLap_Click(object sender, EventArgs e)
        {
            if (maKH <= 0) { MessageBox.Show("Hãy chọn khách hàng"); return; }
            if (dong.Count == 0) { MessageBox.Show("Hóa đơn chưa có điện thoại nào"); return; }
            if (MessageBox.Show("Lập hóa đơn " + UiHelper.Tien(bus.TinhTongDong(dong)) + " cho " + txtKhach.Text + "?",
                "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;

            int maHD;
            string loi = bus.LuuHoaDon(maKH, cboPTTT.Text, dong, tuGioHang, out maHD);
            if (loi != "") { UiHelper.Loi(loi); return; }
            MessageBox.Show("Đã lập hóa đơn số " + maHD + ". Tồn kho và điểm tích lũy đã được cập nhật.", "Thành công");
            LamMoi();
        }

        private void LamMoi()
        {
            dong.Clear();
            tuGioHang = false;
            maKH = 0;
            txtKhach.Clear();
            cboPTTT.SelectedIndex = 0;
            numSL.Value = 1;
            NapSanPham();          // nạp lại để thấy tồn kho mới
            CapNhatLuoi();
        }
    }
}
