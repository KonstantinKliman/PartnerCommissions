using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Accrual.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ReplaceSettingsWithSchemaChanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "settings");

            migrationBuilder.CreateTable(
                name: "schema_changes",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    schema_type = table.Column<string>(type: "text", nullable: false),
                    changed_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_schema_changes", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "ix_schema_changes_changed_at",
                table: "schema_changes",
                column: "changed_at");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "schema_changes");

            migrationBuilder.CreateTable(
                name: "settings",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false),
                    schema_type = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_settings", x => x.id);
                });

            migrationBuilder.InsertData(
                table: "settings",
                columns: new[] { "id", "schema_type" },
                values: new object[] { 1, "Linear" });
        }
    }
}
