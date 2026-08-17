using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProtegePymeRD.Data.Migrations
{
    /// <inheritdoc />
    public partial class AgregarTablaPlanesContinuidad : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PlanesContinuidadDigital",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmpresaId = table.Column<int>(type: "int", nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Estado = table.Column<int>(type: "int", nullable: false),
                    Responsable = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    ContactoEmergencia = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    SistemasCriticos = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    RtoHoras = table.Column<int>(type: "int", nullable: false),
                    RpoHoras = table.Column<int>(type: "int", nullable: false),
                    ProcedimientoRespuesta = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    ProcedimientoRecuperacion = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    PlanComunicacion = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    PlanProbado = table.Column<bool>(type: "bit", nullable: false),
                    FechaUltimaPrueba = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ProximaRevision = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaUltimaActualizacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Observaciones = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlanesContinuidadDigital", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PlanesContinuidadDigital_Empresas_EmpresaId",
                        column: x => x.EmpresaId,
                        principalTable: "Empresas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PlanesContinuidadDigital_EmpresaId",
                table: "PlanesContinuidadDigital",
                column: "EmpresaId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PlanesContinuidadDigital");
        }
    }
}
