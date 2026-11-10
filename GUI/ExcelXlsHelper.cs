using System;
using System.Collections;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using QuanLyBanDienThoai.BUS;
namespace QuanLyBanDienThoai.GUI
{
    // HSSF writes actual Excel 97-2003 binary workbooks, not renamed xlsx files.
    public static class ExcelXlsHelper
    {
        private static PropertyInfo[] Cot(Type type) => type.GetProperties().Where(x => x.CanRead && x.CanWrite && TimKiemHelper.LaKieuDonGian(x.PropertyType)).ToArray();
        public static void Xuat(ArrayList list, Type type, string path)
        {
            if (list != null && list.Count > 65535) throw new InvalidOperationException("XLS tối đa 65535 dòng dữ liệu. Hãy chọn XLSX.");
            using (var book = new HSSFWorkbook())
            {
                var sheet = book.CreateSheet("DuLieu"); var props = Cot(type);
                var head = sheet.CreateRow(0);
                for (int c=0;c<props.Length;c++) head.CreateCell(c).SetCellValue(props[c].Name);
                var date = book.CreateCellStyle(); date.DataFormat = book.CreateDataFormat().GetFormat("dd/MM/yyyy");
                for (int r=0;list!=null && r<list.Count;r++)
                {
                    var row=sheet.CreateRow(r+1);
                    for(int c=0;c<props.Length;c++)
                    {
                        object v=props[c].GetValue(list[r]); var cell=row.CreateCell(c);
                        if(v==null) continue;
                        if(v is DateTime d) {cell.SetCellValue(d);cell.CellStyle=date;}
                        else if(v is bool b) cell.SetCellValue(b);
                        else if(v is string text) cell.SetCellValue(text);
                        else cell.SetCellValue(Convert.ToDouble(v, CultureInfo.InvariantCulture));
                    }
                }
                for(int c=0;c<props.Length;c++) sheet.AutoSizeColumn(c);
                using(var output=File.Create(path)) book.Write(output);
            }
        }
        public static ArrayList Nhap(string path, Type type)
        {
            using(var input=File.OpenRead(path))
            using(var book=new HSSFWorkbook(input))
            {
                var sheet=book.GetSheetAt(0); var head=sheet.GetRow(0); var list=new ArrayList();
                if(head==null) return list;
                var props=Cot(type); var map=new System.Collections.Generic.Dictionary<int,PropertyInfo>();
                for(int c=0;c<head.LastCellNum;c++)
                {
                    string name=head.GetCell(c)?.ToString().Trim();
                    var prop=props.FirstOrDefault(x=>string.Equals(x.Name,name,StringComparison.OrdinalIgnoreCase));
                    if(prop!=null) map[c]=prop;
                }
                if(map.Count==0) throw new FormatException("Không có tiêu đề cột phù hợp.");
                for(int r=1;r<=sheet.LastRowNum;r++)
                {
                    var row=sheet.GetRow(r); if(row==null || !map.Keys.Any(c=>!string.IsNullOrWhiteSpace(row.GetCell(c)?.ToString()))) continue;
                    object obj=Activator.CreateInstance(type);
                    foreach(var pair in map)
                    {
                        var cell=row.GetCell(pair.Key); if(cell==null || string.IsNullOrWhiteSpace(cell.ToString())) continue;
                        try {
                            var t=Nullable.GetUnderlyingType(pair.Value.PropertyType)??pair.Value.PropertyType; object value;
                            if(t==typeof(string)) value=cell.ToString().Trim();
                            else if(t==typeof(DateTime)) {
                                if(cell.CellType==CellType.Numeric && DateUtil.IsCellDateFormatted(cell)) value=cell.DateCellValue;
                                else if(TimKiemHelper.TryParseNgay(cell.ToString(),out DateTime day)) value=day;
                                else throw new FormatException();
                            }
                            else if(t==typeof(bool)) value=cell.BooleanCellValue;
                            else {
                                decimal number;
                                if(cell.CellType==CellType.Numeric) number=(decimal)cell.NumericCellValue;
                                else if(!TimKiemHelper.TryParseSo(cell.ToString(),out number)) throw new FormatException();
                                value=Convert.ChangeType(number,t,CultureInfo.InvariantCulture);
                            }
                            pair.Value.SetValue(obj,value);
                        } catch(Exception ex) {throw new FormatException("Dòng "+(r+1)+", cột "+pair.Value.Name+": dữ liệu không hợp lệ",ex);}
                    }
                    list.Add(obj);
                }
                return list;
            }
        }
    }
}
