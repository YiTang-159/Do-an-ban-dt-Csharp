using System;
using System.Collections;
using System.Data;
using QuanLyBanDienThoai.DTO;

namespace QuanLyBanDienThoai.DAO
{
    // ============================================================================
    //  BẢN TỐI THIỂU phục vụ trang khách hàng (xem bảo hành của mình). FILE NÀY THUỘC VỀ TV4.
    //  Khi TV4 gộp BaoHanhDAO thật thì giữ thêm hàm GetTheoKhachHang.
    // ============================================================================
    public class BaoHanhDAO
    {
        // Bảo hành của các hóa đơn thuộc khách hàng này
        public ArrayList GetTheoKhachHang(int maKH)
        {
            DataTable dt = MyConnection.ExecuteQuery(
                "SELECT b.MaBH, b.SoIMEI, b.NgayBatDau, b.NgayKetThuc, b.TrangThai, b.GhiChu, b.MaHD, b.MaDT, " +
                BieuThucSql.TenHienThiDT + " AS TenHienThi " +
                "FROM BAOHANH b JOIN HOADON h ON b.MaHD = h.MaHD JOIN DIENTHOAI d ON b.MaDT = d.MaDT " +
                "WHERE h.MaKH = @p0 ORDER BY b.NgayBatDau DESC, b.MaBH",
                new object[] { maKH });
            ArrayList ds = new ArrayList();
            foreach (DataRow r in dt.Rows)
            {
                BaoHanh x = new BaoHanh();
                x.MaBH = Convert.ToInt32(r["MaBH"]);
                x.SoIMEI = Convert.ToString(r["SoIMEI"]);
                x.NgayBatDau = Convert.ToDateTime(r["NgayBatDau"]);
                x.NgayKetThuc = Convert.ToDateTime(r["NgayKetThuc"]);
                x.TrangThai = Convert.ToString(r["TrangThai"]);
                x.GhiChu = Convert.ToString(r["GhiChu"]);
                x.MaHD = Convert.ToInt32(r["MaHD"]);
                x.MaDT = Convert.ToInt32(r["MaDT"]);
                x.TenHienThi = Convert.ToString(r["TenHienThi"]);
                ds.Add(x);
            }
            return ds;
        }
    }
}
