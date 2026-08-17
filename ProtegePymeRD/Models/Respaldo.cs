using System.ComponentModel.DataAnnotations;

namespace ProtegePymeRD.Models
{
    public class Respaldo
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Empresa")]
        public int EmpresaId { get; set; }

        public Empresa? Empresa { get; set; }

        [Required(ErrorMessage = "Indica la información protegida.")]
        [StringLength(150)]
        [Display(Name = "Información o recurso protegido")]
        public string RecursoProtegido { get; set; } = string.Empty;

        [Required(ErrorMessage = "Selecciona el tipo de respaldo.")]
        [Display(Name = "Tipo de respaldo")]
        public TipoRespaldo Tipo { get; set; }

        [Required(ErrorMessage = "Indica el proveedor o destino.")]
        [StringLength(150)]
        [Display(Name = "Proveedor o destino")]
        public string ProveedorDestino { get; set; } = string.Empty;

        [Required(ErrorMessage = "Selecciona la frecuencia.")]
        [Display(Name = "Frecuencia")]
        public FrecuenciaRespaldo Frecuencia { get; set; }

        [Required]
        [Display(Name = "Estado del último respaldo")]
        public EstadoRespaldo Estado { get; set; }

        [Required]
        [Display(Name = "Fecha del último respaldo")]
        [DataType(DataType.DateTime)]
        public DateTime FechaUltimoRespaldo { get; set; }

        [Display(Name = "Restauración probada")]
        public bool RestauracionProbada { get; set; }

        [Display(Name = "Fecha de la última prueba")]
        [DataType(DataType.Date)]
        public DateTime? FechaUltimaPrueba { get; set; }

        [StringLength(1000)]
        [Display(Name = "Observaciones")]
        public string? Observaciones { get; set; }

        public DateTime FechaRegistro { get; set; } = DateTime.Now;
    }

    public enum TipoRespaldo
    {
        Nube = 1,
        Local = 2,
        Hibrido = 3
    }

    public enum FrecuenciaRespaldo
    {
        Diario = 1,
        Semanal = 2,
        Quincenal = 3,
        Mensual = 4
    }

    public enum EstadoRespaldo
    {
        Exitoso = 1,
        Fallido = 2,
        Pendiente = 3
    }
}