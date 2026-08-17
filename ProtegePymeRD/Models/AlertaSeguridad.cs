using System.ComponentModel.DataAnnotations;

namespace ProtegePymeRD.Models
{
    public enum TipoAlerta
    {
        [Display(Name = "Malware")]
        Malware = 1,

        [Display(Name = "Fallo de respaldo")]
        FalloRespaldo = 2,

        [Display(Name = "Vulnerabilidad")]
        Vulnerabilidad = 3,

        [Display(Name = "Acceso sospechoso")]
        AccesoSospechoso = 4,

        [Display(Name = "Equipo fuera de cumplimiento")]
        EquipoFueraCumplimiento = 5,

        [Display(Name = "Otro")]
        Otro = 6
    }

    public enum SeveridadAlerta
    {
        [Display(Name = "Informativa")]
        Informativa = 1,

        [Display(Name = "Baja")]
        Baja = 2,

        [Display(Name = "Media")]
        Media = 3,

        [Display(Name = "Alta")]
        Alta = 4,

        [Display(Name = "Crítica")]
        Critica = 5
    }

    public enum EstadoAlerta
    {
        [Display(Name = "Abierta")]
        Abierta = 1,

        [Display(Name = "En proceso")]
        EnProceso = 2,

        [Display(Name = "Resuelta")]
        Resuelta = 3,

        [Display(Name = "Descartada")]
        Descartada = 4
    }

    public class AlertaSeguridad
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Empresa")]
        public int EmpresaId { get; set; }

        public Empresa? Empresa { get; set; }

        [Required(ErrorMessage = "Escribe el título de la alerta.")]
        [StringLength(150)]
        [Display(Name = "Título")]
        public string Titulo { get; set; } = string.Empty;

        [Required(ErrorMessage = "Describe la situación detectada.")]
        [StringLength(1000)]
        [Display(Name = "Descripción")]
        public string Descripcion { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Tipo de alerta")]
        public TipoAlerta Tipo { get; set; }

        [Required]
        [Display(Name = "Severidad")]
        public SeveridadAlerta Severidad { get; set; }

        [Required]
        [Display(Name = "Estado")]
        public EstadoAlerta Estado { get; set; } =
            EstadoAlerta.Abierta;

        [Required]
        [Display(Name = "Fecha de detección")]
        public DateTime FechaDeteccion { get; set; } =
            DateTime.Now;

        [Display(Name = "Fecha de resolución")]
        public DateTime? FechaResolucion { get; set; }

        [StringLength(150)]
        [Display(Name = "Responsable")]
        public string? Responsable { get; set; }

        [StringLength(1000)]
        [Display(Name = "Acción tomada")]
        public string? AccionTomada { get; set; }

        [Display(Name = "Fecha de registro")]
        public DateTime FechaRegistro { get; set; } =
            DateTime.Now;
    }
}