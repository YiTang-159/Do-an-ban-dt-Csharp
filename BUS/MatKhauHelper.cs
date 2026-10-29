using System;
using System.Security.Cryptography;
using System.Text;

namespace QuanLyBanDienThoai.BUS
{
    // Băm mật khẩu bằng SHA-256, trả về 64 ký tự hex chữ thường (đúng định dạng lưu ở TAIKHOAN.MatKhau)
    public static class MatKhauHelper
    {
        public static string Bam(string matKhau)
        {
            byte[] b = SHA256.HashData(Encoding.UTF8.GetBytes(matKhau ?? ""));
            return Convert.ToHexString(b).ToLowerInvariant();
        }
    }
}
