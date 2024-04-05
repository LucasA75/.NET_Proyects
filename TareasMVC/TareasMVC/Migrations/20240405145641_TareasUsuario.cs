using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TareasMVC.Migrations
{
    /// <inheritdoc />
    public partial class TareasUsuario : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UsuarioCreacionId",
                table: "Tasks",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_UsuarioCreacionId",
                table: "Tasks",
                column: "UsuarioCreacionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Tasks_AspNetUsers_UsuarioCreacionId",
                table: "Tasks",
                column: "UsuarioCreacionId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tasks_AspNetUsers_UsuarioCreacionId",
                table: "Tasks");

            migrationBuilder.DropIndex(
                name: "IX_Tasks_UsuarioCreacionId",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "UsuarioCreacionId",
                table: "Tasks");
        }
    }
}
