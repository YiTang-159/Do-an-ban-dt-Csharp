using System;
using System.Collections;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using QuanLyBanDienThoai.DTO;

namespace QuanLyBanDienThoai.GUI
{
    // Xuất hóa đơn bán hàng ra file PDF dùng QuestPDF (TV3)
    public static class InHoaDon
    {
        private static string Tien(decimal v) => v.ToString("#,##0") + " đ";

        public static void Xuat(HoaDon hd, ArrayList chiTiet, string duongDan)
        {
            if (hd == null) throw new ArgumentNullException(nameof(hd));
            if (chiTiet == null) throw new ArgumentNullException(nameof(chiTiet));

            QuestPDF.Settings.License = LicenseType.Community;

            Document.Create(doc => doc.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(30);
                page.DefaultTextStyle(x => x.FontFamily("Arial").FontSize(10));

                // Tiêu đề đầu trang
                page.Header().Column(col =>
                {
                    col.Item().AlignCenter().Text("CỬA HÀNG QUẢN LÝ BÁN ĐIỆN THOẠI").Bold().FontSize(12).FontColor(Colors.Grey.Darken2);
                    col.Item().AlignCenter().Text("HÓA ĐƠN BÁN HÀNG").Bold().FontSize(16).FontColor(Colors.Blue.Darken2);
                    col.Item().AlignCenter().Text($"Số HĐ: HD{hd.MaHD:D6}  |  Ngày lập: {hd.NgayLap:dd/MM/yyyy HH:mm}").FontSize(9).Italic();
                    col.Item().PaddingTop(5).LineHorizontal(1).LineColor(Colors.Grey.Lighten1);
                });

                // Nội dung hóa đơn
                page.Content().PaddingVertical(10).Column(col =>
                {
                    col.Spacing(6);

                    // Thông tin khách hàng, nhân viên lập, hình thức thanh toán
                    col.Item().Row(row =>
                    {
                        row.RelativeItem().Column(c =>
                        {
                            c.Item().Text($"Khách hàng: {hd.TenKhachHang}").SemiBold();
                            c.Item().Text($"Phương thức thanh toán: {hd.PhuongThucTT}");
                        });
                        row.RelativeItem().Column(c =>
                        {
                            c.Item().Text($"Nhân viên lập: {hd.TenNhanVien}").SemiBold();
                            c.Item().Text($"Mã KH: {hd.MaKH}  |  Mã NV: {hd.MaNV}");
                        });
                    });

                    col.Item().PaddingTop(6).Text("DANH SÁCH SẢN PHẨM").SemiBold().FontSize(11);

                    // Bảng chi tiết sản phẩm
                    col.Item().Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.ConstantColumn(30);   // STT
                            columns.RelativeColumn(4);    // Tên điện thoại
                            columns.ConstantColumn(40);   // SL
                            columns.RelativeColumn(2);    // Đơn giá
                            columns.RelativeColumn(2);    // Khuyến mãi
                            columns.RelativeColumn(2.5f); // Thành tiền
                        });

                        // Tiêu đề cột
                        table.Header(h =>
                        {
                            h.Cell().BorderBottom(1).Padding(4).AlignCenter().Text("STT").Bold();
                            h.Cell().BorderBottom(1).Padding(4).Text("Tên điện thoại").Bold();
                            h.Cell().BorderBottom(1).Padding(4).AlignCenter().Text("SL").Bold();
                            h.Cell().BorderBottom(1).Padding(4).AlignRight().Text("Đơn giá").Bold();
                            h.Cell().BorderBottom(1).Padding(4).Text("Khuyến mãi").Bold();
                            h.Cell().BorderBottom(1).Padding(4).AlignRight().Text("Thành tiền").Bold();
                        });

                        // Các dòng dữ liệu
                        int stt = 1;
                        foreach (object o in chiTiet)
                        {
                            ChiTietHoaDon ct = (ChiTietHoaDon)o;
                            table.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2).Padding(4).AlignCenter().Text(stt.ToString());
                            table.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2).Padding(4).Text(ct.TenHienThi ?? $"Mã ĐT: {ct.MaDT}");
                            table.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2).Padding(4).AlignCenter().Text(ct.SoLuong.ToString());
                            table.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2).Padding(4).AlignRight().Text(Tien(ct.DonGia));
                            table.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2).Padding(4).Text(string.IsNullOrEmpty(ct.TenKM) ? "-" : ct.TenKM);
                            table.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2).Padding(4).AlignRight().Text(Tien(ct.ThanhTien));
                            stt++;
                        }
                    });

                    // Tổng tiền thanh toán
                    col.Item().PaddingTop(10).AlignRight().Column(c =>
                    {
                        c.Item().Text($"TỔNG CỘNG THANH TOÁN: {Tien(hd.TongTien)}").Bold().FontSize(12).FontColor(Colors.Red.Darken1);
                    });

                    // Lời cảm ơn
                    col.Item().PaddingTop(20).AlignCenter().Text("Cảm ơn quý khách đã tin tưởng và ủng hộ cửa hàng!").Italic().FontSize(10);
                });

                // Chân trang đánh số trang
                page.Footer().AlignCenter().Text(t =>
                {
                    t.Span("Trang ");
                    t.CurrentPageNumber();
                    t.Span(" / ");
                    t.TotalPages();
                });
            })).GeneratePdf(duongDan);
        }
    }
}

