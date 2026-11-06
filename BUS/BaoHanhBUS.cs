using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using QuanLyBanDienThoai.DAO;
using QuanLyBanDienThoai.DTO;
namespace QuanLyBanDienThoai.BUS
{
    public class BaoHanhBUS
    {
        private readonly BaoHanhDAO dao=new BaoHanhDAO();
        public ArrayList GetAll()=>dao.GetAll();
        // Giao diện trang khách hàng đang sử dụng hàm này.
        public ArrayList GetCuaToi()=>PhienDangNhap.LaKhachHang?dao.GetTheoKhachHang(PhienDangNhap.MaKH):new ArrayList();
        public ArrayList TimKiem(string text)=>TimKiemHelper.TimTuKhoa(GetAll(),text,"MaBH","SoIMEI","TenHienThi","TrangThai","MaHD");
        public ArrayList TimNangCao(List<DieuKien> dk)=>TimKiemHelper.Loc(GetAll(),dk);
        public DateTime LayNgayKetThucMacDinh(DateTime tuNgay)
        {
            int thang=(int)ThamSoBUS.LayGiaTri("ThangBaoHanhMacDinh");
            if(thang<1 || thang>120) thang=12;
            return tuNgay.Date.AddMonths(thang);
        }
        private string KiemTra(BaoHanh x,bool sua)
        {
            if(x==null)return "Thông tin bảo hành không hợp lệ";
            x.SoIMEI=(x.SoIMEI??"").Trim();x.TrangThai=(x.TrangThai??"").Trim();x.GhiChu=(x.GhiChu??"").Trim();
            if(!Regex.IsMatch(x.SoIMEI,@"^[0-9]{15}$"))return "IMEI phải có đúng 15 chữ số";
            if(x.MaHD<=0||x.MaDT<=0)return "Phải chọn hóa đơn và điện thoại";
            if(x.NgayKetThuc.Date<x.NgayBatDau.Date)return "Ngày kết thúc không được trước ngày bắt đầu";
            if(x.TrangThai.Length==0)return "Phải chọn trạng thái bảo hành";
            try {
                if(!dao.CoTrongHoaDon(x.MaHD,x.MaDT))return "Điện thoại không thuộc hóa đơn đã chọn";
                foreach(BaoHanh b in dao.GetAll())
                    if(string.Equals(b.SoIMEI,x.SoIMEI,StringComparison.OrdinalIgnoreCase) && (!sua||b.MaBH!=x.MaBH))
                        return "IMEI này đã được đăng ký bảo hành";
                return "";
            }catch(Exception ex){return "Lỗi kiểm tra bảo hành: "+ex.Message;}
        }
        public string Them(BaoHanh x)=>Ghi(x,false);
        public string Sua(BaoHanh x)=>Ghi(x,true);
        private string Ghi(BaoHanh x,bool sua)
        {
            if(sua&&(x==null||x.MaBH<=0))return "Hãy chọn phiếu bảo hành cần sửa";
            string err=KiemTra(x,sua);if(err!="")return err;
            try{return (sua?dao.Update(x):dao.Insert(x))>0?"":"Không tìm thấy phiếu bảo hành";}
            catch(Exception ex){return "Không thể lưu phiếu bảo hành: "+ex.Message;}
        }
        public string Xoa(int maBH)
        {
            if(maBH<=0)return "Hãy chọn phiếu bảo hành cần xóa";
            try{return dao.Delete(maBH)>0?"":"Không tìm thấy phiếu bảo hành";}
            catch(Exception ex){return "Không thể xóa bảo hành: "+ex.Message;}
        }
        public string NhapDanhSach(ArrayList ds,out int thanhCong)
        {
            thanhCong=0;var errors=new System.Text.StringBuilder();
            for(int i=0;i<ds.Count;i++) {
                BaoHanh b=ds[i] as BaoHanh;if(b==null)continue;
                string err=Them(b);if(err=="")thanhCong++;else errors.AppendLine("Dòng "+(i+2)+": "+err);
            }
            return errors.ToString();
        }
    }
}
