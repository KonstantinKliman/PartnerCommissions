using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Accrual.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "events",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    external_id = table.Column<string>(type: "text", nullable: false),
                    user_external_id = table.Column<string>(type: "text", nullable: false),
                    profit = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_events", x => x.id);
                });

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

            migrationBuilder.CreateTable(
                name: "commissions",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    event_id = table.Column<Guid>(type: "uuid", nullable: false),
                    beneficiary_external_id = table.Column<string>(type: "text", nullable: false),
                    level = table.Column<int>(type: "integer", nullable: false),
                    amount = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: false),
                    schema_type = table.Column<string>(type: "text", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    paid_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_commissions", x => x.id);
                    table.ForeignKey(
                        name: "fk_commissions_events_event_id",
                        column: x => x.event_id,
                        principalTable: "events",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "settings",
                columns: new[] { "id", "schema_type" },
                values: new object[] { 1, "Linear" });

            migrationBuilder.CreateIndex(
                name: "ix_commissions_created_at",
                table: "commissions",
                column: "created_at",
                filter: "paid_at IS NULL");

            migrationBuilder.CreateIndex(
                name: "ix_commissions_event_id_beneficiary_external_id",
                table: "commissions",
                columns: new[] { "event_id", "beneficiary_external_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_events_external_id",
                table: "events",
                column: "external_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_events_user_external_id",
                table: "events",
                column: "user_external_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "commissions");

            migrationBuilder.DropTable(
                name: "settings");

            migrationBuilder.DropTable(
                name: "events");
        }
    }
}
