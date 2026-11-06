using System;
using System.Collections;
using System.Data;
using QuanLyBanDienThoai.DTO;
namespace QuanLyBanDienThoai.DAO
{
    public class KhuyenMaiDAO
    {
        public ArrayList GetAll()
        {
            DataTable table = MyConnection.ExecuteQuery("SELECT MaKM,TenKM,NgayBatDau,NgayKetThuc,MoTa FROM KHUYENMAI ORDER BY MaKM DESC");
            ArrayList list = new ArrayList();
            foreach (DataRow r in table.Rows) list.Add(new KhuyenMai {
                MaKM = Convert.ToInt32(r["MaKM"]), TenKM = Convert.ToString(r["TenKM"]),
                NgayBatDau = Convert.ToDateTime(r["NgayBatDau"]), NgayKetThuc = Convert.ToDateTime(r["NgayKetThuc"]),
                MoTa = Convert.ToString(r["MoTa"])
            });
            return list;
        }
        public int Insert(KhuyenMai x) => MyConnection.ExecuteNonQuery(
            "INSERT INTO KHUYENMAI(TenKM,NgayBatDau,NgayKetThuc,MoTa) VALUES(@p0,@p1,@p2,@p3)",
            new object[]{x.TenKM,x.NgayBatDau.Date,x.NgayKetThuc.Date,x.MoTa});
        public int Update(KhuyenMai x) => MyConnection.ExecuteNonQuery(
            "UPDATE KHUYENMAI SET TenKM=@p0,NgayBatDau=@p1,NgayKetThuc=@p2,MoTa=@p3 WHERE MaKM=@p4",
            new object[]{x.TenKM,x.NgayBatDau.Date,x.NgayKetThuc.Date,x.MoTa,x.MaKM});
        public int Delete(int id) => MyConnection.ExecuteNonQuery("DELETE FROM KHUYENMAI WHERE MaKM=@p0",new object[]{id});
    }
}
