using System;
using System.Collections;
using System.Collections.Generic;
using QuanLyBanDienThoai.DAO;
using QuanLyBanDienThoai.DTO;
namespace QuanLyBanDienThoai.BUS
{
    public class ChiTietKhuyenMaiBUS
    {
        private readonly ChiTietKhuyenMaiDAO dao=new ChiTietKhuyenMaiDAO();
        public ArrayList GetAll()=>dao.GetAll();
        public ArrayList GetTheoKhuyenMai(int maKM)=>dao.GetTheoKhuyenMai(maKM);
        public ArrayList TimKiem(string key)=>TimKiemHelper.TimTuKhoa(GetAll(),key,"MaKM","MaDT","TenKM","TenHienThi");
        public ArrayList TimNangCao(List<DieuKien> dk)=>TimKiemHelper.Loc(GetAll(),dk);
        private string KiemTra(ChiTietKhuyenMai x)
        {
            if(x==null||x.MaKM<=0||x.MaDT<=0)return "Phải chọn khuyến mãi và điện thoại";
            if(x.PhanTramGiam<=0||x.PhanTramGiam>100)return "Phần trăm giảm phải > 0 và <= 100";
            bool coKM=false,coDT=false;
            foreach(KhuyenMai k in new KhuyenMaiDAO().GetAll())if(k.MaKM==x.MaKM)coKM=true;
            foreach(DienThoai d in new DienThoaiDAO().GetAll())if(d.MaDT==x.MaDT)coDT=true;
            if(!coKM||!coDT)return "Khuyến mãi hoặc điện thoại không tồn tại";
            return "";
        }
        public string Them(ChiTietKhuyenMai x)
        {
            string err=KiemTra(x);if(err!="")return err;
            foreach(ChiTietKhuyenMai c in dao.GetAll())if(c.MaKM==x.MaKM&&c.MaDT==x.MaDT)return "Điện thoại đã nằm trong khuyến mãi này";
            try{return dao.Insert(x)>0?"":"Không thể thêm";}catch(Exception ex){return "Lỗi thêm chi tiết: "+ex.Message;}
        }
        public string Sua(ChiTietKhuyenMai x)
        {
            string err=KiemTra(x);if(err!="")return err;
            try{return dao.Update(x)>0?"":"Không tìm thấy chi tiết";}catch(Exception ex){return "Lỗi sửa chi tiết: "+ex.Message;}
        }
        public string Xoa(int maKM,int maDT)
        {
            try{return dao.Delete(maKM,maDT)>0?"":"Không tìm thấy chi tiết";}catch(Exception ex){return "Lỗi xóa chi tiết: "+ex.Message;}
        }
        public string NhapDanhSach(ArrayList ds,out int thanhCong)
        {
            thanhCong=0;var errors=new System.Text.StringBuilder();
            for(int i=0;i<ds.Count;i++) {
                ChiTietKhuyenMai c=ds[i] as ChiTietKhuyenMai;if(c==null)continue;
                string err=Them(c);if(err=="")thanhCong++;else errors.AppendLine("Dòng "+(i+2)+": "+err);
            }
            return errors.ToString();
        }
    }
}
