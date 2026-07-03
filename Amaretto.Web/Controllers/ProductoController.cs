using Amaretto.Application.Services.Implementations;
using Amaretto.Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Amaretto.Web.Controllers
{
    public class ProductoController : Controller
    {
        private readonly IServiceProducto _serviceProducto;
        private readonly IServiceCategoria _serviceCategoria;

        public ProductoController(IServiceProducto serviceProducto, IServiceCategoria serviceCategoria)
        {
            _serviceProducto = serviceProducto;
            _serviceCategoria = serviceCategoria;
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
                    return RedirectToAction("Index");

                var producto = await _serviceProducto.FindByIdAsync(id);

                if (producto == null)
                    throw new Exception("Producto no existente");

                // Productos relacionados: misma categoría, excluyendo el actual, máximo 4
                var todos = await _serviceProducto.ListAsync();

                ViewBag.Relacionados = todos
                    .Where(p => p.IdCategoria == producto.IdCategoria
                             && p.IdProducto != producto.IdProducto
                             && p.Estado)          // solo activos
                    .OrderBy(_ => Guid.NewGuid())  // orden aleatorio para variedad
                    .Take(4)
                    .ToList();

                return View(producto);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<IActionResult> Catalogo()
        {
            var lista = await _serviceProducto.ListAsync();
            ViewBag.Categorias = await _serviceCategoria.ListAsync();
            return View(lista);
        }

        [HttpGet]
        public async Task<IActionResult> Filtrar(
            string? estado,
            decimal? precioMax,
            List<int>? categoriaIds,
            string? ordenarPor)
        {
            var productos = await _serviceProducto.FilterAsync(estado, precioMax, categoriaIds, ordenarPor);

            var model = productos.Select(p => new
            {
                idProducto = p.IdProducto,
                nombre = p.Nombre,
                precio = p.Precio,
                esPersonalizable = p.EsPersonalizable,
                imagen1 = p.Imagen1,
                imagen2 = p.Imagen2
            });

            return Json(model);
        }
    }
}