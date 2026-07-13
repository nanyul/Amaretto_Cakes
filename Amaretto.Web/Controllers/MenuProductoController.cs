using Amaretto.Application.DTOs;
using Amaretto.Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
namespace Amaretto.Web.Controllers
{
    public class MenuProductoController : Controller
    {
        private readonly IServiceMenuProducto _serviceMenuProducto;
        private readonly IServiceProducto _serviceProducto;

        public MenuProductoController(IServiceMenuProducto serviceMenuProducto, IServiceProducto serviceProducto)
        {
            _serviceMenuProducto = serviceMenuProducto;
            _serviceProducto = serviceProducto;
        }

        [HttpGet]
        public async Task<ActionResult> Index()
        {
            var menu = await _serviceMenuProducto.ListAsync();
            if (menu == null)
            {
                return NotFound("No existe un menú disponible actualmente.");
            }
            return View(menu);

        }

        [HttpGet]
        public async Task<ActionResult> Catalogo()
        {
            var menu = await _serviceMenuProducto.ListAsync();
            if (menu == null)
            {
                return NotFound("No existe un menú disponible actualmente.");
            }
            return View(menu);
        }

        [HttpGet]
        public async Task<ActionResult> Details(int? id)
        {
            try
            {
                if (id == null)
                {
                    return RedirectToAction(nameof(Index));
                }
                var @object = await _serviceMenuProducto.FindByIdAsync(id.Value);
                if (@object == null)
                {
                    throw new Exception("Menú no existente");
                }
                // Validar que el menú esté disponible
                if (!@object.Estado)
                {
                    return RedirectToAction(nameof(Index));
                    throw new Exception("El menú no se encuentra disponible");
                }
                return View(@object);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        // CREAR
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var productos = await _serviceProducto.ListAsync();
            ViewBag.ListProductos = productos.Where(p => p.Estado).ToList();
            return View(new MenuProductoDTO { Estado = true });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(MenuProductoDTO dto, string[] selectedProductos)
        {
            ModelState.Remove("IdMenuProducto");

            if (selectedProductos == null || selectedProductos.Length == 0)
                ModelState.AddModelError("", "Debe seleccionar al menos un producto");

            if (dto.FechaFin < dto.FechaInicio)
                ModelState.AddModelError("FechaFin", "La fecha fin debe ser posterior a la fecha inicio");

            if (!ModelState.IsValid)
            {
                var productos = await _serviceProducto.ListAsync();
                ViewBag.ListProductos = productos.Where(p => p.Estado).ToList();
                return View(dto);
            }

            await _serviceMenuProducto.AddAsync(dto, selectedProductos);

            TempData["Mensaje"] = Util.SweetAlertHelper.Mensaje(
                "Crear Menú",
                "Menú de productos creado", Util.SweetAlertMessageType.success);

            return RedirectToAction("Index");
        }

        // EDITAR
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var dto = await _serviceMenuProducto.FindByIdAsync(id);
            if (dto == null)
                return RedirectToAction("Index");

            var productos = await _serviceProducto.ListAsync();
            ViewBag.ListProductos = productos.Where(p => p.Estado).ToList();
            ViewBag.SelectedProductos = dto.MenuDetalleProducto.Select(x => x.IdProducto).ToList();
            return View(dto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, MenuProductoDTO dto, string[] selectedProductos)
        {
            ModelState.Remove("IdMenuProducto");

            if (selectedProductos == null || selectedProductos.Length == 0)
                ModelState.AddModelError("", "Debe seleccionar al menos un producto");

            if (dto.FechaFin < dto.FechaInicio)
                ModelState.AddModelError("FechaFin", "La fecha fin debe ser posterior a la fecha inicio");

            if (!ModelState.IsValid)
            {
                var productos = await _serviceProducto.ListAsync();
                ViewBag.ListProductos = productos.Where(p => p.Estado).ToList();
                ViewBag.SelectedProductos = selectedProductos?.ToList() ?? new List<string>();
                return View(dto);
            }

            await _serviceMenuProducto.UpdateAsync(id, dto, selectedProductos);

            TempData["Mensaje"] = Util.SweetAlertHelper.Mensaje(
                "Editar Menú",
                "Menú de productos actualizado", Util.SweetAlertMessageType.success);

            return RedirectToAction("Index");
        }
    }
}
