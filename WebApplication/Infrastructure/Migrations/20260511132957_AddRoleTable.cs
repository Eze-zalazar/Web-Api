using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddRoleTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // 1. Create ROLE table first
            migrationBuilder.CreateTable(
                name: "ROLE",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ROLE", x => x.Id);
                });

            // 2. Insert basic roles so we have valid IDs
            migrationBuilder.Sql("INSERT INTO ROLE (Name) VALUES ('Admin')");
            migrationBuilder.Sql("INSERT INTO ROLE (Name) VALUES ('User')");

            // 3. Add RoleId to USER with default value 2 (User)
            migrationBuilder.AddColumn<int>(
                name: "RoleId",
                table: "USER",
                type: "int",
                nullable: false,
                defaultValue: 2);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "AUDIT_LOG",
                type: "datetime2(3)",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.CreateIndex(
                name: "IX_USER_RoleId",
                table: "USER",
                column: "RoleId");

            migrationBuilder.AddForeignKey(
                name: "FK_USER_ROLE_RoleId",
                table: "USER",
                column: "RoleId",
                principalTable: "ROLE",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_USER_ROLE_RoleId",
                table: "USER");

            migrationBuilder.DropTable(
                name: "ROLE");

            migrationBuilder.DropIndex(
                name: "IX_USER_RoleId",
                table: "USER");

            migrationBuilder.DropColumn(
                name: "RoleId",
                table: "USER");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "AUDIT_LOG",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2(3)");
        }
    }
}
