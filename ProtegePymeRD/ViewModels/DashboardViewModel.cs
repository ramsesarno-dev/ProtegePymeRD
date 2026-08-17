using ProtegePymeRD.Models;

namespace ProtegePymeRD.ViewModels
{
    public class DashboardViewModel
    {
        // Empresas
        public int TotalEmpresas { get; set; }

        public int EmpresasActivas { get; set; }

        public int EvaluacionesPendientes { get; set; }

        public int ProtegeScorePromedio { get; set; }

        public int MetaPiloto { get; set; } = 10;

        public int PorcentajePiloto
        {
            get
            {
                if (MetaPiloto == 0)
                {
                    return 0;
                }

                int porcentaje =
                    TotalEmpresas * 100 / MetaPiloto;

                return Math.Min(porcentaje, 100);
            }
        }

        // ProtegeBackup
        public int TotalRespaldos { get; set; }

        public int RespaldosExitosos { get; set; }

        public int RestauracionesVerificadas { get; set; }

        public int PorcentajeRespaldosExitosos
        {
            get
            {
                if (TotalRespaldos == 0)
                {
                    return 0;
                }

                return (int)Math.Round(
                    RespaldosExitosos * 100.0 /
                    TotalRespaldos);
            }
        }

        // ProtegeAccess
        public int TotalCuentasCriticas { get; set; }

        public int CuentasCriticasActivas { get; set; }

        public int CuentasConMfa { get; set; }

        public int CuentasConMinimoPrivilegio { get; set; }

        public int PorcentajeCoberturaMfa
        {
            get
            {
                if (CuentasCriticasActivas == 0)
                {
                    return 0;
                }

                return (int)Math.Round(
                    CuentasConMfa * 100.0 /
                    CuentasCriticasActivas);
            }
        }

        // ProtegeAlert
        public int TotalAlertas { get; set; }

        public int AlertasPendientes { get; set; }

        public int AlertasCriticasAbiertas { get; set; }

        public int AlertasResueltas { get; set; }

        public int PorcentajeAlertasResueltas
        {
            get
            {
                if (TotalAlertas == 0)
                {
                    return 0;
                }

                return (int)Math.Round(
                    AlertasResueltas * 100.0 /
                    TotalAlertas);
            }
        }

        // ProtegeHuman
        public int TotalCapacitaciones { get; set; }

        public int CapacitacionesPendientes { get; set; }

        public int CapacitacionesCompletadas { get; set; }

        public int ParticipantesCapacitados { get; set; }

        public int ParticipantesAprobados { get; set; }

        public int PorcentajeAprobacionCapacitaciones
        {
            get
            {
                if (ParticipantesCapacitados == 0)
                {
                    return 0;
                }

                int porcentaje = (int)Math.Round(
                    ParticipantesAprobados * 100.0 /
                    ParticipantesCapacitados);

                return Math.Min(porcentaje, 100);
            }
        }

        // Plan de continuidad digital
        public int TotalPlanesContinuidad { get; set; }

        public int PlanesActivos { get; set; }

        public int PlanesProbados { get; set; }

        public int PlanesRequierenRevision { get; set; }

        public int PorcentajePlanesProbados
        {
            get
            {
                if (TotalPlanesContinuidad == 0)
                {
                    return 0;
                }

                int porcentaje = (int)Math.Round(
                    PlanesProbados * 100.0 /
                    TotalPlanesContinuidad);

                return Math.Min(porcentaje, 100);
            }
        }

        public List<Empresa> EmpresasRecientes { get; set; } =
            new();
    }
}