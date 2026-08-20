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


        // =====================================================
        // PROTEGESCORE - 100 PUNTOS
        // =====================================================

        [Required(ErrorMessage = "Selecciona Sí o No.")]
        [Display(Name = "¿La empresa tiene un inventario actualizado de equipos, sistemas y cuentas?")]
        public bool? TieneInventarioActivos { get; set; }


        [Required(ErrorMessage = "Selecciona Sí o No.")]
        [Display(Name = "¿Las cuentas críticas utilizan autenticación multifactor MFA?")]
        public bool? UtilizaMfa { get; set; }


        [Required(ErrorMessage = "Selecciona Sí o No.")]
        [Display(Name = "¿Se realizan respaldos periódicos de la información crítica?")]
        public bool? RealizaRespaldos { get; set; }


        [Required(ErrorMessage = "Selecciona Sí o No.")]
        [Display(Name = "¿Se ha comprobado que los respaldos pueden restaurarse correctamente?")]
        public bool? PruebaRestauraciones { get; set; }


        [Required(ErrorMessage = "Selecciona Sí o No.")]
        [Display(Name = "¿Los equipos cuentan con protección endpoint o antivirus activa y actualizada?")]
        public bool? TieneProteccionEndpoint { get; set; }


        [Required(ErrorMessage = "Selecciona Sí o No.")]
        [Display(Name = "¿Los sistemas operativos y programas se mantienen actualizados?")]
        public bool? MantieneActualizaciones { get; set; }


        [Required(ErrorMessage = "Selecciona Sí o No.")]
        [Display(Name = "¿La empresa aplica buenas prácticas de contraseñas y control de accesos?")]
        public bool? GestionaContrasenasAccesos { get; set; }


        [Required(ErrorMessage = "Selecciona Sí o No.")]
        [Display(Name = "¿Los empleados reciben capacitación sobre phishing, fraude y seguridad digital?")]
        public bool? CapacitaEmpleados { get; set; }


        [Required(ErrorMessage = "Selecciona Sí o No.")]
        [Display(Name = "¿Existe un plan de continuidad con responsables y prioridades de recuperación?")]
        public bool? TienePlanContinuidad { get; set; }


        // =====================================================
        // RESULTADO
        // =====================================================

        [Range(0, 100)]
        public int Puntuacion { get; set; }


        [Display(Name = "Fecha de evaluación")]
        public DateTime FechaEvaluacion { get; set; }
            = DateTime.Now;


        [StringLength(256)]
        public string? UsuarioEvaluador { get; set; }


        [StringLength(1000)]
        [Display(Name = "Observaciones")]
        public string? Observaciones { get; set; }


        // =====================================================
        // NIVEL
        // =====================================================

        [NotMapped]
        public string Nivel
        {
            get
            {
                return Puntuacion switch
                {
                    >= 90 => "Excelente",
                    >= 75 => "Bueno",
                    >= 60 => "Moderado",
                    >= 40 => "Bajo",
                    _ => "Crítico"
                };
            }
        }
    }
}