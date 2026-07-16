using Amaretto.Application.DTOs;
using Amaretto.Application.Services.Implementations;
using Amaretto.Application.Services.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

namespace Amaretto.Web.Controllers
{
    public class ProductoController : Controller
    {
        private readonly IServiceProducto _serviceProducto;
        private readonly IServiceCategoria _serviceCategoria;
        private readonly IServiceIngrediente _serviceIngrediente;
        private readonly IWebHostEnvironment _webHostEnvironment;

        // Límites de validación para las imágenes subidas
        private const long TamanoMaximoImagen = 5 * 1024 * 1024; // 5 MB
        private const int TamanoMaximoImagenMB = 5;
        private static readonly string[] ExtensionesPermitidas = { ".jpg", ".jpeg", ".png" };

        public ProductoController(IServiceProducto serviceProducto, IServiceCategoria serviceCategoria, IServiceIngrediente serviceIngrediente, IWebHostEnvironment webHostEnvironment)
        {
            _serviceProducto = serviceProducto;
            _serviceCategoria = serviceCategoria;
            _serviceIngrediente = serviceIngrediente;
            _webHostEnvironment = webHostEnvironment;
        }

        [HttpGet]
        public async Task<ActionResult> Index(string? nombre, int? categoriaId, string? estado)
        {
            var collection = await _serviceProducto.ListAsync();

            IEnumerable<ProductoDTO> filtrado = collection;

            if (!string.IsNullOrWhiteSpace(nombre))
                filtrado = filtrado.Where(p => !string.IsNullOrEmpty(p.Nombre)
                    && p.Nombre.Contains(nombre, StringComparison.OrdinalIgnoreCase));

            if (categoriaId.HasValue && categoriaId.Value > 0)
                filtrado = filtrado.Where(p => p.IdCategoria == categoriaId.Value);

            if (estado == "disponible")
                filtrado = filtrado.Where(p => p.Estado);
            else if (estado == "inactivo")
                filtrado = filtrado.Where(p => !p.Estado);

            // Más reciente primero (aproximado por el consecutivo del ID, sin campo de fecha en BD)
            var ordenado = filtrado
                .OrderByDescending(p => p.IdProducto)
                .ToList();

            ViewBag.ListCategorias = await _serviceCategoria.ListAsync();
            ViewBag.FiltroNombre = nombre;
            ViewBag.FiltroCategoriaId = categoriaId;
            ViewBag.FiltroEstado = estado;

            return View(ordenado);
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


        // CREAR
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            ViewBag.ListCategorias = await _serviceCategoria.ListAsync();
            ViewBag.ListIngredientes = await _serviceIngrediente.ListAsync();
            return View(new ProductoDTO { Estado = true }); // disponible por defecto
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductoDTO dto, IFormFile imagenFile, IFormFile imagenFile2, int[] selectedIngredientes)
        {
            ModelState.Remove("IdProducto");

            if (!string.IsNullOrWhiteSpace(dto.Nombre) && await ExisteNombreDuplicadoAsync(dto.Nombre))
            {
                ModelState.AddModelError("Nombre", "Ya existe un producto con este nombre");
            }

            if (imagenFile != null)
            {
                if (!EsImagenValida(imagenFile))
                    ModelState.AddModelError("Imagen1", $"La imagen debe ser JPG o PNG y no superar los {TamanoMaximoImagenMB}MB");
                else
                    dto.Imagen1 = await GuardarImagenAsync(imagenFile);
            }

            if (imagenFile2 != null)
            {
                if (!EsImagenValida(imagenFile2))
                    ModelState.AddModelError("Imagen2", $"La imagen debe ser JPG o PNG y no superar los {TamanoMaximoImagenMB}MB");
                else
                    dto.Imagen2 = await GuardarImagenAsync(imagenFile2);
            }
            else
            {
                ModelState.AddModelError("Imagen1", "La imagen del producto es requerida");
            }

            if (dto.IdCategoria <= 0)
                ModelState.AddModelError("IdCategoria", "Debe seleccionar una categoría");

            if (selectedIngredientes == null || selectedIngredientes.Length == 0)
                ModelState.AddModelError("", "Debe seleccionar al menos un ingrediente");

            if (dto.Precio <= 0)
                ModelState.AddModelError("Precio", "El precio debe ser mayor a 0");

            if (!ModelState.IsValid)
            {
                ViewBag.ListCategorias = await _serviceCategoria.ListAsync();
                ViewBag.ListIngredientes = await _serviceIngrediente.ListAsync();
                return View(dto);
            }

            await _serviceProducto.AddAsync(dto, selectedIngredientes);

            TempData["Mensaje"] = Util.SweetAlertHelper.Mensaje(
                "Crear Producto",
                "Producto creado", Util.SweetAlertMessageType.success);

            return RedirectToAction("Index");
        }

        // EDITAR
        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            var dto = await _serviceProducto.FindByIdAsync(id);
            if (dto == null)
                return RedirectToAction("Index");

            ViewBag.ListCategorias = await _serviceCategoria.ListAsync();
            ViewBag.ListIngredientes = await _serviceIngrediente.ListAsync();
            ViewBag.SelectedIngredientes = dto.ProductoIngrediente.Select(x => x.IdIngrediente).ToList();
            return View(dto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, ProductoDTO dto, IFormFile? imagenFile, IFormFile? imagenFile2, int[] selectedIngredientes)
        {
            ModelState.Remove("IdProducto");

            var productoActual = await _serviceProducto.FindByIdAsync(id);

            if (!string.IsNullOrWhiteSpace(dto.Nombre) && await ExisteNombreDuplicadoAsync(dto.Nombre, id))
            {
                ModelState.AddModelError("Nombre", "Ya existe un producto con este nombre");
            }

            if (imagenFile != null)
            {
                if (!EsImagenValida(imagenFile))
                {
                    ModelState.AddModelError("Imagen1", $"La imagen debe ser JPG o PNG y no superar los {TamanoMaximoImagenMB}MB");
                }
                else
                {
                    EliminarImagenAnterior(productoActual?.Imagen1);
                    dto.Imagen1 = await GuardarImagenAsync(imagenFile);
                }
            }
            else if (string.IsNullOrEmpty(dto.Imagen1) && !string.IsNullOrEmpty(productoActual?.Imagen1))
            {
                // El usuario eliminó la imagen existente sin subir una nueva
                EliminarImagenAnterior(productoActual.Imagen1);
            }

            if (imagenFile2 != null)
            {
                if (!EsImagenValida(imagenFile2))
                {
                    ModelState.AddModelError("Imagen2", $"La imagen debe ser JPG o PNG y no superar los {TamanoMaximoImagenMB}MB");
                }
                else
                {
                    EliminarImagenAnterior(productoActual?.Imagen2);
                    dto.Imagen2 = await GuardarImagenAsync(imagenFile2);
                }
            }
            else if (string.IsNullOrEmpty(dto.Imagen2) && !string.IsNullOrEmpty(productoActual?.Imagen2))
            {
                EliminarImagenAnterior(productoActual.Imagen2);
            }

            if (dto.IdCategoria <= 0)
                ModelState.AddModelError("IdCategoria", "Debe seleccionar una categoría");

            if (selectedIngredientes == null || selectedIngredientes.Length == 0)
                ModelState.AddModelError("", "Debe seleccionar al menos un ingrediente");

            if (dto.Precio <= 0)
                ModelState.AddModelError("Precio", "El precio debe ser mayor a 0");

            if (!ModelState.IsValid)
            {
                ViewBag.ListCategorias = await _serviceCategoria.ListAsync();
                ViewBag.ListIngredientes = await _serviceIngrediente.ListAsync();
                ViewBag.SelectedIngredientes = selectedIngredientes?.ToList() ?? new List<int>();
                return View(dto);
            }

            await _serviceProducto.UpdateAsync(id, dto, selectedIngredientes);

            TempData["Mensaje"] = Util.SweetAlertHelper.Mensaje(
                "Editar Producto",
                "Producto actualizado", Util.SweetAlertMessageType.success);

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

        // Verifica si ya existe un producto con el mismo nombre (sin distinguir mayúsculas/minúsculas
        // ni espacios extra). Si se pasa idExcluido, ese producto no se toma en cuenta (caso Edit).
        private async Task<bool> ExisteNombreDuplicadoAsync(string nombre, string? idExcluido = null)
        {
            var productos = await _serviceProducto.ListAsync();

            return productos.Any(p =>
                !string.IsNullOrEmpty(p.Nombre)
                && p.Nombre.Trim().Equals(nombre.Trim(), StringComparison.OrdinalIgnoreCase)
                && (idExcluido == null || p.IdProducto.ToString() != idExcluido));
        }

        // Valida que la imagen tenga un formato permitido y no exceda el tamaño máximo permitido
        private static bool EsImagenValida(IFormFile imagenFile)
        {
            if (imagenFile == null || imagenFile.Length == 0 || imagenFile.Length > TamanoMaximoImagen)
                return false;

            var extension = Path.GetExtension(imagenFile.FileName)?.ToLowerInvariant();
            return !string.IsNullOrEmpty(extension) && ExtensionesPermitidas.Contains(extension);
        }

        // Elimina del disco la imagen anterior del producto, si existe físicamente
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