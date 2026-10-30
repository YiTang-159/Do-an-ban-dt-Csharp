using System; using System.Collections; using System.Data; using QuanLyBanDienThoai.DTO;
namespace QuanLyBanDienThoai.DAO { public class ThamSoDAO {
 public ArrayList GetAll(){var ds=new ArrayList(); foreach(DataRow r in MyConnection.ExecuteQuery("SELECT TenThamSo,GiaTri,MoTa FROM THAMSO ORDER BY TenThamSo").Rows)ds.Add(new ThamSo{TenThamSo=Convert.ToString(r[0]),GiaTri=Convert.ToDecimal(r[1]),MoTa=Convert.ToString(r[2])});return ds;}
 public decimal? Lay(string ten){object v=MyConnection.ExecuteScalar("SELECT GiaTri FROM THAMSO WHERE TenThamSo=@p0",new object[]{ten});return v==null||v==DBNull.Value?(decimal?)null:Convert.ToDecimal(v);}
 public int Insert(ThamSo x)=>MyConnection.ExecuteNonQuery("INSERT INTO THAMSO(TenThamSo,GiaTri,MoTa) VALUES(@p0,@p1,@p2)",new object[]{x.TenThamSo,x.GiaTri,x.MoTa});
 public int Update(ThamSo x)=>MyConnection.ExecuteNonQuery("UPDATE THAMSO SET GiaTri=@p0,MoTa=@p1 WHERE TenThamSo=@p2",new object[]{x.GiaTri,x.MoTa,x.TenThamSo});
 public int Delete(string ten)=>MyConnection.ExecuteNonQuery("DELETE FROM THAMSO WHERE TenThamSo=@p0",new object[]{ten});
} }