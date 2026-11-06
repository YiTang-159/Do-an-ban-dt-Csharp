using System;
using System.Data;
namespace QuanLyBanDienThoai.DTO
{
    public class KetQuaThongKe
    {
        public DateTime TuNgay { get; set; }
        public DateTime DenNgay { get; set; }
        public decimal TongThu { get; set; }
        public decimal TongChi { get; set; }
        public decimal LoiNhuan { get { return TongThu-TongChi; } }
        public DataTable TheoNhanVien { get; set; }
        public DataTable TheoKhachHang { get; set; }
        public DataTable TheoSanPham { get; set; }
    }
}
