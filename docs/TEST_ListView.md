# Kiểm tra main-sub ListView
Đã đổi FormHoaDon, FormPhieuNhap, FormKhuyenMai sang hai ListView dạng Details. Chọn dòng cha sẽ lấy mã và nạp chi tiết; bỏ chọn, tìm không thấy hoặc làm mới sẽ xóa chi tiết cũ. DTO/DataRow được lưu trong ListViewItem.Tag. Không thay DAO, BUS, kết nối và dữ liệu SQL.

Cập nhật vào project hiện có: chép 4 file GUI/ListViewHelper.cs, GUI/FormHoaDon.cs, GUI/FormPhieuNhap.cs và GUI/FormKhuyenMai.cs. SDK project tự nhận file .cs mới. Chạy dotnet restore, dotnet build, dotnet run trên Windows.

1. Hóa đơn: chọn hóa đơn thứ nhất rồi thứ hai; đối chiếu MaHD và chi tiết trên SQL. Thử tìm không có kết quả, ô Chỉ hóa đơn tôi lập và Hiện tất cả.
2. Phiếu nhập: chọn từng phiếu; chi tiết đúng MaPN; Làm mới không giữ chi tiết phiếu trước. Tổng tiền nhập vẫn đúng.
3. Khuyến mãi: chọn từng mã; chi tiết đúng MaKM; ô nhập tên/ngày/mô tả được điền; thử sửa, tìm nâng cao và tìm rỗng.
4. Danh sách không có dữ liệu phải hiển thị tiêu đề cột và không phát sinh lỗi.

Chưa build hoặc chạy GUI/SQL trong môi trường này do không có dotnet và Windows. Các ca trên cần được chạy thực tế trước khi chốt Đạt.
