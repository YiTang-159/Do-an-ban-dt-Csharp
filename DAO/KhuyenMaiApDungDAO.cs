using System;
using System.Collections;
using System.Data;
using QuanLyBanDienThoai.DTO;

namespace QuanLyBanDienThoai.DAO
{
    // Chỉ ĐỌC khuyến mãi đang có hiệu lực để tính giá bán (TV4 vẫn là người quản lý bảng KHUYENMAI, CHITIETKHUYENMAI)
    public class KhuyenMaiApDungDAO
    {
        // Mọi cặp (khuyến mãi, điện thoại) có hiệu lực trong ngày, phần trăm cao xếp trước
        public ArrayList GetApDung(DateTime ngay)
        {
            DataTable dt = MyConnection.ExecuteQuery(
                "SELECT k.MaKM, k.TenKM, c.MaDT, c.PhanTramGiam " +
                "FROM KHUYENMAI k JOIN CHITIETKHUYENMAI c ON k.MaKM = c.MaKM " +
                "WHERE @p0 BETWEEN k.NgayBatDau AND k.NgayKetThuc " +
                "ORDER BY c.MaDT, c.PhanTramGiam DESC, k.MaKM",
                new object[] { ngay.Date });
            ArrayList ds = new ArrayList();
            foreach (DataRow r in dt.Rows)
            {
                KhuyenMaiApDung x = new KhuyenMaiApDung();
                x.MaKM = Convert.ToInt32(r["MaKM"]);
                x.TenKM = Convert.ToString(r["TenKM"]);
                x.MaDT = Convert.ToInt32(r["MaDT"]);
                x.PhanTramGiam = Convert.ToDecimal(r["PhanTramGiam"]);
                ds.Add(x);
            }
            return ds;
        }
    }
}
