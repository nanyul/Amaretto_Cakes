using Amaretto.Application.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Amaretto.Application.Services.Implementations
{
    public class ServiceUsuarioActual : IServiceUsuarioActual
    {
        private readonly IHttpContextAccessor _accessor;

        public ServiceUsuarioActual(IHttpContextAccessor accessor)
        {
            _accessor = accessor;
        }

        private ClaimsPrincipal? User => _accessor.HttpContext?.User;

        public bool EstaAutenticado => User?.Identity?.IsAuthenticated ?? false;

        public int IdUsuario => EstaAutenticado 
            ? int.Parse(User!.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0") 
            : 0;

        public string Email => EstaAutenticado ? User!.FindFirst(ClaimTypes.Email)?.Value ?? "" : "";

        public string NombreCompleto => EstaAutenticado ? User!.FindFirst(ClaimTypes.Name)?.Value ?? "" : "";

        public int IdRol => EstaAutenticado 
            ? int.Parse(User!.FindFirst("IdRol")?.Value ?? "0") 
            : 0;

        public string NombreRol => EstaAutenticado ? User!.FindFirst("NombreRol")?.Value ?? "" : "";

        public string? Telefono => EstaAutenticado ? User!.FindFirst("Telefono")?.Value : null;

        public string? Direccion => EstaAutenticado ? User!.FindFirst("Direccion")?.Value : null;
    }
}