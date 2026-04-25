using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AgregueNombreMayuscula : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AuditLogs_Users_UserId",
                table: "AuditLogs");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AuditLogs",
                table: "AuditLogs");

            migrationBuilder.RenameTable(
                name: "AuditLogs",
                newName: "AUDIT_LOG");

            migrationBuilder.RenameIndex(
                name: "IX_AuditLogs_UserId",
                table: "AUDIT_LOG",
                newName: "IX_AUDIT_LOG_UserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AUDIT_LOG",
                table: "AUDIT_LOG",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AUDIT_LOG_Users_UserId",
                table: "AUDIT_LOG",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AUDIT_LOG_Users_UserId",
                table: "AUDIT_LOG");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AUDIT_LOG",
                table: "AUDIT_LOG");

            migrationBuilder.RenameTable(
                name: "AUDIT_LOG",
                newName: "AuditLogs");

            migrationBuilder.RenameIndex(
                name: "IX_AUDIT_LOG_UserId",
                table: "AuditLogs",
                newName: "IX_AuditLogs_UserId");

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
        }
    }
}
