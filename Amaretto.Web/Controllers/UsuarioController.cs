using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Amaretto.Application.DTOs;
using Amaretto.Application.Services;
using Amaretto.Application.Services.Interfaces;

namespace Amaretto.Web.Controllers;

/// <summary>
/// Mantenimiento de usuarios. Solo el Administrador puede entrar: el rol se
/// resuelve a partir del usuario identificado en la sesión.
/// </summary>
[Authorize]
public class UsuarioController : Controller
{
    private const int ROL_ADMINISTRADOR = 1;

    private readonly IServiceUsuario _usuarioService;
    private readonly IServiceUsuarioActual _usuarioActual;

    public UsuarioController(IServiceUsuario usuarioService, IServiceUsuarioActual usuarioActual)
    {
        _usuarioService = usuarioService;
        _usuarioActual = usuarioActual;
    }

    private bool EsAdministrador => _usuarioActual.IdRol == ROL_ADMINISTRADOR;

    public async Task<IActionResult> Index()
    {
        if (!EsAdministrador) return RedirectToAction("Forbidden", "Login");

        var usuarios = await _usuarioService.ListAsync();
        return View(usuarios);
    }

    public async Task<IActionResult> Details(int id)
    {
        if (!EsAdministrador) return RedirectToAction("Forbidden", "Login");

        var usuario = await _usuarioService.FindByIdAsync(id);
        if (usuario == null) return NotFound();
        return View(usuario);
    }

    // CREAR

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        if (!EsAdministrador) return RedirectToAction("Forbidden", "Login");

        ViewBag.ListRoles = await _usuarioService.ListarRolesAsync();
        return View(new UsuarioMantenimientoDTO { Estado = true });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(UsuarioMantenimientoDTO dto)
    {
        if (!EsAdministrador) return RedirectToAction("Forbidden", "Login");

        if (!ModelState.IsValid)
            return await VolverAlFormulario("Create", dto);

        var resultado = await _usuarioService.CrearAsync(dto);

        if (!resultado.Success)
        {
            ModelState.AddModelError("", resultado.Message ?? "No se pudo crear el usuario");
            return await VolverAlFormulario("Create", dto);
        }

        TempData["Mensaje"] = Util.SweetAlertHelper.Mensaje(
            "Crear Usuario",
            resultado.Message ?? "Usuario creado",
            Util.SweetAlertMessageType.success);

        return RedirectToAction("Index");
    }

    // EDITAR

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        if (!EsAdministrador) return RedirectToAction("Forbidden", "Login");

        var dto = await _usuarioService.ObtenerParaEdicionAsync(id);
        if (dto == null) return RedirectToAction("Index");

        ViewBag.ListRoles = await _usuarioService.ListarRolesAsync();
        return View(dto);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(UsuarioMantenimientoDTO dto)
    {
        if (!EsAdministrador) return RedirectToAction("Forbidden", "Login");

        if (!ModelState.IsValid)
            return await VolverAlFormulario("Edit", dto);

        var resultado = await _usuarioService.ActualizarAsync(dto);

        if (!resultado.Success)
        {
            ModelState.AddModelError("", resultado.Message ?? "No se pudo actualizar el usuario");
            return await VolverAlFormulario("Edit", dto);
        }

        TempData["Mensaje"] = Util.SweetAlertHelper.Mensaje(
            "Editar Usuario",
            resultado.Message ?? "Usuario actualizado",
            Util.SweetAlertMessageType.success);

        return RedirectToAction("Index");
    }

    /// <summary>
    /// Devuelve el formulario recargando la lista de roles y sin arrastrar la
    /// contraseña escrita, para no reenviarla al navegador.
    /// </summary>
    private async Task<IActionResult> VolverAlFormulario(string vista, UsuarioMantenimientoDTO dto)
    {
        dto.Password = null;
        dto.ConfirmPassword = null;
        ModelState.Remove(nameof(dto.Password));
        ModelState.Remove(nameof(dto.ConfirmPassword));

        ViewBag.ListRoles = await _usuarioService.ListarRolesAsync();
        return View(vista, dto);
    }
}
