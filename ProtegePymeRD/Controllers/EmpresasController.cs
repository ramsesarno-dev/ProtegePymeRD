using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProtegePymeRD.Data;
using ProtegePymeRD.Models;

namespace ProtegePymeRD.Controllers
{
    [Authorize(Roles = "Administrador,Analista")]
    public class EmpresasController : Controller
    {
        private readonly ApplicationDbContext _context;

        public EmpresasController(
            ApplicationDbContext context)
        {
            _context = context;
        }

        // Administradores y analistas pueden consultar.
        // GET: Empresas
        public async Task<IActionResult> Index(
            string? buscar)
        {
            ViewBag.TotalEmpresas =
                await _context.Empresas.CountAsync();

            ViewBag.EmpresasActivas =
                await _context.Empresas.CountAsync(
                    empresa => empresa.Activa);

            IQueryable<Empresa> consulta =
                _context.Empresas.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(buscar))
            {
                buscar = buscar.Trim();

                consulta = consulta.Where(empresa =>
                    empresa.Nombre.Contains(buscar) ||
                    empresa.Rnc.Contains(buscar) ||
                    empresa.Sector.Contains(buscar));
            }

            ViewBag.Buscar = buscar;

            var empresas = await consulta
                .OrderByDescending(empresa =>
                    empresa.FechaRegistro)
                .ToListAsync();

            return View(empresas);
        }

        // Administradores y analistas pueden consultar.
        // GET: Empresas/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var empresa = await _context.Empresas
                .AsNoTracking()
                .FirstOrDefaultAsync(empresa =>
                    empresa.Id == id);

            if (empresa == null)
            {
                return NotFound();
            }

            return View(empresa);
        }

        // Solo el administrador puede crear empresas.
        // GET: Empresas/Create
        [Authorize(Roles = "Administrador")]
        public IActionResult Create()
        {
            return View();
        }

        // Solo el administrador puede crear empresas.
        // POST: Empresas/Create
        [Authorize(Roles = "Administrador")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind(
                "Nombre,Rnc,Sector,CantidadEmpleados," +
                "CorreoContacto,Telefono,Plan")]
            Empresa empresa)
        {
            if (!string.IsNullOrWhiteSpace(empresa.Rnc))
            {
                empresa.Rnc = empresa.Rnc.Trim();

                bool rncDuplicado =
                    await _context.Empresas.AnyAsync(
                        empresaGuardada =>
                            empresaGuardada.Rnc ==
                            empresa.Rnc);

                if (rncDuplicado)
                {
                    ModelState.AddModelError(
                        nameof(Empresa.Rnc),
                        "Ya existe una empresa registrada " +
                        "con este RNC o cédula.");
                }
            }

            if (!ModelState.IsValid)
            {
                return View(empresa);
            }

            empresa.Nombre = empresa.Nombre.Trim();
            empresa.Sector = empresa.Sector.Trim();

            empresa.CorreoContacto =
                empresa.CorreoContacto.Trim();

            empresa.Telefono =
                empresa.Telefono?.Trim();

            empresa.ProtegeScore = 0;
            empresa.FechaRegistro = DateTime.Now;
            empresa.Activa = true;

            _context.Empresas.Add(empresa);
            await _context.SaveChangesAsync();

            TempData["MensajeExito"] =
                "La empresa fue registrada correctamente.";

            return RedirectToAction(nameof(Index));
        }

        // Solo el administrador puede editar empresas.
        // GET: Empresas/Edit/5
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var empresa =
                await _context.Empresas.FindAsync(id);

            if (empresa == null)
            {
                return NotFound();
            }

            return View(empresa);
        }

        // Solo el administrador puede editar empresas.
        // POST: Empresas/Edit/5
        [Authorize(Roles = "Administrador")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id,
            [Bind(
                "Id,Nombre,Rnc,Sector,CantidadEmpleados," +
                "CorreoContacto,Telefono,Plan,Activa")]
            Empresa empresa)
        {
            if (id != empresa.Id)
            {
                return NotFound();
            }

            var empresaExistente =
                await _context.Empresas
                    .FirstOrDefaultAsync(
                        empresaGuardada =>
                            empresaGuardada.Id == id);

            if (empresaExistente == null)
            {
                return NotFound();
            }

            if (!string.IsNullOrWhiteSpace(empresa.Rnc))
            {
                empresa.Rnc = empresa.Rnc.Trim();

                bool rncDuplicado =
                    await _context.Empresas.AnyAsync(
                        empresaGuardada =>
                            empresaGuardada.Rnc ==
                                empresa.Rnc &&
                            empresaGuardada.Id !=
                                empresa.Id);

                if (rncDuplicado)
                {
                    ModelState.AddModelError(
                        nameof(Empresa.Rnc),
                        "Ya existe otra empresa registrada " +
                        "con este RNC o cédula.");
                }
            }

            if (!ModelState.IsValid)
            {
                return View(empresa);
            }

            empresaExistente.Nombre =
                empresa.Nombre.Trim();

            empresaExistente.Rnc =
                empresa.Rnc.Trim();

            empresaExistente.Sector =
                empresa.Sector.Trim();

            empresaExistente.CantidadEmpleados =
                empresa.CantidadEmpleados;

            empresaExistente.CorreoContacto =
                empresa.CorreoContacto.Trim();

            empresaExistente.Telefono =
                empresa.Telefono?.Trim();

            empresaExistente.Plan =
                empresa.Plan;

            empresaExistente.Activa =
                empresa.Activa;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                bool existeEmpresa =
                    await _context.Empresas.AnyAsync(
                        empresaGuardada =>
                            empresaGuardada.Id == id);

                if (!existeEmpresa)
                {
                    return NotFound();
                }

                throw;
            }

            TempData["MensajeExito"] =
                "La empresa fue actualizada correctamente.";

            return RedirectToAction(nameof(Index));
        }

        // Solo el administrador puede abrir la eliminación.
        // GET: Empresas/Delete/5
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var empresa = await _context.Empresas
                .AsNoTracking()
                .FirstOrDefaultAsync(empresa =>
                    empresa.Id == id);

            if (empresa == null)
            {
                return NotFound();
            }

            return View(empresa);
        }

        // Solo el administrador puede confirmar la eliminación.
        // POST: Empresas/Delete/5
        [Authorize(Roles = "Administrador")]
        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(
            int id)
        {
            var empresa =
                await _context.Empresas.FindAsync(id);

            if (empresa == null)
            {
                return NotFound();
            }

            _context.Empresas.Remove(empresa);
            await _context.SaveChangesAsync();

            TempData["MensajeExito"] =
                "La empresa fue eliminada correctamente.";

            return RedirectToAction(nameof(Index));
        }
    }
}