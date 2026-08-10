using Amaretto.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Amaretto.Web.Controllers
{
    [Authorize]
    public class NotificacionController : Controller
    {
        private readonly IServiceNotificacion _serviceNotificacion;

        public NotificacionController(IServiceNotificacion serviceNotificacion)
        {
            _serviceNotificacion = serviceNotificacion;
        }

        [HttpGet]
        public async Task<IActionResult> Listar()
        {
            var lista = await _serviceNotificacion.ListarAsync();
            var noLeidas = await _serviceNotificacion.ContarNoLeidasAsync();

            return Json(new
            {
                noLeidas,
                notificaciones = lista.Select(n => new
                {
                    n.IdNotificacion,
                    n.IdPedido,
                    n.Titulo,
                    n.Mensaje,
                    n.Leida,
                    n.CorreoEnviado,
                    fecha = n.FechaCreacion.ToString("dd/MM/yyyy hh:mm tt")
                })
            });
        }

        [HttpPost]
        public async Task<IActionResult> MarcarLeidas()
        {
            await _serviceNotificacion.MarcarLeidasAsync();
            return Json(new { success = true });
        }
    }
}
