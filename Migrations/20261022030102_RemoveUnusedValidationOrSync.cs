using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuanLyKyTucXa_UNETI7_TI17A1ND.Migrations
{
    /// <inheritdoc />
    public partial class RemoveUnusedValidationOrSync : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "NguyenVongLoaiPhong",
                table: "DangKys",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "NguyenVongLoaiPhong",
                table: "DangKys",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");
        }
    }
}
