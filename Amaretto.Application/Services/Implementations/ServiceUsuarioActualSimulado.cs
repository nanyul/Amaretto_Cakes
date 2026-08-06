using Amaretto.Application.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Amaretto.Application.Services.Implementations
{
    public class ServiceUsuarioActualSimulado : IServiceUsuarioActual
    {
        private readonly IHttpContextAccessor _accessor;

        public ServiceUsuarioActualSimulado(IHttpContextAccessor accessor)
        {
            _accessor = accessor;
        }

        private ISession Session => _accessor.HttpContext!.Session;

        public int IdUsuario => Session.GetInt32("SimIdUsuario") ?? 0;
        public string Rol => Session.GetString("SimRol") ?? "Cliente";

    }
}
