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

Sau đó mở tiếp `Database/TV2_BoSungRangBuoc.sql` và **Execute** (thêm các CHECK về SĐT và thành tiền phiếu nhập; chạy nhiều lần không lỗi). Có thể chạy thêm `docs/TV4_KiemTraDuLieu.sql` để đối chiếu dữ liệu.

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

> **Cách gọn nhất:** đặt biến môi trường `QLDT_CONNECTION` bằng chuỗi kết nối của máy mình, chương trình sẽ ưu tiên dùng nó và không cần sửa mã nguồn.
>
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

Quy trình Git, tên nhánh, cách tạo Pull Request xem trong [CONTRIBUTING.md](CONTRIBUTING.md). Sơ đồ CSDL: [bản chỉnh sửa bằng draw.io](ERD_CuaHangDienThoai.drawio) và [bản xem PNG](ERD_CuaHangDienThoai.drawio.png).

## Ghi chú thiết kế (bổ sung)

- **Tiếng Việt / chuỗi kết nối:** CSDL là SQL Server, cột chữ dùng `NVARCHAR` nên không cần thêm `?useUnicode=yes&characterEncoding=UTF-8` (tham số này dành cho MySQL/Java).
- **Mật khẩu:** băm SHA-256 không salt, đủ cho đồ án học tập; hệ thống thật nên dùng bcrypt/PBKDF2 có salt.
- **Tính duy nhất:** `UNIQUE` ở CSDL cho tên hãng, tên loại, tên khuyến mãi, bộ (TenDT, RAM, BoNho, MauSac); BUS kiểm tra trước để báo lỗi dễ hiểu.
- **Hóa đơn:** khi lưu, BUS tính lại giá theo giá niêm yết và khuyến mãi hiện hành; nếu đã đổi thì yêu cầu thêm lại dòng.
- **Nhập hàng:** nhân viên lập phiếu là người đang đăng nhập.
- **Chạy lại script SQL** (`Database/QuanLyBanDienThoai.sql`) để áp dụng các ràng buộc UNIQUE mới.

### Cập nhật khóa ngoại cho database đã có dữ liệu

Dừng ứng dụng, mở và Execute `Database/CapNhat_KhoaNgoai_ERD.sql` trong SSMS, sau đó build và chạy mã nguồn v3. Script cài khóa ngoại và trigger trong một transaction, tính lại tổng chứng từ từ các dòng chi tiết và hoàn tác toàn bộ nếu dữ liệu chi tiết cũ vi phạm quy tắc. Với database thử nghiệm mới, chạy `Database/QuanLyBanDienThoai.sql` để tạo đủ schema, trigger và dữ liệu mẫu. Script tạo lại có lệnh DROP TABLE.

### Kiểm thử và giải thích khi vấn đáp

- Chạy `Database/KiemThu_RangBuoc_ERD.sql` trên database demo và xem tab Messages. Bốn ca thử dữ liệu sai đều dùng transaction và hoàn tác.
- Hóa đơn và phiếu nhập bắt đầu có tổng bằng 0. Trigger tính lại tổng từ các dòng chi tiết trong cùng transaction lưu chứng từ. WinForms vẫn cập nhật tồn kho và điểm trong transaction của DAO.
- SQL kiểm tra ngày lập hóa đơn nằm trong thời hạn khuyến mãi, tính thành tiền theo đơn giá lưu trên dòng và mức giảm, đồng thời khóa mức giảm đã dùng để giữ lịch sử. Muốn đổi mức giảm đã dùng thì tạo chương trình mới.
- SQL kiểm tra số phiếu bảo hành theo cặp hóa đơn và điện thoại, kể cả thao tác nhiều dòng. Khi hai phiên ghi cạnh tranh, một phiên có thể phải thực hiện lại nếu SQL Server hủy giao dịch do deadlock.
- Các kiểm tra bảo đảm IMEI đúng định dạng và duy nhất. Việc xác nhận IMEI của đúng máy vật lý cần dữ liệu IMEI theo từng máy từ khâu nhập hoặc bán.
