using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProtegePymeRD.Data.Migrations
{
    /// <inheritdoc />
    public partial class AgregarTablaDiagnosticos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Diagnosticos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmpresaId = table.Column<int>(type: "int", nullable: false),
                    TieneInventarioActivos = table.Column<bool>(type: "bit", nullable: false),
                    GestionaVulnerabilidades = table.Column<bool>(type: "bit", nullable: false),
                    UtilizaMfa = table.Column<bool>(type: "bit", nullable: false),
                    MantieneActualizaciones = table.Column<bool>(type: "bit", nullable: false),
                    CapacitaEmpleados = table.Column<bool>(type: "bit", nullable: false),
                    MonitoreaAlertas = table.Column<bool>(type: "bit", nullable: false),
                    TienePlanIncidentes = table.Column<bool>(type: "bit", nullable: false),
                    TieneResponsables = table.Column<bool>(type: "bit", nullable: false),
                    RealizaRespaldos = table.Column<bool>(type: "bit", nullable: false),
                    PruebaRestauraciones = table.Column<bool>(type: "bit", nullable: false),
                    Puntuacion = table.Column<int>(type: "int", nullable: false),
                    FechaEvaluacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UsuarioEvaluador = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Observaciones = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Diagnosticos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Diagnosticos_Empresas_EmpresaId",
                        column: x => x.EmpresaId,
                        principalTable: "Empresas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Diagnosticos_EmpresaId",
                table: "Diagnosticos",
                column: "EmpresaId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Diagnosticos");
        }
    }
}
