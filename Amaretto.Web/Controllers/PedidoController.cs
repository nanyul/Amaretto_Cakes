using Amaretto.Application.DTOs;
using Amaretto.Application.Services.Interfaces;
using Amaretto.Infraestructure.Repository.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Amaretto.Web.Controllers
{
    public class PedidoController : Controller
    {
        private readonly IServicePedido _servicePedido;
        private readonly IServiceCarrito _serviceCarrito;
        private readonly IRepositoryUsuario _repoUsuario;

        public PedidoController(IServicePedido servicePedido, IServiceCarrito serviceCarrito, IRepositoryUsuario repoUsuario)
        {
            _servicePedido = servicePedido;
            _serviceCarrito = serviceCarrito;
            _repoUsuario = repoUsuario;
        }

        [HttpGet]
        public async Task<IActionResult> Registrar()
        {
            if (!_serviceCarrito.ObtenerCarrito().Any())
            {
                TempData["Mensaje"] = "Swal.fire({icon:'info', title:'Tu pedido está vacío', text:'Agregá productos antes de registrar el pedido.'});";
                return RedirectToAction("Catalogo", "Producto");
            }

            var vm = await _servicePedido.PrepararRegistroAsync();
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

        // ---- Simulador temporal de usuario (quitar cuando exista login) ----

        [HttpGet]
        public async Task<IActionResult> ObtenerUsuariosPorRol(string rol)
        {
            var usuarios = await _repoUsuario.ObtenerPorRolAsync(rol);
            var lista = usuarios.Select(u => new { u.IdUsuario, u.NombreCompleto });
            return Json(lista);
        }

        // Búsqueda por ID para autocompletar el campo Encargado
        [HttpGet]
        public async Task<IActionResult> ObtenerUsuarioPorId(int id)
        {
            var usuario = await _repoUsuario.FindByIdAsync(id);
            if (usuario == null)
                return NotFound(new { success = false, mensaje = "No existe un usuario con ese ID." });

            return Json(new
            {
                success = true,
                idUsuario = usuario.IdUsuario,
                nombreCompleto = usuario.NombreCompleto,
                telefono = usuario.Telefono,
                email = usuario.Email,
                direccion = usuario.Direccion
            });
        }

        [HttpPost]
        public IActionResult SimularUsuario(int idUsuario, string rol)
        {
            HttpContext.Session.SetInt32("SimIdUsuario", idUsuario);
            HttpContext.Session.SetString("SimRol", rol);
            return Json(new { success = true });
        }
    }
}