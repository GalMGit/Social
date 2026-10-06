using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Social.Comments.Migrations
{
    /// <inheritdoc />
    public partial class AddIndexKnownUsers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_KnownUsers_UserId",
                table: "KnownUsers",
                column: "UserId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_KnownUsers_UserId",
                table: "KnownUsers");
        }
    }
}
