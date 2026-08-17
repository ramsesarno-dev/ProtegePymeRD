using System.ComponentModel.DataAnnotations;

namespace ProtegePymeRD.Models
{
    public class CuentaCritica
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Empresa")]
        public int EmpresaId { get; set; }

        public Empresa? Empresa { get; set; }

        [Required(ErrorMessage = "Indica el servicio o sistema.")]
        [StringLength(150)]
        [Display(Name = "Servicio o sistema")]
        public string NombreServicio { get; set; } = string.Empty;

        [Required(ErrorMessage = "Indica el usuario o correo.")]
        [StringLength(150)]
        [Display(Name = "Usuario o correo asociado")]
        public string UsuarioCorreo { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Categoría")]
        public CategoriaCuenta Categoria { get; set; }

        [Required]
        [Display(Name = "Nivel de privilegio")]
        public NivelPrivilegio NivelPrivilegio { get; set; }

        [Display(Name = "Autenticación multifactor MFA")]
        public bool TieneMfa { get; set; }

        [Display(Name = "Cumple con mínimo privilegio")]
        public bool CumpleMinimoPrivilegio { get; set; }

        [Display(Name = "Cuenta activa")]
        public bool Activa { get; set; } = true;

        [Required(ErrorMessage = "Indica la fecha de revisión.")]
        [DataType(DataType.Date)]
        [Display(Name = "Fecha de última revisión")]
        public DateTime FechaRevision { get; set; } = DateTime.Today;

        [Required(ErrorMessage = "Indica el responsable de la cuenta.")]
        [StringLength(150)]
        [Display(Name = "Responsable")]
        public string Responsable { get; set; } = string.Empty;

        [StringLength(1000)]
        [Display(Name = "Observaciones")]
        public string? Observaciones { get; set; }

        public DateTime FechaRegistro { get; set; } = DateTime.Now;
    }

    public enum CategoriaCuenta
    {
        Correo = 1,
        Banca = 2,
        Nube = 3,
        SistemaEmpresarial = 4,
        RedSocial = 5,
        AdministracionTecnica = 6,
        Otro = 7
    }

    public enum NivelPrivilegio
    {
        Estandar = 1,
        Administrador = 2,
        Propietario = 3,
        Compartida = 4
    }
}