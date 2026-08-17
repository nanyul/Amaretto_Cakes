using Amaretto.Application.DTOs;
using Amaretto.Application.Services.Interfaces;
using Amaretto.Infraestructure.Data;
using Microsoft.AspNetCore.Mvc;

namespace Amaretto.Controllers;

public class PersonalizacionController : Controller
{
    private readonly IServicePersonalizacion _servicePersonalizacion;
    private readonly IServiceCarrito _serviceCarrito;
    private readonly AmarettoContext _context;
    private readonly IWebHostEnvironment _webHostEnvironment;

    private const long TamanoMaximoImagen = 5 * 1024 * 1024; // 5 MB
    private const int TamanoMaximoImagenMB = 5;
    private static readonly string[] ExtensionesPermitidas = { ".jpg", ".jpeg", ".png" };

    public PersonalizacionController(
        IServicePersonalizacion servicePersonalizacion,
        IServiceCarrito serviceCarrito,
        AmarettoContext context,
        IWebHostEnvironment webHostEnvironment)
    {
        _servicePersonalizacion = servicePersonalizacion;
        _serviceCarrito = serviceCarrito;
        _context = context;
        _webHostEnvironment = webHostEnvironment;
    }

    [HttpGet]
    public IActionResult Index(string idProducto, int cantidad = 1)
    {
        var producto = _context.Producto.Find(idProducto);
        if (producto == null) return NotFound();
        if (!producto.EsPersonalizable)
        {
            TempData["Mensaje"] = "Swal.fire({icon:'warning', title:'Este producto no admite personalización'});";
            return RedirectToAction("Details", "Producto", new { id = idProducto });
        }
        var vm = new PersonalizacionViewModel
        {
            IdProducto = producto.IdProducto,
            NombreProducto = producto.Nombre,
            Descripcion = producto.Descripcion,
            Imagen = producto.Imagen1,
            PrecioBase = producto.Precio,
            Cantidad = cantidad < 1 ? 1 : cantidad,
            Tematicas = _servicePersonalizacion.ObtenerTematicas()
        };
        return View(vm);
    }

    [HttpPost]
    [RequestSizeLimit(6_000_000)]
    public async Task<IActionResult> Guardar([FromForm] PersonalizacionDTO dto, IFormFile? imagenReferencia)
    {
        if (!ModelState.IsValid)
            return BadRequest(new { success = false, mensaje = "Completá todos los campos obligatorios." });

        if (dto.TipoDecoracion == "Temática de catálogo" && string.IsNullOrWhiteSpace(dto.DecoracionDetalle))
            return BadRequest(new { success = false, mensaje = "Seleccioná una temática del catálogo." });

        var producto = _context.Producto.Find(dto.IdProducto);
        if (producto == null)
            return NotFound(new { success = false, mensaje = "Producto no encontrado." });

        if (dto.TipoDecoracion == "Imagen de referencia")
        {
            if (imagenReferencia == null)
                return BadRequest(new { success = false, mensaje = "Subí una imagen de referencia." });

            if (!EsImagenValida(imagenReferencia))
                return BadRequest(new { success = false, mensaje = $"La imagen debe ser JPG o PNG y no superar los {TamanoMaximoImagenMB}MB." });

            dto.RutaImagenReferencia = await GuardarImagenAsync(imagenReferencia);
        }

        dto.PrecioExtra = _servicePersonalizacion.CalcularPrecioExtra(dto);

        var item = new CarritoItem
        {
            IdItem = producto.IdProducto,
            Tipo = "producto",
            Nombre = $"{producto.Nombre} (Personalizado)",
            Precio = producto.Precio + dto.PrecioExtra,
            Cantidad = dto.Cantidad < 1 ? 1 : dto.Cantidad,
            Imagen = producto.Imagen1,
            Personalizacion = dto
        };

        _serviceCarrito.AgregarPersonalizado(item);
        return Json(new { success = true, cantidadTotal = _serviceCarrito.ObtenerCantidadTotal() });
    }

    private async Task<string> GuardarImagenAsync(IFormFile imagenFile)
    {
        string carpeta = Path.Combine(_webHostEnvironment.WebRootPath, "images", "personalizaciones");
        if (!Directory.Exists(carpeta))
            Directory.CreateDirectory(carpeta);

        string nombreArchivo = $"{Guid.NewGuid()}{Path.GetExtension(imagenFile.FileName)}";
        string rutaCompleta = Path.Combine(carpeta, nombreArchivo);

        using (var stream = new FileStream(rutaCompleta, FileMode.Create))
        {
            await imagenFile.CopyToAsync(stream);
        }

        return $"/images/personalizaciones/{nombreArchivo}";
    }

    private static bool EsImagenValida(IFormFile imagenFile)
    {
        if (imagenFile == null || imagenFile.Length == 0 || imagenFile.Length > TamanoMaximoImagen)
            return false;

        var extension = Path.GetExtension(imagenFile.FileName)?.ToLowerInvariant();
        return !string.IsNullOrEmpty(extension) && ExtensionesPermitidas.Contains(extension);
    }
}