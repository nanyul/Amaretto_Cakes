using Amaretto.Application.DTOs;
using Amaretto.Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Amaretto.Web.Controllers
{
    public class ComboController : Controller
    {
        private readonly IServiceCombo _serviceCombo;
        private readonly IServiceCategoria _serviceCategoria;
        private readonly IServiceProducto _serviceProducto;
        private readonly IWebHostEnvironment _webHostEnvironment;

        // Límites de validación para las imágenes subidas
        private const long TamanoMaximoImagen = 5 * 1024 * 1024; // 5 MB
        private const int TamanoMaximoImagenMB = 5;
        private static readonly string[] ExtensionesPermitidas = { ".jpg", ".jpeg", ".png" };

        public ComboController(IServiceCombo serviceCombo, IServiceCategoria serviceCategoria, IServiceProducto serviceProducto, IWebHostEnvironment webHostEnvironment)
        {
            _serviceCombo = serviceCombo;
            _serviceCategoria = serviceCategoria;
            _serviceProducto = serviceProducto;
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<ActionResult> Index(string? nombre, int? categoriaId, string? estado)
        {
            var collection = await _serviceCombo.ListAsync();

            IEnumerable<ComboDTO> filtrado = collection;

            if (!string.IsNullOrWhiteSpace(nombre))
                filtrado = filtrado.Where(c => !string.IsNullOrEmpty(c.Nombre)
                    && c.Nombre.Contains(nombre, StringComparison.OrdinalIgnoreCase));

            if (categoriaId.HasValue && categoriaId.Value > 0)
                filtrado = filtrado.Where(c => c.Producto.Any(p => p.IdCategoria == categoriaId.Value));

            if (estado == "disponible")
                filtrado = filtrado.Where(c => c.Estado);
            else if (estado == "inactivo")
                filtrado = filtrado.Where(c => !c.Estado);

            // Más reciente primero (aproximado por el consecutivo del ID)
            var ordenado = filtrado
                .OrderByDescending(c => c.IdCombo)
                .ToList();

            ViewBag.ListCategorias = await _serviceCategoria.ListAsync();
            ViewBag.FiltroNombre = nombre;
            ViewBag.FiltroCategoriaId = categoriaId;
            ViewBag.FiltroEstado = estado;

            return View(ordenado);
        }

        public async Task<ActionResult> Details(string? id)
        {
            if (id == null)
                return RedirectToAction("Index");

            var combo = await _serviceCombo.FindByIdAsync(id);

            if (combo == null)
                throw new Exception("Combo no existente");

            // Combos relacionados: cualquier otro combo activo, máximo 4
            var todos = await _serviceCombo.ListAsync();

            ViewBag.Relacionados = todos
                .Where(c => c.IdCombo != combo.IdCombo
                         && c.Estado)              // solo activos
                .OrderBy(_ => Guid.NewGuid())      // orden aleatorio para variedad
                .Take(4)
                .ToList();

            return View(combo);
        }

        public async Task<IActionResult> Catalogo()
        {
            var lista = await _serviceCombo.ListAsync();
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

        // CREAR
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            ViewBag.ListProductos = await _serviceProducto.ListAsync();
            return View(new ComboDTO { Estado = true }); // disponible por defecto
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ComboDTO dto, IFormFile imagenFile, IFormFile imagenFile2, string[] selectedProductos)
        {
            ModelState.Remove("IdCombo");
            ModelState.Remove("Imagen1");
            ModelState.Remove("Imagen2");

            if (imagenFile != null)
            {
                if (!EsImagenValida(imagenFile))
                    ModelState.AddModelError("", $"La imagen 1 debe ser JPG o PNG y no superar los {TamanoMaximoImagenMB}MB");
                else
                    dto.Imagen1 = await GuardarImagenAsync(imagenFile);
            }

            if (imagenFile2 != null)
            {
                if (!EsImagenValida(imagenFile2))
                    ModelState.AddModelError("", $"La imagen 2 debe ser JPG o PNG y no superar los {TamanoMaximoImagenMB}MB");
                else
                    dto.Imagen2 = await GuardarImagenAsync(imagenFile2);
            }

            if (imagenFile == null || imagenFile2 == null)
            {
                ModelState.AddModelError("", "Las dos imágenes del combo son requeridas");
            }

            if (selectedProductos == null || selectedProductos.Length == 0)
                ModelState.AddModelError("", "Debe seleccionar al menos un producto para el combo");

            if (dto.Precio <= 0)
                ModelState.AddModelError("Precio", "El precio debe ser mayor a 0");

            if (!ModelState.IsValid)
            {
                ViewBag.ListProductos = await _serviceProducto.ListAsync();
                return View(dto);
            }

            await _serviceCombo.AddAsync(dto, selectedProductos);

            TempData["Mensaje"] = Util.SweetAlertHelper.Mensaje(
                "Crear Combo",
                "Combo creado", Util.SweetAlertMessageType.success);

            return RedirectToAction("Index");
        }

        // EDITAR
        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            var dto = await _serviceCombo.FindByIdAsync(id);
            if (dto == null)
                return RedirectToAction("Index");

            ViewBag.ListProductos = await _serviceProducto.ListAsync();
            ViewBag.SelectedProductos = dto.Producto.Select(p => p.IdProducto).ToList();
            return View(dto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, ComboDTO dto, IFormFile? imagenFile, IFormFile? imagenFile2, string[] selectedProductos)
        {
            ModelState.Remove("IdCombo");
            ModelState.Remove("Imagen1");
            ModelState.Remove("Imagen2");

            var comboActual = await _serviceCombo.FindByIdAsync(id);

            if (imagenFile != null)
            {
                if (!EsImagenValida(imagenFile))
                {
                    ModelState.AddModelError("", $"La imagen 1 debe ser JPG o PNG y no superar los {TamanoMaximoImagenMB}MB");
                }
                else
                {
                    EliminarImagenAnterior(comboActual?.Imagen1);
                    dto.Imagen1 = await GuardarImagenAsync(imagenFile);
                }
            }

            if (imagenFile2 != null)
            {
                if (!EsImagenValida(imagenFile2))
                {
                    ModelState.AddModelError("", $"La imagen 2 debe ser JPG o PNG y no superar los {TamanoMaximoImagenMB}MB");
                }
                else
                {
                    EliminarImagenAnterior(comboActual?.Imagen2);
                    dto.Imagen2 = await GuardarImagenAsync(imagenFile2);
                }
            }

            if (string.IsNullOrEmpty(dto.Imagen1) || string.IsNullOrEmpty(dto.Imagen2))
            {
                ModelState.AddModelError("", "Las dos imágenes del combo son requeridas");
            }

            if (selectedProductos == null || selectedProductos.Length == 0)
                ModelState.AddModelError("", "Debe seleccionar al menos un producto para el combo");

            if (dto.Precio <= 0)
                ModelState.AddModelError("Precio", "El precio debe ser mayor a 0");

            if (!ModelState.IsValid)
            {
                ViewBag.ListProductos = await _serviceProducto.ListAsync();
                ViewBag.SelectedProductos = selectedProductos?.ToList() ?? new List<string>();
                return View(dto);
            }

            await _serviceCombo.UpdateAsync(id, dto, selectedProductos);

            TempData["Mensaje"] = Util.SweetAlertHelper.Mensaje(
                "Editar Combo",
                "Combo actualizado", Util.SweetAlertMessageType.success);

            return RedirectToAction("Index");
        }

        private async Task<string> GuardarImagenAsync(IFormFile imagenFile)
        {
            string carpeta = Path.Combine(_webHostEnvironment.WebRootPath, "images", "productos");
            if (!Directory.Exists(carpeta))
                Directory.CreateDirectory(carpeta);

            string nombreArchivo = $"{Guid.NewGuid()}{Path.GetExtension(imagenFile.FileName)}";
            string rutaCompleta = Path.Combine(carpeta, nombreArchivo);

            using (var stream = new FileStream(rutaCompleta, FileMode.Create))
            {
                await imagenFile.CopyToAsync(stream);
            }

            return $"/images/productos/{nombreArchivo}";
        }

        // Valida que la imagen tenga un formato permitido y no exceda el tamaño máximo permitido
        private static bool EsImagenValida(IFormFile imagenFile)
        {
            if (imagenFile == null || imagenFile.Length == 0 || imagenFile.Length > TamanoMaximoImagen)
                return false;

            var extension = Path.GetExtension(imagenFile.FileName)?.ToLowerInvariant();
            return !string.IsNullOrEmpty(extension) && ExtensionesPermitidas.Contains(extension);
        }

        // Elimina del disco la imagen anterior del combo, si existe físicamente
        private void EliminarImagenAnterior(string? rutaImagenRelativa)
        {
            if (string.IsNullOrEmpty(rutaImagenRelativa))
                return;

            var rutaFisica = Path.Combine(
                _webHostEnvironment.WebRootPath,
                rutaImagenRelativa.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));

            if (System.IO.File.Exists(rutaFisica))
            {
                try
                {
                    System.IO.File.Delete(rutaFisica);
                }
                catch (IOException)
                {
                    // No se interrumpe el flujo si el archivo no puede eliminarse (p. ej. en uso)
                }
            }
        }

    }
}