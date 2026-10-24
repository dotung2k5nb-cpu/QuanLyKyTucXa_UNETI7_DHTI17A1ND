// Họ và tên: Nguyễn Anh Tú
// Mã sinh viên: 23203100027
// Nội dung thực hiện: Ràng buộc DB: mỗi sinh viên tối đa 1 đăng ký Chờ duyệt, 1 đăng ký Đã phân phòng, mỗi đăng ký tối đa 1 phòng Đang ở

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuanLyKyTucXa_UNETI7_TI17A1ND.Migrations
{
    /// <summary>
    /// Filtered unique index: chặn ở tầng CSDL những trường hợp hai request đồng thời cùng lọt qua kiểm tra bằng code.
    ///  - DangKys(MaSinhVien) khi "Chờ duyệt"      -> không tạo nhiều đăng ký chờ duyệt cùng lúc.
    ///  - DangKys(MaSinhVien) khi "Đã phân phòng"  -> mỗi sinh viên chỉ có một đăng ký đang được phân phòng.
    ///  - PhanPhongs(MaDangKy) khi "Đang ở"        -> mỗi đăng ký chỉ có một bản ghi phòng đang ở.
    /// Vì phòng "Đang ở" luôn thuộc đăng ký "Đã phân phòng", hai ràng buộc cuối đảm bảo
    /// một sinh viên không thể có hai phòng "Đang ở" cùng lúc.
    /// Hằng số chuỗi phải khớp với lớp TT trong Models/TT.cs.
    /// </summary>
    public partial class RangBuocDuyNhatChoDuyetVaDangO : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "UX_DangKys_MotChoDuyetMoiSV",
                table: "DangKys",
                column: "MaSinhVien",
                unique: true,
                filter: "[TrangThai] = N'Chờ duyệt'");

            migrationBuilder.CreateIndex(
                name: "UX_DangKys_MotDaPhanPhongMoiSV",
                table: "DangKys",
                column: "MaSinhVien",
                unique: true,
                filter: "[TrangThai] = N'Đã phân phòng'");

            migrationBuilder.CreateIndex(
                name: "UX_PhanPhongs_MotDangOMoiDangKy",
                table: "PhanPhongs",
                column: "MaDangKy",
                unique: true,
                filter: "[TrangThai] = N'Đang ở'");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(name: "UX_DangKys_MotChoDuyetMoiSV", table: "DangKys");
            migrationBuilder.DropIndex(name: "UX_DangKys_MotDaPhanPhongMoiSV", table: "DangKys");
            migrationBuilder.DropIndex(name: "UX_PhanPhongs_MotDangOMoiDangKy", table: "PhanPhongs");
        }
    }
}
