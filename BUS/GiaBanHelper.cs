using System;
using System.Collections;
using System.Collections.Generic;
using QuanLyBanDienThoai.DAO;
using QuanLyBanDienThoai.DTO;

namespace QuanLyBanDienThoai.BUS
{
    // Công thức giá dùng chung cho trang khách hàng, giỏ hàng và lập hóa đơn
    // => khách thấy giá nào thì nhân viên lập hóa đơn đúng giá đó.
    public static class GiaBanHelper
    {
        // Khuyến mãi đang có hiệu lực hôm nay
        public static ArrayList LayKhuyenMaiApDung()
        {
            return new KhuyenMaiApDungDAO().GetApDung(DateTime.Today);
        }

        // Mỗi điện thoại giữ khuyến mãi có phần trăm cao nhất
        public static Dictionary<int, KhuyenMaiApDung> TotNhatTheoDT(ArrayList dsKM)
        {
            Dictionary<int, KhuyenMaiApDung> kq = new Dictionary<int, KhuyenMaiApDung>();
            foreach (object o in dsKM)
            {
                KhuyenMaiApDung km = (KhuyenMaiApDung)o;
                KhuyenMaiApDung cu;
                if (!kq.TryGetValue(km.MaDT, out cu) || km.PhanTramGiam > cu.PhanTramGiam) kq[km.MaDT] = km;
            }
            return kq;
        }

        // Giá sau giảm = giá niêm yết x (100 - phần trăm) / 100
        public static decimal GiaSauGiam(decimal giaNiemYet, decimal phanTram)
        {
            return giaNiemYet * (100m - phanTram) / 100m;
        }

        // Thành tiền = làm tròn(giá sau giảm x số lượng)
        public static decimal ThanhTien(decimal giaNiemYet, decimal phanTram, int soLuong)
        {
            return Math.Round(GiaSauGiam(giaNiemYet, phanTram) * soLuong, 0, MidpointRounding.AwayFromZero);
        }
    }
}
