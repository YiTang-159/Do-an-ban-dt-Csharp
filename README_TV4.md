# TV4 – Hướng dẫn chạy và kiểm tra

## Nội dung đã bổ sung

- DTO, DAO, BUS, GUI cho **KHUYENMAI** và **CHITIETKHUYENMAI** (CRUD, tìm kiếm thường/nâng cao, Excel; FormKhuyenMai dạng main–sub).
- Hoàn thiện **BAOHANH** với CRUD, IMEI 15 số, kiểm tra điện thoại thuộc hóa đơn, ngày hết hạn tính từ THAMSO; **giữ nguyên GetCuaToi / GetTheoKhachHang** để trang khách hàng tiếp tục dùng được.
- **Thống kê**: tổng thu, tổng chi, lợi nhuận theo công thức thu trừ chi, 3 bảng chéo Q1–Q4 và TC, khoảng ngày/tháng/quý/năm; PDF bằng QuestPDF.
- Đã bật 4 mục TV4 trong `GUI/MainForm.cs`.
- `docs/KiemThu.docx` (28 ca kiểm thử chưa chạy), `docs/BaoCao.docx` (báo cáo có chỗ điền nhóm/lớp), `docs/TV4_KiemTraDuLieu.sql` (đối chiếu SQL), `docs/SoDo3Lop.png`.

## Chạy trên Windows

1. Giải nén; dùng **Visual Studio 2022** (có workload `.NET desktop development`) hoặc Terminal VS Code/PowerShell với **.NET SDK 8**.
2. Mở Docker Desktop, SQL Server hoặc môi trường SQL Server của nhóm. Cấu hình nguồn tại `DAO/MyConnection.cs` đang **giữ nguyên** `localhost,14333`. Không chỉnh chuỗi kết nối nếu đang dùng cấu hình nhóm.
3. `Database/QuanLyBanDienThoai.sql` chứa toàn bộ schema và dữ liệu mẫu, **có DROP TABLE**: chỉ chạy trên database thử nghiệm; KHÔNG chạy lên database đang có dữ liệu cần giữ.
4. Trong thư mục project:

   ```powershell
   dotnet restore
   dotnet build
   dotnet run
   ```

5. Đăng nhập nhân viên/quản trị, vào **Khuyến mãi – Bảo hành** (3 mục) và **Thống kê** (1 mục).
6. Thử thao tác theo `docs/KiemThu.docx`. Đối chiếu trực tiếp dữ liệu bằng `docs/TV4_KiemTraDuLieu.sql` trên SSMS; trong thống kê chọn từ ngày `01/01/2026` đến `31/12/2026` để bao gồm dữ liệu mẫu năm 2026.
7. Thử **Xuất PDF**; kiểm tra chữ tiếng Việt, số liệu và 3 bảng chéo. Nếu thư viện NuGet tải thất bại, xác nhận internet/NuGet trước khi build.

## Giải thích giới hạn và phụ thuộc

- Phần thống kê lấy tổng tiền hóa đơn và tổng tiền nhập: `Lợi nhuận = Tổng thu - Tổng chi` là chỉ tiêu đơn giản theo yêu cầu phân công, **không phải lợi nhuận ròng**.
- Cột Q1–Q4 cộng các quý theo tháng của dữ liệu lọc. Nếu lọc nhiều năm, số liệu cùng quý được gộp.
- `QuestPDF` được khai báo `2024.12.1` trong `.csproj`. Cần khôi phục NuGet và kiểm tra giấy phép phù hợp khi triển khai thực tế.
- Khuyến mãi đang hoạt động sẽ được TV3 dùng qua `KhuyenMaiApDungDAO` hiện có; các form TV4 sử dụng BUS của TV1, TV3 để chọn điện thoại và hóa đơn.
- Việc kiểm thử WinForms, xuất PDF thực tế, truy cập SQL Server và ghép Pull Request **chưa được thực hiện ở môi trường này**; cần nhóm chạy và ghi kết quả trong tài liệu.

## Danh sách file TV4

- `DTO/KhuyenMai.cs`, `DTO/ChiTietKhuyenMai.cs`, `DTO/BaoHanh.cs` (giữ), `DTO/DongThongKe.cs`, `DTO/KetQuaThongKe.cs`
- `DAO/KhuyenMaiDAO.cs`, `DAO/ChiTietKhuyenMaiDAO.cs`, `DAO/BaoHanhDAO.cs` (mở rộng), `DAO/ThongKeDAO.cs`
- `BUS/KhuyenMaiBUS.cs`, `BUS/ChiTietKhuyenMaiBUS.cs`, `BUS/BaoHanhBUS.cs` (mở rộng), `BUS/ThongKeBUS.cs`
- `GUI/FormKhuyenMai.cs`, `GUI/FormChiTietKhuyenMai.cs`, `GUI/FormBaoHanh.cs`, `GUI/FormThongKe.cs`, `GUI/BaoCaoPdf.cs`, `GUI/Tv4FormHelper.cs`
- `GUI/MainForm.cs` (bật menu TV4), `QuanLyBanDienThoai.csproj` (QuestPDF)
