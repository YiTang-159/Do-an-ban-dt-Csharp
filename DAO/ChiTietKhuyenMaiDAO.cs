using System;
using System.Collections;
using System.Data;
using QuanLyBanDienThoai.DTO;
namespace QuanLyBanDienThoai.DAO
{
    public class ChiTietKhuyenMaiDAO
    {
        public ArrayList GetAll()
        {
            DataTable t=MyConnection.ExecuteQuery("SELECT c.MaKM,c.MaDT,c.PhanTramGiam,k.TenKM,"+
                BieuThucSql.TenHienThiDT+" AS TenHienThi FROM CHITIETKHUYENMAI c "+
                "JOIN KHUYENMAI k ON c.MaKM=k.MaKM JOIN DIENTHOAI d ON c.MaDT=d.MaDT ORDER BY c.MaKM DESC,c.MaDT");
            ArrayList list=new ArrayList();
            foreach(DataRow r in t.Rows) list.Add(new ChiTietKhuyenMai {
                MaKM=Convert.ToInt32(r["MaKM"]),MaDT=Convert.ToInt32(r["MaDT"]),
                PhanTramGiam=Convert.ToDecimal(r["PhanTramGiam"]),
                TenKM=Convert.ToString(r["TenKM"]),TenHienThi=Convert.ToString(r["TenHienThi"])
            });
            return list;
        }
        public ArrayList GetTheoKhuyenMai(int maKM)
        {
            ArrayList list=new ArrayList();
            foreach(ChiTietKhuyenMai x in GetAll()) if(x.MaKM==maKM) list.Add(x);
            return list;
        }
        public int Insert(ChiTietKhuyenMai x)=>MyConnection.ExecuteNonQuery(
            "INSERT INTO CHITIETKHUYENMAI(MaKM,MaDT,PhanTramGiam) VALUES(@p0,@p1,@p2)",
            new object[]{x.MaKM,x.MaDT,x.PhanTramGiam});
        public int Update(ChiTietKhuyenMai x)=>MyConnection.ExecuteNonQuery(
            "UPDATE CHITIETKHUYENMAI SET PhanTramGiam=@p0 WHERE MaKM=@p1 AND MaDT=@p2",
            new object[]{x.PhanTramGiam,x.MaKM,x.MaDT});
        public int Delete(int maKM,int maDT)=>MyConnection.ExecuteNonQuery(
            "DELETE FROM CHITIETKHUYENMAI WHERE MaKM=@p0 AND MaDT=@p1",new object[]{maKM,maDT});
    }
}
