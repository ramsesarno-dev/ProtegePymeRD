using System.ComponentModel.DataAnnotations;

namespace ProtegePymeRD.Models
{
    public class Empresa
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre de la empresa es obligatorio.")]
        [StringLength(150)]
        [Display(Name = "Nombre comercial")]
        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "El RNC o la cédula es obligatorio.")]
        [StringLength(11, MinimumLength = 9,
            ErrorMessage = "El RNC o la cédula debe tener entre 9 y 11 números.")]
        [RegularExpression(@"^\d+$",
            ErrorMessage = "Solo se permiten números.")]
        [Display(Name = "RNC o cédula")]
        public string Rnc { get; set; } = string.Empty;

        [Required(ErrorMessage = "El sector empresarial es obligatorio.")]
        [StringLength(100)]
        [Display(Name = "Sector empresarial")]
        public string Sector { get; set; } = string.Empty;

        [Range(1, 500,
            ErrorMessage = "La cantidad de empleados debe estar entre 1 y 500.")]
        [Display(Name = "Cantidad de empleados")]
        public int CantidadEmpleados { get; set; }

        [Required(ErrorMessage = "El correo de contacto es obligatorio.")]
        [EmailAddress(ErrorMessage = "Introduce un correo electrónico válido.")]
        [StringLength(150)]
        [Display(Name = "Correo de contacto")]
        public string CorreoContacto { get; set; } = string.Empty;

        [Phone(ErrorMessage = "Introduce un número de teléfono válido.")]
        [StringLength(20)]
        [Display(Name = "Teléfono")]
        public string? Telefono { get; set; }

        [Required]
        [Display(Name = "Plan contratado")]
        public PlanServicio Plan { get; set; }

        [Range(0, 100)]
        [Display(Name = "ProtegeScore")]
        public int ProtegeScore { get; set; }

        [Display(Name = "Fecha de incorporación")]
        public DateTime FechaRegistro { get; set; } = DateTime.Now;

        public bool Activa { get; set; } = true;
    }

    public enum PlanServicio
    {
        Esencial = 1,
        Negocio = 2,
        Continuidad = 3
    }
}