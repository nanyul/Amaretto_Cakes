using Amaretto.Application.Services.Implementations;
using Amaretto.Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Amaretto.Web.Controllers
{
    public class ProductoController : Controller
    {
        private readonly IServiceProducto _serviceProducto;

        public ProductoController(IServiceProducto serviceProducto)
        {
            _serviceProducto = serviceProducto;
        }

        [HttpGet]
        public async Task<ActionResult> Index()
        {
            var collection = await _serviceProducto.ListAsync();

            return View(collection);
        }

        public async Task<ActionResult> Details(string? id)
        {
            try
            {
                if (id == null)
                {
                    return RedirectToAction("Index");
                }

                var @object = await _serviceProducto.FindByIdAsync(id);

                if (@object == null)
                {
                    throw new Exception("Producto no existente");

                }

                return View(@object);

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public async Task<IActionResult> Catalogo()
        {
            var lista = await _serviceProducto.ListAsync();
            return View(lista);
        }
    }
}
