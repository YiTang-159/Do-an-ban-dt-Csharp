using System;
using System.Collections;
using System.Drawing;
using System.Windows.Forms;
using QuanLyBanDienThoai.BUS;
using QuanLyBanDienThoai.DTO;

namespace QuanLyBanDienThoai.GUI
{
    // Lập phiếu nhập hàng. Chỉ gọi BUS, không viết SQL.
    //  - Nhân viên lập phiếu là người đang đăng nhập (khóa ô, không cho chọn).
    //  - Nút "…" mở FormChon để chọn nhà cung cấp và điện thoại (kết quả ở biến static FormChon.KetQua).
    //  - Thành tiền = số lượng x giá nhập; tổng tiền = tổng thành tiền; lưu xong tồn kho tăng.
    public class FormNhapHang : Form
    {
        private readonly PhieuNhapHangBUS bus = new PhieuNhapHangBUS();
        private readonly ArrayList dong = new ArrayList();      // các ChiTietPhieuNhapHang đang lập

        private readonly ComboBox cboNCC = new ComboBox();
        private readonly ComboBox cboNV = new ComboBox();
        private readonly ComboBox cboDT = new ComboBox();
        private readonly NumericUpDown numSL = new NumericUpDown();
        private readonly NumericUpDown numGia = new NumericUpDown();
        private readonly DataGridView grid = new DataGridView();
        private readonly Label lblTong = new Label();

        public FormNhapHang()
        {
            Text = "Lập phiếu nhập hàng";
            Width = 1050;
            Height = 680;

            FlowLayoutPanel top = new FlowLayoutPanel { Dock = DockStyle.Top, Height = 180, AutoScroll = true };

            ThemO(top, "Nhà cung cấp", cboNCC);
            ThemNutChon(top, cboNCC, () => new NhaCungCapBUS().GetAll(), new string[] { "MaNCC", "TenNCC", "SDT" }, "MaNCC");
            ThemO(top, "Nhân viên lập (đang đăng nhập)", cboNV);
            ThemO(top, "Điện thoại", cboDT);
            ThemNutChon(top, cboDT, () => new DienThoaiBUS().GetAll(), new string[] { "MaDT", "TenHienThi", "SoLuongTon" }, "MaDT");
            ThemO(top, "Số lượng", numSL);
            ThemO(top, "Giá nhập", numGia);

            numSL.Minimum = 1;
            numSL.Maximum = 100000;
            numGia.Minimum = 1;
            numGia.Maximum = 100000000000m;
            numGia.ThousandsSeparator = true;

            top.Controls.Add(TaoNut("Thêm dòng", 105, ThemDong));
            top.Controls.Add(TaoNut("Xóa dòng", 105, XoaDong));
            top.Controls.Add(TaoNut("Lập phiếu nhập", 125, LuuPhieu));
            top.Controls.Add(TaoNut("Làm mới", 100, LamMoi));

            lblTong.Dock = DockStyle.Bottom;
            lblTong.Height = 35;
            lblTong.Font = new Font(Font, FontStyle.Bold);

            grid.Dock = DockStyle.Fill;
            grid.ReadOnly = true;
            grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            grid.AllowUserToAddRows = false;

            // Fill thêm vào trước, các thanh Top/Bottom thêm sau
            Controls.Add(grid);
            Controls.Add(lblTong);
            Controls.Add(top);

            NapDuLieu();
            CapNhatLuoi();
        }

        private void NapDuLieu()
        {
            try
            {
                Gan(cboNCC, new NhaCungCapBUS().GetAll(), "TenNCC", "MaNCC");
                Gan(cboNV, new NhanVienBUS().GetAll(), "HoTen", "MaNV");
                if (PhienDangNhap.MaNV > 0) cboNV.SelectedValue = PhienDangNhap.MaNV;
                cboNV.Enabled = false;
                Gan(cboDT, new DienThoaiBUS().GetAll(), "TenHienThi", "MaDT");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        // ---------- dựng giao diện ----------

        private static void ThemO(FlowLayoutPanel panel, string tieuDe, Control o)
        {
            Panel p = new Panel { Width = 220, Height = 55 };
            p.Controls.Add(new Label { Text = tieuDe, Dock = DockStyle.Top });
            o.Dock = DockStyle.Bottom;
            p.Controls.Add(o);
            panel.Controls.Add(p);
        }

        private static Button TaoNut(string ten, int rong, Action hanhDong)
        {
            Button b = new Button { Text = ten, Width = rong };
            b.Click += (s, e) => hanhDong();
            return b;
        }

        // Nút "…": mở FormChon, chọn xong thì đặt giá trị cho ComboBox
        private static void ThemNutChon(FlowLayoutPanel panel, ComboBox cbo, Func<ArrayList> layDanhSach, string[] cacCot, string tenKhoa)
        {
            Button b = new Button { Text = "…", Width = 35, Height = 32 };
            b.Click += (s, e) =>
            {
                try
                {
                    object chon = FormChon.Chon(layDanhSach(), cacCot, "Chọn dữ liệu");
                    if (chon != null) cbo.SelectedValue = chon.GetType().GetProperty(tenKhoa).GetValue(chon, null);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            };
            panel.Controls.Add(b);
        }

        private static void Gan(ComboBox cbo, ArrayList ds, string hienThi, string giaTri)
        {
            cbo.DropDownStyle = ComboBoxStyle.DropDownList;
            cbo.DataSource = ds;
            cbo.DisplayMember = hienThi;
            cbo.ValueMember = giaTri;
        }

        private static int MaDangChon(ComboBox cbo)
        {
            return cbo.SelectedValue is int ma ? ma : 0;
        }

        // ---------- xử lý ----------

        private void ThemDong()
        {
            string loi = bus.TaoDong(dong, MaDangChon(cboDT), (int)numSL.Value, numGia.Value);
            if (loi != "") MessageBox.Show(loi);
            else CapNhatLuoi();
        }

        private void XoaDong()
        {
            ChiTietPhieuNhapHang d = grid.CurrentRow == null ? null : grid.CurrentRow.DataBoundItem as ChiTietPhieuNhapHang;
            if (d == null) { MessageBox.Show("Hãy chọn một dòng để xóa"); return; }
            dong.Remove(d);
            CapNhatLuoi();
        }

        private void LuuPhieu()
        {
            int maPN;
            string loi = bus.LuuPhieuNhap(new PhieuNhapHang { MaNCC = MaDangChon(cboNCC) }, dong, out maPN);
            if (loi != "") { MessageBox.Show(loi); return; }
            MessageBox.Show("Đã lưu phiếu nhập #" + maPN + ". Tồn kho đã được cập nhật.");
            dong.Clear();
            CapNhatLuoi();
        }

        private void LamMoi()
        {
            dong.Clear();
            numSL.Value = 1;
            numGia.Value = 1;
            if (cboNCC.Items.Count > 0) cboNCC.SelectedIndex = 0;
            if (cboDT.Items.Count > 0) cboDT.SelectedIndex = 0;
            CapNhatLuoi();
        }

        private void CapNhatLuoi()
        {
            grid.DataSource = null;
            grid.DataSource = dong.ToArray();
            lblTong.Text = "Tổng tiền: " + bus.TinhTongTienNhap(dong).ToString("N0") + " đ";
        }
    }
}
