using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RenameTablesToUpper : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AuditLogs_Users_UserId",
                table: "AuditLogs");

            migrationBuilder.DropForeignKey(
                name: "FK_Reservations_Seats_SeatId",
                table: "Reservations");

            migrationBuilder.DropForeignKey(
                name: "FK_Reservations_Users_UserId",
                table: "Reservations");

            migrationBuilder.DropForeignKey(
                name: "FK_Seats_Sectors_SectorId",
                table: "Seats");

            migrationBuilder.DropForeignKey(
                name: "FK_Sectors_Events_EventId",
                table: "Sectors");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Users",
                table: "Users");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Sectors",
                table: "Sectors");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Seats",
                table: "Seats");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Reservations",
                table: "Reservations");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Events",
                table: "Events");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AuditLogs",
                table: "AuditLogs");

            migrationBuilder.RenameTable(
                name: "Users",
                newName: "USER");

            migrationBuilder.RenameTable(
                name: "Sectors",
                newName: "SECTOR");

            migrationBuilder.RenameTable(
                name: "Seats",
                newName: "SEAT");

            migrationBuilder.RenameTable(
                name: "Reservations",
                newName: "RESERVATION");

            migrationBuilder.RenameTable(
                name: "Events",
                newName: "EVENT");

            migrationBuilder.RenameTable(
                name: "AuditLogs",
                newName: "AUDIT_LOG");

            migrationBuilder.RenameIndex(
                name: "IX_Users_Email",
                table: "USER",
                newName: "IX_USER_Email");

            migrationBuilder.RenameIndex(
                name: "IX_Sectors_EventId",
                table: "SECTOR",
                newName: "IX_SECTOR_EventId");

            migrationBuilder.RenameIndex(
                name: "IX_Seats_SectorId",
                table: "SEAT",
                newName: "IX_SEAT_SectorId");

            migrationBuilder.RenameIndex(
                name: "IX_Reservations_UserId",
                table: "RESERVATION",
                newName: "IX_RESERVATION_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Reservations_SeatId",
                table: "RESERVATION",
                newName: "IX_RESERVATION_SeatId");

            migrationBuilder.RenameIndex(
                name: "IX_AuditLogs_UserId",
                table: "AUDIT_LOG",
                newName: "IX_AUDIT_LOG_UserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_USER",
                table: "USER",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SECTOR",
                table: "SECTOR",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SEAT",
                table: "SEAT",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_RESERVATION",
                table: "RESERVATION",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_EVENT",
                table: "EVENT",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AUDIT_LOG",
                table: "AUDIT_LOG",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AUDIT_LOG_USER_UserId",
                table: "AUDIT_LOG",
                column: "UserId",
                principalTable: "USER",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_RESERVATION_SEAT_SeatId",
                table: "RESERVATION",
                column: "SeatId",
                principalTable: "SEAT",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RESERVATION_USER_UserId",
                table: "RESERVATION",
                column: "UserId",
                principalTable: "USER",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SEAT_SECTOR_SectorId",
                table: "SEAT",
                column: "SectorId",
                principalTable: "SECTOR",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SECTOR_EVENT_EventId",
                table: "SECTOR",
                column: "EventId",
                principalTable: "EVENT",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AUDIT_LOG_USER_UserId",
                table: "AUDIT_LOG");

            migrationBuilder.DropForeignKey(
                name: "FK_RESERVATION_SEAT_SeatId",
                table: "RESERVATION");

            migrationBuilder.DropForeignKey(
                name: "FK_RESERVATION_USER_UserId",
                table: "RESERVATION");

            migrationBuilder.DropForeignKey(
                name: "FK_SEAT_SECTOR_SectorId",
                table: "SEAT");

            migrationBuilder.DropForeignKey(
                name: "FK_SECTOR_EVENT_EventId",
                table: "SECTOR");

            migrationBuilder.DropPrimaryKey(
                name: "PK_USER",
                table: "USER");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SECTOR",
                table: "SECTOR");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SEAT",
                table: "SEAT");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RESERVATION",
                table: "RESERVATION");

            migrationBuilder.DropPrimaryKey(
                name: "PK_EVENT",
                table: "EVENT");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AUDIT_LOG",
                table: "AUDIT_LOG");

            migrationBuilder.RenameTable(
                name: "USER",
                newName: "Users");

            migrationBuilder.RenameTable(
                name: "SECTOR",
                newName: "Sectors");

            migrationBuilder.RenameTable(
                name: "SEAT",
                newName: "Seats");

            migrationBuilder.RenameTable(
                name: "RESERVATION",
                newName: "Reservations");

            migrationBuilder.RenameTable(
                name: "EVENT",
                newName: "Events");

            migrationBuilder.RenameTable(
                name: "AUDIT_LOG",
                newName: "AuditLogs");

            migrationBuilder.RenameIndex(
                name: "IX_USER_Email",
                table: "Users",
                newName: "IX_Users_Email");

            migrationBuilder.RenameIndex(
                name: "IX_SECTOR_EventId",
                table: "Sectors",
                newName: "IX_Sectors_EventId");

            migrationBuilder.RenameIndex(
                name: "IX_SEAT_SectorId",
                table: "Seats",
                newName: "IX_Seats_SectorId");

            migrationBuilder.RenameIndex(
                name: "IX_RESERVATION_UserId",
                table: "Reservations",
                newName: "IX_Reservations_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_RESERVATION_SeatId",
                table: "Reservations",
                newName: "IX_Reservations_SeatId");

            migrationBuilder.RenameIndex(
                name: "IX_AUDIT_LOG_UserId",
                table: "AuditLogs",
                newName: "IX_AuditLogs_UserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Users",
                table: "Users",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Sectors",
                table: "Sectors",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Seats",
                table: "Seats",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Reservations",
                table: "Reservations",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Events",
                table: "Events",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AuditLogs",
                table: "AuditLogs",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AuditLogs_Users_UserId",
                table: "AuditLogs",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Reservations_Seats_SeatId",
                table: "Reservations",
                column: "SeatId",
                principalTable: "Seats",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Reservations_Users_UserId",
                table: "Reservations",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Seats_Sectors_SectorId",
                table: "Seats",
                column: "SectorId",
                principalTable: "Sectors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Sectors_Events_EventId",
                table: "Sectors",
                column: "EventId",
                principalTable: "Events",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
