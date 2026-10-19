using System;
using System.Data;
using QuanLyBanDienThoai.DAO;
using QuanLyBanDienThoai.DTO;

namespace QuanLyBanDienThoai.BUS
{
    public class HangDTBUS
    {
        private HangDTDAO dao = new HangDTDAO();

        public DataTable GetAll()
        {
            return dao.GetAll();
        }

        public DataTable TimKiem(string tuKhoa)
        {
            if (string.IsNullOrWhiteSpace(tuKhoa)) return dao.GetAll();
            return dao.Search(tuKhoa.Trim());
        }

        // Trả về "" nếu hợp lệ, ngược lại trả về thông báo lỗi
        private string KiemTra(HangDT x, bool laThem)
        {
            if (string.IsNullOrWhiteSpace(x.MaHang)) return "Mã hãng không được để trống";
            if (string.IsNullOrWhiteSpace(x.TenHang)) return "Tên hãng không được để trống";
            x.MaHang = x.MaHang.Trim();
            x.TenHang = x.TenHang.Trim();
            if (laThem && dao.TonTai(x.MaHang)) return "Mã hãng đã tồn tại";
            return "";
        }

        public string Them(HangDT x)
        {
            string loi = KiemTra(x, true);
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
            string loi = KiemTra(x, false);
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

        public string Xoa(string maHang)
        {
            try
            {
                if (dao.Delete(maHang) == 0) return "Không tìm thấy hãng cần xóa";
                return "";
            }
            catch (Exception ex)
            {
                return "Không thể xóa (hãng có thể đang được dùng ở điện thoại): " + ex.Message;
            }
        }
    }
}
