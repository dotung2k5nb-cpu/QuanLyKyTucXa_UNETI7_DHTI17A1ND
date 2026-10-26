// Họ và tên: Đỗ Văn Tùng
// Mã sinh viên: 23203100028
// Nội dung thực hiện: Trang chủ và điều hướng theo vai trò

using Microsoft.AspNetCore.Mvc;
using QuanLyKyTucXa_UNETI7_TI17A1ND.Filters;
using QuanLyKyTucXa_UNETI7_TI17A1ND.Models;
namespace QuanLyKyTucXa_UNETI7_TI17A1ND.Controllers;

[Quyen]
public class HomeController : Controller
{
    public IActionResult Index() =>
        HttpContext.Session.GetString("VaiTro") == TT.Admin ? RedirectToAction("Index", "ThongKe") : View();
}
