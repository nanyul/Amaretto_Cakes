using Amaretto.Application.DTOs;
using Amaretto.Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
public class MenuProductoController : Controller
{
    private readonly IServiceMenuProducto _serviceMenuProducto;
    public MenuProductoController(IServiceMenuProducto serviceMenuProducto)
    {
        _serviceMenuProducto = serviceMenuProducto;
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
}
