using System;
using System.Collections;
using System.Collections.Generic;
using QuanLyBanDienThoai.DAO;
using QuanLyBanDienThoai.DTO;

namespace QuanLyBanDienThoai.BUS
{
    // ============================================================================
    //  BẢN TỐI THIỂU phục vụ trang khách hàng và nút "Lấy từ giỏ hàng". FILE NÀY THUỘC VỀ TV3.
    //  Khi TV3 gộp GioHangBUS thật thì giữ GetTheoKhachHang(int) (có giá sau giảm, ThanhTien),
    //  GetCuaToi, ThemVaoGio, DatSoLuong, Xoa mà trang khách hàng đang gọi.
    // ============================================================================
    public class GioHangBUS
    {
        private GioHangDAO dao = new GioHangDAO();

        // Các dòng giỏ của một khách, đã tính giá sau khuyến mãi hôm nay và thành tiền
        public ArrayList GetTheoKhachHang(int maKH)
        {
            Dictionary<int, KhuyenMaiApDung> tot = GiaBanHelper.TotNhatTheoDT(GiaBanHelper.LayKhuyenMaiApDung());
            ArrayList ds = dao.GetTheoKhachHang(maKH);
            foreach (object o in ds)
            {
                GioHang g = (GioHang)o;
                KhuyenMaiApDung km;
                if (tot.TryGetValue(g.MaDT, out km))
                {
                    g.PhanTramGiam = km.PhanTramGiam;
                    g.MaKM = km.MaKM;
                }
                g.GiaSauGiam = GiaBanHelper.GiaSauGiam(g.GiaNiemYet, g.PhanTramGiam);
                g.ThanhTien = GiaBanHelper.ThanhTien(g.GiaNiemYet, g.PhanTramGiam, g.SoLuong);
            }
            return ds;
        }

        // Giỏ hàng của khách đang đăng nhập (mã khách lấy từ phiên đăng nhập, không do form truyền vào)
        public ArrayList GetCuaToi()
        {
            if (!PhienDangNhap.LaKhachHang) return new ArrayList();
            return GetTheoKhachHang(PhienDangNhap.MaKH);
        }

        public decimal TinhTong(ArrayList ds)
        {
            decimal tong = 0;
            foreach (object o in ds) tong += ((GioHang)o).ThanhTien;
            return tong;
        }

        private static string KiemTraKhach()
        {
            return PhienDangNhap.LaKhachHang ? "" : "Chỉ khách hàng mới có giỏ hàng";
        }

        private int SoLuongDangCo(int maDT)
        {
            foreach (object o in dao.GetTheoKhachHang(PhienDangNhap.MaKH))
            {
                GioHang g = (GioHang)o;
                if (g.MaDT == maDT) return g.SoLuong;
            }
            return 0;
        }

        private static int TonKho(int maDT)
        {
            foreach (object o in new DienThoaiDAO().GetAll())
            {
                DienThoai d = (DienThoai)o;
                if (d.MaDT == maDT) return d.SoLuongTon;
            }
            return -1;
        }

        public string ThemVaoGio(int maDT, int soLuong)
        {
            string loi = KiemTraKhach();
            if (loi != "") return loi;
            if (soLuong <= 0) return "Số lượng phải lớn hơn 0";
            try
            {
                int ton = TonKho(maDT);
                if (ton < 0) return "Không tìm thấy điện thoại";
                int daCo = SoLuongDangCo(maDT);
                if (daCo + soLuong > ton)
                    return "Chỉ còn " + ton + " máy trong kho (trong giỏ của bạn đã có " + daCo + ")";
                dao.ThemHoacCong(PhienDangNhap.MaKH, maDT, soLuong);
                return "";
            }
            catch (Exception ex)
            {
                return "Không thể thêm vào giỏ: " + ex.Message;
            }
        }

        public string DatSoLuong(int maDT, int soLuong)
        {
            string loi = KiemTraKhach();
            if (loi != "") return loi;
            if (soLuong <= 0) return "Số lượng phải lớn hơn 0";
            try
            {
                int ton = TonKho(maDT);
                if (ton < 0) return "Không tìm thấy điện thoại";
                if (soLuong > ton) return "Chỉ còn " + ton + " máy trong kho";
                if (dao.DatSoLuong(PhienDangNhap.MaKH, maDT, soLuong) == 0) return "Điện thoại không có trong giỏ";
                return "";
            }
            catch (Exception ex)
            {
                return "Không thể cập nhật: " + ex.Message;
            }
        }

        public string Xoa(int maDT)
        {
            string loi = KiemTraKhach();
            if (loi != "") return loi;
            try
            {
                if (dao.Delete(PhienDangNhap.MaKH, maDT) == 0) return "Điện thoại không có trong giỏ";
                return "";
            }
            catch (Exception ex)
            {
                return "Không thể xóa: " + ex.Message;
            }
        }
    }
}
