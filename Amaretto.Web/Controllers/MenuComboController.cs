using Amaretto.Application.DTOs;
using Amaretto.Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
public class MenuComboController : Controller
{
    private readonly IServiceMenuCombo _serviceMenuCombo;
    public MenuComboController(IServiceMenuCombo serviceMenuCombo)
    {
        _serviceMenuCombo = serviceMenuCombo;
    }
    [HttpGet]
    public async Task<ActionResult> Index()
    {
        var menu = await _serviceMenuCombo.ListAsync();
        if (menu == null)
        {
            return NotFound("No existe un menú disponible actualmente.");
        }
        return View(menu);

    }

    [HttpGet]
    public async Task<ActionResult> Catalogo()
    {
        var menu = await _serviceMenuCombo.ListAsync();
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
            var @object = await _serviceMenuCombo.FindByIdAsync(id.Value);
            if (@object == null)
            {
                throw new Exception("Menú no existente");
            }
            return View(@object);
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }
}