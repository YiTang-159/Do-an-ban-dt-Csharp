using System;
using System.Drawing;
using System.Windows.Forms;

namespace QuanLyBanDienThoai.GUI
{
    // Form chính (menu). Ai làm xong form nào thì BỎ DẤU // ở đúng dòng của mình.
    // Chỉ sửa đúng dòng của mình để tránh xung đột khi gộp code.
    public class MainForm : Form
    {
        public MainForm()
        {
            Text = "Quản lý cửa hàng bán điện thoại";
            Size = new Size(900, 560);
            StartPosition = FormStartPosition.CenterScreen;

            MenuStrip menu = new MenuStrip();

            ToolStripMenuItem mDanhMuc = new ToolStripMenuItem("Danh mục");
            // TV1
            // mDanhMuc.DropDownItems.Add("Hãng điện thoại", null, (s, e) => new FormHangDT().ShowDialog());
            // mDanhMuc.DropDownItems.Add("Loại điện thoại", null, (s, e) => new FormLoaiDienThoai().ShowDialog());
            // mDanhMuc.DropDownItems.Add("Nhà cung cấp", null, (s, e) => new FormNhaCungCap().ShowDialog());
            // mDanhMuc.DropDownItems.Add("Điện thoại", null, (s, e) => new FormDienThoai().ShowDialog());
            // mDanhMuc.DropDownItems.Add("Phiên bản", null, (s, e) => new FormPhienBan().ShowDialog());
            menu.Items.Add(mDanhMuc);

            ToolStripMenuItem mKho = new ToolStripMenuItem("Kho");
            // TV2
            // mKho.DropDownItems.Add("Kho", null, (s, e) => new FormKho().ShowDialog());
            // mKho.DropDownItems.Add("Tồn kho", null, (s, e) => new FormTonKho().ShowDialog());
            menu.Items.Add(mKho);

            ToolStripMenuItem mNhanSu = new ToolStripMenuItem("Nhân sự - Khách hàng");
            // TV2
            // mNhanSu.DropDownItems.Add("Nhân viên", null, (s, e) => new FormNhanVien().ShowDialog());
            // mNhanSu.DropDownItems.Add("Khách hàng", null, (s, e) => new FormKhachHang().ShowDialog());
            menu.Items.Add(mNhanSu);

            ToolStripMenuItem mKhuyenMai = new ToolStripMenuItem("Khuyến mãi");
            // TV4
            // mKhuyenMai.DropDownItems.Add("Khuyến mãi", null, (s, e) => new FormKhuyenMai().ShowDialog());
            // mKhuyenMai.DropDownItems.Add("Chi tiết khuyến mãi", null, (s, e) => new FormChiTietKhuyenMai().ShowDialog());
            menu.Items.Add(mKhuyenMai);

            ToolStripMenuItem mBanHang = new ToolStripMenuItem("Bán hàng");
            // TV3
            // mBanHang.DropDownItems.Add("Lập hóa đơn bán hàng", null, (s, e) => new FormBanHang().ShowDialog());
            // mBanHang.DropDownItems.Add("Danh sách hóa đơn", null, (s, e) => new FormHoaDon().ShowDialog());
            // mBanHang.DropDownItems.Add("Giỏ hàng", null, (s, e) => new FormGioHang().ShowDialog());
            // TV4
            // mBanHang.DropDownItems.Add("Bảo hành", null, (s, e) => new FormBaoHanh().ShowDialog());
            menu.Items.Add(mBanHang);

            ToolStripMenuItem mThoat = new ToolStripMenuItem("Thoát");
            mThoat.Click += (s, e) => Close();
            menu.Items.Add(mThoat);

            Label lb = new Label();
            lb.Text = "QUẢN LÝ CỬA HÀNG BÁN ĐIỆN THOẠI";
            lb.Font = new Font("Segoe UI", 22, FontStyle.Bold);
            lb.Dock = DockStyle.Fill;
            lb.TextAlign = ContentAlignment.MiddleCenter;

            Controls.Add(lb);
            Controls.Add(menu);
            MainMenuStrip = menu;
        }
    }
}
