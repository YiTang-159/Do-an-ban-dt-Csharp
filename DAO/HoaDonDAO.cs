using System;
using System.Collections;
using System.Data;
using System.Data.SqlClient;
using QuanLyBanDienThoai.DTO;

namespace QuanLyBanDienThoai.DAO
{
    // ============================================================================
    //  BẢN TỐI THIỂU (đủ để nhân viên lập hóa đơn và khách xem đơn mua). FILE NÀY THUỘC VỀ TV3.
    //  Khi TV3 gộp HoaDonDAO thật thì giữ các hàm GetAll, GetTheoKhachHang, GetChiTiet, LuuHoaDon
    //  với đúng chữ ký ở đây (HoaDonBUS và các form của phần đăng nhập đang gọi).
    // ============================================================================
    public class HoaDonDAO
    {
        private const string SqlChon =
            "SELECT h.MaHD, h.NgayLap, h.PhuongThucTT, h.TongTien, h.MaKH, h.MaNV, " +
            "k.Ho + N' ' + k.Ten AS TenKhachHang, n.Ho + N' ' + n.Ten AS TenNhanVien " +
            "FROM HOADON h JOIN KHACHHANG k ON h.MaKH = k.MaKH JOIN NHANVIEN n ON h.MaNV = n.MaNV ";

        private static ArrayList DocDanhSach(DataTable dt)
        {
            ArrayList ds = new ArrayList();
            foreach (DataRow r in dt.Rows)
            {
                HoaDon x = new HoaDon();
                x.MaHD = Convert.ToInt32(r["MaHD"]);
                x.NgayLap = Convert.ToDateTime(r["NgayLap"]);
                x.PhuongThucTT = Convert.ToString(r["PhuongThucTT"]);
                x.TongTien = Convert.ToDecimal(r["TongTien"]);
                x.MaKH = Convert.ToInt32(r["MaKH"]);
                x.MaNV = Convert.ToInt32(r["MaNV"]);
                x.TenKhachHang = Convert.ToString(r["TenKhachHang"]);
                x.TenNhanVien = Convert.ToString(r["TenNhanVien"]);
                ds.Add(x);
            }
            return ds;
        }

        public ArrayList GetAll()
        {
            return DocDanhSach(MyConnection.ExecuteQuery(SqlChon + "ORDER BY h.MaHD DESC"));
        }

        public ArrayList GetTheoKhachHang(int maKH)
        {
            return DocDanhSach(MyConnection.ExecuteQuery(
                SqlChon + "WHERE h.MaKH = @p0 ORDER BY h.MaHD DESC", new object[] { maKH }));
        }

        public ArrayList GetChiTiet(int maHD)
        {
            DataTable dt = MyConnection.ExecuteQuery(
                "SELECT c.MaHD, c.MaDT, c.SoLuong, c.DonGia, c.ThanhTien, c.MaKM, " +
                BieuThucSql.TenHienThiDT + " AS TenHienThi, km.TenKM " +
                "FROM CHITIETHOADON c JOIN DIENTHOAI d ON c.MaDT = d.MaDT " +
                "LEFT JOIN KHUYENMAI km ON c.MaKM = km.MaKM " +
                "WHERE c.MaHD = @p0 ORDER BY c.MaDT",
                new object[] { maHD });
            ArrayList ds = new ArrayList();
            foreach (DataRow r in dt.Rows)
            {
                ChiTietHoaDon x = new ChiTietHoaDon();
                x.MaHD = Convert.ToInt32(r["MaHD"]);
                x.MaDT = Convert.ToInt32(r["MaDT"]);
                x.SoLuong = Convert.ToInt32(r["SoLuong"]);
                x.DonGia = Convert.ToDecimal(r["DonGia"]);
                x.ThanhTien = Convert.ToDecimal(r["ThanhTien"]);
                x.MaKM = r["MaKM"] == DBNull.Value ? (int?)null : Convert.ToInt32(r["MaKM"]);
                x.TenHienThi = Convert.ToString(r["TenHienThi"]);
                x.TenKM = Convert.ToString(r["TenKM"]);
                ds.Add(x);
            }
            return ds;
        }

        // Lập hóa đơn trong MỘT transaction:
        //   1. thêm HOADON
        //   2. mỗi dòng: TRỪ SoLuongTon (UPDATE ... WHERE SoLuongTon >= @sl; không có dòng nào bị ảnh hưởng = hết hàng) rồi thêm CHITIETHOADON
        //   3. cộng DiemTichLuy cho khách
        //   4. nếu hóa đơn lập từ giỏ hàng: xóa các dòng giỏ hàng đã mua
        // Lỗi giữa chừng thì Rollback toàn bộ. Trả về MaHD mới; lỗi thì ném Exception (BUS bắt).
        public int LuuHoaDon(HoaDon hd, ArrayList chiTiet, int diemCong, bool xoaGioHang)
        {
            using (SqlConnection conn = MyConnection.TaoKetNoi())
            {
                conn.Open();
                using (SqlTransaction tran = conn.BeginTransaction())
                {
                    try
                    {
                        string sqlHD =
                            "INSERT INTO HOADON(NgayLap, PhuongThucTT, TongTien, MaKH, MaNV) " +
                            "VALUES(@p0, @p1, @p2, @p3, @p4); SELECT CAST(SCOPE_IDENTITY() AS INT)";
                        int maHD;
                        using (SqlCommand cmd = new SqlCommand(sqlHD, conn, tran))
                        {
                            MyConnection.GanThamSo(cmd, sqlHD,
                                new object[] { hd.NgayLap, hd.PhuongThucTT, hd.TongTien, hd.MaKH, hd.MaNV });
                            maHD = Convert.ToInt32(cmd.ExecuteScalar());
                        }

                        foreach (object o in chiTiet)
                        {
                            ChiTietHoaDon ct = (ChiTietHoaDon)o;

                            string sqlTru =
                                "UPDATE DIENTHOAI SET SoLuongTon = SoLuongTon - @p0 WHERE MaDT = @p1 AND SoLuongTon >= @p0";
                            using (SqlCommand cmd = new SqlCommand(sqlTru, conn, tran))
                            {
                                MyConnection.GanThamSo(cmd, sqlTru, new object[] { ct.SoLuong, ct.MaDT });
                                if (cmd.ExecuteNonQuery() == 0)
                                    throw new Exception("Điện thoại mã " + ct.MaDT + " đã hết hàng hoặc không đủ số lượng");
                            }

                            string sqlCT =
                                "INSERT INTO CHITIETHOADON(MaHD, MaDT, SoLuong, DonGia, ThanhTien, MaKM) " +
                                "VALUES(@p0, @p1, @p2, @p3, @p4, @p5)";
                            using (SqlCommand cmd = new SqlCommand(sqlCT, conn, tran))
                            {
                                MyConnection.GanThamSo(cmd, sqlCT,
                                    new object[] { maHD, ct.MaDT, ct.SoLuong, ct.DonGia, ct.ThanhTien, ct.MaKM });
                                cmd.ExecuteNonQuery();
                            }

                            if (xoaGioHang)
                            {
                                string sqlGio = "DELETE FROM GIOHANG WHERE MaKH = @p0 AND MaDT = @p1";
                                using (SqlCommand cmd = new SqlCommand(sqlGio, conn, tran))
                                {
                                    MyConnection.GanThamSo(cmd, sqlGio, new object[] { hd.MaKH, ct.MaDT });
                                    cmd.ExecuteNonQuery();
                                }
                            }
                        }

                        string sqlDiem = "UPDATE KHACHHANG SET DiemTichLuy = DiemTichLuy + @p0 WHERE MaKH = @p1";
                        using (SqlCommand cmd = new SqlCommand(sqlDiem, conn, tran))
                        {
                            MyConnection.GanThamSo(cmd, sqlDiem, new object[] { diemCong, hd.MaKH });
                            cmd.ExecuteNonQuery();
                        }

                        tran.Commit();
                        return maHD;
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
