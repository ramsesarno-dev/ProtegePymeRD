using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProtegePymeRD.ViewModels;

namespace ProtegePymeRD.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class UsuariosController : Controller
    {
        private readonly UserManager<IdentityUser>
            _userManager;

        private readonly RoleManager<IdentityRole>
            _roleManager;

        public UsuariosController(
            UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        // GET: Usuarios
        public async Task<IActionResult> Index()
        {
            var usuariosIdentity =
                await _userManager.Users
                    .AsNoTracking()
                    .OrderBy(usuario => usuario.Email)
                    .ToListAsync();

            string? usuarioActualId =
                _userManager.GetUserId(User);

            var usuarios =
                new List<UsuarioListaViewModel>();

            foreach (var usuario in usuariosIdentity)
            {
                var roles =
                    await _userManager
                        .GetRolesAsync(usuario);

                usuarios.Add(
                    new UsuarioListaViewModel
                    {
                        Id = usuario.Id,

                        Correo =
                            usuario.Email ??
                            usuario.UserName ??
                            "Sin correo",

                        Rol = roles.Any()
                            ? string.Join(", ", roles)
                            : "Sin rol",

                        EsUsuarioActual =
                            usuario.Id ==
                            usuarioActualId
                    });
            }

            return View(usuarios);
        }

        // GET: Usuarios/Create
        public IActionResult Create()
        {
            return View(
                new CrearUsuarioViewModel
                {
                    Rol = "Analista"
                });
        }

        // POST: Usuarios/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            CrearUsuarioViewModel modelo)
        {
            string[] rolesPermitidos =
            {
                "Administrador",
                "Analista"
            };

            if (!rolesPermitidos.Contains(modelo.Rol))
            {
                ModelState.AddModelError(
                    nameof(modelo.Rol),
                    "El rol seleccionado no es válido.");
            }

            if (!ModelState.IsValid)
            {
                return View(modelo);
            }

            modelo.Correo = modelo.Correo.Trim();

            var usuarioExistente =
                await _userManager.FindByEmailAsync(
                    modelo.Correo);

            if (usuarioExistente != null)
            {
                ModelState.AddModelError(
                    nameof(modelo.Correo),
                    "Ya existe un usuario con este correo.");

                return View(modelo);
            }

            bool existeRol =
                await _roleManager.RoleExistsAsync(
                    modelo.Rol);

            if (!existeRol)
            {
                ModelState.AddModelError(
                    nameof(modelo.Rol),
                    "El rol seleccionado no existe.");

                return View(modelo);
            }

            var usuario = new IdentityUser
            {
                UserName = modelo.Correo,
                Email = modelo.Correo,

                // El usuario es creado internamente
                // por un administrador.
                EmailConfirmed = true
            };

            var resultadoUsuario =
                await _userManager.CreateAsync(
                    usuario,
                    modelo.Contrasena);

            if (!resultadoUsuario.Succeeded)
            {
                foreach (var error in
                    resultadoUsuario.Errors)
                {
                    ModelState.AddModelError(
                        string.Empty,
                        error.Description);
                }

                return View(modelo);
            }

            var resultadoRol =
                await _userManager.AddToRoleAsync(
                    usuario,
                    modelo.Rol);

            if (!resultadoRol.Succeeded)
            {
                await _userManager.DeleteAsync(usuario);

                foreach (var error in resultadoRol.Errors)
                {
                    ModelState.AddModelError(
                        string.Empty,
                        error.Description);
                }

                return View(modelo);
            }

            TempData["MensajeExito"] =
                $"El usuario {modelo.Correo} fue creado " +
                $"con el rol {modelo.Rol}.";

            return RedirectToAction(nameof(Index));
        }

        // POST: Usuarios/CambiarRol
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CambiarRol(
            string id,
            string rol)
        {
            string[] rolesPermitidos =
            {
                "Administrador",
                "Analista"
            };

            if (!rolesPermitidos.Contains(rol))
            {
                return BadRequest();
            }

            var usuario =
                await _userManager.FindByIdAsync(id);

            if (usuario == null)
            {
                return NotFound();
            }

            string? usuarioActualId =
                _userManager.GetUserId(User);

            if (usuario.Id == usuarioActualId &&
                rol != "Administrador")
            {
                TempData["MensajeAdvertencia"] =
                    "No puedes quitarte tu propio rol " +
                    "de Administrador.";

                return RedirectToAction(nameof(Index));
            }

            var rolesActuales =
                await _userManager.GetRolesAsync(usuario);

            bool esAdministrador =
                rolesActuales.Contains("Administrador");

            if (esAdministrador &&
                rol != "Administrador")
            {
                var administradores =
                    await _userManager.GetUsersInRoleAsync(
                        "Administrador");

                if (administradores.Count <= 1)
                {
                    TempData["MensajeAdvertencia"] =
                        "Debe existir al menos un Administrador.";

                    return RedirectToAction(nameof(Index));
                }
            }

            if (rolesActuales.Any())
            {
                var resultadoEliminacion =
                    await _userManager.RemoveFromRolesAsync(
                        usuario,
                        rolesActuales);

                if (!resultadoEliminacion.Succeeded)
                {
                    TempData["MensajeAdvertencia"] =
                        "No se pudo actualizar el rol.";

                    return RedirectToAction(nameof(Index));
                }
            }

            var resultadoAsignacion =
                await _userManager.AddToRoleAsync(
                    usuario,
                    rol);

            if (!resultadoAsignacion.Succeeded)
            {
                foreach (string rolAnterior in rolesActuales)
                {
                    await _userManager.AddToRoleAsync(
                        usuario,
                        rolAnterior);
                }

                TempData["MensajeAdvertencia"] =
                    "No se pudo asignar el nuevo rol.";

                return RedirectToAction(nameof(Index));
            }

            TempData["MensajeExito"] =
                $"El rol de {usuario.Email} fue actualizado.";

            return RedirectToAction(nameof(Index));
        }

        // POST: Usuarios/Eliminar
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Eliminar(string id)
        {
            var usuario =
                await _userManager.FindByIdAsync(id);

            if (usuario == null)
            {
                return NotFound();
            }

            string? usuarioActualId =
                _userManager.GetUserId(User);

            if (usuario.Id == usuarioActualId)
            {
                TempData["MensajeAdvertencia"] =
                    "No puedes eliminar tu propia cuenta.";

                return RedirectToAction(nameof(Index));
            }

            bool esAdministrador =
                await _userManager.IsInRoleAsync(
                    usuario,
                    "Administrador");

            if (esAdministrador)
            {
                var administradores =
                    await _userManager.GetUsersInRoleAsync(
                        "Administrador");

                if (administradores.Count <= 1)
                {
                    TempData["MensajeAdvertencia"] =
                        "No puedes eliminar al único Administrador.";

                    return RedirectToAction(nameof(Index));
                }
            }

            var resultado =
                await _userManager.DeleteAsync(usuario);

            if (!resultado.Succeeded)
            {
                TempData["MensajeAdvertencia"] =
                    "No se pudo eliminar el usuario.";

                return RedirectToAction(nameof(Index));
            }

            TempData["MensajeExito"] =
                $"El usuario {usuario.Email} fue eliminado.";

            return RedirectToAction(nameof(Index));
        }
    }
}