using System;
using System.Collections;
using System.Data;
using System.Data.SqlClient;
using QuanLyBanDienThoai.DTO;

namespace QuanLyBanDienThoai.DAO
{
    // Chỉ nơi này có SQL của bảng TAIKHOAN. Không có hàm tìm kiếm: BUS lọc trên ArrayList.
    public class TaiKhoanDAO
    {
        private const string SqlChon =
            "SELECT t.TenDangNhap, t.MatKhau, t.VaiTro, t.MaNV, t.MaKH, t.TrangThai, " +
            "CASE WHEN t.MaNV IS NOT NULL THEN n.Ho + N' ' + n.Ten ELSE k.Ho + N' ' + k.Ten END AS HoTen " +
            "FROM TAIKHOAN t " +
            "LEFT JOIN NHANVIEN n ON t.MaNV = n.MaNV " +
            "LEFT JOIN KHACHHANG k ON t.MaKH = k.MaKH ";

        private static TaiKhoan DocDong(DataRow r)
        {
            TaiKhoan x = new TaiKhoan();
            x.TenDangNhap = Convert.ToString(r["TenDangNhap"]);
            x.MatKhau = Convert.ToString(r["MatKhau"]);
            x.VaiTro = Convert.ToString(r["VaiTro"]);
            x.MaNV = r["MaNV"] == DBNull.Value ? (int?)null : Convert.ToInt32(r["MaNV"]);
            x.MaKH = r["MaKH"] == DBNull.Value ? (int?)null : Convert.ToInt32(r["MaKH"]);
            x.TrangThai = Convert.ToBoolean(r["TrangThai"]);
            x.HoTen = Convert.ToString(r["HoTen"]);
            return x;
        }

        public ArrayList GetAll()
        {
            DataTable dt = MyConnection.ExecuteQuery(SqlChon + "ORDER BY t.VaiTro, t.TenDangNhap");
            ArrayList ds = new ArrayList();
            foreach (DataRow r in dt.Rows) ds.Add(DocDong(r));
            return ds;
        }

        // null nếu không có tài khoản này
        public TaiKhoan GetByTenDangNhap(string tenDangNhap)
        {
            DataTable dt = MyConnection.ExecuteQuery(SqlChon + "WHERE t.TenDangNhap = @p0", new object[] { tenDangNhap });
            return dt.Rows.Count == 0 ? null : DocDong(dt.Rows[0]);
        }

        public int Insert(TaiKhoan x)
        {
            return MyConnection.ExecuteNonQuery(
                "INSERT INTO TAIKHOAN(TenDangNhap, MatKhau, VaiTro, MaNV, MaKH, TrangThai) VALUES(@p0, @p1, @p2, @p3, @p4, @p5)",
                new object[] { x.TenDangNhap, x.MatKhau, x.VaiTro, x.MaNV, x.MaKH, x.TrangThai });
        }

        public int UpdateMatKhau(string tenDangNhap, string matKhauDaBam)
        {
            return MyConnection.ExecuteNonQuery(
                "UPDATE TAIKHOAN SET MatKhau = @p0 WHERE TenDangNhap = @p1",
                new object[] { matKhauDaBam, tenDangNhap });
        }

        public int UpdateTrangThai(string tenDangNhap, bool hoatDong)
        {
            return MyConnection.ExecuteNonQuery(
                "UPDATE TAIKHOAN SET TrangThai = @p0 WHERE TenDangNhap = @p1",
                new object[] { hoatDong, tenDangNhap });
        }

        public int Delete(string tenDangNhap)
        {
            return MyConnection.ExecuteNonQuery(
                "DELETE FROM TAIKHOAN WHERE TenDangNhap = @p0",
                new object[] { tenDangNhap });
        }

        // Đăng ký khách hàng mới: thêm KHACHHANG rồi thêm TAIKHOAN trong MỘT transaction.
        // Lỗi giữa chừng (trùng SDT, Email, tên đăng nhập...) thì Rollback, không để lại khách hàng "mồ côi".
        // Trả về MaKH vừa tạo.
        public int DangKyKhachHang(KhachHang kh, TaiKhoan tk)
        {
            using (SqlConnection conn = MyConnection.TaoKetNoi())
            {
                conn.Open();
                using (SqlTransaction tran = conn.BeginTransaction())
                {
                    try
                    {
                        string sqlKH =
                            "INSERT INTO KHACHHANG(Ho, Ten, SDT, Email, DiaChi, NgaySinh, DiemTichLuy) " +
                            "VALUES(@p0, @p1, @p2, @p3, @p4, @p5, 0); SELECT CAST(SCOPE_IDENTITY() AS INT)";
                        int maKH;
                        using (SqlCommand cmd = new SqlCommand(sqlKH, conn, tran))
                        {
                            MyConnection.GanThamSo(cmd, sqlKH,
                                new object[] { kh.Ho, kh.Ten, kh.SDT, kh.Email, kh.DiaChi, kh.NgaySinh });
                            maKH = Convert.ToInt32(cmd.ExecuteScalar());
                        }

                        string sqlTK =
                            "INSERT INTO TAIKHOAN(TenDangNhap, MatKhau, VaiTro, MaNV, MaKH, TrangThai) " +
                            "VALUES(@p0, @p1, @p2, NULL, @p3, 1)";
                        using (SqlCommand cmd = new SqlCommand(sqlTK, conn, tran))
                        {
                            MyConnection.GanThamSo(cmd, sqlTK,
                                new object[] { tk.TenDangNhap, tk.MatKhau, VaiTroTK.KhachHang, maKH });
                            cmd.ExecuteNonQuery();
                        }

                        tran.Commit();
                        return maKH;
                    }
                    catch
                    {
                        tran.Rollback();
                        throw;
                    }
                }
            }
        }
    }
}
