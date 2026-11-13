# Bản sửa logic (13/11/2026)

## Đã sửa
- ASP.NET Core Razor: đăng nhập cookie bằng **tài khoản Admin** đang lưu trong TAIKHOAN; không cho truy cập trang quản lý hãng hoặc POST thêm/sửa/xóa khi chưa có quyền. Có đăng xuất và kiểm tra trạng thái tài khoản trong SQL Server mỗi request.
- WinForms: chỉ Admin thấy Thống kê và Tham số; BUS chặn nhân viên sửa/xóa tham số trực tiếp.
- Hóa đơn: kiểm tra chi tiết trùng mã, số lượng âm, phương thức thanh toán ngoài danh sách; dùng transaction như cũ. Khi lập từ giỏ, chỉ xóa dòng nếu số lượng còn khớp; giỏ bị thay đổi sẽ rollback hóa đơn/tồn kho/điểm và yêu cầu nạp lại.

## Giới hạn còn lại
- Chưa có quy trình đặt hàng online riêng (đơn hàng, xác nhận, giao hàng, hủy) hay đối soát thanh toán ngân hàng/thẻ; ứng dụng hiện vận hành theo quy trình **nhân viên lập hóa đơn tại quầy**. Không mặc định thanh toán chuyển khoản/thẻ thành công chỉ vì chọn phương thức; nhân viên phải xác nhận ngoài hệ thống trước khi lập.
- Nếu nhân viên sửa thủ công dòng hóa đơn sau khi nạp giỏ, nên nạp lại giỏ trước khi lập. Bản này chủ động từ chối khi giỏ/số lượng không khớp để tránh sai lệch dữ liệu.
- Chưa build, chạy WinForms/Web hoặc test SQL Server thực tế trong môi trường này. Test theo README và checklist bên dưới trước khi demo.

## Test nhanh
1. Web: chạy `dotnet run --project Web/QuanLyBanDienThoai.Web.csproj` (máy cần .NET 10 SDK), truy cập trang chủ chưa login phải chuyển sang /Login.
2. Đăng nhập tài khoản khách/nhân viên: từ chối; Admin: vào danh mục và thêm/sửa/xóa hãng được. Đăng xuất, POST sửa/xóa trực tiếp phải bị chặn.
3. WinForms: đăng nhập nhân viên không thấy Thống kê, Tham số; Admin vẫn thấy. Thử sửa tham số từ BUS khi không phải Admin phải bị từ chối.
4. Giỏ: khách có 1 máy; nhân viên nạp giỏ, khách đổi số lượng thành 2; nhân viên lập hóa đơn phải báo lỗi, không trừ kho/điểm, không xóa giỏ.
5. Giỏ không đổi: lập hóa đơn một lần thành công, trừ tồn kho, cộng điểm, xóa đúng dòng giỏ. Nhập hóa đơn thiếu hàng phải rollback.
6. Chuyển khoản/thẻ: chỉ bấm lập hóa đơn sau khi nhân viên xác minh đã nhận tiền; hiện chưa tích hợp cổng xác thực thanh toán.

**Không chạy lại `Database/QuanLyBanDienThoai.sql` trên dữ liệu thật**: script tạo mới có lệnh DROP TABLE. Bản sửa này không yêu cầu thay đổi schema.
