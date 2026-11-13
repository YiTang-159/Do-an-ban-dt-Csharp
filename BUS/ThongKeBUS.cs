using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using QuanLyBanDienThoai.DAO;
using QuanLyBanDienThoai.DTO;

namespace QuanLyBanDienThoai.BUS
{
    // So sánh nhãn dạng "10 - Tên" theo số đứng đầu (2 trước 10), sau đó theo chữ.
    // Hậu tố " [năm]" được giữ nguyên.
    public class SoSanhNhan : IComparer<string>
    {
        private static int LayMa(string s)
        {
            int i = 0;
            while (i < s.Length && char.IsDigit(s[i])) i++;
            int v;
            return i > 0 && int.TryParse(s.Substring(0, i), out v) ? v : int.MaxValue;
        }

        public int Compare(string a, string b)
        {
            int c = LayMa(a).CompareTo(LayMa(b));
            return c != 0 ? c : string.Compare(a, b, StringComparison.CurrentCultureIgnoreCase);
        }
    }

    public class ThongKeBUS
    {
        private readonly ThongKeDAO dao = new ThongKeDAO();

        private static decimal Tong(ArrayList list)
        {
            decimal sum = 0;
            foreach (DongThongKe x in list) sum += x.SoTien;
            return sum;
        }

        // Lập bảng chéo: mỗi dòng là một đối tượng, cột Q1..Q4 theo quý, TC = tổng cộng; dòng cuối "TC".
        // Nếu dữ liệu trải nhiều năm thì mỗi đối tượng được tách thành từng dòng theo năm
        // (nhãn "... [năm]") để không cộng gộp Q1 của các năm khác nhau.
        public static DataTable LapBangCheo(ArrayList list)
        {
            DataTable t = new DataTable();
            t.Columns.Add("DoiTuong", typeof(string));
            for (int q = 1; q <= 4; q++) t.Columns.Add("Q" + q, typeof(decimal));
            t.Columns.Add("TC", typeof(decimal));

            HashSet<int> cacNam = new HashSet<int>();
            foreach (DongThongKe x in list) cacNam.Add(x.Ngay.Year);
            bool nhieuNam = cacNam.Count > 1;

            SortedDictionary<string, decimal[]> cacDong = new SortedDictionary<string, decimal[]>(new SoSanhNhan());
            foreach (DongThongKe x in list)
            {
                string ten = x.DoiTuong ?? "(Không tên)";
                if (nhieuNam) ten += " [" + x.Ngay.Year + "]";
                if (!cacDong.ContainsKey(ten)) cacDong[ten] = new decimal[4];
                int quy = (x.Ngay.Month - 1) / 3;       // 0..3
                cacDong[ten][quy] += x.SoTien;
            }

            decimal[] tongCot = new decimal[4];
            foreach (KeyValuePair<string, decimal[]> p in cacDong)
            {
                decimal tc = 0;
                foreach (decimal v in p.Value) tc += v;
                t.Rows.Add(p.Key, p.Value[0], p.Value[1], p.Value[2], p.Value[3], tc);
                for (int i = 0; i < 4; i++) tongCot[i] += p.Value[i];
            }
            t.Rows.Add("TC", tongCot[0], tongCot[1], tongCot[2], tongCot[3],
                tongCot[0] + tongCot[1] + tongCot[2] + tongCot[3]);
            return t;
        }

        // Tổng thu = tổng hóa đơn, tổng chi = tổng phiếu nhập, lợi nhuận = thu - chi (tính trong KetQuaThongKe).
        // Trả về "" nếu thành công.
        public string LayThongKe(DateTime tuNgay, DateTime denNgay, out KetQuaThongKe result)
        {
            result = null;
            if (tuNgay.Date > denNgay.Date) return "Ngày bắt đầu không được sau ngày kết thúc";
            if (denNgay.Date >= DateTime.MaxValue.Date) return "Ngày kết thúc ngoài phạm vi hỗ trợ";
            try
            {
                KetQuaThongKe k = new KetQuaThongKe();
                k.TuNgay = tuNgay.Date;
                k.DenNgay = denNgay.Date;
                k.TongThu = Tong(dao.LayThu(tuNgay, denNgay));
                k.TongChi = Tong(dao.LayChi(tuNgay, denNgay));
                k.TheoNhanVien = LapBangCheo(dao.TheoNhanVien(tuNgay, denNgay));
                k.TheoKhachHang = LapBangCheo(dao.TheoKhachHang(tuNgay, denNgay));
                k.TheoSanPham = LapBangCheo(dao.TheoSanPham(tuNgay, denNgay));
                result = k;
                return "";
            }
            catch (Exception ex)
            {
                return "Không thể thống kê: " + ex.Message;
            }
        }
    }
}
