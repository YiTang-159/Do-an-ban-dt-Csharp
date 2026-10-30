using System; using System.Collections; using QuanLyBanDienThoai.DAO; using QuanLyBanDienThoai.DTO;
namespace QuanLyBanDienThoai.BUS { public class ThamSoBUS {
 readonly ThamSoDAO dao=new ThamSoDAO();
 public static decimal LayGiaTri(string ten){decimal macDinh=ten switch {"SoTienMotDiem"=>100000,"TuoiNhanVienToiThieu"=>18,"NamRaMatToiThieu"=>2000,"ThangBaoHanhMacDinh"=>12,_=>0}; return new ThamSoDAO().Lay(ten)??macDinh;}
 public ArrayList GetAll()=>dao.GetAll();
 public ArrayList TimKiem(string s)=>TimKiemHelper.TimTuKhoa(GetAll(),s,"TenThamSo","MoTa");
 public string Them(ThamSo x)=>Ghi(x,false);
 public string Sua(ThamSo x)=>Ghi(x,true);
 string Ghi(ThamSo x,bool sua){if(x==null||string.IsNullOrWhiteSpace(x.TenThamSo))return "Tên tham số không được trống";if(x.GiaTri<=0)return "Giá trị phải lớn hơn 0";try{return (sua?dao.Update(x):dao.Insert(x))>0?"":"Không ghi được dữ liệu";}catch(Exception e){return e.Message;}}
 public string Xoa(string ten){try{return dao.Delete(ten)>0?"":"Không tìm thấy tham số";}catch(Exception e){return e.Message;}}
} }