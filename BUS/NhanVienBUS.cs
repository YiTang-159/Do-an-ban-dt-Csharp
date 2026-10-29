using System;
using System.Collections;
using QuanLyBanDienThoai.DAO;
using QuanLyBanDienThoai.DTO;

namespace QuanLyBanDienThoai.BUS
{
    // ============================================================================
    //  BẢN TỐI THIỂU (GetAll, GetById, TimKiem). FILE NÀY THUỘC VỀ TV2.
    //  Khi TV2 gộp NhanVienBUS thật (Them, Sua, Xoa, TimNangCao, kiểm tra tuổi...) thì lấy bản của TV2;
    //  chỉ cần đảm bảo còn GetById(int).
    // ============================================================================
    public class NhanVienBUS
    {
        private NhanVienDAO dao = new NhanVienDAO();

        public ArrayList GetAll()
        {
            return dao.GetAll();
        }

        public NhanVien GetById(int maNV)
        {
            return dao.GetById(maNV);
        }

        public ArrayList TimKiem(string tuKhoa)
        {
            return TimKiemHelper.TimTuKhoa(dao.GetAll(), tuKhoa, "MaNV", "HoTen", "SDT", "Email", "ChucVu");
        }
    }
}
