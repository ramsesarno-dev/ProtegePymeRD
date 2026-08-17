using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProtegePymeRD.Data.Migrations
{
    /// <inheritdoc />
    public partial class AgregarTablaRespaldos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Respaldos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmpresaId = table.Column<int>(type: "int", nullable: false),
                    RecursoProtegido = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Tipo = table.Column<int>(type: "int", nullable: false),
                    ProveedorDestino = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Frecuencia = table.Column<int>(type: "int", nullable: false),
                    Estado = table.Column<int>(type: "int", nullable: false),
                    FechaUltimoRespaldo = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RestauracionProbada = table.Column<bool>(type: "bit", nullable: false),
                    FechaUltimaPrueba = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Observaciones = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    FechaRegistro = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Respaldos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Respaldos_Empresas_EmpresaId",
                        column: x => x.EmpresaId,
                        principalTable: "Empresas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Respaldos_EmpresaId",
                table: "Respaldos",
                column: "EmpresaId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Respaldos");
        }
    }
}
