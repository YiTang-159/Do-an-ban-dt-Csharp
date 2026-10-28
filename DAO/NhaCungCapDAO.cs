using System;
using System.Collections;
using System.Data;
using QuanLyBanDienThoai.DTO;

namespace QuanLyBanDienThoai.DAO
{
    public class NhaCungCapDAO
    {
        public ArrayList GetAll()
        {
            DataTable dt = MyConnection.ExecuteQuery(
                "SELECT MaNCC, TenNCC, DiaChi, SDT, Email FROM NHACUNGCAP ORDER BY MaNCC");
            ArrayList ds = new ArrayList();
            foreach (DataRow r in dt.Rows)
            {
                NhaCungCap x = new NhaCungCap();
                x.MaNCC = Convert.ToInt32(r["MaNCC"]);
                x.TenNCC = Convert.ToString(r["TenNCC"]);
                x.DiaChi = Convert.ToString(r["DiaChi"]);
                x.SDT = Convert.ToString(r["SDT"]);
                x.Email = Convert.ToString(r["Email"]);
                ds.Add(x);
            }
            return ds;
        }

        public int Insert(NhaCungCap x)
        {
            return MyConnection.ExecuteNonQuery(
                "INSERT INTO NHACUNGCAP(TenNCC, DiaChi, SDT, Email) VALUES(@p0, @p1, @p2, @p3)",
                new object[] { x.TenNCC, x.DiaChi, x.SDT, x.Email });
        }

        public int Update(NhaCungCap x)
        {
            return MyConnection.ExecuteNonQuery(
                "UPDATE NHACUNGCAP SET TenNCC = @p0, DiaChi = @p1, SDT = @p2, Email = @p3 WHERE MaNCC = @p4",
                new object[] { x.TenNCC, x.DiaChi, x.SDT, x.Email, x.MaNCC });
        }

        // Số phiếu nhập hàng của nhà cung cấp này (0 = chưa có phiếu nào)
        public int DemPhieuNhap(int maNCC)
        {
            object kq = MyConnection.ExecuteScalar(
                "SELECT COUNT(*) FROM PHIEUNHAPHANG WHERE MaNCC = @p0",
                new object[] { maNCC });
            return kq == null || kq == DBNull.Value ? 0 : Convert.ToInt32(kq);
        }

        public int Delete(int maNCC)
        {
            return MyConnection.ExecuteNonQuery(
                "DELETE FROM NHACUNGCAP WHERE MaNCC = @p0",
                new object[] { maNCC });
        }
    }
}
