// Họ và tên: Đỗ Văn Tùng
// Mã sinh viên: 23203100028
// Nội dung thực hiện: Đổi mật khẩu

using System.ComponentModel.DataAnnotations;
namespace QuanLyKyTucXa_UNETI7_TI17A1ND.Models;

public class DoiMatKhauVM
{
    [Required(ErrorMessage = "Vui lòng nhập mật khẩu hiện tại"), DataType(DataType.Password), Display(Name = "Mật khẩu hiện tại")]
    public string MatKhauCu { get; set; }

    [Required(ErrorMessage = "Vui lòng nhập mật khẩu mới"), StringLength(100, MinimumLength = 8, ErrorMessage = "Mật khẩu mới phải từ 8 đến 100 ký tự"),
     DataType(DataType.Password), Display(Name = "Mật khẩu mới")]
    public string MatKhauMoi { get; set; }

    [Required(ErrorMessage = "Vui lòng nhập lại mật khẩu mới"), DataType(DataType.Password), Compare(nameof(MatKhauMoi), ErrorMessage = "Xác nhận mật khẩu không khớp"),
     Display(Name = "Nhập lại mật khẩu mới")]
    public string XacNhan { get; set; }
}
