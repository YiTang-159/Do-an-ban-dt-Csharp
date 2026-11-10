using System;
using System.Collections;
using System.Drawing;
using System.Windows.Forms;
using QuanLyBanDienThoai.BUS;
using QuanLyBanDienThoai.DTO;

namespace QuanLyBanDienThoai.GUI
{
    // Form main-sub: lưới hóa đơn ở trên (1), bấm một hóa đơn thì lưới chi tiết hóa đơn ở dưới (n) hiện theo mã.
    // Sử dụng ListView theo chuẩn cập nhật của nhánh main.
    public class FormHoaDon : Form
    {
        private HoaDonBUS bus = new HoaDonBUS();
        private ArrayList dsHienTai = new ArrayList();
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

            txtTim = UiHelper.O(75, 11, 180);
            txtTim.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Enter)
                {
                    e.SuppressKeyPress = true;
                    NapHoaDon();
                }
            };

            Button btnTim = UiHelper.Nut("Tìm", 265, 8, 60);
            btnTim.Click += (s, e) => NapHoaDon();

            Button btnTatCa = UiHelper.Nut("Hiện tất cả", 330, 8, 85);
            btnTatCa.Click += (s, e) => TaiDuLieu();

            chkCuaToi = new CheckBox();
            chkCuaToi.Text = "Chỉ hóa đơn tôi lập";
            chkCuaToi.AutoSize = true;
            chkCuaToi.Location = new Point(425, 13);
            chkCuaToi.CheckedChanged += (s, e) => NapHoaDon();

            Button btnXuatExcel = UiHelper.Nut("Xuất Excel", 575, 8, 90);
            btnXuatExcel.Click += btnXuatExcel_Click;

            Button btnInHD = UiHelper.Nut("In hóa đơn (PDF)", 675, 8, 125);
            btnInHD.Click += btnInHD_Click;

            pnlTim.Controls.Add(UiHelper.Nhan("Tìm kiếm:", 10, 8));
            pnlTim.Controls.AddRange(new Control[] { txtTim, btnTim, btnTatCa, chkCuaToi, btnXuatExcel, btnInHD });

            dgvHD = ListViewHelper.Tao();
            dgvHD.SelectedIndexChanged += (s, e) => NapChiTiet();
            dgvCT = ListViewHelper.Tao();

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
            HienDanhSach(ds);
        }

        private void HienDanhSach(ArrayList ds)
        {
            dsHienTai = ds;
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

        private void btnXuatExcel_Click(object sender, EventArgs e)
        {
            ExcelHelper.XuatCoHopThoai(dsHienTai, typeof(HoaDon), "HoaDon");
        }

        private void btnInHD_Click(object sender, EventArgs e)
        {
            HoaDon hd = ListViewHelper.Chon<HoaDon>(dgvHD);
            if (hd == null)
            {
                MessageBox.Show("Hãy chọn một hóa đơn trong danh sách để in", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            ArrayList dsChiTiet = bus.GetChiTiet(hd.MaHD);
            if (dsChiTiet == null || dsChiTiet.Count == 0)
            {
                MessageBox.Show("Hóa đơn này không có dòng sản phẩm nào", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (SaveFileDialog sfd = new SaveFileDialog())
            {
                sfd.Filter = "Tài liệu PDF (*.pdf)|*.pdf";
                sfd.FileName = "HoaDon_HD" + hd.MaHD.ToString("D6") + ".pdf";
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        InHoaDon.Xuat(hd, dsChiTiet, sfd.FileName);
                        if (MessageBox.Show("Đã xuất hóa đơn thành công! Bạn có muốn mở xem file PDF không?", "Thành công",
                            MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                        {
                            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(sfd.FileName) { UseShellExecute = true });
                        }
                    }
                    catch (Exception ex)
                    {
                        UiHelper.Loi("Lỗi khi in hóa đơn: " + ex.Message);
                    }
                }
            }
        }
    }
}
