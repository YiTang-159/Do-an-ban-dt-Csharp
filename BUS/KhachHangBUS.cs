using System;
using System.Collections;
using System.Text.RegularExpressions;
using QuanLyBanDienThoai.DAO;
using QuanLyBanDienThoai.DTO;

namespace QuanLyBanDienThoai.BUS
{
    // ============================================================================
    //  BẢN TỐI THIỂU (GetAll, GetById, TimKiem, KiemTra, CapNhatThongTin). FILE NÀY THUỘC VỀ TV2.
    //  Khi TV2 gộp KhachHangBUS thật (Them, Sua, Xoa, TimNangCao, nhập Excel) thì lấy bản của TV2;
    //  chỉ cần đảm bảo còn GetById(int) và quy tắc kiểm tra dữ liệu ở KiemTra (TaiKhoanBUS dùng khi đăng ký).
    // ============================================================================
    public class KhachHangBUS
    {
        private KhachHangDAO dao = new KhachHangDAO();

        public ArrayList GetAll()
        {
            return dao.GetAll();
        }

        public KhachHang GetById(int maKH)
        {
            return dao.GetById(maKH);
        }

        public ArrayList TimKiem(string tuKhoa)
        {
            return TimKiemHelper.TimTuKhoa(dao.GetAll(), tuKhoa, "MaKH", "HoTen", "SDT", "Email", "DiaChi");
        }

        // Quy tắc của đề: Ho, Ten, DiaChi không trống; SDT đủ 10 chữ số bắt đầu bằng 0; NgaySinh không ở tương lai;
        // SDT và Email không trùng khách hàng khác. Trả về "" nếu hợp lệ.
        public string KiemTra(KhachHang x)
        {
            if (x == null) return "Dữ liệu không hợp lệ";
            x.Ho = (x.Ho ?? "").Trim();
            x.Ten = (x.Ten ?? "").Trim();
            x.SDT = (x.SDT ?? "").Trim();
            x.Email = (x.Email ?? "").Trim();
            x.DiaChi = (x.DiaChi ?? "").Trim();

            if (x.Ho == "") return "Họ không được để trống";
            if (x.Ten == "") return "Tên không được để trống";
            if (!Regex.IsMatch(x.SDT, @"^0\d{9}$")) return "Số điện thoại phải đủ 10 chữ số và bắt đầu bằng 0";
            if (!Regex.IsMatch(x.Email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$")) return "Email không hợp lệ";
            if (x.DiaChi == "") return "Địa chỉ không được để trống";
            if (x.NgaySinh.HasValue && x.NgaySinh.Value.Date > DateTime.Today) return "Ngày sinh không được lớn hơn ngày hiện tại";

            foreach (object o in dao.GetAll())
            {
                KhachHang k = (KhachHang)o;
                if (k.MaKH == x.MaKH) continue;
                if (k.SDT == x.SDT) return "Số điện thoại đã được khách hàng khác sử dụng";
                if (string.Equals(k.Email, x.Email, StringComparison.OrdinalIgnoreCase)) return "Email đã được khách hàng khác sử dụng";
            }
            return "";
        }

        // Khách hàng tự sửa thông tin của mình (DiemTichLuy không đổi)
        public string CapNhatThongTin(KhachHang x)
        {
            string loi = KiemTra(x);
            if (loi != "") return loi;
            try
            {
                if (dao.Update(x) == 0) return "Không tìm thấy khách hàng cần sửa";
                return "";
            }
            catch (Exception ex)
            {
                return "Không thể sửa: " + ex.Message;
            }
        }
        public ArrayList TimNangCao(string tuKhoa)=>TimKiem(tuKhoa);
        public string Them(KhachHang x){string loi=KiemTra(x);if(loi!="")return loi;try{return dao.Insert(x)>0?"":"Không thêm được";}catch(Exception e){return e.Message;}}
        public string Sua(KhachHang x)=>CapNhatThongTin(x);
        public string Xoa(int id){try{return dao.Delete(id)>0?"":"Không tìm thấy khách hàng";}catch(Exception e){return "Không thể xóa (có dữ liệu liên quan): "+e.Message;}}
    }
}
