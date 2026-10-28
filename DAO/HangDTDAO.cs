using System;
using System.Collections;
using System.Data;
using QuanLyBanDienThoai.DTO;

namespace QuanLyBanDienThoai.DAO
{
    // Chỉ nơi này có SQL. Không có hàm tìm kiếm: BUS lọc trên ArrayList.
    public class HangDTDAO
    {
        public ArrayList GetAll()
        {
            DataTable dt = MyConnection.ExecuteQuery(
                "SELECT MaHang, TenHang, QuocGia FROM HANGDT ORDER BY MaHang");
            ArrayList ds = new ArrayList();
            foreach (DataRow r in dt.Rows)
            {
                HangDT x = new HangDT();
                x.MaHang = Convert.ToInt32(r["MaHang"]);
                x.TenHang = Convert.ToString(r["TenHang"]);
                x.QuocGia = Convert.ToString(r["QuocGia"]);
                ds.Add(x);
            }
            return ds;
        }

        // MaHang là IDENTITY nên không có trong câu INSERT
        public int Insert(HangDT x)
        {
            return MyConnection.ExecuteNonQuery(
                "INSERT INTO HANGDT(TenHang, QuocGia) VALUES(@p0, @p1)",
                new object[] { x.TenHang, x.QuocGia });
        }

        public int Update(HangDT x)
        {
            return MyConnection.ExecuteNonQuery(
                "UPDATE HANGDT SET TenHang = @p0, QuocGia = @p1 WHERE MaHang = @p2",
                new object[] { x.TenHang, x.QuocGia, x.MaHang });
        }

        public int Delete(int maHang)
        {
            return MyConnection.ExecuteNonQuery(
                "DELETE FROM HANGDT WHERE MaHang = @p0",
                new object[] { maHang });
        }
    }
}
