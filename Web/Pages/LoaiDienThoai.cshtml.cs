using Microsoft.AspNetCore.Authorization;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using QuanLyBanDienThoai.BUS;
using QuanLyBanDienThoai.DTO;

namespace QuanLyBanDienThoai.Web.Pages;

// GUI (Razor Page) -> BUS -> DAO. Trang này không chứa câu SQL nào.
[Authorize(Roles = "Admin")]
public class LoaiDienThoaiModel(LoaiDienThoaiBUS bus, ILogger<LoaiDienThoaiModel> logger) : PageModel
{
    [BindProperty(SupportsGet = true)]
    public string? TuKhoa { get; set; }

    [BindProperty]
    public LoaiInput Nhap { get; set; } = new();

    [TempData]
    public string? ThongBao { get; set; }

    public IReadOnlyList<LoaiDienThoai> DanhSach { get; private set; } = [];
    public string? LoiTai { get; private set; }

    public IActionResult OnGet(int? sua)
    {
        try
        {
            if (sua.HasValue)
            {
                var loai = bus.GetAll().Cast<LoaiDienThoai>().FirstOrDefault(x => x.MaLoai == sua.Value);
                if (loai is null)
                    return NotFound("Không tìm thấy loại cần sửa.");

                Nhap = new LoaiInput
                {
                    MaLoai = loai.MaLoai,
                    TenLoai = loai.TenLoai,
                    MoTa = loai.MoTa
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

        var loai = new LoaiDienThoai
        {
            MaLoai = Nhap.MaLoai,
            TenLoai = Nhap.TenLoai,
            MoTa = Nhap.MoTa ?? ""
        };

        var loi = loai.MaLoai == 0 ? bus.Them(loai) : bus.Sua(loai);
        return KetQua(loi, loai.MaLoai == 0 ? "Đã thêm loại điện thoại." : "Đã cập nhật loại điện thoại.");
    }

    public IActionResult OnPostXoa(int maLoai)
    {
        if (maLoai <= 0)
            return BadRequest("Mã loại không hợp lệ.");

        ModelState.Clear();
        return KetQua(bus.Xoa(maLoai), "Đã xóa loại điện thoại.");
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
            DanhSach = bus.TimKiem(TuKhoa ?? "").Cast<LoaiDienThoai>().ToArray();
        }
        catch (Exception ex)
        {
            BaoLoiTai(ex);
        }

        return Page();
    }

    private void BaoLoiTai(Exception ex)
    {
        logger.LogError(ex, "Không thể tải danh mục loại điện thoại");
        LoiTai = "Không tải được danh mục. Kiểm tra SQL Server, database QuanLyBanDienThoai và DAO/MyConnection.cs.";
    }

    public class LoaiInput
    {
        [Range(0, int.MaxValue)]
        public int MaLoai { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tên loại.")]
        [StringLength(100, ErrorMessage = "Tên loại tối đa 100 ký tự.")]
        public string TenLoai { get; set; } = "";

        [StringLength(255, ErrorMessage = "Mô tả tối đa 255 ký tự.")]
        public string? MoTa { get; set; }
    }
}
