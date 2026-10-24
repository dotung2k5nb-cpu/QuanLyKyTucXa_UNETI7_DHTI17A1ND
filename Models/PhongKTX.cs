// Họ và tên: Hoàng Tiến Nam
// Mã sinh viên: 23203100029
// Nội dung thực hiện: Quản lý phòng ký túc xá, tìm kiếm, lọc, sắp xếp và phân trang

using System.ComponentModel.DataAnnotations;
namespace QuanLyKyTucXa_UNETI7_TI17A1ND.Models;

public class PhongKTX
{
    [Key] public int MaPhong { get; set; }
    [Required(ErrorMessage = "Nhập tên phòng"), StringLength(50), Display(Name = "Tên phòng")] public string TenPhong { get; set; }
    [Display(Name = "Loại phòng")] public int MaLoaiPhong { get; set; }
    public LoaiPhong LoaiPhong { get; set; }
    [Required(ErrorMessage = "Nhập tòa nhà"), StringLength(50), Display(Name = "Tòa nhà")] public string ToaNha { get; set; }
    [Range(1, 50, ErrorMessage = "Tầng không hợp lệ")] public int Tang { get; set; } = 1;
    [Required, StringLength(10), AllowedValues(TT.Nam, TT.Nu, ErrorMessage = "{0} không hợp lệ"), Display(Name = "Giới tính phòng")] public string GioiTinhPhong { get; set; } = TT.Nam;
    [Required, StringLength(20), AllowedValues(TT.PhongDung, TT.PhongTam, ErrorMessage = "{0} không hợp lệ"), Display(Name = "Trạng thái")] public string TrangThai { get; set; } = TT.PhongDung;
    [StringLength(500), Display(Name = "Mô tả")] public string MoTa { get; set; }
    public List<PhanPhong> PhanPhongs { get; set; } = new();
}
