// Họ và tên: Đỗ Văn Tùng
// Mã sinh viên: 23203100028
// Nội dung thực hiện: Tài khoản, đăng nhập, phân quyền, quản lý loại phòng

using System.ComponentModel.DataAnnotations;
namespace QuanLyKyTucXa_UNETI7_TI17A1ND.Models;

public class LoaiPhong
{
    [Key] public int MaLoaiPhong { get; set; }
    [Required(ErrorMessage = "Nhập tên loại phòng"), StringLength(100), Display(Name = "Tên loại phòng")] public string TenLoaiPhong { get; set; }
    [Range(1, 100, ErrorMessage = "Số người tối đa phải từ 1 đến 100"), Display(Name = "Số người tối đa")] public int SoNguoiToiDa { get; set; }
    [Range(1, 1e9, ErrorMessage = "Đơn giá phải lớn hơn 0 và không quá 1.000.000.000"), Display(Name = "Đơn giá/tháng")] public decimal DonGiaThang { get; set; }
    [StringLength(500), Display(Name = "Mô tả")] public string MoTa { get; set; }
    [Required, StringLength(20), AllowedValues(TT.HoatDong, TT.Ngung, ErrorMessage = "{0} không hợp lệ"), Display(Name = "Trạng thái")] public string TrangThai { get; set; } = TT.HoatDong;
    public List<PhongKTX> Phongs { get; set; } = new();
}
