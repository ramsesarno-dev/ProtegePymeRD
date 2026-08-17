using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProtegePymeRD.Data;
using ProtegePymeRD.Models;

namespace ProtegePymeRD.Controllers
{
    [Authorize]
    public class CapacitacionesSeguridadController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CapacitacionesSeguridadController(
            ApplicationDbContext context)
        {
            _context = context;
        }

        // Mostrar capacitaciones de una empresa
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

            var capacitaciones =
                await _context.CapacitacionesSeguridad
                    .AsNoTracking()
                    .Where(capacitacion =>
                        capacitacion.EmpresaId == empresaId)
                    .OrderByDescending(capacitacion =>
                        capacitacion.FechaProgramada)
                    .ToListAsync();

            ViewBag.Empresa = empresa;

            return View(capacitaciones);
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

            var capacitacion = new CapacitacionSeguridad
            {
                EmpresaId = empresaId,
                Tema = TemaCapacitacion.Phishing,
                DuracionMinutos = 15,
                Estado = EstadoCapacitacion.Programada,
                FechaProgramada = DateTime.Now,
                FechaRegistro = DateTime.Now
            };

            return View(capacitacion);
        }

        // Guardar capacitación
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind(
                "EmpresaId," +
                "Titulo," +
                "Tema," +
                "Descripcion," +
                "DuracionMinutos," +
                "FechaProgramada," +
                "Responsable")]
            CapacitacionSeguridad capacitacion)
        {
            var empresaExiste = await _context.Empresas
                .AnyAsync(empresa =>
                    empresa.Id == capacitacion.EmpresaId);

            if (!empresaExiste)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                ViewBag.Empresa = await _context.Empresas
                    .AsNoTracking()
                    .FirstAsync(empresa =>
                        empresa.Id == capacitacion.EmpresaId);

                return View(capacitacion);
            }

            capacitacion.Estado =
                EstadoCapacitacion.Programada;

            capacitacion.CantidadParticipantes = 0;
            capacitacion.CantidadAprobados = 0;
            capacitacion.FechaCompletada = null;
            capacitacion.FechaRegistro = DateTime.Now;

            _context.CapacitacionesSeguridad.Add(capacitacion);
            await _context.SaveChangesAsync();

            TempData["MensajeExito"] =
                "La capacitación fue programada correctamente.";

            return RedirectToAction(
                nameof(Index),
                new { empresaId = capacitacion.EmpresaId });
        }

        // Mostrar formulario de seguimiento
        [HttpGet]
        public async Task<IActionResult> Completar(int id)
        {
            var capacitacion =
                await _context.CapacitacionesSeguridad
                    .Include(capacitacion =>
                        capacitacion.Empresa)
                    .AsNoTracking()
                    .FirstOrDefaultAsync(capacitacion =>
                        capacitacion.Id == id);

            if (capacitacion == null)
            {
                return NotFound();
            }

            return View(capacitacion);
        }

        // Actualizar capacitación
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Completar(
            int id,
            EstadoCapacitacion estado,
            int cantidadParticipantes,
            int cantidadAprobados,
            string responsable,
            string? observaciones)
        {
            var capacitacion =
                await _context.CapacitacionesSeguridad
                    .FirstOrDefaultAsync(capacitacion =>
                        capacitacion.Id == id);

            if (capacitacion == null)
            {
                return NotFound();
            }

            if (cantidadParticipantes < 0 ||
                cantidadAprobados < 0)
            {
                TempData["MensajeError"] =
                    "Las cantidades no pueden ser negativas.";

                return RedirectToAction(
                    nameof(Completar),
                    new { id });
            }

            if (cantidadAprobados > cantidadParticipantes)
            {
                TempData["MensajeError"] =
                    "Los aprobados no pueden superar a los participantes.";

                return RedirectToAction(
                    nameof(Completar),
                    new { id });
            }

            if (string.IsNullOrWhiteSpace(responsable))
            {
                TempData["MensajeError"] =
                    "Escribe el responsable de la capacitación.";

                return RedirectToAction(
                    nameof(Completar),
                    new { id });
            }

            capacitacion.Estado = estado;
            capacitacion.CantidadParticipantes =
                cantidadParticipantes;

            capacitacion.CantidadAprobados =
                cantidadAprobados;

            capacitacion.Responsable =
                responsable.Trim();

            capacitacion.Observaciones =
                observaciones?.Trim();

            if (estado == EstadoCapacitacion.Completada)
            {
                capacitacion.FechaCompletada =
                    DateTime.Now;
            }
            else
            {
                capacitacion.FechaCompletada = null;
            }

            await _context.SaveChangesAsync();

            TempData["MensajeExito"] =
                "La capacitación fue actualizada correctamente.";

            return RedirectToAction(
                nameof(Index),
                new { empresaId = capacitacion.EmpresaId });
        }

        // Eliminar capacitación
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Eliminar(int id)
        {
            var capacitacion =
                await _context.CapacitacionesSeguridad
                    .FirstOrDefaultAsync(capacitacion =>
                        capacitacion.Id == id);

            if (capacitacion == null)
            {
                return NotFound();
            }

            int empresaId = capacitacion.EmpresaId;

            _context.CapacitacionesSeguridad.Remove(
                capacitacion);

            await _context.SaveChangesAsync();

            TempData["MensajeExito"] =
                "La capacitación fue eliminada correctamente.";

            return RedirectToAction(
                nameof(Index),
                new { empresaId });
        }
    }
}