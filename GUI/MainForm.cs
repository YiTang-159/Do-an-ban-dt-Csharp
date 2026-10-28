using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace QuanLyBanDienThoai.GUI
{
    // Màn hình chính dạng desktop: menu dọc bên trái, form con nhúng vào vùng bên phải.
    // Bấm tên nhóm để mở/đóng các mục con; bấm mục con để mở form.
    // Ai làm xong form nào thì BỎ DẤU // ở đúng dòng ThemMuc của mình (chỉ sửa đúng dòng của mình).
    public class MainForm : Form
    {
        private FlowLayoutPanel pnlMenu;
        private Panel pnlNoiDung;      // vùng bên phải: tiêu đề + form con
        private Panel pnlForm;
        private Label lblTieuDe;
        private Label lblChao;
        private Form formHienTai;
        private List<FlowLayoutPanel> cacNhom = new List<FlowLayoutPanel>();

        public MainForm()
        {
            Text = "Quản lý cửa hàng bán điện thoại";
            Size = new Size(1200, 720);
            MinimumSize = new Size(1000, 600);
            StartPosition = FormStartPosition.CenterScreen;

            // Vùng nội dung bên phải
            pnlNoiDung = new Panel();
            pnlNoiDung.Dock = DockStyle.Fill;
            pnlNoiDung.BackColor = Color.White;

            pnlForm = new Panel();
            pnlForm.Dock = DockStyle.Fill;

            lblTieuDe = new Label();
            lblTieuDe.Dock = DockStyle.Top;
            lblTieuDe.Height = 42;
            lblTieuDe.Font = new Font("Segoe UI", 13, FontStyle.Bold);
            lblTieuDe.TextAlign = ContentAlignment.MiddleLeft;
            lblTieuDe.Padding = new Padding(12, 0, 0, 0);
            lblTieuDe.BackColor = Color.FromArgb(236, 240, 244);
            lblTieuDe.Text = "Trang chủ";

            lblChao = new Label();
            lblChao.Text = "QUẢN LÝ CỬA HÀNG BÁN ĐIỆN THOẠI";
            lblChao.Font = new Font("Segoe UI", 22, FontStyle.Bold);
            lblChao.Dock = DockStyle.Fill;
            lblChao.TextAlign = ContentAlignment.MiddleCenter;
            pnlForm.Controls.Add(lblChao);

            // Fill thêm vào trước, thanh tiêu đề (Top) thêm sau
            pnlNoiDung.Controls.Add(pnlForm);
            pnlNoiDung.Controls.Add(lblTieuDe);

            // Menu dọc bên trái
            pnlMenu = new FlowLayoutPanel();
            pnlMenu.Dock = DockStyle.Left;
            pnlMenu.Width = 235;
            pnlMenu.FlowDirection = FlowDirection.TopDown;
            pnlMenu.WrapContents = false;
            pnlMenu.AutoScroll = true;
            pnlMenu.BackColor = Color.FromArgb(33, 43, 54);
            pnlMenu.Padding = new Padding(5, 10, 5, 5);

            Controls.Add(pnlNoiDung);
            Controls.Add(pnlMenu);

            TaoMenu();
        }

        private void TaoMenu()
        {
            FlowLayoutPanel nDanhMuc = TaoNhom("Danh mục");
            // TV1
            ThemMuc(nDanhMuc, "Hãng điện thoại", () => new FormHangDT());
            ThemMuc(nDanhMuc, "Loại điện thoại", () => new FormLoaiDienThoai());
            ThemMuc(nDanhMuc, "Nhà cung cấp", () => new FormNhaCungCap());
            ThemMuc(nDanhMuc, "Điện thoại", () => new FormDienThoai());

            FlowLayoutPanel nNhapHang = TaoNhom("Nhập hàng");
            // TV2
            // ThemMuc(nNhapHang, "Lập phiếu nhập hàng", () => new FormNhapHang());
            // ThemMuc(nNhapHang, "Danh sách phiếu nhập", () => new FormPhieuNhap());

            FlowLayoutPanel nNhanSu = TaoNhom("Nhân sự - Khách hàng");
            // TV2
            // ThemMuc(nNhanSu, "Nhân viên", () => new FormNhanVien());
            // ThemMuc(nNhanSu, "Khách hàng", () => new FormKhachHang());

            FlowLayoutPanel nKhuyenMai = TaoNhom("Khuyến mãi - Bảo hành");
            // TV4
            // ThemMuc(nKhuyenMai, "Khuyến mãi", () => new FormKhuyenMai());
            // ThemMuc(nKhuyenMai, "Chi tiết khuyến mãi", () => new FormChiTietKhuyenMai());
            // ThemMuc(nKhuyenMai, "Bảo hành", () => new FormBaoHanh());

            FlowLayoutPanel nBanHang = TaoNhom("Bán hàng");
            // TV3
            // ThemMuc(nBanHang, "Lập hóa đơn bán hàng", () => new FormBanHang());
            // ThemMuc(nBanHang, "Danh sách hóa đơn", () => new FormHoaDon());
            // ThemMuc(nBanHang, "Giỏ hàng", () => new FormGioHang());

            FlowLayoutPanel nThongKe = TaoNhom("Thống kê");
            // TV4
            // ThemMuc(nThongKe, "Thống kê thu chi", () => new FormThongKe());

            FlowLayoutPanel nThamSo = TaoNhom("Tham số");
            // TV2
            // ThemMuc(nThamSo, "Bảng tham số", () => new FormThamSo());

            // Mở sẵn nhóm đầu tiên
            nDanhMuc.Visible = true;
        }

        // Tạo nút tiêu đề nhóm và khung chứa các mục con (ẩn sẵn). Bấm tiêu đề thì mở/đóng nhóm.
        private FlowLayoutPanel TaoNhom(string tieuDe)
        {
            Button btn = new Button();
            btn.Text = tieuDe;
            btn.Width = 215;
            btn.Height = 40;
            btn.Margin = new Padding(0, 3, 0, 0);
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 0;
            btn.BackColor = Color.FromArgb(52, 73, 94);
            btn.ForeColor = Color.White;
            btn.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            btn.TextAlign = ContentAlignment.MiddleLeft;
            btn.Padding = new Padding(8, 0, 0, 0);

            FlowLayoutPanel nhom = new FlowLayoutPanel();
            nhom.FlowDirection = FlowDirection.TopDown;
            nhom.WrapContents = false;
            nhom.AutoSize = true;
            nhom.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            nhom.Margin = new Padding(0);
            nhom.Visible = false;

            btn.Click += (s, e) =>
            {
                bool hien = !nhom.Visible;
                foreach (FlowLayoutPanel n in cacNhom) n.Visible = false;
                nhom.Visible = hien;
            };

            cacNhom.Add(nhom);
            pnlMenu.Controls.Add(btn);
            pnlMenu.Controls.Add(nhom);
            return nhom;
        }

        // Thêm một mục con vào nhóm; bấm vào thì tạo form bằng taoForm rồi nhúng vào vùng bên phải
        private void ThemMuc(FlowLayoutPanel nhom, string ten, Func<Form> taoForm)
        {
            Button btn = new Button();
            btn.Text = ten;
            btn.Width = 215;
            btn.Height = 34;
            btn.Margin = new Padding(0, 1, 0, 0);
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 0;
            btn.BackColor = Color.FromArgb(44, 62, 80);
            btn.ForeColor = Color.White;
            btn.TextAlign = ContentAlignment.MiddleLeft;
            btn.Padding = new Padding(22, 0, 0, 0);
            btn.Click += (s, e) =>
            {
                Form f = taoForm();
                lblTieuDe.Text = ten;
                MoFormCon(f);
            };
            nhom.Controls.Add(btn);
        }

        // Nhúng form con (TopLevel = false) vào vùng bên phải, thay form đang mở
        private void MoFormCon(Form f)
        {
            if (formHienTai != null)
            {
                pnlForm.Controls.Remove(formHienTai);
                formHienTai.Dispose();
                formHienTai = null;
            }
            lblChao.Visible = false;

            f.TopLevel = false;
            f.FormBorderStyle = FormBorderStyle.None;
            f.Dock = DockStyle.Fill;
            pnlForm.Controls.Add(f);
            formHienTai = f;
            f.Show();
            f.BringToFront();
        }
    }
}
