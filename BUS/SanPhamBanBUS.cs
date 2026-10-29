using System;
using System.Collections;
using System.Collections.Generic;
using QuanLyBanDienThoai.DAO;
using QuanLyBanDienThoai.DTO;

namespace QuanLyBanDienThoai.BUS
{
    // Danh sách điện thoại kèm giá sau khuyến mãi (trang khách hàng, form bán hàng).
    // Lấy từ DienThoaiDAO (cùng bảng DIENTHOAI mà FormDienThoai quản lý) nên giá và tồn kho luôn thống nhất.
    public class SanPhamBanBUS
    {
        private DienThoaiDAO dao = new DienThoaiDAO();

        public ArrayList GetAll()
        {
            Dictionary<int, KhuyenMaiApDung> tot = GiaBanHelper.TotNhatTheoDT(GiaBanHelper.LayKhuyenMaiApDung());
            ArrayList kq = new ArrayList();
            foreach (object o in dao.GetAll())
            {
                DienThoai d = (DienThoai)o;
                SanPhamBan s = new SanPhamBan();
                s.MaDT = d.MaDT;
                s.TenHienThi = d.TenHienThi;
                s.TenHang = d.TenHang;
                s.GiaNiemYet = d.GiaNiemYet;
                s.SoLuongTon = d.SoLuongTon;
                s.PhanTramGiam = 0;
                KhuyenMaiApDung km;
                if (tot.TryGetValue(d.MaDT, out km))
                {
                    s.PhanTramGiam = km.PhanTramGiam;
                    s.MaKM = km.MaKM;
                    s.TenKM = km.TenKM;
                }
                s.GiaSauGiam = GiaBanHelper.GiaSauGiam(d.GiaNiemYet, s.PhanTramGiam);
                kq.Add(s);
            }
            return kq;
        }

        public ArrayList TimKiem(string tuKhoa)
        {
            return TimKiemHelper.TimTuKhoa(GetAll(), tuKhoa, "MaDT", "TenHienThi", "TenHang", "GiaNiemYet", "TenKM");
        }
    }
}
