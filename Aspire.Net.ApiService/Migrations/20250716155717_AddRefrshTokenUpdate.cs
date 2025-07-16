using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Aspire.Net.ApiService.Migrations
{
    /// <inheritdoc />
    public partial class AddRefrshTokenUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_refresh_tokens_email",
                table: "RefreshTokens");

            migrationBuilder.CreateIndex(
                name: "ix_refresh_tokens_email",
                table: "RefreshTokens",
                column: "email");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_refresh_tokens_email",
                table: "RefreshTokens");

            migrationBuilder.CreateIndex(
                name: "ix_refresh_tokens_email",
                table: "RefreshTokens",
                column: "email",
                unique: true);
        }
    }
}
