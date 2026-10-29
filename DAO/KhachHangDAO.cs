using System;
using System.Collections;
using System.Data;
using QuanLyBanDienThoai.DTO;

namespace QuanLyBanDienThoai.DAO
{
    // ============================================================================
    //  BẢN TỐI THIỂU (GetAll, GetById, Update) để phục vụ đăng ký / trang khách hàng.
    //  FILE NÀY THUỘC VỀ TV2. Khi TV2 gộp KhachHangDAO thật thì lấy bản của TV2;
    //  chỉ cần đảm bảo còn GetById(int), Update(KhachHang) không sửa DiemTichLuy, và cột HoTen.
    //  Thêm khách hàng mới đi qua TaiKhoanDAO.DangKyKhachHang (một transaction cùng với tài khoản).
    // ============================================================================
    public class KhachHangDAO
    {
        private const string SqlChon =
            "SELECT MaKH, Ho, Ten, SDT, Email, DiaChi, NgaySinh, DiemTichLuy, Ho + N' ' + Ten AS HoTen FROM KHACHHANG ";

        private static KhachHang DocDong(DataRow r)
        {
            KhachHang x = new KhachHang();
            x.MaKH = Convert.ToInt32(r["MaKH"]);
            x.Ho = Convert.ToString(r["Ho"]);
            x.Ten = Convert.ToString(r["Ten"]);
            x.SDT = Convert.ToString(r["SDT"]);
            x.Email = Convert.ToString(r["Email"]);
            x.DiaChi = Convert.ToString(r["DiaChi"]);
            x.NgaySinh = r["NgaySinh"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(r["NgaySinh"]);
            x.DiemTichLuy = Convert.ToInt32(r["DiemTichLuy"]);
            x.HoTen = Convert.ToString(r["HoTen"]);
            return x;
        }

        public ArrayList GetAll()
        {
            DataTable dt = MyConnection.ExecuteQuery(SqlChon + "ORDER BY MaKH");
            ArrayList ds = new ArrayList();
            foreach (DataRow r in dt.Rows) ds.Add(DocDong(r));
            return ds;
        }

        public KhachHang GetById(int maKH)
        {
            DataTable dt = MyConnection.ExecuteQuery(SqlChon + "WHERE MaKH = @p0", new object[] { maKH });
            return dt.Rows.Count == 0 ? null : DocDong(dt.Rows[0]);
        }

        // Không sửa DiemTichLuy: cột này chỉ đổi khi lập hóa đơn (HoaDonDAO.LuuHoaDon)
        public int Update(KhachHang x)
        {
            return MyConnection.ExecuteNonQuery(
                "UPDATE KHACHHANG SET Ho = @p0, Ten = @p1, SDT = @p2, Email = @p3, DiaChi = @p4, NgaySinh = @p5 WHERE MaKH = @p6",
                new object[] { x.Ho, x.Ten, x.SDT, x.Email, x.DiaChi, x.NgaySinh, x.MaKH });
        }
    }
}
