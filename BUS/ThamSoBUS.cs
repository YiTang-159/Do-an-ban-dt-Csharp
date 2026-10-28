using System;

namespace QuanLyBanDienThoai.BUS
{
    // ============================================================================
    //  BẢN TẠM của TV1 để project build được. FILE NÀY THUỘC VỀ TV2.
    //  Khi TV2 gộp ThamSoBUS thật (đọc bảng THAMSO) thì lấy bản của TV2, bỏ bản này.
    //  Không đưa file này vào Pull Request của TV1.
    //  Giả định hàm static: ThamSoBUS.LayGiaTri(ten) trả về decimal, có giá trị mặc định.
    // ============================================================================
    public class ThamSoBUS
    {
        public static decimal LayGiaTri(string ten)
        {
            switch (ten)
            {
                case "SoTienMotDiem": return 100000;
                case "TuoiNhanVienToiThieu": return 18;
                case "NamRaMatToiThieu": return 2000;
                case "ThangBaoHanhMacDinh": return 12;
                default: return 0;
            }
        }
    }
}
