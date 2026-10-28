using System;
using System.Data;
using System.Data.SqlClient;
using System.Text.RegularExpressions;

namespace QuanLyBanDienThoai.DAO
{
    // Lớp kết nối CSDL dùng chung. Mọi DAO chỉ gọi lớp này, không tự mở SqlConnection.
    // Quy ước tham số: viết @p0, @p1, ... trong câu SQL; mảng args truyền theo đúng thứ tự (args[0] -> @p0).
    public static class MyConnection
    {
        // Mỗi máy một tên SQL Server: tự sửa ở máy mình, KHÔNG commit dòng này (dùng git add -p).
        // SQL Server Express: Data Source=.\SQLEXPRESS;...
        // Tiếng Việt: cột chữ là NVARCHAR nên không cần thêm tham số mã hóa (useUnicode chỉ dành cho MySQL).
        private const string ChuoiKetNoi =
            @"Data Source=.;Initial Catalog=QuanLyBanDienThoai;Integrated Security=True";

        // Trả về SqlConnection MỚI, CHƯA MỞ. Nơi gọi tự Open() và Dispose().
        // Dùng khi cần tự làm transaction (LuuHoaDon, LuuPhieuNhap).
        public static SqlConnection TaoKetNoi()
        {
            return new SqlConnection(ChuoiKetNoi);
        }

        // Gán tham số @p0, @p1, ... theo thứ tự trong mảng args.
        // Chỉ gán những tham số thật sự có trong câu SQL.
        public static void GanThamSo(SqlCommand cmd, string sql, object[] args)
        {
            if (args == null) return;
            for (int i = 0; i < args.Length; i++)
            {
                string ten = "@p" + i;
                if (!Regex.IsMatch(sql, ten + @"\b")) continue;
                cmd.Parameters.AddWithValue(ten, args[i] ?? DBNull.Value);
            }
        }

        // SELECT nhiều dòng
        public static DataTable ExecuteQuery(string sql, object[] args = null)
        {
            DataTable dt = new DataTable();
            using (SqlConnection conn = TaoKetNoi())
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                GanThamSo(cmd, sql, args);
                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    da.Fill(dt);
                }
            }
            return dt;
        }

        // INSERT, UPDATE, DELETE: trả về số dòng bị ảnh hưởng
        public static int ExecuteNonQuery(string sql, object[] args = null)
        {
            using (SqlConnection conn = TaoKetNoi())
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                GanThamSo(cmd, sql, args);
                conn.Open();
                return cmd.ExecuteNonQuery();
            }
        }

        // Lấy một giá trị (giá, tồn kho, COUNT...)
        public static object ExecuteScalar(string sql, object[] args = null)
        {
            using (SqlConnection conn = TaoKetNoi())
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                GanThamSo(cmd, sql, args);
                conn.Open();
                return cmd.ExecuteScalar();
            }
        }
    }
}
