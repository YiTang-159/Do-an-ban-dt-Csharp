using System;
using System.Collections;
using System.Data;
using QuanLyBanDienThoai.DTO;
namespace QuanLyBanDienThoai.DAO
{
    public class BaoHanhDAO
    {
        private static ArrayList ConvertRows(DataTable table)
        {
            ArrayList list=new ArrayList();
            foreach(DataRow r in table.Rows) list.Add(new BaoHanh {
                MaBH=Convert.ToInt32(r["MaBH"]),SoIMEI=Convert.ToString(r["SoIMEI"]),
                NgayBatDau=Convert.ToDateTime(r["NgayBatDau"]),NgayKetThuc=Convert.ToDateTime(r["NgayKetThuc"]),
                TrangThai=Convert.ToString(r["TrangThai"]),GhiChu=Convert.ToString(r["GhiChu"]),
                MaHD=Convert.ToInt32(r["MaHD"]),MaDT=Convert.ToInt32(r["MaDT"]),
                TenHienThi=Convert.ToString(r["TenHienThi"])
            });
            return list;
        }
        private const string SELECT_SQL = "SELECT b.MaBH,b.SoIMEI,b.NgayBatDau,b.NgayKetThuc,b.TrangThai,b.GhiChu,b.MaHD,b.MaDT," +
            BieuThucSql.TenHienThiDT + " AS TenHienThi FROM BAOHANH b " +
            "JOIN HOADON h ON b.MaHD=h.MaHD JOIN DIENTHOAI d ON b.MaDT=d.MaDT ";
        public ArrayList GetAll() => ConvertRows(MyConnection.ExecuteQuery(SELECT_SQL+"ORDER BY b.MaBH DESC"));
        // Giữ giao diện khách hàng hiện hành, không làm mất chức năng GetCuaToi().
        public ArrayList GetTheoKhachHang(int maKH) => ConvertRows(MyConnection.ExecuteQuery(
            SELECT_SQL+"WHERE h.MaKH=@p0 ORDER BY b.NgayBatDau DESC,b.MaBH",new object[]{maKH}));
        public bool CoTrongHoaDon(int maHD,int maDT) => Convert.ToInt32(MyConnection.ExecuteScalar(
            "SELECT COUNT(*) FROM CHITIETHOADON WHERE MaHD=@p0 AND MaDT=@p1",new object[]{maHD,maDT}))>0;
        public int Insert(BaoHanh x)=>MyConnection.ExecuteNonQuery(
            "INSERT INTO BAOHANH(SoIMEI,NgayBatDau,NgayKetThuc,TrangThai,GhiChu,MaHD,MaDT) VALUES(@p0,@p1,@p2,@p3,@p4,@p5,@p6)",
            new object[]{x.SoIMEI,x.NgayBatDau.Date,x.NgayKetThuc.Date,x.TrangThai,x.GhiChu,x.MaHD,x.MaDT});
        public int Update(BaoHanh x)=>MyConnection.ExecuteNonQuery(
            "UPDATE BAOHANH SET SoIMEI=@p0,NgayBatDau=@p1,NgayKetThuc=@p2,TrangThai=@p3,GhiChu=@p4,MaHD=@p5,MaDT=@p6 WHERE MaBH=@p7",
            new object[]{x.SoIMEI,x.NgayBatDau.Date,x.NgayKetThuc.Date,x.TrangThai,x.GhiChu,x.MaHD,x.MaDT,x.MaBH});
        public int Delete(int id)=>MyConnection.ExecuteNonQuery("DELETE FROM BAOHANH WHERE MaBH=@p0",new object[]{id});
    }
}
