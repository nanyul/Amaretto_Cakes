using Amaretto.Application.DTOs;
using Amaretto.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Amaretto.Web.Controllers
{
    [Authorize]
    public class ReporteController : Controller
    {
        private const int ROL_ADMINISTRADOR = 1;
        private const int ROL_ENCARGADO = 4;

        private readonly IServicePedido _servicePedido;
        private readonly IServiceUsuarioActual _usuarioActual;

        public ReporteController(IServicePedido servicePedido, IServiceUsuarioActual usuarioActual)
        {
            _servicePedido = servicePedido;
            _usuarioActual = usuarioActual;
        }

        private bool EsAdministrador => _usuarioActual.IdRol == ROL_ADMINISTRADOR;

        private bool EsAdminOEncargado =>
            _usuarioActual.IdRol == ROL_ADMINISTRADOR || _usuarioActual.IdRol == ROL_ENCARGADO;

        // REPORTE DE PEDIDOS

        [HttpGet]
        public async Task<IActionResult> ReportePedidos(string? cliente, DateTime? fechaDesde, DateTime? fechaHasta, string? estado)
        {
            if (!EsAdministrador) return RedirectToAction("Forbidden", "Login");

            if (fechaDesde.HasValue && fechaHasta.HasValue && fechaDesde > fechaHasta)
            {
                TempData["Mensaje"] = Util.SweetAlertHelper.Mensaje(
                    "Reporte de Pedidos",
                    "La fecha inicial no puede ser mayor que la fecha final.",
                    Util.SweetAlertMessageType.error);

                return RedirectToAction("ReportePedidos");
            }

            var vm = await _servicePedido.ObtenerReportePedidosAsync(cliente, fechaDesde, fechaHasta, estado);
            return View(vm);
        }

        // GRAFICO DE VENTAS

        [HttpGet]
        public async Task<IActionResult> Grafico(DateTime? fechaDesde, DateTime? fechaHasta)
        {
            if (!EsAdminOEncargado) return RedirectToAction("Forbidden", "Login");

            // Defaults: últimos 30 días
            var desde = fechaDesde ?? DateTime.Today.AddDays(-30);
            var hasta = fechaHasta?.AddDays(1).AddTicks(-1) ?? DateTime.Today.AddDays(1).AddTicks(-1);

            var reporte = await _servicePedido.ObtenerReporteAsync(desde, hasta);

            ViewBag.FechaDesde = desde.ToString("yyyy-MM-dd");
            ViewBag.FechaHasta = hasta.ToString("yyyy-MM-dd");

            // Chart 1: Top 3 Items (Horizontal Bar)
            ViewBag.TopLabels = string.Join(",", reporte.TopItems.Select(t => $"\"{t.Nombre}\""));
            ViewBag.TopValues = string.Join(",", reporte.TopItems.Select(t => t.Cantidad));
            ViewBag.TopTitle = "Top 3 Productos + Combos más Vendidos";

            // Chart 2: Pedidos por Estado (Doughnut)
            ViewBag.EstadoLabels = string.Join(",", reporte.PedidosPorEstado.Select(e => $"\"{e.Estado}\""));
            ViewBag.EstadoValues = string.Join(",", reporte.PedidosPorEstado.Select(e => e.Cantidad));
            ViewBag.EstadoTitle = "Pedidos por Estado";

            // KPIs
            ViewBag.TotalPedidos = reporte.PedidosPorEstado.Sum(e => e.Cantidad);
            ViewBag.TotalItems = reporte.TopItems.Sum(t => t.Cantidad);
            ViewBag.TotalEstados = reporte.PedidosPorEstado.Count;

            return View();
        }
    }
}
