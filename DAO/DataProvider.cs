using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text.RegularExpressions;

namespace QuanLyBanDienThoai.DAO
{
    // Lớp truy cập CSDL dùng chung cho tầng DAO.
    // Quy ước: mọi câu truy vấn SQL chỉ được viết trong tầng DAO.
    public static class DataProvider
    {
        // Nếu dùng SQL Server Express hãy đổi thành:  Data Source=.\SQLEXPRESS;...
        private const string ChuoiKetNoi =
            @"Data Source=.;Initial Catalog=QuanLyBanDienThoai;Integrated Security=True";

        public static SqlConnection TaoKetNoi()
        {
            return new SqlConnection(ChuoiKetNoi);
        }

        // Gán tham số theo thứ tự xuất hiện của @tên trong câu SQL (mỗi tên chỉ tính một lần)
        public static void GanThamSo(SqlCommand cmd, string sql, object[] args)
        {
            if (args == null) return;
            List<string> ten = new List<string>();
            foreach (Match m in Regex.Matches(sql, @"@\w+"))
            {
                if (!ten.Contains(m.Value)) ten.Add(m.Value);
            }
            for (int i = 0; i < ten.Count && i < args.Length; i++)
            {
                cmd.Parameters.AddWithValue(ten[i], args[i] ?? DBNull.Value);
            }
        }

        public static DataTable ExecuteQuery(string sql, object[] args = null)
        {
            DataTable dt = new DataTable();
            using (SqlConnection conn = TaoKetNoi())
            {
                SqlCommand cmd = new SqlCommand(sql, conn);
                GanThamSo(cmd, sql, args);
                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    da.Fill(dt);
                }
            }
            return dt;
        }

        public static int ExecuteNonQuery(string sql, object[] args = null)
        {
            using (SqlConnection conn = TaoKetNoi())
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                GanThamSo(cmd, sql, args);
                return cmd.ExecuteNonQuery();
            }
        }

        public static object ExecuteScalar(string sql, object[] args = null)
        {
            using (SqlConnection conn = TaoKetNoi())
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                GanThamSo(cmd, sql, args);
                return cmd.ExecuteScalar();
            }
        }
    }
}
