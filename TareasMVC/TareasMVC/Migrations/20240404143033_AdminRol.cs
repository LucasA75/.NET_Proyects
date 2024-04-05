using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TareasMVC.Migrations
{
    /// <inheritdoc />
    public partial class AdminRol : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                IF NOT EXISTS (Select Id from AspNetRoles Where Id = '087db09a-34d0-4054-9f98-cc0bea2753ee')
                BEGIN
                INSERT AspNetRoles(Id,[Name],[NormalizedName]) VALUES
                ('087db09a-34d0-4054-9f98-cc0bea2753ee','admin','ADMIN')
                END");

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"DELETE AspNetRoles WHERE Id = '087db09a-34d0-4054-9f98-cc0bea2753ee'");
        }
    }
}
