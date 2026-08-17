using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProtegePymeRD.Models
{
    public enum TemaCapacitacion
    {
        [Display(Name = "Phishing")]
        Phishing = 1,

        [Display(Name = "Fraude digital")]
        FraudeDigital = 2,

        [Display(Name = "Ingeniería social")]
        IngenieriaSocial = 3,

        [Display(Name = "Contraseñas seguras")]
        ContrasenasSeguras = 4,

        [Display(Name = "Uso seguro del correo")]
        UsoSeguroCorreo = 5,

        [Display(Name = "Protección de datos")]
        ProteccionDatos = 6,

        [Display(Name = "Otro")]
        Otro = 7
    }

    public enum EstadoCapacitacion
    {
        [Display(Name = "Programada")]
        Programada = 1,

        [Display(Name = "En curso")]
        EnCurso = 2,

        [Display(Name = "Completada")]
        Completada = 3,

        [Display(Name = "Cancelada")]
        Cancelada = 4
    }

    public class CapacitacionSeguridad
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Empresa")]
        public int EmpresaId { get; set; }

        public Empresa? Empresa { get; set; }

        [Required(ErrorMessage = "Escribe el título de la capacitación.")]
        [StringLength(150)]
        [Display(Name = "Título")]
        public string Titulo { get; set; } = string.Empty;

        [Required(ErrorMessage = "Selecciona el tema.")]
        [Display(Name = "Tema")]
        public TemaCapacitacion Tema { get; set; }

        [Required(ErrorMessage = "Describe el contenido.")]
        [StringLength(1000)]
        [Display(Name = "Descripción")]
        public string Descripcion { get; set; } = string.Empty;

        [Range(
            5,
            240,
            ErrorMessage = "La duración debe estar entre 5 y 240 minutos.")]
        [Display(Name = "Duración en minutos")]
        public int DuracionMinutos { get; set; } = 15;

        [Required]
        [Display(Name = "Estado")]
        public EstadoCapacitacion Estado { get; set; } =
            EstadoCapacitacion.Programada;

        [Required]
        [Display(Name = "Fecha programada")]
        public DateTime FechaProgramada { get; set; } =
            DateTime.Now;

        [Display(Name = "Fecha completada")]
        public DateTime? FechaCompletada { get; set; }

        [Range(
            0,
            10000,
            ErrorMessage = "La cantidad de participantes no es válida.")]
        [Display(Name = "Participantes")]
        public int CantidadParticipantes { get; set; }

        [Range(
            0,
            10000,
            ErrorMessage = "La cantidad de aprobados no es válida.")]
        [Display(Name = "Participantes aprobados")]
        public int CantidadAprobados { get; set; }

        [Required(ErrorMessage = "Escribe el responsable.")]
        [StringLength(150)]
        [Display(Name = "Responsable")]
        public string Responsable { get; set; } = string.Empty;

        [StringLength(1000)]
        [Display(Name = "Observaciones")]
        public string? Observaciones { get; set; }

        [Display(Name = "Fecha de registro")]
        public DateTime FechaRegistro { get; set; } =
            DateTime.Now;

        [NotMapped]
        public int PorcentajeAprobacion
        {
            get
            {
                if (CantidadParticipantes == 0)
                {
                    return 0;
                }

                int porcentaje = (int)Math.Round(
                    CantidadAprobados * 100.0 /
                    CantidadParticipantes);

                return Math.Min(porcentaje, 100);
            }
        }
    }
}