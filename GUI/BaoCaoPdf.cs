using System;
using System.Data;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using QuanLyBanDienThoai.DTO;
namespace QuanLyBanDienThoai.GUI
{
    public static class BaoCaoPdf
    {
        private static string Tien(decimal v)=>v.ToString("#,##0")+" đ";
        public static void Xuat(KetQuaThongKe k,string duongDan)
        {
            if(k==null)throw new ArgumentNullException(nameof(k));
            QuestPDF.Settings.License=LicenseType.Community; // Chỉ sử dụng cho đồ án học tập.
            Document.Create(doc=>doc.Page(page=> {
                page.Size(PageSizes.A4);
                page.Margin(30);
                page.DefaultTextStyle(x=>x.FontFamily("Arial").FontSize(10));
                page.Header().Column(header=> {
                    header.Item().AlignCenter().Text("BÁO CÁO THỐNG KÊ CỬA HÀNG ĐIỆN THOẠI").Bold().FontSize(15);
                    header.Item().AlignCenter().Text("Từ "+k.TuNgay.ToString("dd/MM/yyyy")+" đến "+k.DenNgay.ToString("dd/MM/yyyy"));
                });
                page.Content().PaddingVertical(15).Column(col=> {
                    col.Spacing(8);
                    col.Item().Text("Tổng thu: "+Tien(k.TongThu)).SemiBold();
                    col.Item().Text("Tổng chi: "+Tien(k.TongChi)).SemiBold();
                    col.Item().Text("Lợi nhuận (thu - chi): "+Tien(k.LoiNhuan)).Bold();
                    ChenBang(col,"1. Doanh thu theo nhân viên",k.TheoNhanVien);
                    ChenBang(col,"2. Doanh thu theo khách hàng",k.TheoKhachHang);
                    ChenBang(col,"3. Doanh thu theo sản phẩm",k.TheoSanPham);
                    col.Item().Text("Ghi chú: Q1–Q4 là quý của năm; TC là tổng cộng. Nếu chọn nhiều năm, số liệu cùng quý được cộng gộp.").FontSize(8);
                });
                page.Footer().AlignCenter().Text(t=>{t.Span("Trang ");t.CurrentPageNumber();t.Span(" / ");t.TotalPages();});
            })).GeneratePdf(duongDan);
        }
        private static void ChenBang(ColumnDescriptor col,string heading,DataTable dt)
        {
            col.Item().PaddingTop(12).Text(heading).SemiBold().FontSize(12);
            col.Item().Table(table=> {
                table.ColumnsDefinition(c=> {c.RelativeColumn(3);for(int i=0;i<5;i++)c.RelativeColumn(1.4f);});
                table.Header(h=> {
                    foreach(DataColumn c in dt.Columns) h.Cell().BorderBottom(1).Padding(3).Text(c.ColumnName).Bold().FontSize(8);
                });
                foreach(DataRow r in dt.Rows) {
                    for(int i=0;i<6;i++) {
                        string value=i==0?Convert.ToString(r[i]):Convert.ToDecimal(r[i]).ToString("#,##0");
                        table.Cell().BorderBottom(0.3f).BorderColor(Colors.Grey.Lighten2).Padding(3).Text(value).FontSize(8);
                    }
                }
            });
        }
    }
}
