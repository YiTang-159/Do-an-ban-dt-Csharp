using System;
using System.Collections;
using System.Data;
using System.Data.SqlClient;
using QuanLyBanDienThoai.DTO;

namespace QuanLyBanDienThoai.DAO
{
    public class PhieuNhapHangDAO
    {
        // Trả về ArrayList DTO (giống các DAO khác) để BUS tìm kiếm trên ArrayList.
        public ArrayList GetAll()
        {
            DataTable dt = MyConnection.ExecuteQuery(
                "SELECT p.MaPN, p.NgayNhap, p.MaNCC, n.TenNCC, p.MaNV, v.Ho + N' ' + v.Ten AS TenNhanVien " +
                "FROM PHIEUNHAPHANG p JOIN NHACUNGCAP n ON p.MaNCC = n.MaNCC JOIN NHANVIEN v ON p.MaNV = v.MaNV " +
                "ORDER BY p.MaPN DESC");
            ArrayList ds = new ArrayList();
            foreach (DataRow r in dt.Rows)
            {
                PhieuNhapHang x = new PhieuNhapHang();
                x.MaPN = Convert.ToInt32(r["MaPN"]);
                x.NgayNhap = Convert.ToDateTime(r["NgayNhap"]);
                x.TongTien = Convert.ToDecimal(r["TongTien"]);
                x.MaNCC = Convert.ToInt32(r["MaNCC"]);
                x.TenNCC = Convert.ToString(r["TenNCC"]);
                x.MaNV = Convert.ToInt32(r["MaNV"]);
                x.TenNhanVien = Convert.ToString(r["TenNhanVien"]);
                ds.Add(x);
            }
            return ds;
        }

        public ArrayList GetChiTiet(int maPN)
        {
            DataTable dt = MyConnection.ExecuteQuery(
                "SELECT c.MaPN, c.MaDT, d.TenDT, c.SoLuong, c.DonGiaNhap, c.ThanhTien " +
                "FROM CHITIETPHIEUNHAPHANG c JOIN DIENTHOAI d ON c.MaDT = d.MaDT WHERE c.MaPN = @p0 ORDER BY c.MaDT",
                new object[] { maPN });
            ArrayList ds = new ArrayList();
            foreach (DataRow r in dt.Rows)
            {
                ChiTietPhieuNhapHang x = new ChiTietPhieuNhapHang();
                x.MaPN = Convert.ToInt32(r["MaPN"]);
                x.MaDT = Convert.ToInt32(r["MaDT"]);
                x.TenDT = Convert.ToString(r["TenDT"]);
                x.SoLuong = Convert.ToInt32(r["SoLuong"]);
                x.DonGiaNhap = Convert.ToDecimal(r["DonGiaNhap"]);
                x.ThanhTien = Convert.ToDecimal(r["ThanhTien"]);
                ds.Add(x);
            }
            return ds;
        }

        // Lưu phiếu nhập và cộng SoLuongTon trong MỘT transaction; lỗi thì Rollback.
        public int LuuPhieuNhap(PhieuNhapHang p, ArrayList ds)
        {
            using (SqlConnection c = MyConnection.TaoKetNoi())
            {
                c.Open();
                using (SqlTransaction tx = c.BeginTransaction())
                {
                    try
                    {
                        string sqlPN = "INSERT INTO PHIEUNHAPHANG(NgayNhap, TongTien, MaNCC, MaNV) " +
                                       "VALUES(GETDATE(), 0, @p0, @p1); SELECT CAST(SCOPE_IDENTITY() AS INT)";
                        int id;
                        using (SqlCommand cmd = new SqlCommand(sqlPN, c, tx))
                        {
                            MyConnection.GanThamSo(cmd, sqlPN, new object[] { p.MaNCC, p.MaNV });
                            id = Convert.ToInt32(cmd.ExecuteScalar());
                        }
                        foreach (object o in ds)
                        {
                            ChiTietPhieuNhapHang d = (ChiTietPhieuNhapHang)o;
                            string sqlCT = "INSERT INTO CHITIETPHIEUNHAPHANG(MaPN, MaDT, SoLuong, DonGiaNhap, ThanhTien) " +
                                           "VALUES(@p0, @p1, @p2, @p3, @p4)";
                            using (SqlCommand cmd = new SqlCommand(sqlCT, c, tx))
                            {
                                MyConnection.GanThamSo(cmd, sqlCT, new object[] { id, d.MaDT, d.SoLuong, d.DonGiaNhap, d.ThanhTien });
                                cmd.ExecuteNonQuery();
                            }
                            string sqlTon = "UPDATE DIENTHOAI SET SoLuongTon = SoLuongTon + @p0 WHERE MaDT = @p1";
                            using (SqlCommand cmd = new SqlCommand(sqlTon, c, tx))
                            {
                                MyConnection.GanThamSo(cmd, sqlTon, new object[] { d.SoLuong, d.MaDT });
                                if (cmd.ExecuteNonQuery() != 1) throw new Exception("Điện thoại không tồn tại: " + d.MaDT);
                            }
                        }
                        tx.Commit();
                        return id;
                    }
                    catch
                    {
                        if (tx.Connection != null)
                            tx.Rollback();
                        throw;
                    }
                }
            }
        }
    }
}
