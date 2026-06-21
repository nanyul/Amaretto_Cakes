using Amaretto.Application.DTOs;
using Amaretto.Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

public class ComboController : Controller
{
    private readonly IServiceCombo _serviceCombo;

    public ComboController(IServiceCombo serviceCombo)
    {
        _serviceCombo = serviceCombo;
    }

    [HttpGet]
    public async Task<ActionResult> Index()
    {
        var collection = await _serviceCombo.ListAsync();

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

            var @object = await _serviceCombo.FindByIdAsync(id);

            if (@object == null)
            {
                throw new Exception("Libro no existente");

            }

            return View(@object);

        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }
}