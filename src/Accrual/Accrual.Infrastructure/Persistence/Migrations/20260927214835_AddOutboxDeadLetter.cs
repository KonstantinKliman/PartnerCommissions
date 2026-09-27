using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Accrual.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddOutboxDeadLetter : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_outbox_messages_next_attempt_at",
                table: "outbox_messages");

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "dead_lettered_at",
                table: "outbox_messages",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "ix_outbox_messages_next_attempt_at",
                table: "outbox_messages",
                column: "next_attempt_at",
                filter: "processed_at IS NULL AND dead_lettered_at IS NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_outbox_messages_next_attempt_at",
                table: "outbox_messages");

            migrationBuilder.DropColumn(
                name: "dead_lettered_at",
                table: "outbox_messages");

            migrationBuilder.CreateIndex(
                name: "ix_outbox_messages_next_attempt_at",
                table: "outbox_messages",
                column: "next_attempt_at",
                filter: "processed_at IS NULL");
        }
    }
}
