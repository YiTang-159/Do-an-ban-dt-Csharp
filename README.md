# Quản lý cửa hàng bán điện thoại

Đồ án học phần Lập trình C#: ứng dụng C# Windows Forms (.NET 8) + SQL Server, mô hình 3 lớp GUI - BUS - DAO (kèm DTO), quản lý mã nguồn bằng Git.

## Cấu trúc thư mục

| Thư mục | Nội dung |
|---|---|
| `DTO/` | Lớp thực thể, mỗi bảng một lớp |
| `DAO/` | Truy cập CSDL (chứa câu SQL), dùng chung lớp `MyConnection` |
| `BUS/` | Kiểm tra dữ liệu, nghiệp vụ, tìm kiếm trên ArrayList |
| `GUI/` | Các form Windows Forms: `FormDangNhap` (vào đầu tiên), `MainForm` (admin, nhân viên), `FormTrangKhachHang` (khách hàng) |
| `Database/` | Script SQL Server tạo CSDL `QuanLyBanDienThoai` |
| `docker-compose.yml` | Chạy SQL Server bằng Docker |

## Yêu cầu cài đặt

- Visual Studio 2022 trở lên, có workload **.NET desktop development**, .NET 8 SDK
- Docker Desktop
- Azure Data Studio hoặc SQL Server Management Studio (SSMS)

## Cài đặt và chạy

### Bước 1. Bật SQL Server bằng Docker

Mở Docker Desktop, đợi đến khi báo "Engine running". Trong thư mục project chạy:

```
docker compose up -d
```

Đợi khoảng 30 giây để SQL Server khởi động, kiểm tra bằng `docker ps` (thấy container `qlbandt-sql` trạng thái `Up`).

Thông tin kết nối của container:

| Mục | Giá trị |
|---|---|
| Server | `localhost,14333` (chú ý dấu phẩy, cổng **14333** chứ không phải 1433) |
| Tài khoản | `sa` |
| Mật khẩu | `Qldt@12345` |

### Bước 2. Tạo cơ sở dữ liệu

Dùng Azure Data Studio hoặc SSMS kết nối vào server ở bước 1 (SQL Server Authentication, tick **Trust Server Certificate**), mở file `Database/QuanLyBanDienThoai.sql` rồi bấm **Execute**. Script tạo database `QuanLyBanDienThoai` cùng 16 bảng (14 bảng ERD, `THAMSO`, `TAIKHOAN`) và dữ liệu mẫu, có thể chạy lại nhiều lần (chạy lại sẽ xóa dữ liệu cũ).

Nếu máy có sẵn `sqlcmd`, có thể chạy bằng dòng lệnh:

```
sqlcmd -S localhost,14333 -U sa -P "Qldt@12345" -C -i Database\QuanLyBanDienThoai.sql
```

### Bước 3. Cấu hình chuỗi kết nối

Mở `DAO/MyConnection.cs`, sửa hằng `ChuoiKetNoi` cho khớp với server ở bước 1. Có hai cách:

**Cách A: dùng tài khoản `sa` (đơn giản nhất)**

```csharp
@"Data Source=localhost,14333;Initial Catalog=QuanLyBanDienThoai;User Id=sa;Password=Qldt@12345;TrustServerCertificate=True"
```

**Cách B: giữ tài khoản `qldt` / `12345`**

Tài khoản `qldt` chưa có sẵn trong container, cần tạo một lần: kết nối bằng `sa`, mở New Query và chạy:

```sql
CREATE LOGIN qldt WITH PASSWORD = '12345', CHECK_POLICY = OFF;
USE QuanLyBanDienThoai;
CREATE USER qldt FOR LOGIN qldt;
ALTER ROLE db_owner ADD MEMBER qldt;
```

Sau đó chỉ cần đổi `Data Source=localhost` thành `Data Source=localhost,14333` trong chuỗi kết nối.

> **Lưu ý:** chuỗi kết nối là cấu hình riêng của mỗi máy. Không commit dòng này lên Git (dùng `git add -p` và bỏ qua dòng đó), xem thêm [CONTRIBUTING.md](CONTRIBUTING.md).

### Bước 4. Chạy chương trình

Mở file `QuanLyBanDienThoai.csproj` bằng Visual Studio (**File → Open → Project/Solution**, không dùng "Open Folder"), chọn project `QuanLyBanDienThoai` làm Startup Project rồi bấm **F5**. Hoặc chạy trong terminal:

```
dotnet run
```

## Đăng nhập và phân quyền

Chương trình mở ra màn hình **Đăng nhập**. Chọn đúng vai trò rồi nhập tài khoản:

| Vai trò | Tài khoản mẫu | Mật khẩu | Sau khi đăng nhập |
|---|---|---|---|
| Quản trị viên (Admin) | `admin` (gắn với nhân viên NV1) | `admin123` | `MainForm` đầy đủ menu, thêm mục **Tài khoản** |
| Nhân viên | `dung`, `duc` | `nv123456` | `MainForm` chỉ có nhóm **Bán hàng** (lập hóa đơn, danh sách hóa đơn) |
| Khách hàng | `an`, `binh`, `nam` | `kh123456` | `FormTrangKhachHang`: cửa hàng, giỏ hàng, đơn mua, bảo hành, thông tin cá nhân |

Khách hàng mới bấm **Đăng ký khách hàng** ngay trên màn hình đăng nhập.

**Dữ liệu thống nhất với ERD:** bảng `TAIKHOAN` (ngoài ERD, giống `THAMSO`) chỉ lưu tên đăng nhập, mã băm SHA-256 của mật khẩu, vai trò và khóa ngoại tới `NHANVIEN` (Admin, Nhân viên) hoặc `KHACHHANG` (Khách hàng). Vì vậy:

- Khách hàng chỉ thấy giỏ hàng, hóa đơn, bảo hành của đúng `MaKH` của mình (mã lấy từ phiên đăng nhập, không do form truyền vào).
- Nhân viên lập hóa đơn thì `HOADON.MaNV` chính là tài khoản đang đăng nhập; hóa đơn đó hiện ngay ở trang "Đơn mua" của khách.
- Đăng ký khách hàng thêm `KHACHHANG` và `TAIKHOAN` trong một transaction.
- Khách đặt hàng vào `GIOHANG`; nhân viên bấm **Lấy từ giỏ hàng** ở form bán hàng để lập hóa đơn (trừ tồn kho, cộng điểm tích lũy, xóa dòng giỏ đã mua).

## Gặp lỗi thường gặp

| Lỗi | Nguyên nhân và cách xử lý |
|---|---|
| `Login failed for user 'qldt'` | Chuỗi kết nối đang trỏ cổng 1433 (SQL Server cài trên máy) hoặc chưa tạo user `qldt`. Đổi thành `localhost,14333` và làm theo Bước 3. |
| `Login failed for user 'sa'` | Sai mật khẩu, hoặc đang kết nối nhầm cổng. Mật khẩu của container là `Qldt@12345`. |
| `Cannot open database "QuanLyBanDienThoai"` | Chưa chạy script ở Bước 2. |
| `A network-related or instance-specific error` | Container chưa chạy hoặc chưa khởi động xong. Chạy `docker ps`, nếu chưa có thì `docker compose up -d` và đợi thêm. |
| `port is already allocated` | Cổng 14333 đã bị chiếm. Đổi số cổng bên trái trong `docker-compose.yml` và sửa cùng số đó trong chuỗi kết nối. |
| Build failed nhưng không thấy lỗi | Đang mở bằng "Open Folder". Mở lại bằng file `.csproj`, hoặc chạy `dotnet build` để xem lỗi chi tiết. |

Dừng hoặc xóa dữ liệu container:

```
docker compose stop        # tạm dừng, giữ dữ liệu
docker compose down -v     # xóa container và toàn bộ dữ liệu
```

## Làm việc nhóm

Quy trình Git, tên nhánh, cách tạo Pull Request xem trong [CONTRIBUTING.md](CONTRIBUTING.md). Sơ đồ CSDL: `ERD_CuaHangDienThoai.drawio.png`.
