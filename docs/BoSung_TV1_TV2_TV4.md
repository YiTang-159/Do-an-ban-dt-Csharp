# Bổ sung TV1 TV2 TV4

## Phạm vi
Bổ sung code TV1, TV2 và xử lý một số tình huống thống kê TV4. Giữ nguyên toàn bộ file bán hàng, hóa đơn, giỏ hàng thuộc TV3. Không thay chuỗi kết nối hoặc SQL của nhóm.

## Chạy chương trình
1. Giải nén ZIP ra thư mục mới.
2. Mở terminal tại thư mục chứa QuanLyBanDienThoai.csproj trên Windows có .NET SDK 8.
3. Chạy `dotnet restore`, sau đó `dotnet build`, rồi `dotnet run`.
4. Dùng CSDL hiện có và cấu hình kết nối của nhóm. Không cần chạy lại script tạo CSDL để dùng các thay đổi này.

## TV1
ExcelHelper vẫn giữ các hàm gọi cũ. Đuôi .xls được đọc/ghi bằng NPOI HSSF, đuôi .xlsx dùng ClosedXML. Không đổi đuôi file để giả lập định dạng. Đã thêm NPOI 2.7.2 vào project, cần restore NuGet. XLS giới hạn 65535 dòng dữ liệu và một hàng tiêu đề; dữ liệu lớn hơn cần chọn XLSX.

## TV2
- Nhân viên và khách hàng: thêm Nhập Excel, Xuất Excel và Tìm nâng cao.
- Tìm nâng cao gọi TimKiemHelper.Loc trên ArrayList trong BUS; hỗ trợ điều kiện từ FormTimNangCao.
- Import thêm mới, không ghi đè bản ghi hiện có. Mã đã có hoặc trùng trong file bị từ chối; mã của dòng hợp lệ do SQL Server sinh. Các dòng tiếp tục được kiểm tra qua BUS, gồm SDT/email, tuổi hoặc ngày sinh. Dòng hợp lệ được lưu; dòng lỗi có thông báo số dòng, không rollback toàn file.
- File xuất từ dữ liệu hiện có sẽ chứa mã cũ: nhập nguyên file sẽ báo trùng. Để thêm bản ghi mới, bỏ mã hoặc đặt 0 và đổi thông tin duy nhất như SDT/email.
- Nhân viên: giới tính và chức vụ chuyển sang ComboBox; chức vụ có sẵn trong CSDL được bổ sung vào danh sách chọn.
- Nhập hàng: thêm ba nút … chọn nhà cung cấp, nhân viên, điện thoại qua FormChon.KetQua; thêm Làm mới để bỏ các dòng đang lập.
- Danh sách phiếu nhập: thêm tổng tiền nhập theo danh sách hiển thị.

## TV4
- Đổi khoảng ngày làm mất hiệu lực dữ liệu PDF cũ: nhãn và 3 bảng được xóa, phải bấm Thống kê lại trước khi xem hoặc xuất.
- Khoảng ngày trải nhiều năm vẫn thống kê được: mỗi đối tượng được tách thành từng dòng theo năm (ví dụ "2 - Phạm Thị Dung [2026]"), nên Q1–Q4 của các năm khác nhau không bị cộng gộp.
- Các chức năng khuyến mãi, chi tiết khuyến mãi, bảo hành và PDF thống kê giữ nguyên.

## Kiểm thử cần thực hiện trên Windows SQL Server
Tất cả mục dưới đây CHƯA được chạy thực tế trong môi trường phát triển bản bổ sung. Điền kết quả sau khi nhóm chạy.

| Mã | Thao tác | Mong đợi | Thực tế |
|---|---|---|---|
| BS01 | Restore và build | Không lỗi biên dịch, tải được NPOI | Chưa chạy |
| BS02 | Xuất nhân viên/khách ra XLS và XLSX | Excel mở đúng, tiếng Việt/ngày/số không lỗi | Chưa chạy |
| BS03 | Nhập dòng mới, mã 0, SDT/email mới | Lưu và hiển thị lại | Chưa chạy |
| BS04 | Nhập mã cũ, SDT/email trùng và mã lặp trong file | Báo số dòng lỗi; không thêm bản ghi trùng | Chưa chạy |
| BS05 | Nhập ngày/số sai kiểu | Báo dòng và tên cột, không crash | Chưa chạy |
| BS06 | Nhân viên chưa đủ tuổi hoặc ngày vào làm trước ngày sinh | BUS từ chối | Chưa chạy |
| BS07 | Tìm nâng cao AND OR NOT, ngày, số, chứa | Kết quả đúng trên ArrayList | Chưa chạy |
| BS08 | Bấm … tại ba ô nhập hàng rồi chọn/hủy | Chọn điền đúng; hủy giữ dữ liệu cũ | Chưa chạy |
| BS09 | Thêm dòng nhập rồi Làm mới | Xóa phiếu tạm, không ghi SQL | Chưa chạy |
| BS10 | Lập phiếu nhập, mở danh sách | Tồn kho tăng; tổng tiền nhãn bằng tổng các phiếu | Chưa chạy |
| BS11 | Thống kê rồi thay ngày, bấm PDF | Yêu cầu thống kê lại | Chưa chạy |
| BS12 | Chọn khoảng qua hai năm | Mỗi đối tượng tách dòng theo năm, TC cột đúng | Chưa chạy |
| BS13 | Xem thống kê, xuất PDF | Tổng thu/chi, ba bảng, dấu tiếng Việt đúng | Chưa chạy |
| BS14 | Chạy lại các ca khuyến mãi/bảo hành trong KiemThu.docx | Không lỗi hồi quy | Chưa chạy |
| BS15 | Chạy bán hàng/giỏ hàng hiện có | Không lỗi do các hàm dùng chung thay đổi | Chưa chạy |

## Nội dung chuyển vào báo cáo cuối kỳ
TV1: mô tả định dạng XLS/XLSX, thư viện NPOI/ClosedXML, luồng form → BUS → DAO và điều kiện tìm nâng cao.
TV2: thêm ảnh ba nút chọn nhập hàng, bộ lọc nhân viên/khách hàng, nhập/xuất Excel, tổng tiền nhập; thuyết minh đầu vào là danh sách DTO, đầu ra là số dòng thành công và lỗi theo dòng, kiểm tra tại BUS trước khi ghi DAO.
TV4: ghi cách chọn cùng một năm cho bảng Q1–Q4, cơ chế yêu cầu thống kê lại khi đổi ngày; chạy các ca và điền kết quả thật vào KiemThu.docx. Bổ sung ảnh ứng dụng, thông tin nhóm và link sản phẩm vào BaoCao.docx trước khi nộp. File Word gốc chưa được hoàn tất vì còn thiếu thông tin và ảnh thực tế của nhóm.

## Giới hạn xác minh
Đã kiểm tra cấu trúc mã, tham chiếu nội bộ và phạm vi thay đổi. Môi trường hiện tại không có dotnet/Windows/SQL Server nên chưa build, chạy GUI, thử Excel/PDF hay xác nhận giao dịch SQL. Không đánh dấu các ca là Đạt khi chưa chạy.
