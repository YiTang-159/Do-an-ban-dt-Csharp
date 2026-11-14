using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddRazorPages(options => options.Conventions.AuthorizeFolder("/"));
builder.Services.AddScoped<QuanLyBanDienThoai.BUS.HangDTBUS>();
builder.Services.AddScoped<QuanLyBanDienThoai.BUS.LoaiDienThoaiBUS>();
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Login";
        options.AccessDeniedPath = "/Denied";
        options.Cookie.HttpOnly = true;
        options.Cookie.SameSite = SameSiteMode.Lax;
        options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
        options.ExpireTimeSpan = TimeSpan.FromHours(4);
    });
builder.Services.AddAuthorization();
var app = builder.Build();
if (!app.Environment.IsDevelopment()) { app.UseExceptionHandler("/Error"); app.UseHsts(); }
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
// Kiểm tra lại trạng thái/vai trò ở DB cho mỗi request, tránh tài khoản đã bị khóa vẫn tiếp tục thao tác.
app.Use(async (ctx, next) =>
{
    if (ctx.User.Identity?.IsAuthenticated == true)
    {
        var username = ctx.User.Identity.Name;
        try
        {
            var account = new QuanLyBanDienThoai.DAO.TaiKhoanDAO().GetByTenDangNhap(username ?? "");
            if (account == null || !account.TrangThai || account.VaiTro != QuanLyBanDienThoai.DTO.VaiTroTK.Admin)
            {
                await ctx.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                ctx.Response.Redirect("/Login");
                return;
            }
        }
        catch { ctx.Response.StatusCode = 503; await ctx.Response.WriteAsync("Không kiểm tra được tài khoản trong SQL Server."); return; }
    }
    await next();
});
app.UseAuthorization();
app.MapRazorPages();
app.Run();
