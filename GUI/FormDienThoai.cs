using System;
using System.Collections;
using System.Drawing;
using System.Windows.Forms;
using QuanLyBanDienThoai.BUS;
using QuanLyBanDienThoai.DTO;

namespace QuanLyBanDienThoai.GUI
{
    // Form quản lý điện thoại. Chỉ gọi BUS, không viết SQL.
    // SoLuongTon chỉ để xem (khóa ô): chỉ nhập hàng mới tăng, bán hàng mới giảm.
    public class FormDienThoai : Form
    {
        private DienThoaiBUS bus = new DienThoaiBUS();
        private HangDTBUS busHang = new HangDTBUS();
        private LoaiDienThoaiBUS busLoai = new LoaiDienThoaiBUS();
        private ArrayList dsHienTai = new ArrayList();
        private DataGridView dgv;
        private TextBox txtTimKiem, txtMa, txtTen, txtHeDieuHanh, txtNamRaMat, txtRAM, txtBoNho;
        private TextBox txtMauSac, txtGia, txtTon, txtMoTa;
        private ComboBox cboHang, cboLoai;

        public FormDienThoai()
        {
            Text = "Quản lý điện thoại";
            TaoGiaoDien();
            TaiComboBox();
            TaiDuLieu();
        }

        private static Label Nhan(string text, int x, int y)
        {
            Label l = new Label();
            l.Text = text;
            l.Location = new Point(x, y + 4);
            l.AutoSize = true;
            return l;
        }

        private static Button Nut(string text, int x, int y, int w)
        {
            Button b = new Button();
            b.Text = text;
            b.Location = new Point(x, y);
            b.Size = new Size(w, 30);
            return b;
        }

        private static TextBox O(int x, int y, int w)
        {
            TextBox t = new TextBox();
            t.Location = new Point(x, y);
            t.Width = w;
            return t;
        }

        private static ComboBox Cbo(int x, int y, int w)
        {
            ComboBox c = new ComboBox();
            c.DropDownStyle = ComboBoxStyle.DropDownList;
            c.Location = new Point(x, y);
            c.Width = w;
            return c;
        }

        private void TaoGiaoDien()
        {
            dgv = new DataGridView();
            dgv.Dock = DockStyle.Fill;
            GridHelper.CauHinh(dgv);
            dgv.SelectionChanged += dgv_SelectionChanged;

            Panel pnlTim = new Panel();
            pnlTim.Dock = DockStyle.Top;
            pnlTim.Height = 46;
            txtTimKiem = O(75, 11, 200);
            txtTimKiem.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Enter)
                {
                    e.SuppressKeyPress = true;
                    HienDanhSach(bus.TimKiem(txtTimKiem.Text));
                }
            };
            Button btnTim = Nut("Tìm", 285, 8, 65);
            btnTim.Click += (s, e) => HienDanhSach(bus.TimKiem(txtTimKiem.Text));
            Button btnHienTatCa = Nut("Hiện tất cả", 355, 8, 95);
            btnHienTatCa.Click += (s, e) => TaiDuLieu();
            Button btnTimNangCao = Nut("Tìm nâng cao", 455, 8, 110);
            btnTimNangCao.Click += btnTimNangCao_Click;
            Button btnXuatExcel = Nut("Xuất Excel", 570, 8, 95);
            btnXuatExcel.Click += (s, e) => ExcelHelper.XuatCoHopThoai(dsHienTai, typeof(DienThoai), "DienThoai");
            Button btnNhapExcel = Nut("Nhập Excel", 670, 8, 95);
            btnNhapExcel.Click += btnNhapExcel_Click;
            pnlTim.Controls.Add(Nhan("Tìm kiếm:", 10, 8));
            pnlTim.Controls.Add(txtTimKiem);
            pnlTim.Controls.AddRange(new Control[] { btnTim, btnHienTatCa, btnTimNangCao, btnXuatExcel, btnNhapExcel });

            // ô nhập: cột trái x=15/115, cột phải x=360/460
            Panel pnlNhap = new Panel();
            pnlNhap.Dock = DockStyle.Bottom;
            pnlNhap.Height = 255;
            txtMa = O(115, 12, 80);
            txtMa.ReadOnly = true;      // mã tự tăng (IDENTITY)
            txtTen = O(115, 42, 220);
            txtHeDieuHanh = O(115, 72, 220);
            txtNamRaMat = O(115, 102, 100);
            txtRAM = O(115, 132, 100);
            txtBoNho = O(115, 162, 100);
            txtMauSac = O(460, 12, 220);
            txtGia = O(460, 42, 150);
            txtTon = O(460, 72, 100);
            txtTon.ReadOnly = true;     // không cho nhập SoLuongTon
            cboHang = Cbo(460, 102, 220);
            cboLoai = Cbo(460, 132, 220);
            txtMoTa = O(460, 162, 220);

            Button btnThem = Nut("Thêm", 15, 205, 80);
            btnThem.Click += btnThem_Click;
            Button btnSua = Nut("Sửa", 105, 205, 80);
            btnSua.Click += btnSua_Click;
            Button btnXoa = Nut("Xóa", 195, 205, 80);
            btnXoa.Click += btnXoa_Click;
            Button btnLamMoi = Nut("Làm mới", 285, 205, 80);
            btnLamMoi.Click += (s, e) => { TaiComboBox(); TaiDuLieu(); };

            pnlNhap.Controls.Add(Nhan("Mã ĐT:", 15, 12));
            pnlNhap.Controls.Add(txtMa);
            pnlNhap.Controls.Add(Nhan("Tên ĐT:", 15, 42));
            pnlNhap.Controls.Add(txtTen);
            pnlNhap.Controls.Add(Nhan("Hệ điều hành:", 15, 72));
            pnlNhap.Controls.Add(txtHeDieuHanh);
            pnlNhap.Controls.Add(Nhan("Năm ra mắt:", 15, 102));
            pnlNhap.Controls.Add(txtNamRaMat);
            pnlNhap.Controls.Add(Nhan("RAM (GB):", 15, 132));
            pnlNhap.Controls.Add(txtRAM);
            pnlNhap.Controls.Add(Nhan("Bộ nhớ (GB):", 15, 162));
            pnlNhap.Controls.Add(txtBoNho);
            pnlNhap.Controls.Add(Nhan("Màu sắc:", 360, 12));
            pnlNhap.Controls.Add(txtMauSac);
            pnlNhap.Controls.Add(Nhan("Giá niêm yết:", 360, 42));
            pnlNhap.Controls.Add(txtGia);
            pnlNhap.Controls.Add(Nhan("Số lượng tồn:", 360, 72));
            pnlNhap.Controls.Add(txtTon);
            pnlNhap.Controls.Add(Nhan("Hãng:", 360, 102));
            pnlNhap.Controls.Add(cboHang);
            pnlNhap.Controls.Add(Nhan("Loại:", 360, 132));
            pnlNhap.Controls.Add(cboLoai);
            pnlNhap.Controls.Add(Nhan("Mô tả:", 360, 162));
            pnlNhap.Controls.Add(txtMoTa);
            pnlNhap.Controls.AddRange(new Control[] { btnThem, btnSua, btnXoa, btnLamMoi });

            Controls.Add(dgv);
            Controls.Add(pnlNhap);
            Controls.Add(pnlTim);
        }

        // Dữ liệu ComboBox lấy từ BUS của bảng cha (hãng, loại)
        private void TaiComboBox()
        {
            cboHang.DisplayMember = "TenHang";
            cboHang.ValueMember = "MaHang";
            cboHang.DataSource = busHang.GetAll();
            cboLoai.DisplayMember = "TenLoai";
            cboLoai.ValueMember = "MaLoai";
            cboLoai.DataSource = busLoai.GetAll();
        }

        private void TaiDuLieu()
        {
            txtTimKiem.Clear();
            HienDanhSach(bus.GetAll());
        }

        private void HienDanhSach(ArrayList ds)
        {
            dsHienTai = ds;
            GridHelper.GanDuLieu(dgv, ds, typeof(DienThoai));
            GridHelper.BoChon(dgv);
            XoaO();
        }

        private void XoaO()
        {
            txtMa.Clear();
            txtTen.Clear();
            txtHeDieuHanh.Clear();
            txtNamRaMat.Clear();
            txtRAM.Clear();
            txtBoNho.Clear();
            txtMauSac.Clear();
            txtGia.Clear();
            txtTon.Clear();
            txtMoTa.Clear();
            if (cboHang.Items.Count > 0) cboHang.SelectedIndex = 0;
            if (cboLoai.Items.Count > 0) cboLoai.SelectedIndex = 0;
        }

        private static int DocInt(string s)
        {
            int v;
            return int.TryParse(s.Trim(), out v) ? v : 0;
        }

        private static decimal DocTien(string s)
        {
            decimal v;
            return TimKiemHelper.TryParseSo(s, out v) ? v : 0;
        }

        private DienThoai DocForm()
        {
            DienThoai x = new DienThoai();
            x.MaDT = DocInt(txtMa.Text);
            x.TenDT = txtTen.Text.Trim();
            x.HeDieuHanh = txtHeDieuHanh.Text.Trim();
            x.NamRaMat = DocInt(txtNamRaMat.Text);
            x.RAM = DocInt(txtRAM.Text);
            x.BoNho = DocInt(txtBoNho.Text);
            x.MauSac = txtMauSac.Text.Trim();
            x.GiaNiemYet = DocTien(txtGia.Text);
            x.SoLuongTon = DocInt(txtTon.Text);
            x.MoTa = txtMoTa.Text.Trim();
            x.MaHang = cboHang.SelectedValue == null ? 0 : Convert.ToInt32(cboHang.SelectedValue);
            x.MaLoai = cboLoai.SelectedValue == null ? 0 : Convert.ToInt32(cboLoai.SelectedValue);
            return x;
        }

        private void dgv_SelectionChanged(object sender, EventArgs e)
        {
            if (dgv.CurrentRow == null) return;
            DienThoai x = dgv.CurrentRow.DataBoundItem as DienThoai;
            if (x == null) return;
            txtMa.Text = x.MaDT.ToString();
            txtTen.Text = x.TenDT;
            txtHeDieuHanh.Text = x.HeDieuHanh;
            txtNamRaMat.Text = x.NamRaMat.ToString();
            txtRAM.Text = x.RAM.ToString();
            txtBoNho.Text = x.BoNho.ToString();
            txtMauSac.Text = x.MauSac;
            txtGia.Text = x.GiaNiemYet.ToString("#,##0");
            txtTon.Text = x.SoLuongTon.ToString();
            txtMoTa.Text = x.MoTa;
            cboHang.SelectedValue = x.MaHang;
            cboLoai.SelectedValue = x.MaLoai;
        }

        private void btnThem_Click(object sender, EventArgs e)
        {
            string loi = bus.Them(DocForm());
            if (loi != "") { MessageBox.Show(loi, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }
            MessageBox.Show("Thêm thành công");
            TaiDuLieu();
        }

        private void btnSua_Click(object sender, EventArgs e)
        {
            if (txtMa.Text == "") { MessageBox.Show("Hãy chọn một dòng trên lưới để sửa"); return; }
            string loi = bus.Sua(DocForm());
            if (loi != "") { MessageBox.Show(loi, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }
            MessageBox.Show("Sửa thành công");
            TaiDuLieu();
        }

        private void btnXoa_Click(object sender, EventArgs e)
        {
            if (txtMa.Text == "") { MessageBox.Show("Hãy chọn một dòng trên lưới để xóa"); return; }
            if (MessageBox.Show("Xóa điện thoại \"" + txtTen.Text + "\"?", "Xác nhận",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;
            string loi = bus.Xoa(DocInt(txtMa.Text));
            if (loi != "") { MessageBox.Show(loi, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }
            MessageBox.Show("Xóa thành công");
            TaiDuLieu();
        }

        private void btnTimNangCao_Click(object sender, EventArgs e)
        {
            using (FormTimNangCao f = new FormTimNangCao(typeof(DienThoai)))
            {
                if (f.ShowDialog() != DialogResult.OK) return;
                try
                {
                    HienDanhSach(bus.TimNangCao(f.KetQua));
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Lỗi tìm kiếm");
                }
            }
        }

        private void btnNhapExcel_Click(object sender, EventArgs e)
        {
            ArrayList ds = ExcelHelper.NhapCoHopThoai(typeof(DienThoai));
            if (ds == null) return;
            int soThanhCong;
            string loi = bus.NhapDanhSach(ds, out soThanhCong);
            string tb = "Đã nhập thành công " + soThanhCong + "/" + ds.Count + " dòng.";
            if (loi != "") tb += "\n\n" + loi;
            MessageBox.Show(tb, "Nhập Excel", MessageBoxButtons.OK,
                loi == "" ? MessageBoxIcon.Information : MessageBoxIcon.Warning);
            TaiDuLieu();
        }
    }
}
