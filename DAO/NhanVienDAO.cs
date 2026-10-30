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
        public int Insert(NhanVien x) => MyConnection.ExecuteNonQuery("INSERT INTO NHANVIEN(Ho,Ten,GioiTinh,NgaySinh,SDT,Email,ChucVu,NgayVaoLam) VALUES(@p0,@p1,@p2,@p3,@p4,@p5,@p6,@p7)", new object[]{x.Ho,x.Ten,x.GioiTinh,x.NgaySinh,x.SDT,x.Email,x.ChucVu,x.NgayVaoLam});
        public int Update(NhanVien x) => MyConnection.ExecuteNonQuery("UPDATE NHANVIEN SET Ho=@p0,Ten=@p1,GioiTinh=@p2,NgaySinh=@p3,SDT=@p4,Email=@p5,ChucVu=@p6,NgayVaoLam=@p7 WHERE MaNV=@p8", new object[]{x.Ho,x.Ten,x.GioiTinh,x.NgaySinh,x.SDT,x.Email,x.ChucVu,x.NgayVaoLam,x.MaNV});
        public int Delete(int id) => MyConnection.ExecuteNonQuery("DELETE FROM NHANVIEN WHERE MaNV=@p0",new object[]{id});
    }
}
