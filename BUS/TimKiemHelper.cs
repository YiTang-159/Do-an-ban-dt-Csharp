using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace QuanLyBanDienThoai.BUS
{
    // Cách nối một điều kiện với điều kiện đứng trước nó
    public enum PhepNoi { And, Or }

    // Một điều kiện tìm kiếm: [NOT] Truong PhepToan GiaTri
    public class DieuKien
    {
        public string Truong { get; set; }      // tên thuộc tính của DTO, ví dụ "GiaNiemYet"
        public string PhepToan { get; set; }    // =, <>, >, >=, <, <=, chứa
        public string GiaTri { get; set; }
        public PhepNoi Noi { get; set; }        // nối với điều kiện đứng trước (điều kiện đầu tiên bỏ qua)
        public bool PhuDinh { get; set; }       // true = NOT

        public DieuKien()
        {
            PhepToan = "=";
            Noi = PhepNoi.And;
        }

        public DieuKien(string truong, string phepToan, string giaTri, PhepNoi noi, bool phuDinh)
        {
            Truong = truong;
            PhepToan = phepToan;
            GiaTri = giaTri;
            Noi = noi;
            PhuDinh = phuDinh;
        }

        // Chuỗi hiển thị, ví dụ: "AND NOT TenHang = Apple"
        public string MoTa(bool laDauTien)
        {
            string s = "";
            if (!laDauTien) s = (Noi == PhepNoi.And ? "AND " : "OR ");
            if (PhuDinh) s += "NOT ";
            return s + Truong + " " + PhepToan + " " + GiaTri;
        }
    }

    // Tìm kiếm trên ArrayList (viết trong BUS, không có câu SQL WHERE LIKE).
    // Mọi BUS dùng chung: TimKiem(tuKhoa) gọi TimTuKhoa, TimNangCao(dieuKien) gọi Loc.
    public static class TimKiemHelper
    {
        public static readonly string[] CacPhepToan = { "=", "<>", ">", ">=", "<", "<=", "chứa" };

        // Lọc ArrayList theo danh sách điều kiện.
        // Thứ tự ưu tiên giống SQL: AND tính trước, OR tính sau. NOT áp dụng cho từng điều kiện.
        public static ArrayList Loc(ArrayList ds, List<DieuKien> dieuKien)
        {
            ArrayList kq = new ArrayList();
            if (ds == null) return kq;
            if (dieuKien == null || dieuKien.Count == 0)
            {
                kq.AddRange(ds);
                return kq;
            }
            foreach (object item in ds)
            {
                if (ThoaMan(item, dieuKien)) kq.Add(item);
            }
            return kq;
        }

        // Tìm theo từ khóa: bản ghi nào có một trong các trường chứa từ khóa (không phân biệt hoa thường, dấu)
        public static ArrayList TimTuKhoa(ArrayList ds, string tuKhoa, params string[] cacTruong)
        {
            if (string.IsNullOrWhiteSpace(tuKhoa))
            {
                ArrayList tatCa = new ArrayList();
                if (ds != null) tatCa.AddRange(ds);
                return tatCa;
            }
            List<DieuKien> dk = new List<DieuKien>();
            foreach (string t in cacTruong)
            {
                dk.Add(new DieuKien(t, "chứa", tuKhoa.Trim(), PhepNoi.Or, false));
            }
            return Loc(ds, dk);
        }

        // Danh sách tên trường (thuộc tính kiểu đơn giản) của một DTO, dùng cho FormTimNangCao
        public static string[] LayDanhSachTruong(Type kieu)
        {
            List<string> ten = new List<string>();
            foreach (PropertyInfo p in kieu.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                if (p.CanRead && LaKieuDonGian(p.PropertyType)) ten.Add(p.Name);
            }
            return ten.ToArray();
        }

        public static bool LaKieuDonGian(Type kieu)
        {
            Type goc = Nullable.GetUnderlyingType(kieu) ?? kieu;
            return goc.IsPrimitive || goc == typeof(string) || goc == typeof(decimal) || goc == typeof(DateTime);
        }

        // Kiểm tra một điều kiện trước khi lọc. Trả về "" nếu hợp lệ, ngược lại là thông báo lỗi.
        public static string KiemTraDieuKien(Type kieu, DieuKien dk)
        {
            if (dk == null || string.IsNullOrWhiteSpace(dk.Truong)) return "Hãy chọn trường";
            PropertyInfo p = kieu.GetProperty(dk.Truong);
            if (p == null) return "Không có trường \"" + dk.Truong + "\"";
            if (Array.IndexOf(CacPhepToan, dk.PhepToan) < 0) return "Hãy chọn phép toán";
            if (string.IsNullOrWhiteSpace(dk.GiaTri)) return "Giá trị không được để trống";
            if (dk.PhepToan == "chứa") return "";

            Type goc = Nullable.GetUnderlyingType(p.PropertyType) ?? p.PropertyType;
            decimal so;
            DateTime ngay;
            if (LaKieuSo(goc) && !TryParseSo(dk.GiaTri, out so))
                return "Trường " + dk.Truong + " là số, giá trị \"" + dk.GiaTri + "\" không hợp lệ";
            if (goc == typeof(DateTime) && !TryParseNgay(dk.GiaTri, out ngay))
                return "Trường " + dk.Truong + " là ngày, nhập dạng dd/MM/yyyy";
            return "";
        }

        // Đọc số người dùng gõ: chấp nhận 10000000, 10.000.000 hoặc 10,000,000
        public static bool TryParseSo(string s, out decimal kq)
        {
            kq = 0;
            if (string.IsNullOrWhiteSpace(s)) return false;
            s = s.Trim();
            if (Regex.IsMatch(s, @"^\d{1,3}([.,]\d{3})+$")) s = Regex.Replace(s, @"[.,]", "");
            return decimal.TryParse(s, NumberStyles.Number, CultureInfo.CurrentCulture, out kq)
                || decimal.TryParse(s, NumberStyles.Number, CultureInfo.InvariantCulture, out kq);
        }

        public static bool TryParseNgay(string s, out DateTime kq)
        {
            kq = DateTime.MinValue;
            if (string.IsNullOrWhiteSpace(s)) return false;
            s = s.Trim();
            string[] dang = { "dd/MM/yyyy", "d/M/yyyy", "yyyy-MM-dd" };
            return DateTime.TryParseExact(s, dang, CultureInfo.InvariantCulture, DateTimeStyles.None, out kq)
                || DateTime.TryParse(s, CultureInfo.CurrentCulture, DateTimeStyles.None, out kq);
        }

        // ---------------- phần xử lý bên trong ----------------

        private static bool ThoaMan(object item, List<DieuKien> dk)
        {
            bool ketQuaOr = false;      // kết quả các nhóm AND đã đóng, nối với nhau bằng OR
            bool nhomAnd = true;        // nhóm AND đang tính
            for (int i = 0; i < dk.Count; i++)
            {
                bool v = DanhGia(item, dk[i]);
                if (i == 0)
                {
                    nhomAnd = v;
                }
                else if (dk[i].Noi == PhepNoi.And)
                {
                    nhomAnd = nhomAnd && v;
                }
                else
                {
                    ketQuaOr = ketQuaOr || nhomAnd;
                    nhomAnd = v;
                }
            }
            return ketQuaOr || nhomAnd;
        }

        private static bool DanhGia(object item, DieuKien dk)
        {
            PropertyInfo p = item.GetType().GetProperty(dk.Truong);
            if (p == null) throw new ArgumentException("Không có trường \"" + dk.Truong + "\"");
            object giaTri = p.GetValue(item, null);
            bool kq = SoSanh(giaTri, p.PropertyType, dk.PhepToan, dk.GiaTri ?? "");
            return dk.PhuDinh ? !kq : kq;
        }

        private static bool SoSanh(object giaTri, Type kieu, string phepToan, string nhap)
        {
            if (phepToan == "chứa")
            {
                return BoDau(ChuoiHoa(giaTri)).Contains(BoDau(nhap.Trim()));
            }

            Type goc = Nullable.GetUnderlyingType(kieu) ?? kieu;
            int cmp;
            if (LaKieuSo(goc))
            {
                if (giaTri == null) return phepToan == "<>";
                decimal b;
                if (!TryParseSo(nhap, out b))
                    throw new FormatException("Giá trị \"" + nhap + "\" không phải là số");
                cmp = Convert.ToDecimal(giaTri).CompareTo(b);
            }
            else if (goc == typeof(DateTime))
            {
                if (giaTri == null) return phepToan == "<>";
                DateTime b;
                if (!TryParseNgay(nhap, out b))
                    throw new FormatException("Giá trị \"" + nhap + "\" không phải là ngày (dd/MM/yyyy)");
                cmp = ((DateTime)giaTri).Date.CompareTo(b.Date);
            }
            else
            {
                string a = giaTri == null ? "" : giaTri.ToString().Trim();
                cmp = string.Compare(a, nhap.Trim(), StringComparison.CurrentCultureIgnoreCase);
            }
            return ApDung(phepToan, cmp);
        }

        private static bool ApDung(string phepToan, int cmp)
        {
            switch (phepToan)
            {
                case "=": return cmp == 0;
                case "<>": return cmp != 0;
                case ">": return cmp > 0;
                case ">=": return cmp >= 0;
                case "<": return cmp < 0;
                case "<=": return cmp <= 0;
                default: throw new ArgumentException("Phép toán không hợp lệ: " + phepToan);
            }
        }

        private static bool LaKieuSo(Type t)
        {
            return t == typeof(int) || t == typeof(long) || t == typeof(short) || t == typeof(byte)
                || t == typeof(uint) || t == typeof(ulong) || t == typeof(ushort) || t == typeof(sbyte)
                || t == typeof(decimal) || t == typeof(double) || t == typeof(float);
        }

        private static string ChuoiHoa(object v)
        {
            if (v == null) return "";
            if (v is DateTime) return ((DateTime)v).ToString("dd/MM/yyyy");
            return Convert.ToString(v, CultureInfo.InvariantCulture);
        }

        // Bỏ dấu tiếng Việt và đưa về chữ thường để "chứa" không phân biệt dấu
        private static string BoDau(string s)
        {
            if (string.IsNullOrEmpty(s)) return "";
            string d = s.Normalize(NormalizationForm.FormD);
            StringBuilder sb = new StringBuilder();
            foreach (char c in d)
            {
                if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark) sb.Append(c);
            }
            return sb.ToString().Replace('đ', 'd').Replace('Đ', 'D').ToLowerInvariant();
        }
    }
}
