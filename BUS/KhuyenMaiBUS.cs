using System;
using System.Collections;
using System.Collections.Generic;
using QuanLyBanDienThoai.DAO;
using QuanLyBanDienThoai.DTO;
namespace QuanLyBanDienThoai.BUS
{
    public class KhuyenMaiBUS
    {
        private readonly KhuyenMaiDAO dao=new KhuyenMaiDAO();
        public ArrayList GetAll()=>dao.GetAll();
        public ArrayList TimKiem(string key)=>TimKiemHelper.TimTuKhoa(GetAll(),key,"MaKM","TenKM","MoTa");
        public ArrayList TimNangCao(List<DieuKien> dk)=>TimKiemHelper.Loc(GetAll(),dk);
        private string KiemTra(KhuyenMai x)
        {
            if(x==null)return "Thiếu dữ liệu khuyến mãi";
            x.TenKM=(x.TenKM??"").Trim();x.MoTa=(x.MoTa??"").Trim();
            if(x.TenKM.Length==0)return "Tên khuyến mãi không được trống";
            if(x.NgayKetThuc.Date<x.NgayBatDau.Date)return "Ngày kết thúc phải từ ngày bắt đầu trở đi";
            return "";
        }
        public string Them(KhuyenMai x)=>Ghi(x,false);
        public string Sua(KhuyenMai x)=>Ghi(x,true);
        private string Ghi(KhuyenMai x,bool sua)
        {
            string loi=KiemTra(x);if(loi!="")return loi;
            if(sua&&x.MaKM<=0)return "Hãy chọn khuyến mãi cần sửa";
            try {return (sua?dao.Update(x):dao.Insert(x))>0?"":"Không tìm thấy khuyến mãi";}
            catch(Exception ex){return "Lỗi lưu khuyến mãi: "+ex.Message;}
        }
        public string Xoa(int maKM)
        {
            if(maKM<=0)return "Hãy chọn khuyến mãi cần xóa";
            try{return dao.Delete(maKM)>0?"":"Không tìm thấy khuyến mãi";}
            catch(Exception ex){return "Không thể xóa khuyến mãi đang được dùng bởi chi tiết hoặc hóa đơn: "+ex.Message;}
        }
        public string NhapDanhSach(ArrayList ds,out int thanhCong)
        {
            thanhCong=0;var loi=new System.Text.StringBuilder();var maDaCo=new HashSet<int>();
            foreach(KhuyenMai x in GetAll())maDaCo.Add(x.MaKM);
            for(int i=0;i<ds.Count;i++) {
                KhuyenMai x=ds[i] as KhuyenMai;if(x==null)continue;
                string err=x.MaKM>0&&maDaCo.Contains(x.MaKM)?"Trùng mã khuyến mãi":Them(x);
                if(err=="")thanhCong++;else loi.AppendLine("Dòng "+(i+2)+": "+err);
            }
            return loi.ToString();
        }
    }
}
