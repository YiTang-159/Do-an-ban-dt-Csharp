using System;
using System.Collections;
using System.Text.RegularExpressions;
using QuanLyBanDienThoai.DAO;
using QuanLyBanDienThoai.DTO;

namespace QuanLyBanDienThoai.BUS
{
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
            if (!PhienDangNhap.LaKhachHang || PhienDangNhap.MaKH <= 0 || x == null || x.MaKH != PhienDangNhap.MaKH)
                return "Khách hàng chỉ được cập nhật hồ sơ của mình";

            string loi = PhienDangNhap.KiemTraConHieuLuc();
            return loi == "" ? LuuThongTin(x) : loi;
        }

        private string LuuThongTin(KhachHang x)
        {
            try
            {
                string loi = KiemTra(x);
                if (loi != "") return loi;
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
        public string Sua(KhachHang x)
        {
            if (!PhienDangNhap.LaNhanSu) return "Chỉ nhân sự được quản lý hồ sơ khách hàng";
            string loi = PhienDangNhap.KiemTraConHieuLuc();
            return loi == "" ? LuuThongTin(x) : loi;
        }
        public string Xoa(int id){try{return dao.Delete(id)>0?"":"Không tìm thấy khách hàng";}catch(Exception e){return "Không thể xóa (có dữ liệu liên quan): "+e.Message;}}

        public ArrayList TimNangCao(System.Collections.Generic.List<DieuKien> conditions) => TimKiemHelper.Loc(dao.GetAll(), conditions);
        public string NhapDanhSach(ArrayList list, out int count)
        {
            count=0; var errors=new System.Text.StringBuilder();
            var keys=new System.Collections.Generic.HashSet<int>();
            foreach(KhachHang old in dao.GetAll()) keys.Add(old.MaKH);
            for(int i=0;i<list.Count;i++) {
                if(list[i] is not KhachHang x) {errors.AppendLine("Dòng "+(i+2)+": sai kiểu dữ liệu");continue;}
                string error;
                if(x.MaKH>0 && !keys.Add(x.MaKH)) error="Mã đã tồn tại hoặc bị trùng trong file";
                else { x.MaKH=0; try { error=Them(x); } catch(Exception ex) {error=ex.Message;} }
                if(error=="") count++; else errors.AppendLine("Dòng "+(i+2)+": "+error);
            }
            return errors.ToString();
        }
    }
}
