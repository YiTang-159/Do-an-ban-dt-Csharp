# Quy trình làm việc với Git

## Nhánh

- `main`: luôn build được, **không commit trực tiếp**. Chỉ gộp bằng Pull Request.
- Mỗi người một nhánh: `tv1-san-pham`, `tv2-kho-nhanvien`, `tv3-ban-hang`, `tv4-khuyenmai-baohanh`.

## Mỗi ngày

```
git checkout main
git pull origin main
git checkout <nhánh của mình>
git merge main
```

Làm việc, commit thường xuyên rồi đẩy lên:

```
git add <các file của mình>
git commit -m "feat(kho): thêm DTO, DAO, BUS, form Kho"
git push -u origin <nhánh của mình>
```

Làm xong một bảng thì tạo Pull Request vào `main`, không đợi làm hết cả cụm.

## Tin nhắn commit

- `feat(<bảng>): ...` thêm chức năng
- `fix(<bảng>): ...` sửa lỗi
- `docs: ...` sửa tài liệu

## Duyệt Pull Request

TV1 duyệt PR của mọi người, TV4 duyệt PR của TV1. Kiểm tra: build không lỗi, BUS và GUI không có SQL, tên lớp đúng quy ước, chạy thử form vừa làm, không có `bin/` hay `obj/`.

## Chỗ dễ xung đột

- `GUI/MainForm.cs`: chỉ bỏ dấu `//` ở đúng dòng của mình.
- `DAO/DataProvider.cs`: không commit dòng chuỗi kết nối riêng của máy mình (dùng `git add -p`).
- File SQL trong `Database/`: chỉ TV2 sửa. Cần đổi cột thì tạo Issue.

## Không được làm

- Không `git push --force`.
- Không commit trực tiếp lên `main`.
- Không commit `bin/`, `obj/`, `.vs/`, mật khẩu hay thông tin riêng.
