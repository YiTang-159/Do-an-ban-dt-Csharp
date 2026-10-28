using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using QuanLyBanDienThoai.DAO;
using QuanLyBanDienThoai.DTO;

namespace QuanLyBanDienThoai.BUS
{
    public class LoaiDienThoaiBUS
    {
        private LoaiDienThoaiDAO dao = new LoaiDienThoaiDAO();

        public ArrayList GetAll()
        {
            return dao.GetAll();
        }

        public ArrayList TimKiem(string tuKhoa)
        {
            return TimKiemHelper.TimTuKhoa(dao.GetAll(), tuKhoa, "MaLoai", "TenLoai", "MoTa");
        }

        public ArrayList TimNangCao(List<DieuKien> dieuKien)
        {
            return TimKiemHelper.Loc(dao.GetAll(), dieuKien);
        }

        private string KiemTra(LoaiDienThoai x)
        {
            if (x == null) return "Dữ liệu không hợp lệ";
            x.TenLoai = (x.TenLoai ?? "").Trim();
            x.MoTa = (x.MoTa ?? "").Trim();
            if (x.TenLoai == "") return "Tên loại không được để trống";
            return "";
        }

        public string Them(LoaiDienThoai x)
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

        public string Sua(LoaiDienThoai x)
        {
            string loi = KiemTra(x);
            if (loi != "") return loi;
            try
            {
                if (dao.Update(x) == 0) return "Không tìm thấy loại cần sửa";
                return "";
            }
            catch (Exception ex)
            {
                return "Không thể sửa: " + ex.Message;
            }
        }

        public string Xoa(int maLoai)
        {
            try
            {
                int soDT = 0;
                foreach (DienThoai d in new DienThoaiDAO().GetAll())
                {
                    if (d.MaLoai == maLoai) soDT++;
                }
                if (soDT > 0) return "Không thể xóa: loại đang có " + soDT + " điện thoại";

                if (dao.Delete(maLoai) == 0) return "Không tìm thấy loại cần xóa";
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
            foreach (LoaiDienThoai l in dao.GetAll()) maDaCo.Add(l.MaLoai);

            for (int i = 0; i < ds.Count; i++)
            {
                LoaiDienThoai x = ds[i] as LoaiDienThoai;
                if (x == null) continue;
                string loi;
                if (x.MaLoai > 0 && maDaCo.Contains(x.MaLoai)) loi = "Mã " + x.MaLoai + " đã tồn tại";
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
