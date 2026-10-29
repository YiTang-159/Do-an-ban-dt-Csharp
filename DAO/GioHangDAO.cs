using System;
using System.Collections;
using System.Data;
using QuanLyBanDienThoai.DTO;

namespace QuanLyBanDienThoai.DAO
{
    // ============================================================================
    //  BẢN TỐI THIỂU phục vụ trang khách hàng. FILE NÀY THUỘC VỀ TV3 (bảng GIOHANG).
    //  Khi TV3 gộp GioHangDAO thật thì giữ các hàm GetTheoKhachHang, ThemHoacCong, DatSoLuong, Delete.
    // ============================================================================
    public class GioHangDAO
    {
        public ArrayList GetTheoKhachHang(int maKH)
        {
            DataTable dt = MyConnection.ExecuteQuery(
                "SELECT g.MaKH, g.MaDT, g.SoLuong, g.NgayThem, d.GiaNiemYet, d.SoLuongTon, " +
                BieuThucSql.TenHienThiDT + " AS TenHienThi " +
                "FROM GIOHANG g JOIN DIENTHOAI d ON g.MaDT = d.MaDT " +
                "WHERE g.MaKH = @p0 ORDER BY g.NgayThem, g.MaDT",
                new object[] { maKH });
            ArrayList ds = new ArrayList();
            foreach (DataRow r in dt.Rows)
            {
                GioHang x = new GioHang();
                x.MaKH = Convert.ToInt32(r["MaKH"]);
                x.MaDT = Convert.ToInt32(r["MaDT"]);
                x.SoLuong = Convert.ToInt32(r["SoLuong"]);
                x.NgayThem = Convert.ToDateTime(r["NgayThem"]);
                x.GiaNiemYet = Convert.ToDecimal(r["GiaNiemYet"]);
                x.SoLuongTon = Convert.ToInt32(r["SoLuongTon"]);
                x.TenHienThi = Convert.ToString(r["TenHienThi"]);
                ds.Add(x);
            }
            return ds;
        }

        // Đã có trong giỏ thì cộng thêm số lượng, chưa có thì thêm dòng mới (NgayThem lấy mặc định của bảng)
        public int ThemHoacCong(int maKH, int maDT, int soLuong)
        {
            return MyConnection.ExecuteNonQuery(
                "IF EXISTS (SELECT 1 FROM GIOHANG WHERE MaKH = @p0 AND MaDT = @p1) " +
                "UPDATE GIOHANG SET SoLuong = SoLuong + @p2 WHERE MaKH = @p0 AND MaDT = @p1 " +
                "ELSE INSERT INTO GIOHANG(MaKH, MaDT, SoLuong) VALUES(@p0, @p1, @p2)",
                new object[] { maKH, maDT, soLuong });
        }

        public int DatSoLuong(int maKH, int maDT, int soLuong)
        {
            return MyConnection.ExecuteNonQuery(
                "UPDATE GIOHANG SET SoLuong = @p0 WHERE MaKH = @p1 AND MaDT = @p2",
                new object[] { soLuong, maKH, maDT });
        }

        public int Delete(int maKH, int maDT)
        {
            return MyConnection.ExecuteNonQuery(
                "DELETE FROM GIOHANG WHERE MaKH = @p0 AND MaDT = @p1",
                new object[] { maKH, maDT });
        }
    }
}
