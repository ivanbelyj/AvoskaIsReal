using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AvoskaIsReal.Migrations
{
    public partial class Renameauthorforeignkeyandnavigationproperty : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Articles_AspNetUsers_AuthorId",
                table: "Articles");

            migrationBuilder.RenameColumn(
                name: "AuthorId",
                table: "Articles",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Articles_AuthorId",
                table: "Articles",
                newName: "IX_Articles_UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Articles_AspNetUsers_UserId",
                table: "Articles",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Articles_AspNetUsers_UserId",
                table: "Articles");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "Articles",
                newName: "AuthorId");

            migrationBuilder.RenameIndex(
                name: "IX_Articles_UserId",
                table: "Articles",
                newName: "IX_Articles_AuthorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Articles_AspNetUsers_AuthorId",
                table: "Articles",
                column: "AuthorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
