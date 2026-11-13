using System;
using System.Collections;
using System.Drawing;
using System.Windows.Forms;
using QuanLyBanDienThoai.BUS;
using QuanLyBanDienThoai.DTO;

namespace QuanLyBanDienThoai.GUI
{
    // Form main-sub: ListView phiếu nhập (1) ở trên, bấm một phiếu thì ListView chi tiết (n) ở dưới.
    public class FormPhieuNhap : Form
    {
        private readonly PhieuNhapHangBUS bus = new PhieuNhapHangBUS();
        private readonly ListView master = ListViewHelper.Tao(), detail = ListViewHelper.Tao();
        private readonly Label total = new Label { Dock = DockStyle.Bottom, Height = 35 };
        private readonly TextBox txtTim = new TextBox { Width = 200 };

        public FormPhieuNhap()
        {
            Text = "Danh sách phiếu nhập và chi tiết"; Width = 1100; Height = 700;
            FlowLayoutPanel top = new FlowLayoutPanel { Dock = DockStyle.Top, Height = 44 };
            top.Controls.Add(new Label { Text = "Tìm kiếm", AutoSize = true, Padding = new Padding(4, 8, 0, 0) });
            top.Controls.Add(txtTim);
            txtTim.TextChanged += (s, e) => Hien(bus.TimKiem(txtTim.Text));
            Button advanced = new Button { Text = "Tìm nâng cao", Width = 110 };
            advanced.Click += (s, e) =>
            {
                try
                {
                    using (FormTimNangCao f = new FormTimNangCao(typeof(PhieuNhapHang)))
                        if (f.ShowDialog() == DialogResult.OK) Hien(bus.TimNangCao(f.KetQua));
                }
                catch (Exception ex) { MessageBox.Show(ex.Message); }
            };
            top.Controls.Add(advanced);
            Button refresh = new Button { Text = "Làm mới", Width = 90 };
            refresh.Click += (s, e) => { txtTim.Clear(); Hien(bus.GetAll()); };
            top.Controls.Add(refresh);

            SplitContainer split = new SplitContainer { Size = new Size(1000, 600), Dock = DockStyle.Fill, Orientation = Orientation.Horizontal, SplitterDistance = 300 };
            split.Panel1.Controls.Add(master); split.Panel2.Controls.Add(detail);
            Controls.Add(split); Controls.Add(top); Controls.Add(total);
            master.SelectedIndexChanged += (s, e) => LoadDetail();
            try { Hien(bus.GetAll()); } catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        private void Hien(ArrayList ds)
        {
            try
            {
                ListViewHelper.Gan(master, ds, "MaPN|Mã phiếu", "NgayNhap|Ngày nhập", "TongTien|Tổng tiền", "TenNCC|Nhà cung cấp", "TenNhanVien|Nhân viên");
                total.Text = "Tổng tiền nhập (" + ds.Count + " phiếu): " + bus.TinhTongTien(ds).ToString("N0") + " đ";
                LoadDetail();
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        private void LoadDetail()
        {
            try
            {
                PhieuNhapHang p = ListViewHelper.Chon<PhieuNhapHang>(master);
                ArrayList ct = p == null ? new ArrayList() : bus.GetChiTiet(p.MaPN);
                ListViewHelper.Gan(detail, ct, "MaPN|Mã phiếu", "MaDT|Mã điện thoại", "TenDT|Điện thoại", "SoLuong|Số lượng", "DonGiaNhap|Giá nhập", "ThanhTien|Thành tiền");
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }
    }
}
