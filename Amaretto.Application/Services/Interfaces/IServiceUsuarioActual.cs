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
        string Rol { get; }
    }
}
