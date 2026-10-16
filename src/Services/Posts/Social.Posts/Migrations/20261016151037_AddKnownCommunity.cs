using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Social.Posts.Migrations
{
    /// <inheritdoc />
    public partial class AddKnownCommunity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "KnownCommunities",
                columns: table => new
                {
                    CommunityId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KnownCommunities", x => x.CommunityId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_KnownCommunities_CommunityId",
                table: "KnownCommunities",
                column: "CommunityId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "KnownCommunities");
        }
    }
}
