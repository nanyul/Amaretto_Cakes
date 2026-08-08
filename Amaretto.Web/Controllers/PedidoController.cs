using Amaretto.Application.DTOs;
using Amaretto.Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Amaretto.Web.Controllers
{
    public class PedidoController : Controller
    {
        private readonly IServicePedido _servicePedido;
        private readonly IServiceCarrito _serviceCarrito;
        private readonly IServiceUsuario _serviceUsuario;
        private readonly IServiceUsuarioActual _usuarioActual;

        public PedidoController(IServicePedido servicePedido, IServiceCarrito serviceCarrito, IServiceUsuario serviceUsuario, IServiceUsuarioActual usuarioActual)
        {
            _servicePedido = servicePedido;
            _serviceCarrito = serviceCarrito;
            _serviceUsuario = serviceUsuario;
            _usuarioActual = usuarioActual;
        }

        [HttpGet]
        public async Task<IActionResult> Registrar()
        {
            if (!_serviceCarrito.ObtenerCarrito().Any())
            {
                TempData["Mensaje"] = "Swal.fire({icon:'info', title:'Tu pedido est\u00e1 vac\u00edo', text:'Agreg\u00e1 productos antes de registrar el pedido.'});";
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
    }
}