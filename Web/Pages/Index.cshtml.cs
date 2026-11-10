using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using QuanLyBanDienThoai.BUS;
using QuanLyBanDienThoai.DTO;

namespace QuanLyBanDienThoai.Web.Pages;

public class IndexModel(HangDTBUS bus, ILogger<IndexModel> logger) : PageModel
{
    [BindProperty(SupportsGet = true)]
    public string? TuKhoa { get; set; }

    [BindProperty]
    public HangInput Nhap { get; set; } = new();

    [TempData]
    public string? ThongBao { get; set; }

    public IReadOnlyList<HangDT> DanhSach { get; private set; } = [];
    public string? LoiTai { get; private set; }

    public IActionResult OnGet(int? sua)
    {
        try
        {
            if (sua.HasValue)
            {
                var hang = bus.GetAll().Cast<HangDT>().FirstOrDefault(x => x.MaHang == sua.Value);
                if (hang is null)
                    return NotFound("Không tìm thấy hãng cần sửa.");

                Nhap = new HangInput
                {
                    MaHang = hang.MaHang,
                    TenHang = hang.TenHang,
                    QuocGia = hang.QuocGia
                };
            }
        }
        catch (Exception ex)
        {
            BaoLoiTai(ex);
            return Page();
        }

        return HienTrang();
    }

    public IActionResult OnPostLuu()
    {
        if (!ModelState.IsValid)
            return HienTrang();

        var hang = new HangDT
        {
            MaHang = Nhap.MaHang,
            TenHang = Nhap.TenHang,
            QuocGia = Nhap.QuocGia ?? ""
        };

        var loi = hang.MaHang == 0 ? bus.Them(hang) : bus.Sua(hang);
        return KetQua(loi, hang.MaHang == 0 ? "Đã thêm hãng điện thoại." : "Đã cập nhật hãng điện thoại.");
    }

    public IActionResult OnPostXoa(int maHang)
    {
        if (maHang <= 0)
            return BadRequest("Mã hãng không hợp lệ.");

        ModelState.Clear();
        return KetQua(bus.Xoa(maHang), "Đã xóa hãng điện thoại.");
    }

    private IActionResult KetQua(string loi, string thanhCong)
    {
        if (loi.Length == 0)
        {
            ThongBao = thanhCong;
            return RedirectToPage(new { TuKhoa });
        }

        ModelState.AddModelError(string.Empty, loi);
        return HienTrang();
    }

    private IActionResult HienTrang()
    {
        try
        {
            DanhSach = bus.TimKiem(TuKhoa ?? "").Cast<HangDT>().ToArray();
        }
        catch (Exception ex)
        {
            BaoLoiTai(ex);
        }

        return Page();
    }

    private void BaoLoiTai(Exception ex)
    {
        logger.LogError(ex, "Không thể tải danh mục hãng điện thoại");
        LoiTai = "Không tải được danh mục. Kiểm tra SQL Server, database QuanLyBanDienThoai và DAO/MyConnection.cs.";
    }

    public class HangInput
    {
        [Range(0, int.MaxValue)]
        public int MaHang { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tên hãng.")]
        [StringLength(100, ErrorMessage = "Tên hãng tối đa 100 ký tự.")]
        public string TenHang { get; set; } = "";

        [StringLength(50, ErrorMessage = "Quốc gia tối đa 50 ký tự.")]
        public string? QuocGia { get; set; }
    }
}
