using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProtegePymeRD.Data;
using ProtegePymeRD.Models;

namespace ProtegePymeRD.Controllers
{
    [Authorize]
    public class RespaldosController : Controller
    {
        private readonly ApplicationDbContext _context;

        public RespaldosController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(int? empresaId)
        {
            IQueryable<Respaldo> consulta = _context.Respaldos
                .Include(respaldo => respaldo.Empresa);

            if (empresaId.HasValue)
            {
                consulta = consulta.Where(respaldo =>
                    respaldo.EmpresaId == empresaId.Value);

                var empresa = await _context.Empresas
                    .AsNoTracking()
                    .FirstOrDefaultAsync(empresa =>
                        empresa.Id == empresaId.Value);

                if (empresa == null)
                {
                    return NotFound();
                }

                ViewBag.EmpresaNombre = empresa.Nombre;
                ViewBag.EmpresaId = empresa.Id;
            }

            var respaldos = await consulta
                .AsNoTracking()
                .OrderByDescending(respaldo =>
                    respaldo.FechaUltimoRespaldo)
                .ToListAsync();

            ViewBag.TotalRespaldos = respaldos.Count;

            ViewBag.RespaldosExitosos = respaldos.Count(respaldo =>
                respaldo.Estado == EstadoRespaldo.Exitoso);

            ViewBag.RestauracionesProbadas =
                respaldos.Count(respaldo =>
                    respaldo.RestauracionProbada);

            return View(respaldos);
        }

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

            ViewBag.EmpresaNombre = empresa.Nombre;

            var respaldo = new Respaldo
            {
                EmpresaId = empresa.Id,
                FechaUltimoRespaldo = DateTime.Now
            };

            return View(respaldo);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind(
                "EmpresaId," +
                "RecursoProtegido," +
                "Tipo," +
                "ProveedorDestino," +
                "Frecuencia," +
                "Estado," +
                "FechaUltimoRespaldo," +
                "RestauracionProbada," +
                "FechaUltimaPrueba," +
                "Observaciones")]
            Respaldo respaldo)
        {
            var empresa = await _context.Empresas
                .AsNoTracking()
                .FirstOrDefaultAsync(empresa =>
                    empresa.Id == respaldo.EmpresaId);

            if (empresa == null)
            {
                return NotFound();
            }

            if (respaldo.FechaUltimoRespaldo == default)
            {
                ModelState.AddModelError(
                    nameof(Respaldo.FechaUltimoRespaldo),
                    "Indica la fecha del último respaldo.");
            }

            if (respaldo.FechaUltimoRespaldo >
                DateTime.Now.AddMinutes(5))
            {
                ModelState.AddModelError(
                    nameof(Respaldo.FechaUltimoRespaldo),
                    "La fecha del respaldo no puede estar en el futuro.");
            }

            if (respaldo.RestauracionProbada &&
                respaldo.FechaUltimaPrueba == null)
            {
                ModelState.AddModelError(
                    nameof(Respaldo.FechaUltimaPrueba),
                    "Indica cuándo se probó la restauración.");
            }

            if (!respaldo.RestauracionProbada)
            {
                respaldo.FechaUltimaPrueba = null;
            }

            if (ModelState.IsValid)
            {
                respaldo.RecursoProtegido =
                    respaldo.RecursoProtegido.Trim();

                respaldo.ProveedorDestino =
                    respaldo.ProveedorDestino.Trim();

                respaldo.Observaciones =
                    respaldo.Observaciones?.Trim();

                respaldo.FechaRegistro = DateTime.Now;

                _context.Respaldos.Add(respaldo);
                await _context.SaveChangesAsync();

                TempData["MensajeExito"] =
                    "El respaldo fue registrado correctamente.";

                return RedirectToAction(
                    nameof(Index),
                    new { empresaId = respaldo.EmpresaId });
            }

            ViewBag.EmpresaNombre = empresa.Nombre;

            return View(respaldo);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Eliminar(int id)
        {
            var respaldo = await _context.Respaldos
                .FirstOrDefaultAsync(respaldo =>
                    respaldo.Id == id);

            if (respaldo == null)
            {
                return NotFound();
            }

            int empresaId = respaldo.EmpresaId;

            _context.Respaldos.Remove(respaldo);
            await _context.SaveChangesAsync();

            TempData["MensajeExito"] =
                "El registro de respaldo fue eliminado.";

            return RedirectToAction(
                nameof(Index),
                new { empresaId });
        }
    }
}