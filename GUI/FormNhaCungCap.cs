using System;
using System.Collections;
using System.Drawing;
using System.Windows.Forms;
using QuanLyBanDienThoai.BUS;
using QuanLyBanDienThoai.DTO;

namespace QuanLyBanDienThoai.GUI
{
    // Form quản lý nhà cung cấp. Chỉ gọi BUS, không viết SQL.
    public class FormNhaCungCap : Form
    {
        private NhaCungCapBUS bus = new NhaCungCapBUS();
        private ArrayList dsHienTai = new ArrayList();
        private DataGridView dgv;
        private TextBox txtTimKiem, txtMa, txtTen, txtDiaChi, txtSDT, txtEmail;

        public FormNhaCungCap()
        {
            Text = "Quản lý nhà cung cấp";
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

        private static TextBox O(int x, int y, int w)
        {
            TextBox t = new TextBox();
            t.Location = new Point(x, y);
            t.Width = w;
            return t;
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
            btnXuatExcel.Click += (s, e) => ExcelHelper.XuatCoHopThoai(dsHienTai, typeof(NhaCungCap), "NhaCungCap");
            Button btnNhapExcel = Nut("Nhập Excel", 670, 8, 95);
            btnNhapExcel.Click += btnNhapExcel_Click;
            pnlTim.Controls.Add(Nhan("Tìm kiếm:", 10, 8));
            pnlTim.Controls.Add(txtTimKiem);
            pnlTim.Controls.AddRange(new Control[] { btnTim, btnHienTatCa, btnTimNangCao, btnXuatExcel, btnNhapExcel });

            Panel pnlNhap = new Panel();
            pnlNhap.Dock = DockStyle.Bottom;
            pnlNhap.Height = 165;
            txtMa = O(110, 12, 80);
            txtMa.ReadOnly = true;      // mã tự tăng (IDENTITY), không cho nhập
            txtTen = O(110, 42, 280);
            txtDiaChi = O(110, 72, 280);
            txtSDT = O(500, 12, 200);
            txtEmail = O(500, 42, 200);
            Button btnThem = Nut("Thêm", 15, 120, 80);
            btnThem.Click += btnThem_Click;
            Button btnSua = Nut("Sửa", 105, 120, 80);
            btnSua.Click += btnSua_Click;
            Button btnXoa = Nut("Xóa", 195, 120, 80);
            btnXoa.Click += btnXoa_Click;
            Button btnLamMoi = Nut("Làm mới", 285, 120, 80);
            btnLamMoi.Click += (s, e) => TaiDuLieu();
            pnlNhap.Controls.Add(Nhan("Mã NCC:", 15, 12));
            pnlNhap.Controls.Add(txtMa);
            pnlNhap.Controls.Add(Nhan("Tên NCC:", 15, 42));
            pnlNhap.Controls.Add(txtTen);
            pnlNhap.Controls.Add(Nhan("Địa chỉ:", 15, 72));
            pnlNhap.Controls.Add(txtDiaChi);
            pnlNhap.Controls.Add(Nhan("SĐT:", 420, 12));
            pnlNhap.Controls.Add(txtSDT);
            pnlNhap.Controls.Add(Nhan("Email:", 420, 42));
            pnlNhap.Controls.Add(txtEmail);
            pnlNhap.Controls.AddRange(new Control[] { btnThem, btnSua, btnXoa, btnLamMoi });

            Controls.Add(dgv);
            Controls.Add(pnlNhap);
            Controls.Add(pnlTim);
        }

        private void TaiDuLieu()
        {
            txtTimKiem.Clear();
            HienDanhSach(bus.GetAll());
        }

        private void HienDanhSach(ArrayList ds)
        {
            dsHienTai = ds;
            GridHelper.GanDuLieu(dgv, ds, typeof(NhaCungCap));
            GridHelper.BoChon(dgv);
            XoaO();
        }

        private void XoaO()
        {
            txtMa.Clear();
            txtTen.Clear();
            txtDiaChi.Clear();
            txtSDT.Clear();
            txtEmail.Clear();
        }

        private NhaCungCap DocForm()
        {
            NhaCungCap x = new NhaCungCap();
            int ma;
            int.TryParse(txtMa.Text, out ma);
            x.MaNCC = ma;
            x.TenNCC = txtTen.Text.Trim();
            x.DiaChi = txtDiaChi.Text.Trim();
            x.SDT = txtSDT.Text.Trim();
            x.Email = txtEmail.Text.Trim();
            return x;
        }

        private void dgv_SelectionChanged(object sender, EventArgs e)
        {
            if (dgv.CurrentRow == null) return;
            NhaCungCap x = dgv.CurrentRow.DataBoundItem as NhaCungCap;
            if (x == null) return;
            txtMa.Text = x.MaNCC.ToString();
            txtTen.Text = x.TenNCC;
            txtDiaChi.Text = x.DiaChi;
            txtSDT.Text = x.SDT;
            txtEmail.Text = x.Email;
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
            if (MessageBox.Show("Xóa nhà cung cấp \"" + txtTen.Text + "\"?", "Xác nhận",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;
            string loi = bus.Xoa(DocForm().MaNCC);
            if (loi != "") { MessageBox.Show(loi, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }
            MessageBox.Show("Xóa thành công");
            TaiDuLieu();
        }

        private void btnTimNangCao_Click(object sender, EventArgs e)
        {
            using (FormTimNangCao f = new FormTimNangCao(typeof(NhaCungCap)))
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
            ArrayList ds = ExcelHelper.NhapCoHopThoai(typeof(NhaCungCap));
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
