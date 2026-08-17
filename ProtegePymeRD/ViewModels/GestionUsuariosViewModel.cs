using System.ComponentModel.DataAnnotations;

namespace ProtegePymeRD.ViewModels
{
    public class CrearUsuarioViewModel
    {
        [Required(ErrorMessage = "El correo es obligatorio.")]
        [EmailAddress(ErrorMessage = "Introduce un correo válido.")]
        [Display(Name = "Correo electrónico")]
        public string Correo { get; set; } = string.Empty;

        [Required(ErrorMessage = "La contraseña es obligatoria.")]
        [StringLength(
            100,
            MinimumLength = 8,
            ErrorMessage =
                "La contraseña debe tener al menos 8 caracteres.")]
        [DataType(DataType.Password)]
        [Display(Name = "Contraseña")]
        public string Contrasena { get; set; } = string.Empty;

        [Required(ErrorMessage = "Confirma la contraseña.")]
        [DataType(DataType.Password)]
        [Compare(
            nameof(Contrasena),
            ErrorMessage = "Las contraseñas no coinciden.")]
        [Display(Name = "Confirmar contraseña")]
        public string ConfirmarContrasena { get; set; } =
            string.Empty;

        [Required(ErrorMessage = "Selecciona un rol.")]
        [Display(Name = "Rol")]
        public string Rol { get; set; } = "Analista";
    }

    public class UsuarioListaViewModel
    {
        public string Id { get; set; } = string.Empty;

        public string Correo { get; set; } = string.Empty;

        public string Rol { get; set; } = string.Empty;

        public bool EsUsuarioActual { get; set; }
    }
}