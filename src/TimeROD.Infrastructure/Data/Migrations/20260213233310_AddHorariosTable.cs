using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace TimeROD.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddHorariosTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "HorarioId",
                table: "empleados",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "HorarioId",
                table: "areas",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "horarios",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nombre = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    HoraEntrada = table.Column<TimeSpan>(type: "interval", nullable: false),
                    HoraSalida = table.Column<TimeSpan>(type: "interval", nullable: false),
                    ToleranciaMinutos = table.Column<int>(type: "integer", nullable: false),
                    Activo = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_horarios", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_empleados_HorarioId",
                table: "empleados",
                column: "HorarioId");

            migrationBuilder.CreateIndex(
                name: "IX_areas_HorarioId",
                table: "areas",
                column: "HorarioId");

            migrationBuilder.AddForeignKey(
                name: "FK_areas_horarios_HorarioId",
                table: "areas",
                column: "HorarioId",
                principalTable: "horarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_empleados_horarios_HorarioId",
                table: "empleados",
                column: "HorarioId",
                principalTable: "horarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_areas_horarios_HorarioId",
                table: "areas");

            migrationBuilder.DropForeignKey(
                name: "FK_empleados_horarios_HorarioId",
                table: "empleados");

            migrationBuilder.DropTable(
                name: "horarios");

            migrationBuilder.DropIndex(
                name: "IX_empleados_HorarioId",
                table: "empleados");

            migrationBuilder.DropIndex(
                name: "IX_areas_HorarioId",
                table: "areas");

            migrationBuilder.DropColumn(
                name: "HorarioId",
                table: "empleados");

            migrationBuilder.DropColumn(
                name: "HorarioId",
                table: "areas");
        }
    }
}
