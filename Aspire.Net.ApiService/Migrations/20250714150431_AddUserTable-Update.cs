using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Aspire.Net.ApiService.Migrations
{
    /// <inheritdoc />
    public partial class AddUserTableUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable(
                name: "users",
                newName: "Users");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable(
                name: "Users",
                newName: "users");
        }
    }
}
