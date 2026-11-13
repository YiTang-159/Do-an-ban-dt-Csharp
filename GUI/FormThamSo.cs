using System;
using System.Collections;
using System.Drawing;
using System.Windows.Forms;
using QuanLyBanDienThoai.BUS;
using QuanLyBanDienThoai.DTO;

namespace QuanLyBanDienThoai.GUI
{
    // Quản lý bảng tham số (chỉ Admin thấy). Chỉ gọi BUS, không viết SQL.
    public class FormThamSo : Form
    {
        private readonly ThamSoBUS bus = new ThamSoBUS();
        private readonly DataGridView grid = new DataGridView();
        private readonly TextBox txtTim = new TextBox();
        private readonly TextBox txtTen = new TextBox();
        private readonly TextBox txtGiaTri = new TextBox();
        private readonly TextBox txtMoTa = new TextBox();

        public FormThamSo()
        {
            Text = "Quản lý tham số";
            Width = 1000;
            Height = 700;

            // Thanh tìm kiếm và các nút
            FlowLayoutPanel top = new FlowLayoutPanel { Dock = DockStyle.Top, Height = 42 };
            top.Controls.Add(new Label { Text = "Tìm kiếm", AutoSize = true, Padding = new Padding(5, 10, 0, 0) });
            txtTim.Width = 180;
            txtTim.TextChanged += (s, e) => TaiLuoi(txtTim.Text);
            top.Controls.Add(txtTim);
            top.Controls.Add(TaoNut("Thêm", Them));
            top.Controls.Add(TaoNut("Sửa", Sua));
            top.Controls.Add(TaoNut("Xóa", Xoa));
            top.Controls.Add(TaoNut("Làm mới", LamMoi));

            // Ba ô nhập
            FlowLayoutPanel fields = new FlowLayoutPanel { Dock = DockStyle.Top, Height = 80 };
            fields.Controls.Add(TaoO("Tên tham số", txtTen, 240));
            fields.Controls.Add(TaoO("Giá trị (> 0)", txtGiaTri, 160));
            fields.Controls.Add(TaoO("Mô tả", txtMoTa, 420));

            grid.Dock = DockStyle.Fill;
            grid.ReadOnly = true;
            grid.AutoGenerateColumns = true;
            grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            grid.MultiSelect = false;
            grid.AllowUserToAddRows = false;
            grid.SelectionChanged += (s, e) => DoVaoO();

            // Fill thêm vào trước, các thanh Top thêm sau
            Controls.Add(grid);
            Controls.Add(fields);
            Controls.Add(top);

            TaiLuoi("");
        }

        private static Button TaoNut(string ten, Action hanhDong)
        {
            Button b = new Button { Text = ten, Width = 90 };
            b.Click += (s, e) => hanhDong();
            return b;
        }

        private static Panel TaoO(string nhan, TextBox o, int rong)
        {
            Panel p = new Panel { Width = rong, Height = 56 };
            p.Controls.Add(new Label { Text = nhan, Dock = DockStyle.Top });
            o.Dock = DockStyle.Bottom;
            p.Controls.Add(o);
            return p;
        }

        private void TaiLuoi(string tuKhoa)
        {
            try
            {
                grid.DataSource = null;
                grid.DataSource = string.IsNullOrWhiteSpace(tuKhoa) ? bus.GetAll() : bus.TimKiem(tuKhoa);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        // Chọn dòng nào thì đổ lên 3 ô nhập
        private void DoVaoO()
        {
            ThamSo x = grid.CurrentRow == null ? null : grid.CurrentRow.DataBoundItem as ThamSo;
            if (x == null) return;
            txtTen.Text = x.TenThamSo;
            txtGiaTri.Text = x.GiaTri.ToString("0.##");
            txtMoTa.Text = x.MoTa;
        }

        private ThamSo DocTuO()
        {
            decimal giaTri;
            if (!TimKiemHelper.TryParseSo(txtGiaTri.Text, out giaTri))
                throw new FormatException("Giá trị phải là số");
            return new ThamSo { TenThamSo = txtTen.Text.Trim(), GiaTri = giaTri, MoTa = txtMoTa.Text.Trim() };
        }

        private void LamMoi()
        {
            txtTen.Clear();
            txtGiaTri.Clear();
            txtMoTa.Clear();
            txtTim.Clear();
            TaiLuoi("");
        }

        private void KetQua(string loi)
        {
            if (loi != "") MessageBox.Show(loi, "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            else TaiLuoi(txtTim.Text);
        }

        private void Them()
        {
            try { KetQua(bus.Them(DocTuO())); }
            catch (FormatException ex) { MessageBox.Show(ex.Message); }
        }

        private void Sua()
        {
            ThamSo cu = grid.CurrentRow == null ? null : grid.CurrentRow.DataBoundItem as ThamSo;
            if (cu == null) { MessageBox.Show("Hãy chọn một tham số trong danh sách"); return; }
            try
            {
                ThamSo moi = DocTuO();
                // Tên là khóa chính nên không đổi tên được; muốn tên khác thì Thêm mới
                if (!string.Equals(moi.TenThamSo, cu.TenThamSo, StringComparison.Ordinal))
                {
                    MessageBox.Show("Không đổi được tên tham số. Hãy trả tên về \"" + cu.TenThamSo + "\" hoặc dùng nút Thêm.");
                    return;
                }
                KetQua(bus.Sua(moi));
            }
            catch (FormatException ex) { MessageBox.Show(ex.Message); }
        }

        private void Xoa()
        {
            ThamSo cu = grid.CurrentRow == null ? null : grid.CurrentRow.DataBoundItem as ThamSo;
            if (cu == null) { MessageBox.Show("Hãy chọn một tham số trong danh sách"); return; }
            if (MessageBox.Show("Xóa tham số " + cu.TenThamSo + "?", "Xác nhận", MessageBoxButtons.YesNo) != DialogResult.Yes) return;
            KetQua(bus.Xoa(cu.TenThamSo));
        }
    }
}
