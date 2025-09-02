using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AutService.JwAuthLogin.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class CampoPremium : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "premium",
                table: "Users",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "ultimoPago",
                table: "Users",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "premium",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "ultimoPago",
                table: "Users");
        }
    }
}
