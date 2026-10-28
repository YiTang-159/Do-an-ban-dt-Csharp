using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using QuanLyBanDienThoai.DAO;
using QuanLyBanDienThoai.DTO;

namespace QuanLyBanDienThoai.BUS
{
    public class HangDTBUS
    {
        private HangDTDAO dao = new HangDTDAO();

        public ArrayList GetAll()
        {
            return dao.GetAll();
        }

        // Tìm trên ArrayList (không có WHERE LIKE ở DAO)
        public ArrayList TimKiem(string tuKhoa)
        {
            return TimKiemHelper.TimTuKhoa(dao.GetAll(), tuKhoa, "MaHang", "TenHang", "QuocGia");
        }

        public ArrayList TimNangCao(List<DieuKien> dieuKien)
        {
            return TimKiemHelper.Loc(dao.GetAll(), dieuKien);
        }

        // Trả về "" nếu hợp lệ, ngược lại trả về thông báo lỗi
        private string KiemTra(HangDT x)
        {
            if (x == null) return "Dữ liệu không hợp lệ";
            x.TenHang = (x.TenHang ?? "").Trim();
            x.QuocGia = (x.QuocGia ?? "").Trim();
            if (x.TenHang == "") return "Tên hãng không được để trống";
            return "";
        }

        public string Them(HangDT x)
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

        public string Sua(HangDT x)
        {
            string loi = KiemTra(x);
            if (loi != "") return loi;
            try
            {
                if (dao.Update(x) == 0) return "Không tìm thấy hãng cần sửa";
                return "";
            }
            catch (Exception ex)
            {
                return "Không thể sửa: " + ex.Message;
            }
        }

        public string Xoa(int maHang)
        {
            try
            {
                // Hãng đang có điện thoại thì không cho xóa
                int soDT = 0;
                foreach (DienThoai d in new DienThoaiDAO().GetAll())
                {
                    if (d.MaHang == maHang) soDT++;
                }
                if (soDT > 0) return "Không thể xóa: hãng đang có " + soDT + " điện thoại";

                if (dao.Delete(maHang) == 0) return "Không tìm thấy hãng cần xóa";
                return "";
            }
            catch (Exception ex)
            {
                return "Không thể xóa: " + ex.Message;
            }
        }

        // Nhập từ danh sách đọc ở file Excel. Dòng có mã đã tồn tại thì báo lỗi; còn lại thêm mới.
        // Trả về "" nếu mọi dòng đều thành công, ngược lại là danh sách lỗi theo dòng.
        public string NhapDanhSach(ArrayList ds, out int soThanhCong)
        {
            soThanhCong = 0;
            StringBuilder loiChung = new StringBuilder();
            int soLoi = 0;
            HashSet<int> maDaCo = new HashSet<int>();
            foreach (HangDT h in dao.GetAll()) maDaCo.Add(h.MaHang);

            for (int i = 0; i < ds.Count; i++)
            {
                HangDT x = ds[i] as HangDT;
                if (x == null) continue;
                string loi;
                if (x.MaHang > 0 && maDaCo.Contains(x.MaHang)) loi = "Mã " + x.MaHang + " đã tồn tại";
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
