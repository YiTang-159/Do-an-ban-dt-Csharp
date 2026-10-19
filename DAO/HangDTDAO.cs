using System;
using System.Data;
using QuanLyBanDienThoai.DTO;

namespace QuanLyBanDienThoai.DAO
{
    public class HangDTDAO
    {
        public DataTable GetAll()
        {
            return DataProvider.ExecuteQuery(
                "SELECT MaHang, TenHang, QuocGia FROM HANGDT ORDER BY MaHang");
        }

        public DataTable Search(string tuKhoa)
        {
            return DataProvider.ExecuteQuery(
                "SELECT MaHang, TenHang, QuocGia FROM HANGDT " +
                "WHERE MaHang LIKE @kw OR TenHang LIKE @kw OR QuocGia LIKE @kw ORDER BY MaHang",
                new object[] { "%" + tuKhoa + "%" });
        }

        public bool TonTai(string maHang)
        {
            object kq = DataProvider.ExecuteScalar(
                "SELECT COUNT(*) FROM HANGDT WHERE MaHang = @p0",
                new object[] { maHang });
            return Convert.ToInt32(kq) > 0;
        }

        public int Insert(HangDT x)
        {
            return DataProvider.ExecuteNonQuery(
                "INSERT INTO HANGDT(MaHang, TenHang, QuocGia) VALUES(@p0, @p1, @p2)",
                new object[] { x.MaHang, x.TenHang, x.QuocGia });
        }

        public int Update(HangDT x)
        {
            return DataProvider.ExecuteNonQuery(
                "UPDATE HANGDT SET TenHang = @p0, QuocGia = @p1 WHERE MaHang = @p2",
                new object[] { x.TenHang, x.QuocGia, x.MaHang });
        }

        public int Delete(string maHang)
        {
            return DataProvider.ExecuteNonQuery(
                "DELETE FROM HANGDT WHERE MaHang = @p0",
                new object[] { maHang });
        }
    }
}
