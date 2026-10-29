using System;
using System.Collections;
using QuanLyBanDienThoai.DAO;
using QuanLyBanDienThoai.DTO;

namespace QuanLyBanDienThoai.BUS
{
    // ============================================================================
    //  BẢN TỐI THIỂU phục vụ trang khách hàng. FILE NÀY THUỘC VỀ TV4.
    //  Khi TV4 gộp BaoHanhBUS thật thì giữ thêm hàm GetCuaToi().
    // ============================================================================
    public class BaoHanhBUS
    {
        private BaoHanhDAO dao = new BaoHanhDAO();

        // Bảo hành các máy khách đang đăng nhập đã mua
        public ArrayList GetCuaToi()
        {
            if (!PhienDangNhap.LaKhachHang) return new ArrayList();
            return dao.GetTheoKhachHang(PhienDangNhap.MaKH);
        }
    }
}
