using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Accrual.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddUserEventsIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_events_user_external_id",
                table: "events");

            migrationBuilder.CreateIndex(
                name: "ix_events_user_external_id_created_at_id",
                table: "events",
                columns: new[] { "user_external_id", "created_at", "id" },
                descending: new[] { false, true, true });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_events_user_external_id_created_at_id",
                table: "events");

            migrationBuilder.CreateIndex(
                name: "ix_events_user_external_id",
                table: "events",
                column: "user_external_id");
        }
    }
}
