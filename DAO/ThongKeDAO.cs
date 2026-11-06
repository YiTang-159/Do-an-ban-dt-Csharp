using System;
using System.Collections;
using System.Data;
using QuanLyBanDienThoai.DTO;
namespace QuanLyBanDienThoai.DAO
{
    public class ThongKeDAO
    {
        // Lấy dữ liệu thô trong khoảng [từ ngày, đến ngày + 1) để không bỏ sót giờ phút cuối ngày.
        // Không tổng hợp bảng chéo trong DAO: BUS chịu trách nhiệm tính các cột Q1..Q4, TC.
        private static ArrayList Doc(DataTable table)
        {
            ArrayList list=new ArrayList();
            foreach(DataRow r in table.Rows) list.Add(new DongThongKe {
                DoiTuong=Convert.ToString(r["DoiTuong"]),Ngay=Convert.ToDateTime(r["Ngay"]),
                SoTien=Convert.ToDecimal(r["SoTien"])
            });
            return list;
        }
        public ArrayList LayThu(DateTime tuNgay,DateTime denNgay)=>Doc(MyConnection.ExecuteQuery(
            "SELECT CAST(h.MaHD AS NVARCHAR(20)) AS DoiTuong,h.NgayLap AS Ngay,h.TongTien AS SoTien "+
            "FROM HOADON h WHERE h.NgayLap>=@p0 AND h.NgayLap<@p1",
            new object[]{tuNgay.Date,denNgay.Date.AddDays(1)}));
        public ArrayList LayChi(DateTime tuNgay,DateTime denNgay)=>Doc(MyConnection.ExecuteQuery(
            "SELECT CAST(p.MaPN AS NVARCHAR(20)) AS DoiTuong,p.NgayNhap AS Ngay,p.TongTien AS SoTien "+
            "FROM PHIEUNHAPHANG p WHERE p.NgayNhap>=@p0 AND p.NgayNhap<@p1",
            new object[]{tuNgay.Date,denNgay.Date.AddDays(1)}));
        public ArrayList TheoNhanVien(DateTime tuNgay,DateTime denNgay)=>Doc(MyConnection.ExecuteQuery(
            "SELECT CAST(n.MaNV AS NVARCHAR(20))+N' - '+n.Ho+N' '+n.Ten AS DoiTuong,"+
            "h.NgayLap AS Ngay,h.TongTien AS SoTien FROM HOADON h JOIN NHANVIEN n ON h.MaNV=n.MaNV " +
            "WHERE h.NgayLap>=@p0 AND h.NgayLap<@p1",
            new object[]{tuNgay.Date,denNgay.Date.AddDays(1)}));
        public ArrayList TheoKhachHang(DateTime tuNgay,DateTime denNgay)=>Doc(MyConnection.ExecuteQuery(
            "SELECT CAST(k.MaKH AS NVARCHAR(20))+N' - '+k.Ho+N' '+k.Ten AS DoiTuong,"+
            "h.NgayLap AS Ngay,h.TongTien AS SoTien FROM HOADON h JOIN KHACHHANG k ON h.MaKH=k.MaKH " +
            "WHERE h.NgayLap>=@p0 AND h.NgayLap<@p1",
            new object[]{tuNgay.Date,denNgay.Date.AddDays(1)}));
        public ArrayList TheoSanPham(DateTime tuNgay,DateTime denNgay)=>Doc(MyConnection.ExecuteQuery(
            "SELECT CAST(d.MaDT AS NVARCHAR(20))+N' - '+" + BieuThucSql.TenHienThiDT +
            " AS DoiTuong,h.NgayLap AS Ngay,c.ThanhTien AS SoTien " +
            "FROM CHITIETHOADON c JOIN HOADON h ON c.MaHD=h.MaHD JOIN DIENTHOAI d ON c.MaDT=d.MaDT "+
            "WHERE h.NgayLap>=@p0 AND h.NgayLap<@p1",
            new object[]{tuNgay.Date,denNgay.Date.AddDays(1)}));
    }
}
