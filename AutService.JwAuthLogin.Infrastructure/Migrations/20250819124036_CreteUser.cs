using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AutService.JwAuthLogin.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class CreteUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Edad",
                table: "Users",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Sexo",
                table: "Users",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Edad",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Sexo",
                table: "Users");
        }
    }
}
