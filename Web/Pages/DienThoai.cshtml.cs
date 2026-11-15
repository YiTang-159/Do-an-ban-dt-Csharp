using Microsoft.AspNetCore.Authorization;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using QuanLyBanDienThoai.BUS;
using QuanLyBanDienThoai.DTO;

namespace QuanLyBanDienThoai.Web.Pages;

[Authorize(Roles = "Admin")]
public class DienThoaiModel(DienThoaiBUS bus, HangDTBUS hangBus, LoaiDienThoaiBUS loaiBus, ILogger<DienThoaiModel> logger) : PageModel
{
    [BindProperty(SupportsGet = true)]
    public string? TuKhoa { get; set; }

    [BindProperty]
    public DienThoaiInput Nhap { get; set; } = new();

    [TempData]
    public string? ThongBao { get; set; }

    public IReadOnlyList<DienThoai> DanhSach { get; private set; } = [];
    public IReadOnlyList<SelectListItem> DsHang { get; private set; } = [];
    public IReadOnlyList<SelectListItem> DsLoai { get; private set; } = [];
    public string? LoiTai { get; private set; }
    public int TonHienTai { get; private set; }
    public class DienThoaiInput
    {
        public int MaDT { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tên điện thoại.")]
        [StringLength(100, ErrorMessage = "Tên điện thoại tối đa 100 ký tự.")]
        public string TenDT { get; set; } = "";

        [StringLength(50, ErrorMessage = "Hệ điều hành tối đa 50 ký tự.")]
        public string? HeDieuHanh { get; set; }

        [Range(1901, 2100, ErrorMessage = "Năm ra mắt không hợp lệ.")]
        public int? NamRaMat { get; set; }

        [Range(1, 1024, ErrorMessage = "RAM phải lớn hơn 0.")]
        public int RAM { get; set; }

        [Range(1, 100000, ErrorMessage = "Bộ nhớ phải lớn hơn 0.")]
        public int BoNho { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập màu sắc.")]
        [StringLength(50, ErrorMessage = "Màu sắc tối đa 50 ký tự.")]
        public string MauSac { get; set; } = "";

        [Range(1, 999999999999, ErrorMessage = "Giá niêm yết phải lớn hơn 0.")]
        public decimal GiaNiemYet { get; set; }

        [StringLength(255, ErrorMessage = "Mô tả tối đa 255 ký tự.")]
        public string? MoTa { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Hãy chọn hãng.")]
        public int MaHang { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Hãy chọn loại.")]
        public int MaLoai { get; set; }
    }
    public IActionResult OnGet(int? sua)
    {
        try
        {
            if (sua.HasValue)
            {
                var dt = bus.GetAll().Cast<DienThoai>().FirstOrDefault(x => x.MaDT == sua.Value);
                if (dt is null)
                    return NotFound("Không tìm thấy điện thoại cần sửa.");

                Nhap = new DienThoaiInput
                {
                    MaDT = dt.MaDT,
                    TenDT = dt.TenDT,
                    HeDieuHanh = dt.HeDieuHanh,
                    NamRaMat = dt.NamRaMat == 0 ? null : dt.NamRaMat,
                    RAM = dt.RAM,
                    BoNho = dt.BoNho,
                    MauSac = dt.MauSac,
                    GiaNiemYet = dt.GiaNiemYet,
                    MoTa = dt.MoTa,
                    MaHang = dt.MaHang,
                    MaLoai = dt.MaLoai
                };
                TonHienTai = dt.SoLuongTon;
            }
        }
        catch (Exception ex)
        {
            BaoLoiTai(ex);
            return Page();
        }

        return HienTrang();
    }
    public IActionResult OnPostLuu()
    {
        if (!ModelState.IsValid)
            return HienTrang();

        var dt = new DienThoai
        {
            MaDT = Nhap.MaDT,
            TenDT = Nhap.TenDT,
            HeDieuHanh = Nhap.HeDieuHanh ?? "",
            NamRaMat = Nhap.NamRaMat ?? 0,
            RAM = Nhap.RAM,
            BoNho = Nhap.BoNho,
            MauSac = Nhap.MauSac,
            GiaNiemYet = Nhap.GiaNiemYet,
            MoTa = Nhap.MoTa ?? "",
            MaHang = Nhap.MaHang,
            MaLoai = Nhap.MaLoai
        };

        var loi = dt.MaDT == 0 ? bus.Them(dt) : bus.Sua(dt);
        return KetQua(loi, dt.MaDT == 0 ? "Đã thêm điện thoại." : "Đã cập nhật điện thoại.");
    }
    public IActionResult OnPostXoa(int maDT)
    {
        if (maDT <= 0)
            return BadRequest("Mã điện thoại không hợp lệ.");

        ModelState.Clear();
        return KetQua(bus.Xoa(maDT), "Đã xóa điện thoại.");
    }
    private IActionResult KetQua(string loi, string thanhCong)
    {
        if (loi.Length == 0)
        {
            ThongBao = thanhCong;
            return RedirectToPage(new { TuKhoa });
        }

        ModelState.AddModelError(string.Empty, loi);
        return HienTrang();
    }
    private IActionResult HienTrang()
    {
        try
        {
            DanhSach = bus.TimKiem(TuKhoa ?? "").Cast<DienThoai>().ToArray();
            DsHang = hangBus.GetAll().Cast<HangDT>()
                .Select(h => new SelectListItem(h.TenHang, h.MaHang.ToString())).ToArray();
            DsLoai = loaiBus.GetAll().Cast<LoaiDienThoai>()
                .Select(l => new SelectListItem(l.TenLoai, l.MaLoai.ToString())).ToArray();

            if (Nhap.MaDT > 0 && TonHienTai == 0)
                TonHienTai = DanhSach.FirstOrDefault(x => x.MaDT == Nhap.MaDT)?.SoLuongTon ?? 0;
        }
        catch (Exception ex)
        {
            BaoLoiTai(ex);
        }

        return Page();
    }
    private void BaoLoiTai(Exception ex)
    {
        logger.LogError(ex, "Không thể tải danh mục điện thoại");
        LoiTai = "Không tải được danh mục. Kiểm tra SQL Server, database QuanLyBanDienThoai và DAO/MyConnection.cs.";
    }
}