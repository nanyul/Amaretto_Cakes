using Amaretto.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Amaretto.Web.Controllers
{
    [Authorize]
    public class ReporteController : Controller
    {
        private const int ROL_ADMINISTRADOR = 1;

        private readonly IServicePedido _servicePedido;
        private readonly IServiceUsuarioActual _usuarioActual;

        public ReporteController(IServicePedido servicePedido, IServiceUsuarioActual usuarioActual)
        {
            _servicePedido = servicePedido;
            _usuarioActual = usuarioActual;
        }

        private bool EsAdministrador => _usuarioActual.IdRol == ROL_ADMINISTRADOR;

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

            var vm = await _servicePedido.ObtenerReporteAsync(cliente, fechaDesde, fechaHasta, estado);
            return View(vm);
        }
    }
}
