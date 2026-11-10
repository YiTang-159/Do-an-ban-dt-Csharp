var builder = WebApplication.CreateBuilder(args);
builder.Services.AddRazorPages();
builder.Services.AddScoped<QuanLyBanDienThoai.BUS.HangDTBUS>();

var app = builder.Build();
app.UseStaticFiles();
app.MapRazorPages();
app.Run();
