using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProtegePymeRD.Data;
using ProtegePymeRD.Models;

namespace ProtegePymeRD.Controllers
{
    [Authorize]
    public class CuentasCriticasController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CuentasCriticasController(
            ApplicationDbContext context)
        {
            _context = context;
        }

        // Historial de cuentas de una empresa
        public async Task<IActionResult> Index(int empresaId)
        {
            var empresa = await _context.Empresas
                .AsNoTracking()
                .FirstOrDefaultAsync(empresa =>
                    empresa.Id == empresaId);

            if (empresa == null)
            {
                return NotFound();
            }

            var cuentas = await _context.CuentasCriticas
                .AsNoTracking()
                .Where(cuenta =>
                    cuenta.EmpresaId == empresaId)
                .OrderByDescending(cuenta =>
                    cuenta.FechaRevision)
                .ToListAsync();

            ViewBag.Empresa = empresa;

            return View(cuentas);
        }

        // Mostrar formulario
        [HttpGet]
        public async Task<IActionResult> Create(int empresaId)
        {
            var empresa = await _context.Empresas
                .AsNoTracking()
                .FirstOrDefaultAsync(empresa =>
                    empresa.Id == empresaId);

            if (empresa == null)
            {
                return NotFound();
            }

            ViewBag.Empresa = empresa;

            var cuenta = new CuentaCritica
            {
                EmpresaId = empresaId,
                Activa = true,
                FechaRevision = DateTime.Now,
                FechaRegistro = DateTime.Now
            };

            return View(cuenta);
        }

        // Guardar cuenta
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind(
                "EmpresaId," +
                "NombreServicio," +
                "UsuarioCorreo," +
                "Categoria," +
                "NivelPrivilegio," +
                "TieneMfa," +
                "CumpleMinimoPrivilegio," +
                "Activa," +
                "FechaRevision," +
                "Responsable," +
                "Observaciones")]
            CuentaCritica cuenta)
        {
            var empresaExiste = await _context.Empresas
                .AnyAsync(empresa =>
                    empresa.Id == cuenta.EmpresaId);

            if (!empresaExiste)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                ViewBag.Empresa = await _context.Empresas
                    .AsNoTracking()
                    .FirstAsync(empresa =>
                        empresa.Id == cuenta.EmpresaId);

                return View(cuenta);
            }

            cuenta.FechaRegistro = DateTime.Now;

            _context.CuentasCriticas.Add(cuenta);
            await _context.SaveChangesAsync();

            TempData["MensajeExito"] =
                "La cuenta crítica fue registrada correctamente.";

            return RedirectToAction(
                nameof(Index),
                new { empresaId = cuenta.EmpresaId });
        }

        // Eliminar cuenta
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Eliminar(int id)
        {
            var cuenta = await _context.CuentasCriticas
                .FirstOrDefaultAsync(cuenta =>
                    cuenta.Id == id);

            if (cuenta == null)
            {
                return NotFound();
            }

            int empresaId = cuenta.EmpresaId;

            _context.CuentasCriticas.Remove(cuenta);
            await _context.SaveChangesAsync();

            TempData["MensajeExito"] =
                "La cuenta crítica fue eliminada correctamente.";

            return RedirectToAction(
                nameof(Index),
                new { empresaId });
        }
    }
}