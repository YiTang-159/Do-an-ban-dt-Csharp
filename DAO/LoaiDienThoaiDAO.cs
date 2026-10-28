using System;
using System.Collections;
using System.Data;
using QuanLyBanDienThoai.DTO;

namespace QuanLyBanDienThoai.DAO
{
    public class LoaiDienThoaiDAO
    {
        public ArrayList GetAll()
        {
            DataTable dt = MyConnection.ExecuteQuery(
                "SELECT MaLoai, TenLoai, MoTa FROM LOAIDIENTHOAI ORDER BY MaLoai");
            ArrayList ds = new ArrayList();
            foreach (DataRow r in dt.Rows)
            {
                LoaiDienThoai x = new LoaiDienThoai();
                x.MaLoai = Convert.ToInt32(r["MaLoai"]);
                x.TenLoai = Convert.ToString(r["TenLoai"]);
                x.MoTa = Convert.ToString(r["MoTa"]);
                ds.Add(x);
            }
            return ds;
        }

        public int Insert(LoaiDienThoai x)
        {
            return MyConnection.ExecuteNonQuery(
                "INSERT INTO LOAIDIENTHOAI(TenLoai, MoTa) VALUES(@p0, @p1)",
                new object[] { x.TenLoai, x.MoTa });
        }

        public int Update(LoaiDienThoai x)
        {
            return MyConnection.ExecuteNonQuery(
                "UPDATE LOAIDIENTHOAI SET TenLoai = @p0, MoTa = @p1 WHERE MaLoai = @p2",
                new object[] { x.TenLoai, x.MoTa, x.MaLoai });
        }

        public int Delete(int maLoai)
        {
            return MyConnection.ExecuteNonQuery(
                "DELETE FROM LOAIDIENTHOAI WHERE MaLoai = @p0",
                new object[] { maLoai });
        }
    }
}
