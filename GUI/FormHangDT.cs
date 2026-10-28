using System;
using System.Collections;
using System.Drawing;
using System.Windows.Forms;
using QuanLyBanDienThoai.BUS;
using QuanLyBanDienThoai.DTO;

namespace QuanLyBanDienThoai.GUI
{
    // Form quản lý hãng điện thoại. Chỉ gọi BUS, không viết SQL.
    public class FormHangDT : Form
    {
        private HangDTBUS bus = new HangDTBUS();
        private ArrayList dsHienTai = new ArrayList();
        private DataGridView dgv;
        private TextBox txtTimKiem, txtMa, txtTen, txtQuocGia;

        public FormHangDT()
        {
            Text = "Quản lý hãng điện thoại";
            TaoGiaoDien();
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

        private void TaoGiaoDien()
        {
            dgv = new DataGridView();
            dgv.Dock = DockStyle.Fill;
            GridHelper.CauHinh(dgv);
            dgv.SelectionChanged += dgv_SelectionChanged;

            // thanh tìm kiếm phía trên
            Panel pnlTim = new Panel();
            pnlTim.Dock = DockStyle.Top;
            pnlTim.Height = 46;
            txtTimKiem = new TextBox();
            txtTimKiem.Location = new Point(75, 11);
            txtTimKiem.Width = 200;
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
            btnXuatExcel.Click += (s, e) => ExcelHelper.XuatCoHopThoai(dsHienTai, typeof(HangDT), "HangDT");
            Button btnNhapExcel = Nut("Nhập Excel", 670, 8, 95);
            btnNhapExcel.Click += btnNhapExcel_Click;
            pnlTim.Controls.Add(Nhan("Tìm kiếm:", 10, 8));
            pnlTim.Controls.Add(txtTimKiem);
            pnlTim.Controls.AddRange(new Control[] { btnTim, btnHienTatCa, btnTimNangCao, btnXuatExcel, btnNhapExcel });

            // ô nhập phía dưới
            Panel pnlNhap = new Panel();
            pnlNhap.Dock = DockStyle.Bottom;
            pnlNhap.Height = 160;
            txtMa = new TextBox();
            txtMa.Location = new Point(100, 12);
            txtMa.Width = 80;
            txtMa.ReadOnly = true;      // mã tự tăng (IDENTITY), không cho nhập
            txtTen = new TextBox();
            txtTen.Location = new Point(100, 42);
            txtTen.Width = 300;
            txtQuocGia = new TextBox();
            txtQuocGia.Location = new Point(100, 72);
            txtQuocGia.Width = 300;
            Button btnThem = Nut("Thêm", 15, 115, 80);
            btnThem.Click += btnThem_Click;
            Button btnSua = Nut("Sửa", 105, 115, 80);
            btnSua.Click += btnSua_Click;
            Button btnXoa = Nut("Xóa", 195, 115, 80);
            btnXoa.Click += btnXoa_Click;
            Button btnLamMoi = Nut("Làm mới", 285, 115, 80);
            btnLamMoi.Click += (s, e) => TaiDuLieu();
            pnlNhap.Controls.Add(Nhan("Mã hãng:", 15, 12));
            pnlNhap.Controls.Add(txtMa);
            pnlNhap.Controls.Add(Nhan("Tên hãng:", 15, 42));
            pnlNhap.Controls.Add(txtTen);
            pnlNhap.Controls.Add(Nhan("Quốc gia:", 15, 72));
            pnlNhap.Controls.Add(txtQuocGia);
            pnlNhap.Controls.AddRange(new Control[] { btnThem, btnSua, btnXoa, btnLamMoi });

            // Fill thêm vào trước, các thanh Top/Bottom thêm sau
            Controls.Add(dgv);
            Controls.Add(pnlNhap);
            Controls.Add(pnlTim);
        }

        // Nạp lại toàn bộ danh sách
        private void TaiDuLieu()
        {
            txtTimKiem.Clear();
            HienDanhSach(bus.GetAll());
        }

        private void HienDanhSach(ArrayList ds)
        {
            dsHienTai = ds;
            GridHelper.GanDuLieu(dgv, ds, typeof(HangDT));
            GridHelper.BoChon(dgv);
            XoaO();
        }

        private void XoaO()
        {
            txtMa.Clear();
            txtTen.Clear();
            txtQuocGia.Clear();
        }

        private HangDT DocForm()
        {
            HangDT x = new HangDT();
            int ma;
            int.TryParse(txtMa.Text, out ma);
            x.MaHang = ma;
            x.TenHang = txtTen.Text.Trim();
            x.QuocGia = txtQuocGia.Text.Trim();
            return x;
        }

        private void dgv_SelectionChanged(object sender, EventArgs e)
        {
            if (dgv.CurrentRow == null) return;
            HangDT x = dgv.CurrentRow.DataBoundItem as HangDT;
            if (x == null) return;
            txtMa.Text = x.MaHang.ToString();
            txtTen.Text = x.TenHang;
            txtQuocGia.Text = x.QuocGia;
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
            if (MessageBox.Show("Xóa hãng \"" + txtTen.Text + "\"?", "Xác nhận",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;
            string loi = bus.Xoa(DocForm().MaHang);
            if (loi != "") { MessageBox.Show(loi, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }
            MessageBox.Show("Xóa thành công");
            TaiDuLieu();
        }

        private void btnTimNangCao_Click(object sender, EventArgs e)
        {
            using (FormTimNangCao f = new FormTimNangCao(typeof(HangDT)))
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
            ArrayList ds = ExcelHelper.NhapCoHopThoai(typeof(HangDT));
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
