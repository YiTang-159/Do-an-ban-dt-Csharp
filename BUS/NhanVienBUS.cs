using System;
using System.Collections;
using QuanLyBanDienThoai.DAO;
using QuanLyBanDienThoai.DTO;

namespace QuanLyBanDienThoai.BUS
{
    // ============================================================================
    //  BẢN TỐI THIỂU (GetAll, GetById, TimKiem). FILE NÀY THUỘC VỀ TV2.
    //  Khi TV2 gộp NhanVienBUS thật (Them, Sua, Xoa, TimNangCao, kiểm tra tuổi...) thì lấy bản của TV2;
    //  chỉ cần đảm bảo còn GetById(int).
    // ============================================================================
    public class NhanVienBUS
    {
        private NhanVienDAO dao = new NhanVienDAO();

        public ArrayList GetAll()
        {
            return dao.GetAll();
        }

        public NhanVien GetById(int maNV)
        {
            return dao.GetById(maNV);
        }

        public ArrayList TimKiem(string tuKhoa)
        {
            return TimKiemHelper.TimTuKhoa(dao.GetAll(), tuKhoa, "MaNV", "HoTen", "SDT", "Email", "ChucVu");
        }
        public ArrayList TimNangCao(string tuKhoa) => TimKiem(tuKhoa);
        public string Them(NhanVien x) => Ghi(x,false);
        public string Sua(NhanVien x) => Ghi(x,true);
        private string Ghi(NhanVien x,bool sua){
          if(x==null||string.IsNullOrWhiteSpace(x.Ho)||string.IsNullOrWhiteSpace(x.Ten))return "Họ và tên không được trống";
          if(!System.Text.RegularExpressions.Regex.IsMatch(x.SDT??"",@"^0[0-9]{9}$"))return "SĐT phải có 10 chữ số bắt đầu bằng 0";
          if(!System.Text.RegularExpressions.Regex.IsMatch(x.Email??"",@"^[^@\s]+@[^@\s]+\.[^@\s]+$"))return "Email không hợp lệ";
          if(!x.NgaySinh.HasValue || x.NgaySinh.Value.Date>DateTime.Today.AddYears(-(int)ThamSoBUS.LayGiaTri("TuoiNhanVienToiThieu")))return "Nhân viên chưa đủ tuổi";
          if(x.NgayVaoLam.HasValue && x.NgayVaoLam<x.NgaySinh)return "Ngày vào làm không hợp lệ";
          foreach(NhanVien n in dao.GetAll()){if(n.MaNV==x.MaNV)continue;if(n.SDT==x.SDT)return "Trùng số điện thoại";if(string.Equals(n.Email,x.Email,StringComparison.OrdinalIgnoreCase))return "Trùng email";}
          try{return (sua?dao.Update(x):dao.Insert(x))>0?"":"Không lưu được nhân viên";}catch(Exception e){return e.Message;}
        }
        public string Xoa(int id){try{return dao.Delete(id)>0?"":"Không tìm thấy nhân viên";}catch(Exception e){return "Không thể xóa (có dữ liệu liên quan): "+e.Message;}}
    }
}
