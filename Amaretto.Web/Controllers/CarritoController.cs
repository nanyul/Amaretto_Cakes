using Amaretto.Application.DTOs;
using Amaretto.Application.Services.Interfaces;
using Amaretto.Infraestructure.Data;
using Microsoft.AspNetCore.Mvc;

namespace Amaretto.Controllers;

public class CarritoController : Controller
{
    private readonly IServiceCarrito _carritoService;
    private readonly AmarettoContext _context;

    public CarritoController(IServiceCarrito carritoService, AmarettoContext context)
    {
        _carritoService = carritoService;
        _context = context;
    }

    [HttpPost]
    public IActionResult AgregarProducto(string idProducto, int cantidad)
    {
        if (cantidad <= 0) return BadRequest(new { success = false, mensaje = "Cantidad inválida." });
        var producto = _context.Producto.Find(idProducto);
        if (producto == null) return NotFound(new { success = false, mensaje = "Producto no encontrado." });
        _carritoService.Agregar(new CarritoItem
        {
            IdItem = producto.IdProducto,
            Tipo = "producto",
            Nombre = producto.Nombre,
            Precio = producto.Precio,
            Cantidad = cantidad,
            Imagen = producto.Imagen1
        });
        return Json(new { success = true, cantidadTotal = _carritoService.ObtenerCantidadTotal() });
    }

    [HttpPost]
    public IActionResult AgregarCombo(string idCombo, int cantidad)
    {
        if (cantidad <= 0) return BadRequest(new { success = false, mensaje = "Cantidad inválida." });
        var combo = _context.Combo.Find(idCombo);
        if (combo == null) return NotFound(new { success = false, mensaje = "Combo no encontrado." });
        if (!combo.Estado) return BadRequest(new { success = false, mensaje = "Combo no disponible." });
        _carritoService.Agregar(new CarritoItem
        {
            IdItem = combo.IdCombo,
            Tipo = "combo",
            Nombre = combo.Nombre,
            Precio = combo.Precio,
            Cantidad = cantidad,
            Imagen = combo.Imagen1
        });
        return Json(new { success = true, cantidadTotal = _carritoService.ObtenerCantidadTotal() });
    }

    [HttpGet]
    public IActionResult Cantidad()
    {
        return Json(new { cantidadTotal = _carritoService.ObtenerCantidadTotal() });
    }

    [HttpPost]
    public IActionResult ActualizarCantidad(string idItem, string tipo, int cantidad, string? lineaId = null)
    {
        if (string.IsNullOrEmpty(idItem) || string.IsNullOrEmpty(tipo))
            return BadRequest(new { success = false, mensaje = "Ítem inválido." });

        _carritoService.ActualizarCantidad(idItem, tipo, cantidad, lineaId);
        return Json(new { success = true, cantidadTotal = _carritoService.ObtenerCantidadTotal() });
    }

    [HttpPost]
    public IActionResult Eliminar(string idItem, string tipo, string? lineaId = null)
    {
        if (string.IsNullOrEmpty(idItem) || string.IsNullOrEmpty(tipo))
            return BadRequest(new { success = false, mensaje = "Ítem inválido." });

        _carritoService.Eliminar(idItem, tipo, lineaId);
        return Json(new { success = true, cantidadTotal = _carritoService.ObtenerCantidadTotal() });
    }

    [HttpPost]
    public IActionResult ActualizarObservaciones(string idItem, string tipo, string? observaciones, string? lineaId = null)
    {
        if (string.IsNullOrEmpty(idItem) || string.IsNullOrEmpty(tipo))
            return BadRequest(new { success = false, mensaje = "Ítem inválido." });

        _carritoService.ActualizarObservaciones(idItem, tipo, observaciones, lineaId);
        return Json(new { success = true });
    }
}