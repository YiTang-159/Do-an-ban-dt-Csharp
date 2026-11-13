using System;
using System.Collections;
using QuanLyBanDienThoai.DAO;
using QuanLyBanDienThoai.DTO;

namespace QuanLyBanDienThoai.BUS
{
    // Bảng tham số: hằng số nghiệp vụ do Admin chỉnh, các BUS khác đọc bằng LayGiaTri.
    public class ThamSoBUS
    {
        // 4 tham số mà chương trình bắt buộc dùng: được sửa giá trị, KHÔNG được xóa
        // (xóa thì BUS âm thầm dùng giá trị mặc định, giá trị Admin đặt không còn tác dụng).
        public static readonly string[] ThamSoBatBuoc =
            { "SoTienMotDiem", "TuoiNhanVienToiThieu", "NamRaMatToiThieu", "ThangBaoHanhMacDinh" };

        private readonly ThamSoDAO dao = new ThamSoDAO();

        // Dùng mặc định khi chưa có tham số hoặc dữ liệu cũ không hợp lệ.
        public static decimal LayGiaTri(string ten)
        {
            decimal macDinh;
            switch (ten)
            {
                case "SoTienMotDiem": macDinh = 100000; break;
                case "TuoiNhanVienToiThieu": macDinh = 18; break;
                case "NamRaMatToiThieu": macDinh = 2000; break;
                case "ThangBaoHanhMacDinh": macDinh = 12; break;
                default: macDinh = 0; break;
            }
            decimal? giaTri = new ThamSoDAO().Lay(ten);
            return giaTri.HasValue && KiemTraGiaTri(ten, giaTri.Value) == ""
                ? giaTri.Value
                : macDinh;
        }

        public ArrayList GetAll()
        {
            return dao.GetAll();
        }

        public ArrayList TimKiem(string tuKhoa)
        {
            return TimKiemHelper.TimTuKhoa(GetAll(), tuKhoa, "TenThamSo", "MoTa");
        }

        public string Them(ThamSo x)
        {
            if (!PhienDangNhap.LaAdmin) return "Chỉ Admin được sửa tham số";
            string loi = KiemTra(x);
            if (loi != "") return loi;
            foreach (object o in dao.GetAll())
            {
                if (string.Equals(((ThamSo)o).TenThamSo, x.TenThamSo.Trim(), StringComparison.OrdinalIgnoreCase))
                    return "Tham số \"" + x.TenThamSo + "\" đã tồn tại, hãy dùng nút Sửa";
            }
            return Ghi(x, false);
        }

        public string Sua(ThamSo x)
        {
            if (!PhienDangNhap.LaAdmin) return "Chỉ Admin được sửa tham số";
            string loi = KiemTra(x);
            if (loi != "") return loi;
            return Ghi(x, true);
        }

        public string Xoa(string ten)
        {
            if (!PhienDangNhap.LaAdmin) return "Chỉ Admin được xóa tham số";
            if (Array.IndexOf(ThamSoBatBuoc, ten) >= 0)
                return "Tham số \"" + ten + "\" là tham số bắt buộc của chương trình, chỉ được sửa giá trị, không được xóa";
            try
            {
                return dao.Delete(ten) > 0 ? "" : "Không tìm thấy tham số";
            }
            catch (Exception ex)
            {
                return "Không thể xóa: " + ex.Message;
            }
        }

        private static string KiemTra(ThamSo x)
        {
            if (x == null || string.IsNullOrWhiteSpace(x.TenThamSo)) return "Tên tham số không được trống";
            x.TenThamSo = x.TenThamSo.Trim();
            return KiemTraGiaTri(x.TenThamSo, x.GiaTri);
        }

        private static string KiemTraGiaTri(string ten, decimal giaTri)
        {
            if (giaTri <= 0) return "Giá trị phải lớn hơn 0";

            switch (ten.ToUpperInvariant())
            {
                case "SOTIENMOTDIEM":
                    // Điểm = TongTien / SoTienMotDiem; số quá nhỏ làm điểm tràn kiểu int
                    if (giaTri != decimal.Truncate(giaTri) || giaTri < 1)
                        return "Số tiền một điểm phải là số nguyên từ 1 đồng trở lên";
                    break;
                case "THANGBAOHANHMACDINH":
                    if (giaTri != decimal.Truncate(giaTri) || giaTri > 120)
                        return "Số tháng bảo hành phải là số nguyên từ 1 đến 120";
                    break;
                case "TUOINHANVIENTOITHIEU":
                    if (giaTri != decimal.Truncate(giaTri) || giaTri > 120)
                        return "Tuổi tối thiểu phải là số nguyên từ 1 đến 120";
                    break;
                case "NAMRAMATTOITHIEU":
                    int namHienTai = DateTime.Now.Year;
                    if (giaTri != decimal.Truncate(giaTri) || giaTri < 1901 || giaTri > namHienTai)
                        return "Năm ra mắt tối thiểu phải là số nguyên từ 1901 đến " + namHienTai;
                    break;
            }

            return "";
        }

        private string Ghi(ThamSo x, bool sua)
        {
            try
            {
                int soDong = sua ? dao.Update(x) : dao.Insert(x);
                return soDong > 0 ? "" : "Không ghi được dữ liệu";
            }
            catch (Exception ex)
            {
                return "Không ghi được dữ liệu: " + ex.Message;
            }
        }
    }
}
