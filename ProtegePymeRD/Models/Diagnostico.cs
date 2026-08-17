using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProtegePymeRD.Models
{
    public class Diagnostico
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Empresa")]
        public int EmpresaId { get; set; }

        public Empresa? Empresa { get; set; }

        [Required(ErrorMessage = "Selecciona Sí o No.")]
        [Display(Name = "¿La empresa tiene un inventario actualizado de equipos, sistemas y cuentas?")]
        public bool? TieneInventarioActivos { get; set; }

        [Required(ErrorMessage = "Selecciona Sí o No.")]
        [Display(Name = "¿La empresa identifica y corrige vulnerabilidades de seguridad?")]
        public bool? GestionaVulnerabilidades { get; set; }

        [Required(ErrorMessage = "Selecciona Sí o No.")]
        [Display(Name = "¿Las cuentas críticas utilizan autenticación multifactor MFA?")]
        public bool? UtilizaMfa { get; set; }

        [Required(ErrorMessage = "Selecciona Sí o No.")]
        [Display(Name = "¿Los equipos y programas se mantienen actualizados?")]
        public bool? MantieneActualizaciones { get; set; }

        [Required(ErrorMessage = "Selecciona Sí o No.")]
        [Display(Name = "¿Los empleados reciben capacitación sobre phishing y fraude?")]
        public bool? CapacitaEmpleados { get; set; }

        [Required(ErrorMessage = "Selecciona Sí o No.")]
        [Display(Name = "¿La empresa monitorea alertas y actividades sospechosas?")]
        public bool? MonitoreaAlertas { get; set; }

        [Required(ErrorMessage = "Selecciona Sí o No.")]
        [Display(Name = "¿Existe un procedimiento para responder a incidentes?")]
        public bool? TienePlanIncidentes { get; set; }

        [Required(ErrorMessage = "Selecciona Sí o No.")]
        [Display(Name = "¿Hay responsables definidos para responder a una emergencia digital?")]
        public bool? TieneResponsables { get; set; }

        [Required(ErrorMessage = "Selecciona Sí o No.")]
        [Display(Name = "¿Se realizan respaldos periódicos de la información crítica?")]
        public bool? RealizaRespaldos { get; set; }

        [Required(ErrorMessage = "Selecciona Sí o No.")]
        [Display(Name = "¿Se realizan pruebas para confirmar que los respaldos pueden restaurarse?")]
        public bool? PruebaRestauraciones { get; set; }

        [Range(0, 100)]
        public int Puntuacion { get; set; }

        [Display(Name = "Fecha de evaluación")]
        public DateTime FechaEvaluacion { get; set; } = DateTime.Now;

        [StringLength(256)]
        public string? UsuarioEvaluador { get; set; }

        [StringLength(1000)]
        [Display(Name = "Observaciones")]
        public string? Observaciones { get; set; }

        [NotMapped]
        public string Nivel
        {
            get
            {
                if (Puntuacion >= 80)
                    return "Protección alta";

                if (Puntuacion >= 60)
                    return "Protección moderada";

                if (Puntuacion >= 40)
                    return "Protección baja";

                return "Riesgo crítico";
            }
        }
    }
}