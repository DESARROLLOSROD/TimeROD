using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TimeROD.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddSalidaAnticipadaColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MinutosAnticipados",
                table: "asistencias",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "SalidaAnticipada",
                table: "asistencias",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MinutosAnticipados",
                table: "asistencias");

            migrationBuilder.DropColumn(
                name: "SalidaAnticipada",
                table: "asistencias");
        }
    }
}
