using Amaretto.Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Amaretto.Web.Controllers
{
    public class ComboController : Controller
    {
        private readonly IServiceCombo _serviceCombo;
        private readonly IServiceCategoria _serviceCategoria;

        public ComboController(IServiceCombo serviceCombo, IServiceCategoria serviceCategoria)
        {
            _serviceCombo = serviceCombo;
            _serviceCategoria = serviceCategoria;
        }

        public async Task<ActionResult> Index()
        {
            var collection = await _serviceCombo.ListAsync();
            return View(collection);
        }

        public async Task<ActionResult> Details(string? id)
        {
            if (id == null) return RedirectToAction("Index");
            var @object = await _serviceCombo.FindByIdAsync(id);
            if (@object == null) throw new Exception("Combo no existente");
            return View(@object);
        }

        public async Task<IActionResult> Catalogo()
        {
            var lista = await _serviceCombo.ListAsync();
            ViewBag.Categorias = await _serviceCategoria.ListAsync();
            return View(lista);
        }

        [HttpGet]
        public async Task<IActionResult> Filtrar(string? estado, decimal? precioMax, List<int>? categoriaIds, string? ordenarPor)
        {
            var combos = await _serviceCombo.FilterAsync(estado, precioMax, categoriaIds, ordenarPor);

            var model = combos.Select(c => new
            {
                idCombo = c.IdCombo,
                nombre = c.Nombre,
                precio = c.Precio,
                estado = c.Estado,
                imagen1 = c.Imagen1,
                imagen2 = c.Imagen2
            });

            return Json(model);
        }
    }
}