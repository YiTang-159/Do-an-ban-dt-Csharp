using System;
using System.Collections;
using System.Drawing;
using System.Windows.Forms;
using QuanLyBanDienThoai.BUS;
using QuanLyBanDienThoai.DTO;

namespace QuanLyBanDienThoai.GUI
{
    // Form quản lý giỏ hàng (nhân viên, quản trị viên). Chỉ gọi BUS, không viết SQL.
    // Lưu ý: Bảng GIOHANG có khóa chính kép (MaKH, MaDT). Khi chọn dòng để sửa/xem, khóa không cho đổi 2 ô này.
    public class FormGioHang : Form
    {
        private GioHangBUS bus = new GioHangBUS();
        private KhachHangBUS khBus = new KhachHangBUS();
        private DienThoaiBUS dtBus = new DienThoaiBUS();

        private ArrayList dsHienTai = new ArrayList();
        private DataGridView dgv;
        private TextBox txtTim, txtNgayThem;
        private ComboBox cboKhach, cboDT;
        private NumericUpDown numSL;
        private Button btnChonKH, btnChonDT, btnThem, btnSua, btnXoa, btnLamMoi;

        public FormGioHang()
        {
            Text = "Quản lý giỏ hàng";
            TaoGiaoDien();
            NapComboBox();
            TaiDuLieu();
        }

        private void TaoGiaoDien()
        {
            // Panel tìm kiếm phía trên
            Panel pnlTim = new Panel();
            pnlTim.Dock = DockStyle.Top;
            pnlTim.Height = 46;

            txtTim = UiHelper.O(75, 11, 200);
            txtTim.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Enter)
                {
                    e.SuppressKeyPress = true;
                    HienDanhSach(bus.TimKiem(txtTim.Text));
                }
            };

            Button btnTim = UiHelper.Nut("Tìm", 285, 8, 65);
            btnTim.Click += (s, e) => HienDanhSach(bus.TimKiem(txtTim.Text));

            Button btnHienTatCa = UiHelper.Nut("Hiện tất cả", 355, 8, 95);
            btnHienTatCa.Click += (s, e) => TaiDuLieu();

            Button btnTimNangCao = UiHelper.Nut("Tìm nâng cao", 455, 8, 110);
            btnTimNangCao.Click += btnTimNangCao_Click;

            Button btnXuatExcel = UiHelper.Nut("Xuất Excel", 570, 8, 95);
            btnXuatExcel.Click += (s, e) => ExcelHelper.XuatCoHopThoai(dsHienTai, typeof(GioHang), "GioHang");

            pnlTim.Controls.Add(UiHelper.Nhan("Tìm kiếm:", 10, 8));
            pnlTim.Controls.AddRange(new Control[] { txtTim, btnTim, btnHienTatCa, btnTimNangCao, btnXuatExcel });

            // Panel nhập dữ liệu phía dưới
            Panel pnlNhap = new Panel();
            pnlNhap.Dock = DockStyle.Bottom;
            pnlNhap.Height = 160;

            // Hàng 1: Khách hàng & Điện thoại
            cboKhach = new ComboBox();
            cboKhach.DropDownStyle = ComboBoxStyle.DropDownList;
            cboKhach.Location = new Point(105, 15);
            cboKhach.Width = 240;

            btnChonKH = UiHelper.Nut("…", 350, 13, 35);
            btnChonKH.Click += btnChonKH_Click;

            cboDT = new ComboBox();
            cboDT.DropDownStyle = ComboBoxStyle.DropDownList;
            cboDT.Location = new Point(480, 15);
            cboDT.Width = 280;

            btnChonDT = UiHelper.Nut("…", 765, 13, 35);
            btnChonDT.Click += btnChonDT_Click;

            // Hàng 2: Số lượng & Ngày thêm
            numSL = new NumericUpDown();
            numSL.Location = new Point(105, 55);
            numSL.Width = 90;
            numSL.Minimum = 1;
            numSL.Maximum = 999;
            numSL.Value = 1;

            txtNgayThem = UiHelper.O(480, 55, 160);
            txtNgayThem.ReadOnly = true;

            // Hàng 3: Các nút chức năng
            btnThem = UiHelper.Nut("Thêm", 20, 110, 85);
            btnThem.Click += btnThem_Click;

            btnSua = UiHelper.Nut("Sửa", 115, 110, 85);
            btnSua.Click += btnSua_Click;

            btnXoa = UiHelper.Nut("Xóa", 210, 110, 85);
            btnXoa.Click += btnXoa_Click;

            btnLamMoi = UiHelper.Nut("Làm mới", 305, 110, 85);
            btnLamMoi.Click += (s, e) => LamMoi();

            pnlNhap.Controls.Add(UiHelper.Nhan("Khách hàng:", 20, 15));
            pnlNhap.Controls.AddRange(new Control[] { cboKhach, btnChonKH });
            pnlNhap.Controls.Add(UiHelper.Nhan("Điện thoại:", 405, 15));
            pnlNhap.Controls.AddRange(new Control[] { cboDT, btnChonDT });
            pnlNhap.Controls.Add(UiHelper.Nhan("Số lượng:", 20, 55));
            pnlNhap.Controls.Add(numSL);
            pnlNhap.Controls.Add(UiHelper.Nhan("Ngày thêm:", 405, 55));
            pnlNhap.Controls.Add(txtNgayThem);
            pnlNhap.Controls.AddRange(new Control[] { btnThem, btnSua, btnXoa, btnLamMoi });

            // DataGridView ở giữa
            dgv = new DataGridView();
            dgv.Dock = DockStyle.Fill;
            GridHelper.CauHinh(dgv);
            dgv.SelectionChanged += dgv_SelectionChanged;

            Controls.Add(dgv);
            Controls.Add(pnlNhap);
            Controls.Add(pnlTim);
        }

        private void NapComboBox()
        {
            ArrayList dsKH = khBus.GetAll();
            cboKhach.DisplayMember = "HoTen";
            cboKhach.ValueMember = "MaKH";
            cboKhach.DataSource = dsKH;

            ArrayList dsDT = dtBus.GetAll();
            cboDT.DisplayMember = "TenHienThi";
            cboDT.ValueMember = "MaDT";
            cboDT.DataSource = dsDT;
        }

        private void TaiDuLieu()
        {
            txtTim.Clear();
            HienDanhSach(bus.GetAll());
        }

        private void HienDanhSach(ArrayList ds)
        {
            dsHienTai = ds;
            GridHelper.GanDuLieu(dgv, ds, typeof(GioHang));
            UiHelper.Giau(dgv, "PhanTramGiam", "GiaSauGiam", "ThanhTien", "MaKM");
            UiHelper.TieuDe(dgv, "MaKH", "Mã KH");
            UiHelper.TieuDe(dgv, "TenKhachHang", "Khách hàng");
            UiHelper.TieuDe(dgv, "MaDT", "Mã ĐT");
            UiHelper.TieuDe(dgv, "TenHienThi", "Điện thoại");
            UiHelper.TieuDe(dgv, "SoLuong", "Số lượng");
            UiHelper.TieuDe(dgv, "GiaNiemYet", "Đơn giá");
            UiHelper.TieuDe(dgv, "SoLuongTon", "Tồn kho");
            UiHelper.TieuDe(dgv, "NgayThem", "Ngày thêm");
            LamMoi();
        }

        private void LamMoi()
        {
            GridHelper.BoChon(dgv);
            txtNgayThem.Text = DateTime.Now.ToString("dd/MM/yyyy HH:mm");
            numSL.Value = 1;
            if (cboKhach.Items.Count > 0) cboKhach.SelectedIndex = 0;
            if (cboDT.Items.Count > 0) cboDT.SelectedIndex = 0;

            // Mở lại các ô khóa chính khi thêm mới
            cboKhach.Enabled = true;
            btnChonKH.Enabled = true;
            cboDT.Enabled = true;
            btnChonDT.Enabled = true;
            btnThem.Enabled = true;
        }

        private void dgv_SelectionChanged(object sender, EventArgs e)
        {
            GioHang g = UiHelper.DongDangChon<GioHang>(dgv);
            if (g == null) return;

            cboKhach.SelectedValue = g.MaKH;
            cboDT.SelectedValue = g.MaDT;
            numSL.Value = g.SoLuong > 0 && g.SoLuong <= numSL.Maximum ? g.SoLuong : 1;
            txtNgayThem.Text = g.NgayThem.ToString("dd/MM/yyyy HH:mm");

            // Khóa chính kép (MaKH, MaDT) nên khóa 2 ComboBox khi chọn một dòng sửa
            cboKhach.Enabled = false;
            btnChonKH.Enabled = false;
            cboDT.Enabled = false;
            btnChonDT.Enabled = false;
            btnThem.Enabled = false;
        }

        private void btnChonKH_Click(object sender, EventArgs e)
        {
            KhachHang kh = FormChon.Chon(khBus.GetAll(),
                new string[] { "MaKH", "HoTen", "SDT", "DiaChi" }, "Chọn khách hàng") as KhachHang;
            if (kh != null) cboKhach.SelectedValue = kh.MaKH;
        }

        private void btnChonDT_Click(object sender, EventArgs e)
        {
            DienThoai dt = FormChon.Chon(dtBus.GetAll(),
                new string[] { "MaDT", "TenHienThi", "GiaNiemYet", "SoLuongTon" }, "Chọn điện thoại") as DienThoai;
            if (dt != null) cboDT.SelectedValue = dt.MaDT;
        }

        private void btnThem_Click(object sender, EventArgs e)
        {
            if (cboKhach.SelectedValue == null) { MessageBox.Show("Hãy chọn khách hàng"); return; }
            if (cboDT.SelectedValue == null) { MessageBox.Show("Hãy chọn điện thoại"); return; }

            GioHang x = new GioHang();
            x.MaKH = Convert.ToInt32(cboKhach.SelectedValue);
            x.MaDT = Convert.ToInt32(cboDT.SelectedValue);
            x.SoLuong = (int)numSL.Value;
            x.NgayThem = DateTime.Now;

            string loi = bus.Them(x);
            if (loi != "") { UiHelper.Loi(loi); return; }

            MessageBox.Show("Thêm vào giỏ hàng thành công", "Thông báo");
            TaiDuLieu();
        }

        private void btnSua_Click(object sender, EventArgs e)
        {
            GioHang g = UiHelper.DongDangChon<GioHang>(dgv);
            if (g == null) { MessageBox.Show("Hãy chọn một dòng giỏ hàng để sửa"); return; }

            GioHang x = new GioHang();
            x.MaKH = g.MaKH;
            x.MaDT = g.MaDT;
            x.SoLuong = (int)numSL.Value;

            string loi = bus.Sua(x);
            if (loi != "") { UiHelper.Loi(loi); return; }

            MessageBox.Show("Cập nhật số lượng thành công", "Thông báo");
            TaiDuLieu();
        }

        private void btnXoa_Click(object sender, EventArgs e)
        {
            GioHang g = UiHelper.DongDangChon<GioHang>(dgv);
            if (g == null) { MessageBox.Show("Hãy chọn một dòng giỏ hàng để xóa"); return; }

            if (MessageBox.Show("Xóa sản phẩm " + g.TenHienThi + " khỏi giỏ hàng của " + g.TenKhachHang + "?",
                "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;

            string loi = bus.Xoa(g.MaKH, g.MaDT);
            if (loi != "") { UiHelper.Loi(loi); return; }

            MessageBox.Show("Xóa thành công", "Thông báo");
            TaiDuLieu();
        }

        private void btnTimNangCao_Click(object sender, EventArgs e)
        {
            using (FormTimNangCao f = new FormTimNangCao(typeof(GioHang)))
            {
                if (f.ShowDialog() != DialogResult.OK) return;
                HienDanhSach(bus.TimNangCao(f.KetQua));
            }
        }
    }
}

