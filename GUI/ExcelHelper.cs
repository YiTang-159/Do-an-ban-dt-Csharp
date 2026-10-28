using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Windows.Forms;
using ClosedXML.Excel;
using QuanLyBanDienThoai.BUS;

namespace QuanLyBanDienThoai.GUI
{
    // Xuất ArrayList DTO ra file Excel và nhập ngược lại thành ArrayList DTO.
    // Thư viện: ClosedXML (cài bằng NuGet, định dạng .xlsx). Dòng 1 là tên thuộc tính của DTO.
    // Dữ liệu nhập phải đi qua BUS (xxxBUS.NhapDanhSach) để kiểm tra trùng và hợp lệ.
    public static class ExcelHelper
    {
        // Các thuộc tính kiểu đơn giản, đọc và ghi được, theo thứ tự khai báo
        private static List<PropertyInfo> LayCot(Type kieu)
        {
            List<PropertyInfo> cot = new List<PropertyInfo>();
            foreach (PropertyInfo p in kieu.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                if (p.CanRead && p.CanWrite && TimKiemHelper.LaKieuDonGian(p.PropertyType)) cot.Add(p);
            }
            return cot;
        }

        // Xuất ra file; kiểu DTO lấy từ phần tử đầu (danh sách rỗng thì dùng bản có tham số kiểu)
        public static void Xuat(ArrayList ds, string duongDan)
        {
            if (ds == null || ds.Count == 0) throw new InvalidOperationException("Không có dữ liệu để xuất");
            Xuat(ds, ds[0].GetType(), duongDan);
        }

        public static void Xuat(ArrayList ds, Type kieu, string duongDan)
        {
            List<PropertyInfo> cot = LayCot(kieu);
            using (XLWorkbook wb = new XLWorkbook())
            {
                IXLWorksheet ws = wb.Worksheets.Add("DuLieu");
                for (int c = 0; c < cot.Count; c++)
                {
                    ws.Cell(1, c + 1).Value = cot[c].Name;
                }
                ws.Row(1).Style.Font.Bold = true;

                int dong = 2;
                if (ds != null)
                {
                    foreach (object o in ds)
                    {
                        for (int c = 0; c < cot.Count; c++)
                        {
                            GhiO(ws.Cell(dong, c + 1), cot[c].GetValue(o, null));
                        }
                        dong++;
                    }
                }
                ws.Columns().AdjustToContents();
                wb.SaveAs(duongDan);
            }
        }

        private static void GhiO(IXLCell o, object v)
        {
            if (v == null) return;
            if (v is string) o.Value = (string)v;
            else if (v is DateTime)
            {
                o.Value = (DateTime)v;
                o.Style.DateFormat.Format = "dd/MM/yyyy";
            }
            else if (v is bool) o.Value = (bool)v;
            else if (v is decimal) o.Value = (double)(decimal)v;
            else if (v is int || v is long || v is short || v is byte) o.Value = Convert.ToDouble(v);
            else if (v is double) o.Value = (double)v;
            else if (v is float) o.Value = (double)(float)v;
            else o.Value = Convert.ToString(v, CultureInfo.InvariantCulture);
        }

        // Đọc file Excel thành ArrayList DTO kiểu "kieu". Cột nhận theo tên ở dòng 1 (không phân biệt hoa thường).
        // Gặp ô sai kiểu thì ném FormatException có ghi rõ dòng và cột.
        public static ArrayList Nhap(string duongDan, Type kieu)
        {
            ArrayList kq = new ArrayList();
            List<PropertyInfo> cot = LayCot(kieu);
            using (XLWorkbook wb = new XLWorkbook(duongDan))
            {
                IXLWorksheet ws = wb.Worksheet(1);
                IXLRange vung = ws.RangeUsed();
                if (vung == null) return kq;
                int dongCuoi = vung.LastRow().RowNumber();
                int cotCuoi = vung.LastColumn().ColumnNumber();

                // ánh xạ số cột trong file -> thuộc tính của DTO
                Dictionary<int, PropertyInfo> anhXa = new Dictionary<int, PropertyInfo>();
                for (int c = 1; c <= cotCuoi; c++)
                {
                    string tieuDe = ws.Cell(1, c).GetString().Trim();
                    foreach (PropertyInfo p in cot)
                    {
                        if (string.Equals(p.Name, tieuDe, StringComparison.OrdinalIgnoreCase))
                        {
                            anhXa[c] = p;
                            break;
                        }
                    }
                }
                if (anhXa.Count == 0)
                    throw new FormatException("Dòng 1 của file không có tên cột nào khớp với dữ liệu cần nhập");

                for (int r = 2; r <= dongCuoi; r++)
                {
                    bool dongRong = true;
                    foreach (int c in anhXa.Keys)
                    {
                        if (!ws.Cell(r, c).IsEmpty()) { dongRong = false; break; }
                    }
                    if (dongRong) continue;

                    object dto = Activator.CreateInstance(kieu);
                    foreach (KeyValuePair<int, PropertyInfo> kv in anhXa)
                    {
                        DocO(dto, kv.Value, ws.Cell(r, kv.Key), r, kv.Key);
                    }
                    kq.Add(dto);
                }
            }
            return kq;
        }

        private static void DocO(object dto, PropertyInfo p, IXLCell o, int dong, int cot)
        {
            if (o.IsEmpty()) return;    // để giá trị mặc định
            Type goc = Nullable.GetUnderlyingType(p.PropertyType) ?? p.PropertyType;
            string loiO = "Dòng " + dong + ", cột \"" + p.Name + "\": giá trị không hợp lệ";

            if (goc == typeof(string))
            {
                p.SetValue(dto, o.GetString().Trim(), null);
            }
            else if (goc == typeof(DateTime))
            {
                DateTime d;
                if (!o.TryGetValue(out d) && !TimKiemHelper.TryParseNgay(o.GetString(), out d))
                    throw new FormatException(loiO);
                p.SetValue(dto, d, null);
            }
            else if (goc == typeof(bool))
            {
                bool b;
                if (!o.TryGetValue(out b)) throw new FormatException(loiO);
                p.SetValue(dto, b, null);
            }
            else
            {
                decimal m;
                if (!o.TryGetValue(out m) && !TimKiemHelper.TryParseSo(o.GetString(), out m))
                    throw new FormatException(loiO);
                p.SetValue(dto, Convert.ChangeType(m, goc, CultureInfo.InvariantCulture), null);
            }
        }

        // Có hộp thoại chọn nơi lưu. Trả về true nếu xuất xong.
        public static bool XuatCoHopThoai(ArrayList ds, Type kieu, string tenGoiY)
        {
            using (SaveFileDialog sfd = new SaveFileDialog())
            {
                sfd.Filter = "Excel (*.xlsx)|*.xlsx";
                sfd.FileName = tenGoiY + ".xlsx";
                if (sfd.ShowDialog() != DialogResult.OK) return false;
                try
                {
                    Xuat(ds, kieu, sfd.FileName);
                    MessageBox.Show("Đã xuất " + (ds == null ? 0 : ds.Count) + " dòng ra file:\n" + sfd.FileName,
                        "Xuất Excel", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Không thể xuất file: " + ex.Message, "Lỗi",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
            }
        }

        // Có hộp thoại chọn file. Trả về null nếu hủy hoặc lỗi đọc file.
        public static ArrayList NhapCoHopThoai(Type kieu)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "Excel (*.xlsx)|*.xlsx";
                if (ofd.ShowDialog() != DialogResult.OK) return null;
                try
                {
                    return Nhap(ofd.FileName, kieu);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Không thể đọc file: " + ex.Message, "Lỗi",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return null;
                }
            }
        }
    }
}
