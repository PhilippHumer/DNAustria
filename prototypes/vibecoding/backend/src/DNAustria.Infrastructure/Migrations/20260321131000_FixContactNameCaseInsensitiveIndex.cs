using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DNAustria.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixContactNameCaseInsensitiveIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_contacts_name",
                table: "contacts");

            migrationBuilder.Sql(
                "CREATE UNIQUE INDEX \"IX_contacts_name_ci\" ON contacts (lower(name)) WHERE is_deleted = false");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP INDEX IF EXISTS \"IX_contacts_name_ci\"");

            migrationBuilder.CreateIndex(
                name: "IX_contacts_name",
                table: "contacts",
                column: "name",
                unique: true,
                filter: "is_deleted = false");
        }
    }
}
