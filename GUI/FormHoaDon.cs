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
        private DataGridView dgvHD, dgvCT;
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

            dgvHD = new DataGridView();
            dgvHD.Dock = DockStyle.Fill;
            GridHelper.CauHinh(dgvHD);
            dgvHD.SelectionChanged += (s, e) => NapChiTiet();

            dgvCT = new DataGridView();
            dgvCT.Dock = DockStyle.Fill;
            GridHelper.CauHinh(dgvCT);

            SplitContainer sc = new SplitContainer();
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
            GridHelper.GanDuLieu(dgvHD, ds, typeof(HoaDon));
            UiHelper.Giau(dgvHD, "MaKH", "MaNV");
            UiHelper.TieuDe(dgvHD, "MaHD", "Mã hóa đơn");
            UiHelper.TieuDe(dgvHD, "NgayLap", "Ngày lập");
            UiHelper.TieuDe(dgvHD, "PhuongThucTT", "Thanh toán");
            UiHelper.TieuDe(dgvHD, "TongTien", "Tổng tiền");
            UiHelper.TieuDe(dgvHD, "TenKhachHang", "Khách hàng");
            UiHelper.TieuDe(dgvHD, "TenNhanVien", "Nhân viên lập");
            lblDoanhThu.Text = "  Tổng doanh thu (" + ds.Count + " hóa đơn): " + UiHelper.Tien(bus.TinhTongTien(ds));
            NapChiTiet();
        }

        private void NapChiTiet()
        {
            HoaDon hd = UiHelper.DongDangChon<HoaDon>(dgvHD);
            ArrayList ct = hd == null ? new ArrayList() : bus.GetChiTiet(hd.MaHD);
            GridHelper.GanDuLieu(dgvCT, ct, typeof(ChiTietHoaDon));
            UiHelper.Giau(dgvCT, "MaHD", "MaKM");
            UiHelper.TieuDe(dgvCT, "MaDT", "Mã");
            UiHelper.TieuDe(dgvCT, "TenHienThi", "Điện thoại");
            UiHelper.TieuDe(dgvCT, "SoLuong", "Số lượng");
            UiHelper.TieuDe(dgvCT, "DonGia", "Đơn giá");
            UiHelper.TieuDe(dgvCT, "TenKM", "Khuyến mãi");
            UiHelper.TieuDe(dgvCT, "ThanhTien", "Thành tiền");
        }
    }
}
