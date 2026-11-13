using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using QuanLyBanDienThoai.BUS;
using QuanLyBanDienThoai.DAO;
using QuanLyBanDienThoai.DTO;
namespace QuanLyBanDienThoai.Web.Pages;
[AllowAnonymous]
public class LoginModel : PageModel
{
    [BindProperty, Required] public string TenDangNhap { get; set; } = "";
    [BindProperty, Required] public string MatKhau { get; set; } = "";
    public string? Loi { get; private set; }
    public IActionResult OnGet() => User.Identity?.IsAuthenticated == true ? RedirectToPage("/Index") : Page();
    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid) return Page();
        try
        {
            var tk = new TaiKhoanDAO().GetByTenDangNhap(TenDangNhap.Trim());
            if (tk == null || !tk.TrangThai || tk.VaiTro != VaiTroTK.Admin ||
                !System.Security.Cryptography.CryptographicOperations.FixedTimeEquals(
                    System.Text.Encoding.UTF8.GetBytes(tk.MatKhau ?? ""),
                    System.Text.Encoding.UTF8.GetBytes(MatKhauHelper.Bam(MatKhau))))
            { Loi = "Thông tin đăng nhập không đúng hoặc tài khoản không có quyền Admin."; return Page(); }
            var claims = new[] { new Claim(ClaimTypes.Name, tk.TenDangNhap), new Claim(ClaimTypes.Role, VaiTroTK.Admin) };
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme)));
            return RedirectToPage("/Index");
        }
        catch { Loi = "Không kết nối được SQL Server để xác thực tài khoản."; return Page(); }
    }
}
