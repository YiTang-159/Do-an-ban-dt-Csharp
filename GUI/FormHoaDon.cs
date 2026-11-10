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
    // Hỗ trợ tìm kiếm, tìm nâng cao, xuất Excel và in hóa đơn ra PDF.
    public class FormHoaDon : Form
    {
        private HoaDonBUS bus = new HoaDonBUS();
        private ArrayList dsHienTai = new ArrayList();
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

            txtTim = UiHelper.O(75, 11, 180);
            txtTim.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Enter)
                {
                    e.SuppressKeyPress = true;
                    NapHoaDon();
                }
            };

            Button btnTim = UiHelper.Nut("Tìm", 260, 8, 60);
            btnTim.Click += (s, e) => NapHoaDon();

            Button btnTatCa = UiHelper.Nut("Hiện tất cả", 325, 8, 85);
            btnTatCa.Click += (s, e) => TaiDuLieu();

            chkCuaToi = new CheckBox();
            chkCuaToi.Text = "Chỉ hóa đơn tôi lập";
            chkCuaToi.AutoSize = true;
            chkCuaToi.Location = new Point(415, 13);
            chkCuaToi.CheckedChanged += (s, e) => NapHoaDon();

            Button btnXuatExcel = UiHelper.Nut("Xuất Excel", 660, 8, 90);
            btnXuatExcel.Click += btnXuatExcel_Click;

            Button btnInHD = UiHelper.Nut("In hóa đơn (PDF)", 755, 8, 120);
            btnInHD.Click += btnInHD_Click;

            pnlTim.Controls.Add(UiHelper.Nhan("Tìm kiếm:", 10, 8));
            pnlTim.Controls.AddRange(new Control[] { txtTim, btnTim, btnTatCa, chkCuaToi, btnXuatExcel, btnInHD });

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
            HienDanhSach(ds);
        }

        private void HienDanhSach(ArrayList ds)
        {
            dsHienTai = ds;
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

        private void btnXuatExcel_Click(object sender, EventArgs e)
        {
            ExcelHelper.XuatCoHopThoai(dsHienTai, typeof(HoaDon), "HoaDon");
        }

        private void btnInHD_Click(object sender, EventArgs e)
        {
            HoaDon hd = UiHelper.DongDangChon<HoaDon>(dgvHD);
            if (hd == null)
            {
                MessageBox.Show("Hãy chọn một hóa đơn trong danh sách để in", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            ArrayList ct = bus.GetChiTiet(hd.MaHD);
            if (ct == null || ct.Count == 0)
            {
                MessageBox.Show("Hóa đơn này không có dòng sản phẩm nào", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (SaveFileDialog dlg = new SaveFileDialog())
            {
                dlg.Filter = "Tài liệu PDF (*.pdf)|*.pdf";
                dlg.FileName = "HoaDon_HD" + hd.MaHD.ToString("D6") + ".pdf";
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        InHoaDon.Xuat(hd, ct, dlg.FileName);
                        if (MessageBox.Show("Đã xuất hóa đơn thành công! Bạn có muốn mở xem file PDF không?", "Thành công",
                            MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                        {
                            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(dlg.FileName) { UseShellExecute = true });
                        }
                    }
                    catch (Exception ex)
                    {
                        UiHelper.Loi("Lỗi khi xuất PDF: " + ex.Message);
                    }
                }
            }
        }
    }
}
