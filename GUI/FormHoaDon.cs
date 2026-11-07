using System;
using System.Collections;
using System.Drawing;
using System.Windows.Forms;
using QuanLyBanDienThoai.BUS;
using QuanLyBanDienThoai.DTO;

namespace QuanLyBanDienThoai.GUI
{
    // Form main-sub: lưới hóa đơn ở trên (1), bấm một hóa đơn thì lưới chi tiết hóa đơn ở dưới (n) hiện theo mã.
    // Nhân viên xem được mọi hóa đơn hoặc chỉ hóa đơn do mình lập (ô tick).
    public class FormHoaDon : Form
    {
        private HoaDonBUS bus = new HoaDonBUS();
        private ListView dgvHD, dgvCT;
        private TextBox txtTim;
        private CheckBox chkCuaToi;
        private Label lblDoanhThu;

        public FormHoaDon()
        {
            Text = "Danh sách hóa đơn";
            TaoGiaoDien();
            TaiDuLieu();
        }

        private void TaoGiaoDien()
        {
            Panel pnlTim = new Panel();
            pnlTim.Dock = DockStyle.Top;
            pnlTim.Height = 46;
            txtTim = UiHelper.O(75, 11, 220);
            txtTim.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Enter)
                {
                    e.SuppressKeyPress = true;
                    NapHoaDon();
                }
            };
            Button btnTim = UiHelper.Nut("Tìm", 305, 8, 70);
            btnTim.Click += (s, e) => NapHoaDon();
            Button btnTatCa = UiHelper.Nut("Hiện tất cả", 380, 8, 100);
            btnTatCa.Click += (s, e) => TaiDuLieu();
            chkCuaToi = new CheckBox();
            chkCuaToi.Text = "Chỉ hóa đơn tôi lập";
            chkCuaToi.AutoSize = true;
            chkCuaToi.Location = new Point(495, 13);
            chkCuaToi.CheckedChanged += (s, e) => NapHoaDon();
            pnlTim.Controls.Add(UiHelper.Nhan("Tìm kiếm:", 10, 8));
            pnlTim.Controls.AddRange(new Control[] { txtTim, btnTim, btnTatCa, chkCuaToi });

            dgvHD = ListViewHelper.Tao();
            dgvHD.SelectedIndexChanged += (s,e) => NapChiTiet();
            dgvCT = ListViewHelper.Tao();

            SplitContainer sc = new SplitContainer();
            sc.Size = new Size(1000,600);
            sc.Dock = DockStyle.Fill;
            sc.Orientation = Orientation.Horizontal;
            sc.Panel1.Controls.Add(dgvHD);
            sc.Panel2.Controls.Add(dgvCT);

            lblDoanhThu = new Label();
            lblDoanhThu.Dock = DockStyle.Bottom;
            lblDoanhThu.Height = 34;
            lblDoanhThu.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            lblDoanhThu.ForeColor = Color.FromArgb(200, 40, 40);
            lblDoanhThu.TextAlign = ContentAlignment.MiddleLeft;

            // Fill thêm vào trước, các thanh Top/Bottom thêm sau
            Controls.Add(sc);
            Controls.Add(lblDoanhThu);
            Controls.Add(pnlTim);
        }

        private void TaiDuLieu()
        {
            txtTim.Clear();
            NapHoaDon();
        }

        private void NapHoaDon()
        {
            ArrayList ds = bus.TimKiem(txtTim.Text);
            if (chkCuaToi.Checked) ds = bus.LocCuaToi(ds);
            ListViewHelper.Gan(dgvHD, ds, "MaHD|Mã hóa đơn", "NgayLap|Ngày lập", "PhuongThucTT|Thanh toán", "TongTien|Tổng tiền", "TenKhachHang|Khách hàng", "TenNhanVien|Nhân viên lập");
            lblDoanhThu.Text = "  Tổng doanh thu (" + ds.Count + " hóa đơn): " + UiHelper.Tien(bus.TinhTongTien(ds));
            NapChiTiet();
        }

        private void NapChiTiet()
        {
            HoaDon hd = ListViewHelper.Chon<HoaDon>(dgvHD);
            ArrayList ct = hd == null ? new ArrayList() : bus.GetChiTiet(hd.MaHD);
            ListViewHelper.Gan(dgvCT, ct, "MaDT|Mã", "TenHienThi|Điện thoại", "SoLuong|Số lượng", "DonGia|Đơn giá", "TenKM|Khuyến mãi", "ThanhTien|Thành tiền");
        }
    }
}
