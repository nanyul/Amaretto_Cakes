using Amaretto.Application.DTOs;
using Amaretto.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Amaretto.Web.Controllers
{
    public class LoginController : Controller
    {
        private readonly IServiceUsuario _serviceUsuario;

        public LoginController(IServiceUsuario serviceUsuario)
        {
            _serviceUsuario = serviceUsuario;
        }

        [HttpGet]
        public IActionResult Index()
        {
            if (User.Identity?.IsAuthenticated == true)
                return RedirectToAction("Index", "Home");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LogIn(LoginDTO dto)
        {
            if (!ModelState.IsValid)
                return View("Index", dto);

            var result = await _serviceUsuario.LoginAsync(dto.Email, dto.Password);

            if (!result.Success)
            {
                ModelState.AddModelError("", result.Message ?? "Credenciales inválidas");
                return View("Index", dto);
            }

            var usuario = result.UsuarioDTO!;

            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, usuario.IdUsuario.ToString()),
                new(ClaimTypes.Email, usuario.Email),
                new(ClaimTypes.Name, usuario.NombreCompleto),
                new(ClaimTypes.Role, usuario.NombreRol),
                new("IdRol", usuario.IdRol.ToString()),
                new("NombreRol", usuario.NombreRol),
                new("Telefono", usuario.Telefono ?? ""),
                new("Direccion", usuario.Direccion ?? "")
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            HttpContext.Session.SetInt32("IdUsuario", usuario.IdUsuario);
            HttpContext.Session.SetString("NombreCompleto", usuario.NombreCompleto);
            HttpContext.Session.SetString("Email", usuario.Email);
            HttpContext.Session.SetInt32("IdRol", usuario.IdRol);

            TempData["Mensaje"] = Util.SweetAlertHelper.Mensaje(
                "Bienvenido",
                $"¡Hola, {usuario.NombreCompleto}!",
                Util.SweetAlertMessageType.success);

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult Register()
        {
            if (User.Identity?.IsAuthenticated == true)
                return RedirectToAction("Index", "Home");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterDTO dto)
        {
            if (dto.Password != dto.ConfirmPassword)
                ModelState.AddModelError("ConfirmPassword", "Las contraseñas no coinciden");

            if (dto.Password.Length < 6)
                ModelState.AddModelError("Password", "La contraseña debe tener al menos 6 caracteres");

            if (!ModelState.IsValid)
                return View(dto);

            var result = await _serviceUsuario.RegisterAsync(dto);

            if (!result.Success)
            {
                ModelState.AddModelError("", result.Message ?? "Error en el registro");
                return View(dto);
            }

            var usuario = result.UsuarioDTO!;

            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, usuario.IdUsuario.ToString()),
                new(ClaimTypes.Email, usuario.Email),
                new(ClaimTypes.Name, usuario.NombreCompleto),
                new(ClaimTypes.Role, usuario.NombreRol),
                new("IdRol", usuario.IdRol.ToString()),
                new("NombreRol", usuario.NombreRol),
                new("Telefono", usuario.Telefono ?? ""),
                new("Direccion", usuario.Direccion ?? "")
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            HttpContext.Session.SetInt32("IdUsuario", usuario.IdUsuario);
            HttpContext.Session.SetString("NombreCompleto", usuario.NombreCompleto);
            HttpContext.Session.SetString("Email", usuario.Email);
            HttpContext.Session.SetInt32("IdRol", usuario.IdRol);

            TempData["Mensaje"] = Util.SweetAlertHelper.Mensaje(
                "¡Cuenta creada!",
                $"Bienvenido a Amaretto Cakes, {usuario.NombreCompleto}",
                Util.SweetAlertMessageType.success);

            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LogOff()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            HttpContext.Session.Clear();

            TempData["Mensaje"] = Util.SweetAlertHelper.Mensaje(
                "Hasta pronto",
                "Has cerrado sesión correctamente",
                Util.SweetAlertMessageType.info);

            return RedirectToAction("Index", "Login");
        }

        [HttpGet]
        public async Task<IActionResult> LogOffGet()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            HttpContext.Session.Clear();

            TempData["Mensaje"] = Util.SweetAlertHelper.Mensaje(
                "Hasta pronto",
                "Has cerrado sesión correctamente",
                Util.SweetAlertMessageType.info);

            return RedirectToAction("Index", "Login");
        }

        [HttpGet]
        public IActionResult Forbidden()
        {
            return View();
        }
    }
}