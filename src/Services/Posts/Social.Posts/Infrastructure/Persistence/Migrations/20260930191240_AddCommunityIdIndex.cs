using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Social.Posts.Migrations
{
    /// <inheritdoc />
    public partial class AddCommunityIdIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Posts_CommunityId",
                table: "Posts",
                column: "CommunityId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Posts_CommunityId",
                table: "Posts");
        }
    }
}
