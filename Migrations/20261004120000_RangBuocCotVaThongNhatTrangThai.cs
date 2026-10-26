using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuanLyKyTucXa_UNETI7_TI17A1ND.Migrations
{
    /// <inheritdoc />
    public partial class RangBuocCotVaThongNhatTrangThai : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("UPDATE DangKys SET TrangThai = N'Chờ duyệt' WHERE TrangThai IS NULL");
            migrationBuilder.Sql("UPDATE PhanPhongs SET TrangThai = N'Đang ở' WHERE TrangThai IS NULL");

            migrationBuilder.AlterColumn<string>(
                name: "MaSinhVien",
                table: "DangKys",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "TrangThai",
                table: "DangKys",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "TrangThai",
                table: "PhanPhongs",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "GioiTinhPhong",
                table: "Phongs",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "TrangThai",
                table: "Phongs",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "GioiTinh",
                table: "SinhViens",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "TrangThai",
                table: "SinhViens",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit");
            migrationBuilder.Sql("UPDATE SinhViens SET TrangThai = CASE WHEN TrangThai = N'1' THEN N'Hoạt động' ELSE N'Khóa' END");

            migrationBuilder.AlterColumn<string>(
                name: "TrangThai",
                table: "LoaiPhongs",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit");
            migrationBuilder.Sql("UPDATE LoaiPhongs SET TrangThai = CASE WHEN TrangThai = N'1' THEN N'Hoạt động' ELSE N'Ngừng' END");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("UPDATE SinhViens SET TrangThai = CASE WHEN TrangThai = N'Hoạt động' THEN N'1' ELSE N'0' END");
            migrationBuilder.AlterColumn<bool>(
                name: "TrangThai",
                table: "SinhViens",
                type: "bit",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20);

            migrationBuilder.Sql("UPDATE LoaiPhongs SET TrangThai = CASE WHEN TrangThai = N'Hoạt động' THEN N'1' ELSE N'0' END");
            migrationBuilder.AlterColumn<bool>(
                name: "TrangThai",
                table: "LoaiPhongs",
                type: "bit",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20);

            migrationBuilder.AlterColumn<string>(
                name: "TrangThai",
                table: "DangKys",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20,
                oldNullable: false);

            migrationBuilder.AlterColumn<string>(
                name: "TrangThai",
                table: "PhanPhongs",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20,
                oldNullable: false);

            migrationBuilder.AlterColumn<string>(
                name: "GioiTinhPhong",
                table: "Phongs",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(10)",
                oldMaxLength: 10);

            migrationBuilder.AlterColumn<string>(
                name: "TrangThai",
                table: "Phongs",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20);

            migrationBuilder.AlterColumn<string>(
                name: "GioiTinh",
                table: "SinhViens",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(10)",
                oldMaxLength: 10);

            migrationBuilder.AlterColumn<string>(
                name: "MaSinhVien",
                table: "DangKys",
                type: "nvarchar(20)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20);
        }
    }
}
