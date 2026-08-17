using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProtegePymeRD.Data;
using ProtegePymeRD.Models;

namespace ProtegePymeRD.Controllers
{
    [Authorize]
    public class AlertasSeguridadController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AlertasSeguridadController(
            ApplicationDbContext context)
        {
            _context = context;
        }

        // Mostrar alertas de una empresa
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

            var alertas = await _context.AlertasSeguridad
                .AsNoTracking()
                .Where(alerta =>
                    alerta.EmpresaId == empresaId)
                .OrderByDescending(alerta =>
                    alerta.FechaDeteccion)
                .ToListAsync();

            ViewBag.Empresa = empresa;

            return View(alertas);
        }

        // Mostrar formulario de registro
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

            var alerta = new AlertaSeguridad
            {
                EmpresaId = empresaId,
                Estado = EstadoAlerta.Abierta,
                Severidad = SeveridadAlerta.Media,
                FechaDeteccion = DateTime.Now,
                FechaRegistro = DateTime.Now
            };

            return View(alerta);
        }

        // Guardar alerta
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind(
                "EmpresaId," +
                "Titulo," +
                "Descripcion," +
                "Tipo," +
                "Severidad," +
                "FechaDeteccion," +
                "Responsable")]
            AlertaSeguridad alerta)
        {
            var empresaExiste = await _context.Empresas
                .AnyAsync(empresa =>
                    empresa.Id == alerta.EmpresaId);

            if (!empresaExiste)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                ViewBag.Empresa = await _context.Empresas
                    .AsNoTracking()
                    .FirstAsync(empresa =>
                        empresa.Id == alerta.EmpresaId);

                return View(alerta);
            }

            alerta.Estado = EstadoAlerta.Abierta;
            alerta.FechaResolucion = null;
            alerta.FechaRegistro = DateTime.Now;

            _context.AlertasSeguridad.Add(alerta);
            await _context.SaveChangesAsync();

            TempData["MensajeExito"] =
                "La alerta fue registrada correctamente.";

            return RedirectToAction(
                nameof(Index),
                new { empresaId = alerta.EmpresaId });
        }

        // Mostrar formulario para atender una alerta
        [HttpGet]
        public async Task<IActionResult> Resolver(int id)
        {
            var alerta = await _context.AlertasSeguridad
                .Include(alerta => alerta.Empresa)
                .AsNoTracking()
                .FirstOrDefaultAsync(alerta =>
                    alerta.Id == id);

            if (alerta == null)
            {
                return NotFound();
            }

            return View(alerta);
        }

        // Actualizar estado y respuesta
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Resolver(
            int id,
            EstadoAlerta estado,
            string? responsable,
            string? accionTomada)
        {
            var alerta = await _context.AlertasSeguridad
                .FirstOrDefaultAsync(alerta =>
                    alerta.Id == id);

            if (alerta == null)
            {
                return NotFound();
            }

            alerta.Estado = estado;
            alerta.Responsable = responsable?.Trim();
            alerta.AccionTomada = accionTomada?.Trim();

            if (estado == EstadoAlerta.Resuelta)
            {
                alerta.FechaResolucion = DateTime.Now;
            }
            else
            {
                alerta.FechaResolucion = null;
            }

            await _context.SaveChangesAsync();

            TempData["MensajeExito"] =
                "La alerta fue actualizada correctamente.";

            return RedirectToAction(
                nameof(Index),
                new { empresaId = alerta.EmpresaId });
        }

        // Eliminar alerta
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Eliminar(int id)
        {
            var alerta = await _context.AlertasSeguridad
                .FirstOrDefaultAsync(alerta =>
                    alerta.Id == id);

            if (alerta == null)
            {
                return NotFound();
            }

            int empresaId = alerta.EmpresaId;

            _context.AlertasSeguridad.Remove(alerta);
            await _context.SaveChangesAsync();

            TempData["MensajeExito"] =
                "La alerta fue eliminada correctamente.";

            return RedirectToAction(
                nameof(Index),
                new { empresaId });
        }
    }
}