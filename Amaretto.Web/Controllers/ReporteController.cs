using Amaretto.Application.DTOs;
using Amaretto.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Amaretto.Web.Controllers
{
    [Authorize]
    public class ReporteController : Controller
    {
        private readonly IServicePedido _servicePedido;
        private readonly IServiceUsuarioActual _usuarioActual;

        public ReporteController(IServicePedido servicePedido, IServiceUsuarioActual usuarioActual)
        {
            _servicePedido = servicePedido;
            _usuarioActual = usuarioActual;
        }

        [HttpGet]
        public async Task<IActionResult> Grafico(DateTime? fechaDesde, DateTime? fechaHasta)
        {
            // Verificación rol (reutiliza patrón ServicePedido)
            if (!_usuarioActual.EstaAutenticado || !EsAdminOEncargado(_usuarioActual.IdRol))
                return RedirectToAction("Forbidden", "Login");

            // Defaults: últimos 30 días
            var desde = fechaDesde ?? DateTime.Today.AddDays(-30);
            var hasta = fechaHasta?.AddDays(1).AddTicks(-1) ?? DateTime.Today.AddDays(1).AddTicks(-1);

            var reporte = await _servicePedido.ObtenerReporteAsync(desde, hasta);

            // ViewBag para Charts (patrón del ejemplo)
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

        private bool EsAdminOEncargado(int idRol) => idRol == 1 || idRol == 4; // Admin=1, Encargado=4
    }
}