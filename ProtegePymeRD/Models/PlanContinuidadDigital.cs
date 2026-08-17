using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProtegePymeRD.Models
{
    public enum EstadoPlanContinuidad
    {
        Borrador = 1,
        EnRevision = 2,
        Activo = 3,
        RequiereActualizacion = 4,
        Archivado = 5
    }

    public class PlanContinuidadDigital
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Empresa")]
        public int EmpresaId { get; set; }

        public Empresa? Empresa { get; set; }

        [Required(ErrorMessage = "El nombre del plan es obligatorio.")]
        [StringLength(150)]
        [Display(Name = "Nombre del plan")]
        public string Nombre { get; set; } =
            "Plan de continuidad digital";

        [Required]
        [Display(Name = "Estado")]
        public EstadoPlanContinuidad Estado { get; set; } =
            EstadoPlanContinuidad.Borrador;

        [Required(ErrorMessage = "Indica el responsable del plan.")]
        [StringLength(150)]
        [Display(Name = "Responsable")]
        public string Responsable { get; set; } = string.Empty;

        [Required(ErrorMessage = "Indica un contacto de emergencia.")]
        [StringLength(150)]
        [Display(Name = "Contacto de emergencia")]
        public string ContactoEmergencia { get; set; } = string.Empty;

        [Required(ErrorMessage = "Describe los sistemas críticos.")]
        [StringLength(2000)]
        [Display(Name = "Sistemas y servicios críticos")]
        public string SistemasCriticos { get; set; } = string.Empty;

        [Range(
            1,
            168,
            ErrorMessage = "El tiempo de recuperación debe estar entre 1 y 168 horas.")]
        [Display(Name = "Tiempo máximo de recuperación RTO")]
        public int RtoHoras { get; set; } = 24;

        [Range(
            0,
            168,
            ErrorMessage = "La pérdida máxima de datos debe estar entre 0 y 168 horas.")]
        [Display(Name = "Pérdida máxima aceptable de datos RPO")]
        public int RpoHoras { get; set; } = 24;

        [Required(ErrorMessage = "Describe cómo responder ante un incidente.")]
        [StringLength(2000)]
        [Display(Name = "Procedimiento de respuesta")]
        public string ProcedimientoRespuesta { get; set; } = string.Empty;

        [Required(ErrorMessage = "Describe cómo se recuperarán las operaciones.")]
        [StringLength(2000)]
        [Display(Name = "Procedimiento de recuperación")]
        public string ProcedimientoRecuperacion { get; set; } = string.Empty;

        [Required(ErrorMessage = "Describe cómo se comunicarán durante una emergencia.")]
        [StringLength(2000)]
        [Display(Name = "Plan de comunicación")]
        public string PlanComunicacion { get; set; } = string.Empty;

        [Display(Name = "Plan probado")]
        public bool PlanProbado { get; set; }

        [Display(Name = "Fecha de la última prueba")]
        public DateTime? FechaUltimaPrueba { get; set; }

        [Required]
        [Display(Name = "Próxima revisión")]
        public DateTime ProximaRevision { get; set; } =
            DateTime.Today.AddMonths(6);

        [Display(Name = "Fecha de creación")]
        public DateTime FechaCreacion { get; set; } = DateTime.Now;

        [Display(Name = "Última actualización")]
        public DateTime FechaUltimaActualizacion { get; set; } =
            DateTime.Now;

        [StringLength(1000)]
        [Display(Name = "Observaciones")]
        public string? Observaciones { get; set; }

        [NotMapped]
        public bool RequiereRevision
        {
            get
            {
                return ProximaRevision.Date <= DateTime.Today;
            }
        }

        [NotMapped]
        public string EstadoRevision
        {
            get
            {
                if (RequiereRevision)
                {
                    return "Revisión pendiente";
                }

                if (!PlanProbado)
                {
                    return "Prueba pendiente";
                }

                return "Plan vigente";
            }
        }
    }
}