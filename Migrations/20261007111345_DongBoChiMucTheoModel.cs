using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuanLyKyTucXa_UNETI7_TI17A1ND.Migrations
{
    /// <inheritdoc />
    public partial class DongBoChiMucTheoModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_PhanPhongs_MaDangKy",
                table: "PhanPhongs");

            migrationBuilder.DropIndex(
                name: "IX_DangKys_MaSinhVien",
                table: "DangKys");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_PhanPhongs_MaDangKy",
                table: "PhanPhongs",
                column: "MaDangKy");

            migrationBuilder.CreateIndex(
                name: "IX_DangKys_MaSinhVien",
                table: "DangKys",
                column: "MaSinhVien");
        }
    }
}
