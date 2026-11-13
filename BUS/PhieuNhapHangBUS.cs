using System;
using System.Collections;
using System.Collections.Generic;
using QuanLyBanDienThoai.DAO;
using QuanLyBanDienThoai.DTO;

namespace QuanLyBanDienThoai.BUS
{
    public class PhieuNhapHangBUS
    {
        private readonly PhieuNhapHangDAO dao = new PhieuNhapHangDAO();

        public ArrayList GetAll() { return dao.GetAll(); }
        public ArrayList GetChiTiet(int maPN) { return dao.GetChiTiet(maPN); }

        // Tìm kiếm trên ArrayList (yêu cầu 2)
        public ArrayList TimKiem(string tuKhoa)
        {
            return TimKiemHelper.TimTuKhoa(dao.GetAll(), tuKhoa, "MaPN", "TenNCC", "TenNhanVien", "TongTien");
        }

        // Tìm kiếm nâng cao (yêu cầu 8)
        public ArrayList TimNangCao(List<DieuKien> dieuKien)
        {
            return TimKiemHelper.Loc(dao.GetAll(), dieuKien);
        }

        public decimal TinhTongTien(ArrayList ds)
        {
            decimal tong = 0;
            foreach (object o in ds) tong += ((PhieuNhapHang)o).TongTien;
            return tong;
        }

        public string TaoDong(ArrayList ds, int maDT, int soLuong, decimal gia)
        {
            if (maDT <= 0 || soLuong <= 0 || gia <= 0) return "Chọn điện thoại, số lượng và đơn giá hợp lệ";
            foreach (ChiTietPhieuNhapHang d in ds)
                if (d.MaDT == maDT) return "Điện thoại đã có trong phiếu";
            try
            {
                ds.Add(new ChiTietPhieuNhapHang { MaDT = maDT, SoLuong = soLuong, DonGiaNhap = gia, ThanhTien = checked(soLuong * gia) });
                return "";
            }
            catch (OverflowException) { return "Thành tiền vượt giới hạn"; }
        }

        public decimal TinhTongTienNhap(ArrayList ds)
        {
            decimal tong = 0;
            foreach (ChiTietPhieuNhapHang d in ds) tong = checked(tong + d.ThanhTien);
            return tong;
        }

        // Nhân viên lập phiếu là người đang đăng nhập (không cho form chọn), giống hóa đơn bán hàng.
        public string LuuPhieuNhap(PhieuNhapHang p, ArrayList ds, out int maPN)
        {
            maPN = 0;
            if (!PhienDangNhap.LaNhanSu || PhienDangNhap.MaNV <= 0) return "Chỉ nhân viên hoặc quản trị viên mới được lập phiếu nhập";
            if (p == null || p.MaNCC <= 0) return "Chọn nhà cung cấp";
            if (ds == null || ds.Count == 0) return "Phiếu nhập phải có ít nhất một dòng";
            try
            {
                p.MaNV = PhienDangNhap.MaNV;
                HashSet<int> daCo = new HashSet<int>();
                foreach (ChiTietPhieuNhapHang d in ds)
                {
                    if (d.MaDT <= 0 || d.SoLuong <= 0 || d.DonGiaNhap <= 0 || !daCo.Add(d.MaDT))
                        return "Chi tiết phiếu nhập không hợp lệ hoặc trùng điện thoại";
                    d.ThanhTien = checked(d.SoLuong * d.DonGiaNhap);
                }
                p.TongTien = TinhTongTienNhap(ds);
                maPN = dao.LuuPhieuNhap(p, ds);
                return "";
            }
            catch (Exception e) { return "Không lưu được phiếu nhập: " + e.Message; }
        }
    }
}
