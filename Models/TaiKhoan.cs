// Họ và tên: Đỗ Văn Tùng
// Mã sinh viên: 23203100028
// Nội dung thực hiện: Tài khoản, đăng nhập, phân quyền, quản lý loại phòng

using System.ComponentModel.DataAnnotations;
namespace QuanLyKyTucXa_UNETI7_TI17A1ND.Models;

public class TaiKhoan
{
    [Key] public int MaTaiKhoan { get; set; }
    [Required(ErrorMessage = "Nhập tên đăng nhập"), StringLength(50)] public string TenDangNhap { get; set; }
    [Required(ErrorMessage = "Nhập mật khẩu"), StringLength(100)] public string MatKhau { get; set; }
    [Required(ErrorMessage = "Nhập họ tên"), StringLength(100)] public string HoTen { get; set; }
    [EmailAddress(ErrorMessage = "Email không đúng định dạng"), StringLength(100)] public string Email { get; set; }
    [Required, StringLength(20)] public string VaiTro { get; set; }
    [Required, StringLength(20), AllowedValues(TT.HoatDong, TT.Khoa, ErrorMessage = "{0} không hợp lệ"), Display(Name = "Trạng thái")] public string TrangThai { get; set; } = TT.HoatDong;
}
