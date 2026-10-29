using System;
using System.Collections;
using System.Data;
using QuanLyBanDienThoai.DTO;

namespace QuanLyBanDienThoai.DAO
{
    // ============================================================================
    //  BẢN TỐI THIỂU (GetAll, GetById) để phục vụ đăng nhập. FILE NÀY THUỘC VỀ TV2.
    //  Khi TV2 gộp NhanVienDAO thật (có Insert, Update, Delete) thì lấy bản của TV2;
    //  chỉ cần đảm bảo còn hàm GetById(int) và cột HoTen.
    // ============================================================================
    public class NhanVienDAO
    {
        private const string SqlChon =
            "SELECT MaNV, Ho, Ten, GioiTinh, NgaySinh, SDT, Email, ChucVu, NgayVaoLam, Ho + N' ' + Ten AS HoTen FROM NHANVIEN ";

        private static NhanVien DocDong(DataRow r)
        {
            NhanVien x = new NhanVien();
            x.MaNV = Convert.ToInt32(r["MaNV"]);
            x.Ho = Convert.ToString(r["Ho"]);
            x.Ten = Convert.ToString(r["Ten"]);
            x.GioiTinh = Convert.ToString(r["GioiTinh"]);
            x.NgaySinh = r["NgaySinh"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(r["NgaySinh"]);
            x.SDT = Convert.ToString(r["SDT"]);
            x.Email = Convert.ToString(r["Email"]);
            x.ChucVu = Convert.ToString(r["ChucVu"]);
            x.NgayVaoLam = r["NgayVaoLam"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(r["NgayVaoLam"]);
            x.HoTen = Convert.ToString(r["HoTen"]);
            return x;
        }

        public ArrayList GetAll()
        {
            DataTable dt = MyConnection.ExecuteQuery(SqlChon + "ORDER BY MaNV");
            ArrayList ds = new ArrayList();
            foreach (DataRow r in dt.Rows) ds.Add(DocDong(r));
            return ds;
        }

        public NhanVien GetById(int maNV)
        {
            DataTable dt = MyConnection.ExecuteQuery(SqlChon + "WHERE MaNV = @p0", new object[] { maNV });
            return dt.Rows.Count == 0 ? null : DocDong(dt.Rows[0]);
        }
    }
}
