using Microsoft.AspNetCore.Mvc;
using Amaretto.Application.Services;
using Amaretto.Application.Services.Interfaces;

namespace Amaretto.Web.Controllers;

public class UsuarioController : Controller
{
    private readonly IServiceUsuario _usuarioService;

    public UsuarioController(IServiceUsuario usuarioService)
    {
        _usuarioService = usuarioService;
    }

    public async Task<IActionResult> Index()
    {
        var usuarios = await _usuarioService.ListAsync();
        return View(usuarios);
    }

    public async Task<IActionResult> Details(int id)
    {
        var usuario = await _usuarioService.FindByIdAsync(id);
        if (usuario == null) return NotFound();
        return View(usuario);
    }
}
