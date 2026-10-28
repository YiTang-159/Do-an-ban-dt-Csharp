using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using QuanLyBanDienThoai.DAO;
using QuanLyBanDienThoai.DTO;

namespace QuanLyBanDienThoai.BUS
{
    public class NhaCungCapBUS
    {
        private NhaCungCapDAO dao = new NhaCungCapDAO();

        // TV2 (FormNhapHang) dùng: MaNCC, TenNCC
        public ArrayList GetAll()
        {
            return dao.GetAll();
        }

        public ArrayList TimKiem(string tuKhoa)
        {
            return TimKiemHelper.TimTuKhoa(dao.GetAll(), tuKhoa, "MaNCC", "TenNCC", "DiaChi", "SDT", "Email");
        }

        public ArrayList TimNangCao(List<DieuKien> dieuKien)
        {
            return TimKiemHelper.Loc(dao.GetAll(), dieuKien);
        }

        // Kiểm tra dữ liệu, gồm cả tính duy nhất của SDT và Email (bỏ qua chính nhà cung cấp đang sửa)
        private string KiemTra(NhaCungCap x)
        {
            if (x == null) return "Dữ liệu không hợp lệ";
            x.TenNCC = (x.TenNCC ?? "").Trim();
            x.DiaChi = (x.DiaChi ?? "").Trim();
            x.SDT = (x.SDT ?? "").Trim();
            x.Email = (x.Email ?? "").Trim();

            if (x.TenNCC == "") return "Tên nhà cung cấp không được để trống";
            if (!Regex.IsMatch(x.SDT, @"^0\d{9}$")) return "Số điện thoại phải đủ 10 chữ số và bắt đầu bằng 0";
            if (x.Email == "") return "Email không được để trống";
            if (!Regex.IsMatch(x.Email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$")) return "Email không hợp lệ";

            foreach (NhaCungCap k in dao.GetAll())
            {
                if (k.MaNCC == x.MaNCC) continue;
                if (k.SDT == x.SDT) return "Số điện thoại đã được nhà cung cấp \"" + k.TenNCC + "\" sử dụng";
                if (string.Equals(k.Email, x.Email, StringComparison.OrdinalIgnoreCase))
                    return "Email đã được nhà cung cấp \"" + k.TenNCC + "\" sử dụng";
            }
            return "";
        }

        public string Them(NhaCungCap x)
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

        public string Sua(NhaCungCap x)
        {
            string loi = KiemTra(x);
            if (loi != "") return loi;
            try
            {
                if (dao.Update(x) == 0) return "Không tìm thấy nhà cung cấp cần sửa";
                return "";
            }
            catch (Exception ex)
            {
                return "Không thể sửa: " + ex.Message;
            }
        }

        public string Xoa(int maNCC)
        {
            try
            {
                if (dao.Delete(maNCC) == 0) return "Không tìm thấy nhà cung cấp cần xóa";
                return "";
            }
            catch (Exception ex)
            {
                return "Không thể xóa (nhà cung cấp có thể đang được dùng ở phiếu nhập hàng): " + ex.Message;
            }
        }

        public string NhapDanhSach(ArrayList ds, out int soThanhCong)
        {
            soThanhCong = 0;
            StringBuilder loiChung = new StringBuilder();
            int soLoi = 0;
            HashSet<int> maDaCo = new HashSet<int>();
            foreach (NhaCungCap n in dao.GetAll()) maDaCo.Add(n.MaNCC);

            for (int i = 0; i < ds.Count; i++)
            {
                NhaCungCap x = ds[i] as NhaCungCap;
                if (x == null) continue;
                string loi;
                if (x.MaNCC > 0 && maDaCo.Contains(x.MaNCC)) loi = "Mã " + x.MaNCC + " đã tồn tại";
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
