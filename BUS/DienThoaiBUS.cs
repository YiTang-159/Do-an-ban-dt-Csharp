using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using QuanLyBanDienThoai.DAO;
using QuanLyBanDienThoai.DTO;

namespace QuanLyBanDienThoai.BUS
{
    public class DienThoaiBUS
    {
        private DienThoaiDAO dao = new DienThoaiDAO();

        // TV2, TV3, TV4 dùng: MaDT, TenDT, GiaNiemYet, SoLuongTon, TenHienThi
        public ArrayList GetAll()
        {
            return dao.GetAll();
        }

        public ArrayList TimKiem(string tuKhoa)
        {
            return TimKiemHelper.TimTuKhoa(dao.GetAll(), tuKhoa,
                "MaDT", "TenDT", "HeDieuHanh", "NamRaMat", "RAM", "BoNho", "MauSac", "GiaNiemYet", "TenHang", "TenLoai");
        }

        public ArrayList TimNangCao(List<DieuKien> dieuKien)
        {
            return TimKiemHelper.Loc(dao.GetAll(), dieuKien);
        }

        private string KiemTra(DienThoai x)
        {
            if (x == null) return "Dữ liệu không hợp lệ";
            x.TenDT = (x.TenDT ?? "").Trim();
            x.HeDieuHanh = (x.HeDieuHanh ?? "").Trim();
            x.MauSac = (x.MauSac ?? "").Trim();
            x.MoTa = (x.MoTa ?? "").Trim();

            if (x.TenDT == "") return "Tên điện thoại không được để trống";
            if (x.MauSac == "") return "Màu sắc không được để trống";
            if (x.MaHang <= 0) return "Hãy chọn hãng điện thoại";
            if (x.MaLoai <= 0) return "Hãy chọn loại điện thoại";

            int namToiThieu = (int)ThamSoBUS.LayGiaTri("NamRaMatToiThieu");
            int namHienTai = DateTime.Now.Year;
            if (x.NamRaMat < namToiThieu || x.NamRaMat > namHienTai)
                return "Năm ra mắt phải từ " + namToiThieu + " đến " + namHienTai;

            if (x.RAM <= 0) return "RAM phải lớn hơn 0";
            if (x.BoNho <= 0) return "Bộ nhớ phải lớn hơn 0";
            if (x.GiaNiemYet <= 0) return "Giá niêm yết phải lớn hơn 0";
            if (x.SoLuongTon < 0) return "Số lượng tồn không được âm";
            return "";
        }

        // SoLuongTon của điện thoại mới luôn bằng 0 (DAO tự đặt)
        public string Them(DienThoai x)
        {
            string loi = KiemTra(x);
            if (loi != "") return loi;
            try
            {
                dao.Insert(x);
                return "";
            }
            catch (Exception ex)
            {
                return "Không thể thêm: " + ex.Message;
            }
        }

        // Sửa không làm thay đổi SoLuongTon (DAO không cập nhật cột này)
        public string Sua(DienThoai x)
        {
            string loi = KiemTra(x);
            if (loi != "") return loi;
            try
            {
                if (dao.Update(x) == 0) return "Không tìm thấy điện thoại cần sửa";
                return "";
            }
            catch (Exception ex)
            {
                return "Không thể sửa: " + ex.Message;
            }
        }

        public string Xoa(int maDT)
        {
            try
            {
                if (dao.DemBangDangDung(maDT) > 0)
                    return "Không thể xóa: điện thoại đã được dùng ở hóa đơn, phiếu nhập, khuyến mãi, giỏ hàng hoặc bảo hành";
                if (dao.Delete(maDT) == 0) return "Không tìm thấy điện thoại cần xóa";
                return "";
            }
            catch (Exception ex)
            {
                return "Không thể xóa: " + ex.Message;
            }
        }

        public string NhapDanhSach(ArrayList ds, out int soThanhCong)
        {
            soThanhCong = 0;
            StringBuilder loiChung = new StringBuilder();
            int soLoi = 0;
            HashSet<int> maDaCo = new HashSet<int>();
            foreach (DienThoai d in dao.GetAll()) maDaCo.Add(d.MaDT);

            for (int i = 0; i < ds.Count; i++)
            {
                DienThoai x = ds[i] as DienThoai;
                if (x == null) continue;
                string loi;
                if (x.MaDT > 0 && maDaCo.Contains(x.MaDT)) loi = "Mã " + x.MaDT + " đã tồn tại";
                else loi = Them(x);

                if (loi == "")
                {
                    soThanhCong++;
                }
                else
                {
                    soLoi++;
                    if (soLoi <= 10) loiChung.AppendLine("Dòng " + (i + 2) + ": " + loi);
                }
            }
            if (soLoi > 10) loiChung.AppendLine("... và " + (soLoi - 10) + " dòng lỗi khác");
            return loiChung.ToString();
        }
    }
}
