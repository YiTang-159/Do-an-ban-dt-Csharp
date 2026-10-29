using System;
using System.Collections;
using System.Collections.Generic;
using QuanLyBanDienThoai.DAO;
using QuanLyBanDienThoai.DTO;

namespace QuanLyBanDienThoai.BUS
{
    // ============================================================================
    //  BẢN TỐI THIỂU (đủ để nhân viên lập hóa đơn và khách xem đơn mua). FILE NÀY THUỘC VỀ TV3.
    //  Khi TV3 gộp HoaDonBUS thật thì giữ các hàm form đang gọi: GetAll, GetCuaToi, GetChiTiet, GetChiTietCuaToi,
    //  TimKiem, LocCuaToi, TinhTongTien, GetKhuyenMaiApDung, TaoDong, NapTuGioHang, TinhTongDong, LuuHoaDon.
    // ============================================================================
    public class HoaDonBUS
    {
        private HoaDonDAO dao = new HoaDonDAO();
        private GioHangBUS gioBus = new GioHangBUS();

        public ArrayList GetAll()
        {
            return dao.GetAll();
        }

        public ArrayList GetChiTiet(int maHD)
        {
            return dao.GetChiTiet(maHD);
        }

        public ArrayList TimKiem(string tuKhoa)
        {
            return TimKiemHelper.TimTuKhoa(dao.GetAll(), tuKhoa, "MaHD", "TenKhachHang", "TenNhanVien", "PhuongThucTT");
        }

        // Giữ lại các hóa đơn do nhân viên đang đăng nhập lập
        public ArrayList LocCuaToi(ArrayList ds)
        {
            ArrayList kq = new ArrayList();
            foreach (object o in ds)
            {
                if (((HoaDon)o).MaNV == PhienDangNhap.MaNV) kq.Add(o);
            }
            return kq;
        }

        public decimal TinhTongTien(ArrayList ds)
        {
            decimal tong = 0;
            foreach (object o in ds) tong += ((HoaDon)o).TongTien;
            return tong;
        }

        // ===================== Khách hàng xem đơn của mình =====================

        // Chỉ hóa đơn của khách đang đăng nhập (mã khách lấy từ phiên đăng nhập)
        public ArrayList GetCuaToi()
        {
            if (!PhienDangNhap.LaKhachHang) return new ArrayList();
            return dao.GetTheoKhachHang(PhienDangNhap.MaKH);
        }

        // Chi tiết một hóa đơn của mình; hóa đơn của người khác thì trả về danh sách rỗng
        public ArrayList GetChiTietCuaToi(int maHD)
        {
            foreach (object o in GetCuaToi())
            {
                if (((HoaDon)o).MaHD == maHD) return dao.GetChiTiet(maHD);
            }
            return new ArrayList();
        }

        // ===================== Nhân viên lập hóa đơn =====================

        // Khuyến mãi đang có hiệu lực của một điện thoại, phần trăm cao xếp trước
        public ArrayList GetKhuyenMaiApDung(int maDT)
        {
            ArrayList kq = new ArrayList();
            foreach (object o in GiaBanHelper.LayKhuyenMaiApDung())
            {
                if (((KhuyenMaiApDung)o).MaDT == maDT) kq.Add(o);
            }
            return kq;
        }

        // Thêm một dòng hàng vào hóa đơn đang lập (dong là ArrayList ChiTietHoaDon).
        // Kiểm tra: số lượng > 0, điện thoại chưa có trong hóa đơn (khóa kép MaHD, MaDT), đủ tồn kho,
        // khuyến mãi (nếu chọn) phải đang hiệu lực và áp dụng cho điện thoại này.
        // DonGia = giá niêm yết; ThanhTien = làm tròn(DonGia x (100 - %) / 100 x số lượng).
        public string TaoDong(ArrayList dong, int maDT, int soLuong, int? maKM)
        {
            if (soLuong <= 0) return "Số lượng phải lớn hơn 0";
            try
            {
                foreach (object o in dong)
                {
                    if (((ChiTietHoaDon)o).MaDT == maDT) return "Điện thoại này đã có trong hóa đơn, hãy xóa dòng cũ rồi thêm lại";
                }

                DienThoai dt = null;
                foreach (object o in new DienThoaiDAO().GetAll())
                {
                    if (((DienThoai)o).MaDT == maDT) { dt = (DienThoai)o; break; }
                }
                if (dt == null) return "Không tìm thấy điện thoại";
                if (soLuong > dt.SoLuongTon) return "Không đủ hàng: " + dt.TenHienThi + " chỉ còn " + dt.SoLuongTon + " máy";

                decimal phanTram = 0;
                string tenKM = null;
                if (maKM.HasValue)
                {
                    KhuyenMaiApDung km = null;
                    foreach (object o in GetKhuyenMaiApDung(maDT))
                    {
                        if (((KhuyenMaiApDung)o).MaKM == maKM.Value) { km = (KhuyenMaiApDung)o; break; }
                    }
                    if (km == null) return "Khuyến mãi không áp dụng cho điện thoại này hoặc đã hết hạn";
                    phanTram = km.PhanTramGiam;
                    tenKM = km.TenKM;
                }

                ChiTietHoaDon ct = new ChiTietHoaDon();
                ct.MaDT = maDT;
                ct.SoLuong = soLuong;
                ct.DonGia = dt.GiaNiemYet;
                ct.ThanhTien = GiaBanHelper.ThanhTien(dt.GiaNiemYet, phanTram, soLuong);
                ct.MaKM = maKM;
                ct.TenHienThi = dt.TenHienThi;
                ct.TenKM = tenKM;
                dong.Add(ct);
                return "";
            }
            catch (Exception ex)
            {
                return "Không thể thêm dòng hàng: " + ex.Message;
            }
        }

        // Nạp giỏ hàng của khách vào hóa đơn đang lập. Mỗi dòng vẫn đi qua TaoDong (kiểm tồn kho, tính giá).
        // Dòng nào lỗi thì bỏ qua và ghi vào chuỗi trả về; trả về "" nếu mọi dòng đều nạp được.
        public string NapTuGioHang(int maKH, ArrayList dong)
        {
            ArrayList gio = gioBus.GetTheoKhachHang(maKH);
            if (gio.Count == 0) return "Khách hàng này chưa có hàng trong giỏ";
            string loiChung = "";
            foreach (object o in gio)
            {
                GioHang g = (GioHang)o;
                string loi = TaoDong(dong, g.MaDT, g.SoLuong, g.MaKM);
                if (loi != "") loiChung += g.TenHienThi + ": " + loi + "\n";
            }
            return loiChung;
        }

        public decimal TinhTongDong(ArrayList dong)
        {
            decimal tong = 0;
            foreach (object o in dong) tong += ((ChiTietHoaDon)o).ThanhTien;
            return tong;
        }

        // Lưu hóa đơn. Nhân viên lập hóa đơn là người đang đăng nhập (MaNV lấy từ phiên, không do form chọn).
        // Điểm tích lũy = tổng tiền / SoTienMotDiem (bảng THAMSO), làm tròn xuống.
        // Trả về "" nếu thành công và maHD là mã hóa đơn mới.
        public string LuuHoaDon(int maKH, string phuongThucTT, ArrayList dong, bool tuGioHang, out int maHD)
        {
            maHD = 0;
            if (!PhienDangNhap.LaNhanSu || PhienDangNhap.MaNV <= 0) return "Chỉ nhân viên hoặc quản trị viên mới được lập hóa đơn";
            if (maKH <= 0) return "Hãy chọn khách hàng";
            if (new KhachHangBUS().GetById(maKH) == null) return "Không tìm thấy khách hàng";
            if (dong == null || dong.Count == 0) return "Hóa đơn phải có ít nhất một điện thoại";
            if (string.IsNullOrWhiteSpace(phuongThucTT)) return "Hãy chọn phương thức thanh toán";

            try
            {
                HoaDon hd = new HoaDon();
                hd.NgayLap = DateTime.Now;
                hd.PhuongThucTT = phuongThucTT.Trim();
                hd.TongTien = TinhTongDong(dong);
                hd.MaKH = maKH;
                hd.MaNV = PhienDangNhap.MaNV;

                decimal soTienMotDiem = ThamSoBUS.LayGiaTri("SoTienMotDiem");
                int diem = soTienMotDiem > 0 ? (int)Math.Floor(hd.TongTien / soTienMotDiem) : 0;

                maHD = dao.LuuHoaDon(hd, dong, diem, tuGioHang);
                return "";
            }
            catch (Exception ex)
            {
                return "Không thể lập hóa đơn: " + ex.Message;
            }
        }
    }
}
