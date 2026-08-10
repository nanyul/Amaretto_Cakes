using Amaretto.Application.DTOs;
using Amaretto.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Amaretto.Web.Controllers
{
    [Authorize]
    public class PedidoController : Controller
    {
        private readonly IServicePedido _servicePedido;
        private readonly IServiceCarrito _serviceCarrito;
        private readonly IServiceUsuario _serviceUsuario;
        private readonly IServiceUsuarioActual _usuarioActual;
        private readonly IServiceProducto _serviceProducto;
        private readonly IServiceCombo _serviceCombo;
        private readonly IServiceNotificacion _serviceNotificacion;

        public PedidoController(
            IServicePedido servicePedido,
            IServiceCarrito serviceCarrito,
            IServiceUsuario serviceUsuario,
            IServiceUsuarioActual usuarioActual,
            IServiceProducto serviceProducto,
            IServiceCombo serviceCombo,
            IServiceNotificacion serviceNotificacion)
        {
            _servicePedido = servicePedido;
            _serviceCarrito = serviceCarrito;
            _serviceUsuario = serviceUsuario;
            _usuarioActual = usuarioActual;
            _serviceProducto = serviceProducto;
            _serviceCombo = serviceCombo;
            _serviceNotificacion = serviceNotificacion;
        }

        [HttpGet]
        public async Task<IActionResult> ItemsDisponibles()
        {
            var productos = await _serviceProducto.ListAsync();
            var combos = await _serviceCombo.ListAsync();

            return Json(new
            {
                productos = productos
                    .Where(p => p.Estado)
                    .OrderBy(p => p.Nombre)
                    .Select(p => new { id = p.IdProducto, nombre = p.Nombre, descripcion = p.Descripcion, precio = p.Precio, imagen = p.Imagen1, tipo = "producto" }),
                combos = combos
                    .Where(c => c.Estado)
                    .OrderBy(c => c.Nombre)
                    .Select(c => new { id = c.IdCombo, nombre = c.Nombre, descripcion = c.Descripcion, precio = c.Precio, imagen = c.Imagen1, tipo = "combo" })
            });
        }

        [HttpGet]
        public async Task<IActionResult> Registrar()
        {
            if (!_serviceCarrito.ObtenerCarrito().Any())
            {
                TempData["Mensaje"] = "Swal.fire({icon:'info', title:'Tu pedido est\u00e1 vac\u00edo', text:'Agreg\u00e1 productos antes de registrar el pedido.', toast:true, position:'bottom-end', showConfirmButton:false, timer:1800, timerProgressBar:true});";
                return RedirectToAction("Catalogo", "Producto");
            }

            var vm = await _servicePedido.PrepararRegistroAsync(_usuarioActual);
            return View(vm);
        }

        [HttpGet]
        public IActionResult Resumen(string metodoEntrega)
        {
            var resumen = _servicePedido.ObtenerResumen(metodoEntrega);
            return Json(resumen);
        }

        [HttpPost]
        public async Task<IActionResult> Guardar([FromBody] PedidoRegistroDTO dto)
        {
            try
            {
                var resultado = await _servicePedido.RegistrarPedidoAsync(dto);
                return Json(new { success = true, resultado });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { success = false, mensaje = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> BuscarClientes(string termino)
        {
            if (string.IsNullOrWhiteSpace(termino) || termino.Length < 2)
                return Json(new List<object>());

            var clientes = await _serviceUsuario.BuscarClientesAsync(termino);
            return Json(clientes.Select(c => new {
                c.IdUsuario,
                c.NombreCompleto,
                c.Email,
                c.Telefono,
                c.Direccion
            }));
        }

        // HISTORIAL DE PEDIDOS
        [HttpGet]
        public async Task<IActionResult> Historial(DateTime? fechaDesde, DateTime? fechaHasta, string? estado)
        {
            if (fechaDesde.HasValue && fechaHasta.HasValue && fechaDesde > fechaHasta)
            {
                TempData["Mensaje"] = Util.SweetAlertHelper.Mensaje(
                    "Historial de Pedidos",
                    "La fecha inicial no puede ser mayor que la fecha final.",
                    Util.SweetAlertMessageType.error);

                return RedirectToAction("Historial");
            }

            try
            {
                var vm = await _servicePedido.ObtenerHistorialAsync(fechaDesde, fechaHasta, estado);
                return View(vm);
            }
            catch (UnauthorizedAccessException)
            {
                return RedirectToAction("Forbidden", "Login");
            }
        }

        [HttpGet]
        public async Task<IActionResult> Comprobante(int id)
        {
            if (id <= 0)
                return RedirectToAction("Historial");

            try
            {
                var detalle = await _servicePedido.ObtenerDetalleAsync(id);
                if (detalle == null)
                    return RedirectToAction("Historial");

                ViewBag.Notificacion = await _serviceNotificacion.ObtenerPorPedidoAsync(id);
                return View(detalle);
            }
            catch (UnauthorizedAccessException)
            {
                return RedirectToAction("Forbidden", "Login");
            }
        }

        [HttpGet]
        public async Task<IActionResult> Detalle(int id)
        {
            if (id <= 0)
                return RedirectToAction("Historial");

            try
            {
                var detalle = await _servicePedido.ObtenerDetalleAsync(id);

                if (detalle == null)
                {
                    TempData["Mensaje"] = Util.SweetAlertHelper.Mensaje(
                        "Historial de Pedidos",
                        $"El pedido #{id} no existe.",
                        Util.SweetAlertMessageType.error);

                    return RedirectToAction("Historial");
                }

                return View(detalle);
            }
            catch (UnauthorizedAccessException)
            {
                return RedirectToAction("Forbidden", "Login");
            }
        }
    }
}
