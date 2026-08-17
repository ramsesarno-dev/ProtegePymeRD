using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProtegePymeRD.Data;
using ProtegePymeRD.Models;

namespace ProtegePymeRD.Controllers
{
    [Authorize]
    public class PlanesContinuidadController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PlanesContinuidadController(
            ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: PlanesContinuidad?empresaId=1
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

            var plan = await _context.PlanesContinuidadDigital
                .AsNoTracking()
                .FirstOrDefaultAsync(plan =>
                    plan.EmpresaId == empresaId);

            ViewBag.Empresa = empresa;

            return View(plan);
        }

        // GET: PlanesContinuidad/Create?empresaId=1
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

            var existePlan = await _context.PlanesContinuidadDigital
                .AnyAsync(plan =>
                    plan.EmpresaId == empresaId);

            if (existePlan)
            {
                TempData["MensajeAdvertencia"] =
                    "Esta empresa ya tiene un plan de continuidad.";

                return RedirectToAction(
                    nameof(Index),
                    new { empresaId });
            }

            ViewBag.Empresa = empresa;

            var plan = new PlanContinuidadDigital
            {
                EmpresaId = empresaId,
                Nombre = $"Plan de continuidad de {empresa.Nombre}",
                Estado = EstadoPlanContinuidad.Borrador,
                RtoHoras = 24,
                RpoHoras = 24,
                ProximaRevision = DateTime.Today.AddMonths(6)
            };

            return View(plan);
        }

        // POST: PlanesContinuidad/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            PlanContinuidadDigital plan)
        {
            var empresa = await _context.Empresas
                .AsNoTracking()
                .FirstOrDefaultAsync(empresa =>
                    empresa.Id == plan.EmpresaId);

            if (empresa == null)
            {
                return NotFound();
            }

            var existePlan = await _context.PlanesContinuidadDigital
                .AnyAsync(planExistente =>
                    planExistente.EmpresaId == plan.EmpresaId);

            if (existePlan)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Esta empresa ya tiene un plan de continuidad.");
            }

            if (!ModelState.IsValid)
            {
                ViewBag.Empresa = empresa;
                return View(plan);
            }

            plan.FechaCreacion = DateTime.Now;
            plan.FechaUltimaActualizacion = DateTime.Now;
            plan.PlanProbado = false;
            plan.FechaUltimaPrueba = null;

            _context.PlanesContinuidadDigital.Add(plan);
            await _context.SaveChangesAsync();

            TempData["MensajeExito"] =
                "El plan de continuidad fue creado correctamente.";

            return RedirectToAction(
                nameof(Index),
                new { empresaId = plan.EmpresaId });
        }

        // GET: PlanesContinuidad/Edit/1
        public async Task<IActionResult> Edit(int id)
        {
            var plan = await _context.PlanesContinuidadDigital
                .AsNoTracking()
                .FirstOrDefaultAsync(plan =>
                    plan.Id == id);

            if (plan == null)
            {
                return NotFound();
            }

            var empresa = await _context.Empresas
                .AsNoTracking()
                .FirstOrDefaultAsync(empresa =>
                    empresa.Id == plan.EmpresaId);

            if (empresa == null)
            {
                return NotFound();
            }

            ViewBag.Empresa = empresa;

            return View(plan);
        }

        // POST: PlanesContinuidad/Edit/1
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id,
            PlanContinuidadDigital plan)
        {
            if (id != plan.Id)
            {
                return NotFound();
            }

            var planActual = await _context.PlanesContinuidadDigital
                .FirstOrDefaultAsync(planGuardado =>
                    planGuardado.Id == id);

            if (planActual == null)
            {
                return NotFound();
            }

            var empresa = await _context.Empresas
                .AsNoTracking()
                .FirstOrDefaultAsync(empresa =>
                    empresa.Id == planActual.EmpresaId);

            if (empresa == null)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                ViewBag.Empresa = empresa;
                return View(plan);
            }

            planActual.Nombre = plan.Nombre;
            planActual.Estado = plan.Estado;
            planActual.Responsable = plan.Responsable;
            planActual.ContactoEmergencia =
                plan.ContactoEmergencia;
            planActual.SistemasCriticos =
                plan.SistemasCriticos;
            planActual.RtoHoras = plan.RtoHoras;
            planActual.RpoHoras = plan.RpoHoras;
            planActual.ProcedimientoRespuesta =
                plan.ProcedimientoRespuesta;
            planActual.ProcedimientoRecuperacion =
                plan.ProcedimientoRecuperacion;
            planActual.PlanComunicacion =
                plan.PlanComunicacion;
            planActual.ProximaRevision =
                plan.ProximaRevision;
            planActual.Observaciones =
                plan.Observaciones;
            planActual.FechaUltimaActualizacion =
                DateTime.Now;

            await _context.SaveChangesAsync();

            TempData["MensajeExito"] =
                "El plan de continuidad fue actualizado correctamente.";

            return RedirectToAction(
                nameof(Index),
                new { empresaId = planActual.EmpresaId });
        }

        // POST: PlanesContinuidad/MarcarProbado/1
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarcarProbado(int id)
        {
            var plan = await _context.PlanesContinuidadDigital
                .FirstOrDefaultAsync(plan =>
                    plan.Id == id);

            if (plan == null)
            {
                return NotFound();
            }

            plan.PlanProbado = true;
            plan.FechaUltimaPrueba = DateTime.Now;
            plan.FechaUltimaActualizacion = DateTime.Now;

            if (plan.Estado == EstadoPlanContinuidad.Borrador ||
                plan.Estado == EstadoPlanContinuidad.EnRevision)
            {
                plan.Estado = EstadoPlanContinuidad.Activo;
            }

            await _context.SaveChangesAsync();

            TempData["MensajeExito"] =
                "La prueba del plan fue registrada correctamente.";

            return RedirectToAction(
                nameof(Index),
                new { empresaId = plan.EmpresaId });
        }

        // POST: PlanesContinuidad/Eliminar/1
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Eliminar(int id)
        {
            var plan = await _context.PlanesContinuidadDigital
                .FirstOrDefaultAsync(plan =>
                    plan.Id == id);

            if (plan == null)
            {
                return NotFound();
            }

            int empresaId = plan.EmpresaId;

            _context.PlanesContinuidadDigital.Remove(plan);
            await _context.SaveChangesAsync();

            TempData["MensajeExito"] =
                "El plan de continuidad fue eliminado correctamente.";

            return RedirectToAction(
                nameof(Index),
                new { empresaId });
        }
    }
}