// Họ và tên: Phạm Quang Nhật
// Mã sinh viên: 23203100085
// Nội dung thực hiện: Form thêm/sửa sinh viên kèm đặt mật khẩu đăng nhập

using System.ComponentModel.DataAnnotations;
namespace QuanLyKyTucXa_UNETI7_DHTI17A1ND.Models;

public class SinhVienFormVM : SinhVien
{
    public bool Moi { get; set; }

    [DataType(DataType.Password),
     StringLength(100, MinimumLength = 8, ErrorMessage = "Mật khẩu phải từ 8 đến 100 ký tự"),
     Display(Name = "Mật khẩu đăng nhập")]
    public string MatKhau { get; set; }

    [DataType(DataType.Password),
     Compare(nameof(MatKhau), ErrorMessage = "Xác nhận mật khẩu không khớp"),
     Display(Name = "Xác nhận mật khẩu")]
    public string XacNhan { get; set; }
}
