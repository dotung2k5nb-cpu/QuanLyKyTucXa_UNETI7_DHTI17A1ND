// Họ và tên: Nguyễn Anh Tú
// Mã sinh viên: 23203100027
// Nội dung thực hiện: Ghi nhận 3 filtered unique index vào model (Snapshot)

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuanLyKyTucXa_UNETI7_TI17A1ND.Migrations
{
    /// <summary>
    /// Migration đồng bộ Snapshot: 3 index UX_DangKys_MotChoDuyetMoiSV, UX_DangKys_MotDaPhanPhongMoiSV,
    /// UX_PhanPhongs_MotDangOMoiDangKy đã được tạo bằng migration RangBuocDuyNhatChoDuyetVaDangO (SQL thô);
    /// nay được khai báo trong AppDb.OnModelCreating nên Up/Down để trống, tránh tạo index hai lần.
    /// </summary>
    public partial class GhiNhanChiMucVaoModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
        }
    }
}
