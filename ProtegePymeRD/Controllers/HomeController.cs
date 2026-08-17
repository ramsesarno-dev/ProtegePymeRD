using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProtegePymeRD.Data;
using ProtegePymeRD.Models;
using ProtegePymeRD.ViewModels;

namespace ProtegePymeRD.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        public HomeController(
            ILogger<HomeController> logger,
            ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var empresas = await _context.Empresas
                .AsNoTracking()
                .OrderByDescending(empresa =>
                    empresa.FechaRegistro)
                .ToListAsync();

            var empresasEvaluadas = empresas
                .Where(empresa =>
                    empresa.ProtegeScore > 0)
                .ToList();

            var respaldos = await _context.Respaldos
                .AsNoTracking()
                .ToListAsync();

            var cuentasCriticas =
                await _context.CuentasCriticas
                    .AsNoTracking()
                    .ToListAsync();

            var alertas =
                await _context.AlertasSeguridad
                    .AsNoTracking()
                    .ToListAsync();

            var capacitaciones =
                await _context.CapacitacionesSeguridad
                    .AsNoTracking()
                    .ToListAsync();

            var planesContinuidad =
                await _context.PlanesContinuidadDigital
                    .AsNoTracking()
                    .ToListAsync();

            var modelo = new DashboardViewModel
            {
                // Empresas
                TotalEmpresas = empresas.Count,

                EmpresasActivas = empresas.Count(empresa =>
                    empresa.Activa),

                EvaluacionesPendientes = empresas.Count(empresa =>
                    empresa.ProtegeScore == 0),

                ProtegeScorePromedio = empresasEvaluadas.Any()
                    ? (int)Math.Round(
                        empresasEvaluadas.Average(empresa =>
                            empresa.ProtegeScore))
                    : 0,

                EmpresasRecientes = empresas
                    .Take(5)
                    .ToList(),

                // ProtegeBackup
                TotalRespaldos = respaldos.Count,

                RespaldosExitosos = respaldos.Count(respaldo =>
                    respaldo.Estado ==
                    EstadoRespaldo.Exitoso),

                RestauracionesVerificadas =
                    respaldos.Count(respaldo =>
                        respaldo.RestauracionProbada),

                // ProtegeAccess
                TotalCuentasCriticas =
                    cuentasCriticas.Count,

                CuentasCriticasActivas =
                    cuentasCriticas.Count(cuenta =>
                        cuenta.Activa),

                CuentasConMfa =
                    cuentasCriticas.Count(cuenta =>
                        cuenta.Activa &&
                        cuenta.TieneMfa),

                CuentasConMinimoPrivilegio =
                    cuentasCriticas.Count(cuenta =>
                        cuenta.Activa &&
                        cuenta.CumpleMinimoPrivilegio),

                // ProtegeAlert
                TotalAlertas = alertas.Count,

                AlertasPendientes = alertas.Count(alerta =>
                    alerta.Estado ==
                        EstadoAlerta.Abierta ||
                    alerta.Estado ==
                        EstadoAlerta.EnProceso),

                AlertasCriticasAbiertas =
                    alertas.Count(alerta =>
                        alerta.Severidad ==
                            SeveridadAlerta.Critica &&
                        alerta.Estado !=
                            EstadoAlerta.Resuelta &&
                        alerta.Estado !=
                            EstadoAlerta.Descartada),

                AlertasResueltas = alertas.Count(alerta =>
                    alerta.Estado ==
                    EstadoAlerta.Resuelta),

                // ProtegeHuman
                TotalCapacitaciones =
                    capacitaciones.Count,

                CapacitacionesPendientes =
                    capacitaciones.Count(capacitacion =>
                        capacitacion.Estado ==
                            EstadoCapacitacion.Programada ||
                        capacitacion.Estado ==
                            EstadoCapacitacion.EnCurso),

                CapacitacionesCompletadas =
                    capacitaciones.Count(capacitacion =>
                        capacitacion.Estado ==
                            EstadoCapacitacion.Completada),

                ParticipantesCapacitados =
                    capacitaciones
                        .Where(capacitacion =>
                            capacitacion.Estado ==
                            EstadoCapacitacion.Completada)
                        .Sum(capacitacion =>
                            capacitacion.CantidadParticipantes),

                ParticipantesAprobados =
                    capacitaciones
                        .Where(capacitacion =>
                            capacitacion.Estado ==
                            EstadoCapacitacion.Completada)
                        .Sum(capacitacion =>
                            capacitacion.CantidadAprobados),

                // Plan de continuidad
                TotalPlanesContinuidad =
                    planesContinuidad.Count,

                PlanesActivos =
                    planesContinuidad.Count(plan =>
                        plan.Estado ==
                        EstadoPlanContinuidad.Activo),

                PlanesProbados =
                    planesContinuidad.Count(plan =>
                        plan.PlanProbado),

                PlanesRequierenRevision =
                    planesContinuidad.Count(plan =>
                        plan.ProximaRevision.Date <=
                            DateTime.Today ||
                        plan.Estado ==
                            EstadoPlanContinuidad
                                .RequiereActualizacion)
            };

            return View(modelo);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(
            Duration = 0,
            Location = ResponseCacheLocation.None,
            NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel
            {
                RequestId = Activity.Current?.Id
                    ?? HttpContext.TraceIdentifier
            });
        }
    }
}