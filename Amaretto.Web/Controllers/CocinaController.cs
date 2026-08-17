using Amaretto.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Amaretto.Web.Controllers
{

    [Authorize]
    public class CocinaController : Controller
    {
        private const int ROL_ADMINISTRADOR = 1;
        private const int ROL_COCINERO = 3;
        private const int ROL_ENCARGADO = 4;

        private readonly IServiceCocina _serviceCocina;
        private readonly IServiceUsuarioActual _usuarioActual;

        public CocinaController(IServiceCocina serviceCocina, IServiceUsuarioActual usuarioActual)
        {
            _serviceCocina = serviceCocina;
            _usuarioActual = usuarioActual;
        }

        private bool TieneAcceso =>
            _usuarioActual.IdRol == ROL_COCINERO ||
            _usuarioActual.IdRol == ROL_ENCARGADO ||
            _usuarioActual.IdRol == ROL_ADMINISTRADOR;

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            if (!TieneAcceso) return RedirectToAction("Forbidden", "Login");

            var estaciones = await _serviceCocina.ListarEstacionesAsync();
            return View(estaciones);
        }

        [HttpGet]
        public async Task<IActionResult> Estacion(int id)
        {
            if (!TieneAcceso) return RedirectToAction("Forbidden", "Login");

            var estacion = await _serviceCocina.ObtenerEstacionAsync(id);
            if (estacion == null) return RedirectToAction("Index");

            return View(estacion);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Avanzar(int idCocinaOrden, string nuevoEstado, int idEstacion)
        {
            if (!TieneAcceso) return RedirectToAction("Forbidden", "Login");

            try
            {
                var mensaje = await _serviceCocina.AvanzarTareaAsync(idCocinaOrden, nuevoEstado);

                TempData["Mensaje"] = Util.SweetAlertHelper.Mensaje(
                    "Cocina", mensaje, Util.SweetAlertMessageType.success);
            }
            catch (InvalidOperationException ex)
            {
                TempData["Mensaje"] = Util.SweetAlertHelper.Mensaje(
                    "Cocina", ex.Message, Util.SweetAlertMessageType.error);
            }

            return RedirectToAction("Estacion", new { id = idEstacion });
        }
    }
}
