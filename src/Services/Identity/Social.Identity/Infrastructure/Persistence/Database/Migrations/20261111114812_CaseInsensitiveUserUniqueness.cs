using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Social.Identity.Infrastructure.Persistence.Database.Migrations
{
    /// <inheritdoc />
    public partial class CaseInsensitiveUserUniqueness : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Регистр в Email/Username сохраняется как ввёл пользователь,
            // но уникальность проверяется без его учёта. Индекс по lower()
            // закрывает гонку между AnyAsync и INSERT.
            migrationBuilder.Sql(
                """
                CREATE UNIQUE INDEX IF NOT EXISTS "IX_Users_Email_Lower"
                    ON "Users" (lower("Email"));
                CREATE UNIQUE INDEX IF NOT EXISTS "IX_Users_Username_Lower"
                    ON "Users" (lower("Username"));
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                """
                DROP INDEX IF EXISTS "IX_Users_Email_Lower";
                DROP INDEX IF EXISTS "IX_Users_Username_Lower";
                """);
        }
    }
}
