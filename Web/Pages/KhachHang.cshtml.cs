using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using QuanLyBanDienThoai.BUS;
using QuanLyBanDienThoai.DTO;

namespace QuanLyBanDienThoai.Web.Pages;

[Authorize(Roles = "Admin")]
public class KhachHangModel(KhachHangBUS bus, ILogger<KhachHangModel> logger) : PageModel
{
    [BindProperty(SupportsGet = true)]
    public string? TuKhoa { get; set; }

    [BindProperty]
    public KhachHangInput Nhap { get; set; } = new();

    [TempData]
    public string? ThongBao { get; set; }

    public IReadOnlyList<KhachHang> DanhSach { get; private set; } = [];
    public string? LoiTai { get; private set; }

    public IActionResult OnGet(int? sua)
    {
        try
        {
            if (sua.HasValue)
            {
                var kh = bus.GetAll().Cast<KhachHang>().FirstOrDefault(x => x.MaKH == sua.Value);
                if (kh is null)
                    return NotFound("Không tìm thấy khách hàng cần sửa.");

                Nhap = new KhachHangInput
                {
                    MaKH = kh.MaKH,
                    Ho = kh.Ho,
                    Ten = kh.Ten,
                    SDT = kh.SDT,
                    Email = kh.Email,
                    DiaChi = kh.DiaChi,
                    NgaySinh = kh.NgaySinh,
                    DiemTichLuy = kh.DiemTichLuy
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

        var kh = new KhachHang
        {
            MaKH = Nhap.MaKH,
            Ho = Nhap.Ho.Trim(),
            Ten = Nhap.Ten.Trim(),
            SDT = Nhap.SDT.Trim(),
            Email = Nhap.Email.Trim(),
            DiaChi = Nhap.DiaChi.Trim(),
            NgaySinh = Nhap.NgaySinh
        };

        var loi = kh.MaKH == 0 ? bus.Them(kh) : bus.Sua(kh);
        return KetQua(loi, kh.MaKH == 0 ? "Đã thêm khách hàng thành công." : "Đã cập nhật thông tin khách hàng.");
    }

    public IActionResult OnPostXoa(int maKH)
    {
        if (maKH <= 0)
            return BadRequest("Mã khách hàng không hợp lệ.");

        ModelState.Clear();
        return KetQua(bus.Xoa(maKH), "Đã xóa khách hàng.");
    }

    private IActionResult KetQua(string loi, string thanhCong)
    {
        if (string.IsNullOrEmpty(loi))
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
            DanhSach = bus.TimKiem(TuKhoa ?? "").Cast<KhachHang>().ToArray();
        }
        catch (Exception ex)
        {
            BaoLoiTai(ex);
        }

        return Page();
    }

    private void BaoLoiTai(Exception ex)
    {
        logger.LogError(ex, "Không thể tải danh sách khách hàng");
        LoiTai = "Không tải được danh mục khách hàng. Kiểm tra kết nối SQL Server và database QuanLyBanDienThoai.";
    }

    public class KhachHangInput
    {
        [Range(0, int.MaxValue)]
        public int MaKH { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập họ và tên đệm.")]
        [StringLength(50, ErrorMessage = "Họ tối đa 50 ký tự.")]
        public string Ho { get; set; } = "";

        [Required(ErrorMessage = "Vui lòng nhập tên.")]
        [StringLength(50, ErrorMessage = "Tên tối đa 50 ký tự.")]
        public string Ten { get; set; } = "";

        [Required(ErrorMessage = "Vui lòng nhập số điện thoại.")]
        [RegularExpression(@"^0\d{9}$", ErrorMessage = "Số điện thoại phải gồm 10 chữ số và bắt đầu bằng số 0.")]
        public string SDT { get; set; } = "";

        [Required(ErrorMessage = "Vui lòng nhập email.")]
        [EmailAddress(ErrorMessage = "Email không đúng định dạng.")]
        [StringLength(100, ErrorMessage = "Email tối đa 100 ký tự.")]
        public string Email { get; set; } = "";

        [Required(ErrorMessage = "Vui lòng nhập địa chỉ.")]
        [StringLength(255, ErrorMessage = "Địa chỉ tối đa 255 ký tự.")]
        public string DiaChi { get; set; } = "";

        [DataType(DataType.Date)]
        public DateTime? NgaySinh { get; set; }

        public int DiemTichLuy { get; set; }
    }
}

