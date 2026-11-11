using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using QuanLyBanDienThoai.DAO;
using QuanLyBanDienThoai.DTO;
namespace QuanLyBanDienThoai.BUS
{
    public class ThongKeBUS
    {
        private readonly ThongKeDAO dao=new ThongKeDAO();
        private static decimal Tong(ArrayList list)
        {
            decimal sum=0;
            foreach(DongThongKe x in list)sum+=x.SoTien;
            return sum;
        }
        // Q1..Q4 là quý trong năm; nếu lọc nhiều năm sẽ cộng các quý tương ứng qua các năm.
        public static DataTable LapBangCheo(ArrayList list)
        {
            DataTable t=new DataTable();
            t.Columns.Add("DoiTuong",typeof(string));
            for(int q=1;q<=4;q++)t.Columns.Add("Q"+q,typeof(decimal));
            t.Columns.Add("TC",typeof(decimal));
            SortedDictionary<string,decimal[]> buckets=new SortedDictionary<string,decimal[]>(StringComparer.CurrentCultureIgnoreCase);
            foreach(DongThongKe x in list) {
                string ten=x.DoiTuong??"(Không tên)";
                if(!buckets.ContainsKey(ten))buckets[ten]=new decimal[4];
                int q=(x.Ngay.Month-1)/3;
                buckets[ten][q]+=x.SoTien;
            }
            decimal[] tongCot=new decimal[4];
            foreach(KeyValuePair<string,decimal[]> p in buckets) {
                decimal tc=0;
                foreach(decimal v in p.Value)tc+=v;
                t.Rows.Add(p.Key,p.Value[0],p.Value[1],p.Value[2],p.Value[3],tc);
                for(int i=0;i<4;i++)tongCot[i]+=p.Value[i];
            }
            t.Rows.Add("TC",tongCot[0],tongCot[1],tongCot[2],tongCot[3],
                tongCot[0]+tongCot[1]+tongCot[2]+tongCot[3]);
            return t;
        }
        public string LayThongKe(DateTime tuNgay,DateTime denNgay,out KetQuaThongKe result)
        {
            result=null;
            if(tuNgay.Date>denNgay.Date)return "Ngày bắt đầu không được sau ngày kết thúc";
            if(denNgay.Date>=DateTime.MaxValue.Date)return "Ngày kết thúc ngoài phạm vi hỗ trợ";
            try {
                KetQuaThongKe k=new KetQuaThongKe();
                k.TuNgay=tuNgay.Date;k.DenNgay=denNgay.Date;
                k.TongThu=Tong(dao.LayThu(tuNgay,denNgay));
                k.TongChi=Tong(dao.LayChi(tuNgay,denNgay));
                k.TheoNhanVien=LapBangCheo(dao.TheoNhanVien(tuNgay,denNgay));
                k.TheoKhachHang=LapBangCheo(dao.TheoKhachHang(tuNgay,denNgay));
                k.TheoSanPham=LapBangCheo(dao.TheoSanPham(tuNgay,denNgay));
                result=k;return "";
            }catch(Exception ex){return "Không thể thống kê: "+ex.Message;}
        }
    }
}
