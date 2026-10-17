using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Social.Posts.Migrations
{
    /// <inheritdoc />
    public partial class AddThumbnailImagePath : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ThumbnailImagePath",
                table: "Posts",
                type: "character varying(2048)",
                maxLength: 2048,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ThumbnailImagePath",
                table: "Posts");
        }
    }
}
