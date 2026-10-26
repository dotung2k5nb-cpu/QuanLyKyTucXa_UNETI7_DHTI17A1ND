// Họ và tên: Đỗ Văn Tùng
// Mã sinh viên: 23203100028
// Nội dung thực hiện: Tài khoản, đăng nhập, phân quyền, quản lý loại phòng

using System.ComponentModel.DataAnnotations;
namespace QuanLyKyTucXa_UNETI7_TI17A1ND.Models;

public class LoginVM
{
    [Required(ErrorMessage = "Vui lòng nhập tên đăng nhập"), StringLength(50), Display(Name = "Tên đăng nhập")]
    public string TenDangNhap { get; set; }

    [Required(ErrorMessage = "Vui lòng nhập mật khẩu"), DataType(DataType.Password), Display(Name = "Mật khẩu")]
    public string MatKhau { get; set; }
}
