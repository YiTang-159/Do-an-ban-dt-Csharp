using System;
using System.Collections;
using System.Data;
using QuanLyBanDienThoai.DTO;

namespace QuanLyBanDienThoai.DAO
{
    public class DienThoaiDAO
    {
        // LEFT JOIN để lấy TenHang, TenLoai; TenHienThi = TenDT + RAM + BoNho + MauSac (TV2, TV3, TV4 dùng cho ComboBox)
        public ArrayList GetAll()
        {
            DataTable dt = MyConnection.ExecuteQuery(
                "SELECT d.MaDT, d.TenDT, d.HeDieuHanh, d.NamRaMat, d.RAM, d.BoNho, d.MauSac, " +
                "d.GiaNiemYet, d.SoLuongTon, d.MoTa, d.MaHang, d.MaLoai, h.TenHang, l.TenLoai, " +
                "d.TenDT + N' ' + CAST(d.RAM AS NVARCHAR(10)) + N'GB/' + CAST(d.BoNho AS NVARCHAR(10)) + N'GB ' " +
                "+ ISNULL(d.MauSac, N'') AS TenHienThi " +
                "FROM DIENTHOAI d " +
                "LEFT JOIN HANGDT h ON d.MaHang = h.MaHang " +
                "LEFT JOIN LOAIDIENTHOAI l ON d.MaLoai = l.MaLoai " +
                "ORDER BY d.MaDT");
            ArrayList ds = new ArrayList();
            foreach (DataRow r in dt.Rows)
            {
                DienThoai x = new DienThoai();
                x.MaDT = Convert.ToInt32(r["MaDT"]);
                x.TenDT = Convert.ToString(r["TenDT"]);
                x.HeDieuHanh = Convert.ToString(r["HeDieuHanh"]);
                x.NamRaMat = r["NamRaMat"] == DBNull.Value ? 0 : Convert.ToInt32(r["NamRaMat"]);
                x.RAM = r["RAM"] == DBNull.Value ? 0 : Convert.ToInt32(r["RAM"]);
                x.BoNho = r["BoNho"] == DBNull.Value ? 0 : Convert.ToInt32(r["BoNho"]);
                x.MauSac = Convert.ToString(r["MauSac"]);
                x.GiaNiemYet = r["GiaNiemYet"] == DBNull.Value ? 0 : Convert.ToDecimal(r["GiaNiemYet"]);
                x.SoLuongTon = r["SoLuongTon"] == DBNull.Value ? 0 : Convert.ToInt32(r["SoLuongTon"]);
                x.MoTa = Convert.ToString(r["MoTa"]);
                x.MaHang = r["MaHang"] == DBNull.Value ? 0 : Convert.ToInt32(r["MaHang"]);
                x.MaLoai = r["MaLoai"] == DBNull.Value ? 0 : Convert.ToInt32(r["MaLoai"]);
                x.TenHang = Convert.ToString(r["TenHang"]);
                x.TenLoai = Convert.ToString(r["TenLoai"]);
                x.TenHienThi = Convert.ToString(r["TenHienThi"]);
                ds.Add(x);
            }
            return ds;
        }

        // SoLuongTon luôn bắt đầu bằng 0; chỉ nhập hàng (cộng) và bán hàng (trừ) mới đổi cột này
        public int Insert(DienThoai x)
        {
            return MyConnection.ExecuteNonQuery(
                "INSERT INTO DIENTHOAI(TenDT, HeDieuHanh, NamRaMat, RAM, BoNho, MauSac, GiaNiemYet, SoLuongTon, MoTa, MaHang, MaLoai) " +
                "VALUES(@p0, @p1, @p2, @p3, @p4, @p5, @p6, 0, @p7, @p8, @p9)",
                new object[] { x.TenDT, x.HeDieuHanh, x.NamRaMat, x.RAM, x.BoNho, x.MauSac,
                               x.GiaNiemYet, x.MoTa, x.MaHang, x.MaLoai });
        }

        // KHÔNG sửa cột SoLuongTon
        public int Update(DienThoai x)
        {
            return MyConnection.ExecuteNonQuery(
                "UPDATE DIENTHOAI SET TenDT = @p0, HeDieuHanh = @p1, NamRaMat = @p2, RAM = @p3, BoNho = @p4, " +
                "MauSac = @p5, GiaNiemYet = @p6, MoTa = @p7, MaHang = @p8, MaLoai = @p9 WHERE MaDT = @p10",
                new object[] { x.TenDT, x.HeDieuHanh, x.NamRaMat, x.RAM, x.BoNho, x.MauSac,
                               x.GiaNiemYet, x.MoTa, x.MaHang, x.MaLoai, x.MaDT });
        }

        // Điện thoại đã được dùng ở hóa đơn, phiếu nhập, khuyến mãi, giỏ hàng hoặc bảo hành thì không xóa được.
        // Trả về số bảng đang tham chiếu tới điện thoại này (0 = chưa dùng).
        public int DemBangDangDung(int maDT)
        {
            object kq = MyConnection.ExecuteScalar(
                "SELECT " +
                "(SELECT CASE WHEN EXISTS(SELECT 1 FROM CHITIETHOADON WHERE MaDT = @p0) THEN 1 ELSE 0 END) + " +
                "(SELECT CASE WHEN EXISTS(SELECT 1 FROM CHITIETPHIEUNHAPHANG WHERE MaDT = @p0) THEN 1 ELSE 0 END) + " +
                "(SELECT CASE WHEN EXISTS(SELECT 1 FROM CHITIETKHUYENMAI WHERE MaDT = @p0) THEN 1 ELSE 0 END) + " +
                "(SELECT CASE WHEN EXISTS(SELECT 1 FROM GIOHANG WHERE MaDT = @p0) THEN 1 ELSE 0 END) + " +
                "(SELECT CASE WHEN EXISTS(SELECT 1 FROM BAOHANH WHERE MaDT = @p0) THEN 1 ELSE 0 END)",
                new object[] { maDT });
            return kq == null || kq == DBNull.Value ? 0 : Convert.ToInt32(kq);
        }

        public int Delete(int maDT)
        {
            return MyConnection.ExecuteNonQuery(
                "DELETE FROM DIENTHOAI WHERE MaDT = @p0",
                new object[] { maDT });
        }
    }
}
