namespace QuanLyBanDienThoai.DTO
{
    // Một dòng khuyến mãi đang có hiệu lực cho một điện thoại (KHUYENMAI + CHITIETKHUYENMAI)
    public class KhuyenMaiApDung
    {
        public int MaKM { get; set; }            // 0 = "Không áp dụng"
        public string TenKM { get; set; }
        public int MaDT { get; set; }
        public decimal PhanTramGiam { get; set; }

        public string TenHienThi
        {
            get { return MaKM == 0 ? TenKM : TenKM + " (-" + PhanTramGiam.ToString("0.##") + "%)"; }
        }
    }
}
