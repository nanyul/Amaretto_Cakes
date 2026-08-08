using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Amaretto.Application.Services.Interfaces
{
    public interface IServiceUsuarioActual
    {
        int IdUsuario { get; }
        string Email { get; }
        string NombreCompleto { get; }
        int IdRol { get; }
        string NombreRol { get; }
        string? Telefono { get; }
        string? Direccion { get; }
        bool EstaAutenticado { get; }
    }
}
