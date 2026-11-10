# Chạy trang quản lý hãng điện thoại

Trang web dùng ASP.NET Core Razor Pages trên .NET 10. Mở PowerShell ngay tại thư mục có `QuanLyBanDienThoai.csproj` và thư mục `Web`:

```powershell
dotnet run --project .\Web\QuanLyBanDienThoai.Web.csproj -- --urls http://localhost:5080
```

Mở http://localhost:5080. Cửa sổ PowerShell cần được giữ mở khi dùng trang.

SQL Server dùng chung với ứng dụng WinForms. Cấu hình mặc định nằm trong `DAO/MyConnection.cs` ở thư mục gốc. Nếu container `qlbandt-sql` đang chạy trên cổng 14333 và database `QuanLyBanDienThoai` đã có, dùng ngay database đó. Với môi trường mới, bật SQL bằng `docker compose up -d` ở thư mục gốc, kết nối SSMS đến `localhost,14333` và chạy `Database/QuanLyBanDienThoai.sql`. Script tạo lại các bảng, vì vậy chỉ chạy khi tạo database mới hoặc chủ động đặt lại dữ liệu thử nghiệm.

## Kiểm tra

1. Thêm hãng `Hãng Test Razor` và quốc gia `Việt Nam`.
2. Tìm theo từ khóa `Razor`, chọn **Sửa**, đổi tên thành `Hãng Test Razor 01` và lưu.
3. Mở lại danh mục hãng trong WinForms hoặc chạy `SELECT MaHang, TenHang, QuocGia FROM dbo.HANGDT;` trong database `QuanLyBanDienThoai` để thấy dữ liệu vừa lưu.
4. Xóa hãng thử. Thử xóa một hãng đang có điện thoại và kiểm tra thông báo từ BUS.

## Cấu trúc

`Pages/Index.cshtml` dùng `@page`, `@model` và `@foreach` để tạo giao diện Razor. `Index.cshtml.cs` nhận GET/POST, gọi `HangDTBUS`. BUS dùng `ArrayList` để lọc dữ liệu và gọi DAO. DAO gửi lệnh SQL qua `MyConnection`. Project web liên kết các file BUS, DAO và DTO gốc để hai ứng dụng dùng cùng mã nguồn và cùng dữ liệu.

Trang demo chạy trên localhost và mở trực tiếp màn hình quản lý. Các thao tác trên trang tác động lên bảng HANGDT của database đang kết nối.
