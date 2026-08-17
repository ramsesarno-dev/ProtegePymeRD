using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProtegePymeRD.Data;
using ProtegePymeRD.Models;

namespace ProtegePymeRD.Controllers
{
    [Authorize]
    public class DiagnosticosController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DiagnosticosController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var diagnosticos = await _context.Diagnosticos
                .AsNoTracking()
                .Include(diagnostico => diagnostico.Empresa)
                .OrderByDescending(diagnostico =>
                    diagnostico.FechaEvaluacion)
                .ToListAsync();

            return View(diagnosticos);
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

            var diagnostico = new Diagnostico
            {
                EmpresaId = empresa.Id
            };

            return View(diagnostico);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind(
                "EmpresaId," +
                "TieneInventarioActivos," +
                "GestionaVulnerabilidades," +
                "UtilizaMfa," +
                "MantieneActualizaciones," +
                "CapacitaEmpleados," +
                "MonitoreaAlertas," +
                "TienePlanIncidentes," +
                "TieneResponsables," +
                "RealizaRespaldos," +
                "PruebaRestauraciones," +
                "Observaciones")]
            Diagnostico diagnostico)
        {
            var empresa = await _context.Empresas
                .FirstOrDefaultAsync(empresa =>
                    empresa.Id == diagnostico.EmpresaId);

            if (empresa == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                diagnostico.Puntuacion =
                    CalcularPuntuacion(diagnostico);

                diagnostico.FechaEvaluacion = DateTime.Now;

                diagnostico.UsuarioEvaluador =
                    User.Identity?.Name;

                _context.Diagnosticos.Add(diagnostico);

                empresa.ProtegeScore =
                    diagnostico.Puntuacion;

                await _context.SaveChangesAsync();

                TempData["MensajeExito"] =
                    "El diagnóstico fue completado correctamente.";

                return RedirectToAction(
                    nameof(Resultado),
                    new { id = diagnostico.Id });
            }

            ViewBag.EmpresaNombre = empresa.Nombre;

            return View(diagnostico);
        }

        public async Task<IActionResult> Resultado(int id)
        {
            var diagnostico = await _context.Diagnosticos
                .AsNoTracking()
                .Include(resultado => resultado.Empresa)
                .FirstOrDefaultAsync(resultado =>
                    resultado.Id == id);

            if (diagnostico == null)
            {
                return NotFound();
            }

            return View(diagnostico);
        }

        public async Task<IActionResult> Historial(int empresaId)
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

            var diagnosticos = await _context.Diagnosticos
                .AsNoTracking()
                .Where(diagnostico =>
                    diagnostico.EmpresaId == empresaId)
                .OrderByDescending(diagnostico =>
                    diagnostico.FechaEvaluacion)
                .ToListAsync();

            return View(diagnosticos);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Eliminar(int id)
        {
            var diagnostico = await _context.Diagnosticos
                .FirstOrDefaultAsync(diagnostico =>
                    diagnostico.Id == id);

            if (diagnostico == null)
            {
                return NotFound();
            }

            int empresaId = diagnostico.EmpresaId;

            _context.Diagnosticos.Remove(diagnostico);
            await _context.SaveChangesAsync();

            var empresa = await _context.Empresas
                .FirstOrDefaultAsync(empresa =>
                    empresa.Id == empresaId);

            if (empresa != null)
            {
                var ultimoDiagnostico = await _context.Diagnosticos
                    .Where(resultado =>
                        resultado.EmpresaId == empresaId)
                    .OrderByDescending(resultado =>
                        resultado.FechaEvaluacion)
                    .FirstOrDefaultAsync();

                empresa.ProtegeScore =
                    ultimoDiagnostico?.Puntuacion ?? 0;

                await _context.SaveChangesAsync();
            }

            TempData["MensajeExito"] =
                "La evaluación fue eliminada correctamente.";

            return RedirectToAction(
                nameof(Historial),
                new { empresaId });
        }
        private static int CalcularPuntuacion(
            Diagnostico diagnostico)
        {
            bool?[] respuestas =
            {
                diagnostico.TieneInventarioActivos,
                diagnostico.GestionaVulnerabilidades,
                diagnostico.UtilizaMfa,
                diagnostico.MantieneActualizaciones,
                diagnostico.CapacitaEmpleados,
                diagnostico.MonitoreaAlertas,
                diagnostico.TienePlanIncidentes,
                diagnostico.TieneResponsables,
                diagnostico.RealizaRespaldos,
                diagnostico.PruebaRestauraciones
            };

            int respuestasPositivas =
                respuestas.Count(respuesta =>
                    respuesta == true);

            return respuestasPositivas * 10;
        }
    }
}