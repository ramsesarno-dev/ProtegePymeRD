using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProtegePymeRD.Data.Migrations
{
    /// <inheritdoc />
    public partial class ActualizarModeloProtegeScore : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Los diagnósticos anteriores utilizaban un modelo de
            // puntuación diferente, por lo que no deben mezclarse
            // con el nuevo ProtegeScore.
            migrationBuilder.Sql(
                "DELETE FROM [Diagnosticos];");

            // Al eliminar los diagnósticos anteriores,
            // ninguna empresa debe conservar el score viejo.
            migrationBuilder.Sql(
                "UPDATE [Empresas] SET [ProtegeScore] = 0;");


            // Eliminar controles antiguos que ya no forman parte
            // del nuevo modelo ProtegeScore.
            migrationBuilder.DropColumn(
                name: "GestionaVulnerabilidades",
                table: "Diagnosticos");

            migrationBuilder.DropColumn(
                name: "TieneResponsables",
                table: "Diagnosticos");

            migrationBuilder.DropColumn(
                name: "TienePlanIncidentes",
                table: "Diagnosticos");

            migrationBuilder.DropColumn(
                name: "MonitoreaAlertas",
                table: "Diagnosticos");


            // Agregar los controles nuevos.
            migrationBuilder.AddColumn<bool>(
                name: "TieneProteccionEndpoint",
                table: "Diagnosticos",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "TienePlanContinuidad",
                table: "Diagnosticos",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "GestionaContrasenasAccesos",
                table: "Diagnosticos",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Eliminar las columnas del nuevo modelo.
            migrationBuilder.DropColumn(
                name: "TieneProteccionEndpoint",
                table: "Diagnosticos");

            migrationBuilder.DropColumn(
                name: "TienePlanContinuidad",
                table: "Diagnosticos");

            migrationBuilder.DropColumn(
                name: "GestionaContrasenasAccesos",
                table: "Diagnosticos");


            // Restaurar las columnas del modelo anterior.
            migrationBuilder.AddColumn<bool>(
                name: "GestionaVulnerabilidades",
                table: "Diagnosticos",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "TieneResponsables",
                table: "Diagnosticos",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "TienePlanIncidentes",
                table: "Diagnosticos",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "MonitoreaAlertas",
                table: "Diagnosticos",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
