using System;using System.Collections;using System.Data;using System.Data.SqlClient;using QuanLyBanDienThoai.DTO;
namespace QuanLyBanDienThoai.DAO {public class PhieuNhapHangDAO {
 public DataTable GetAll()=>MyConnection.ExecuteQuery("SELECT p.MaPN,p.NgayNhap,p.TongTien,p.MaNCC,n.TenNCC,p.MaNV,v.Ho+N' '+v.Ten AS TenNhanVien FROM PHIEUNHAPHANG p JOIN NHACUNGCAP n ON p.MaNCC=n.MaNCC JOIN NHANVIEN v ON p.MaNV=v.MaNV ORDER BY p.MaPN DESC");
 public DataTable GetChiTiet(int id)=>MyConnection.ExecuteQuery("SELECT c.MaPN,c.MaDT,d.TenDT,c.SoLuong,c.DonGiaNhap,c.ThanhTien FROM CHITIETPHIEUNHAPHANG c JOIN DIENTHOAI d ON c.MaDT=d.MaDT WHERE c.MaPN=@p0",new object[]{id});
 public int LuuPhieuNhap(PhieuNhapHang p,ArrayList ds){using(SqlConnection c=MyConnection.TaoKetNoi()){c.Open();using(SqlTransaction tx=c.BeginTransaction()){try{
 int id;using(var cmd=new SqlCommand("INSERT INTO PHIEUNHAPHANG(NgayNhap,TongTien,MaNCC,MaNV) OUTPUT INSERTED.MaPN VALUES(GETDATE(),@p0,@p1,@p2)",c,tx)){cmd.Parameters.AddWithValue("@p0",p.TongTien);cmd.Parameters.AddWithValue("@p1",p.MaNCC);cmd.Parameters.AddWithValue("@p2",p.MaNV);id=Convert.ToInt32(cmd.ExecuteScalar());}
 foreach(ChiTietPhieuNhapHang d in ds){using(var cmd=new SqlCommand("INSERT INTO CHITIETPHIEUNHAPHANG(MaPN,MaDT,SoLuong,DonGiaNhap,ThanhTien) VALUES(@p0,@p1,@p2,@p3,@p4)",c,tx)){MyConnection.GanThamSo(cmd,cmd.CommandText,new object[]{id,d.MaDT,d.SoLuong,d.DonGiaNhap,d.ThanhTien});cmd.ExecuteNonQuery();}
 using(var cmd=new SqlCommand("UPDATE DIENTHOAI SET SoLuongTon=SoLuongTon+@p0 WHERE MaDT=@p1",c,tx)){MyConnection.GanThamSo(cmd,cmd.CommandText,new object[]{d.SoLuong,d.MaDT});if(cmd.ExecuteNonQuery()!=1)throw new Exception("Điện thoại không tồn tại: "+d.MaDT);}}
 tx.Commit();return id;
 }catch{tx.Rollback();throw;}}}}
} }