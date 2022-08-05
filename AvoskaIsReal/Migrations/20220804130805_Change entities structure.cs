using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AvoskaIsReal.Migrations
{
    public partial class Changeentitiesstructure : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "TextFields");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Articles");

            migrationBuilder.RenameColumn(
                name: "MetaTitle",
                table: "TextFields",
                newName: "MetaKeywords");

            migrationBuilder.RenameColumn(
                name: "Keywords",
                table: "TextFields",
                newName: "MetaDescription");

            migrationBuilder.RenameColumn(
                name: "MetaTitle",
                table: "Articles",
                newName: "MetaKeywords");

            migrationBuilder.RenameColumn(
                name: "Keywords",
                table: "Articles",
                newName: "MetaDescription");

            migrationBuilder.AddColumn<string>(
                name: "Text",
                table: "TextFields",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "TextFields",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "Articles",
                keyColumn: "Text",
                keyValue: null,
                column: "Text",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "Text",
                table: "Articles",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Text",
                table: "TextFields");

            migrationBuilder.DropColumn(
                name: "Title",
                table: "TextFields");

            migrationBuilder.RenameColumn(
                name: "MetaKeywords",
                table: "TextFields",
                newName: "MetaTitle");

            migrationBuilder.RenameColumn(
                name: "MetaDescription",
                table: "TextFields",
                newName: "Keywords");

            migrationBuilder.RenameColumn(
                name: "MetaKeywords",
                table: "Articles",
                newName: "MetaTitle");

            migrationBuilder.RenameColumn(
                name: "MetaDescription",
                table: "Articles",
                newName: "Keywords");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "TextFields",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "Text",
                table: "Articles",
                type: "longtext",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "longtext")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Articles",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");
        }
    }
}
