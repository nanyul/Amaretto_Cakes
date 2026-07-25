using Amaretto.Application.Services.Interfaces;
using Amaretto.Infraestructure.Repository.Interfaces;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Amaretto.Application.Services.Implementations
{
    public class ServicioDesactivacionMenus : IServicioDesactivacionMenus
    {
        private readonly IRepositoryTareaMenuVencido _repositorio;
        private readonly ITareaProgramadaEstado _estado;
        private readonly ILogger<ServicioDesactivacionMenus> _logger;

        public ServicioDesactivacionMenus(
            IRepositoryTareaMenuVencido repositorio,
            ITareaProgramadaEstado estado,
            ILogger<ServicioDesactivacionMenus> logger)
        {
            _repositorio = repositorio;
            _estado = estado;
            _logger = logger;
        }

        public async Task<bool> DebeEjecutarseAsync()
        {
            return await _repositorio.ExistenMenusVencidosActivosAsync();
        }

        public async Task EjecutarTareaAsync(CancellationToken cancellationToken)
        {
            var afectados = await _repositorio.DesactivarMenusVencidosAsync();

            var detalle = afectados
                .Select(a => $"{a.Tipo}: {a.Nombre} (venció el {a.FechaFin:dd/MM/yyyy})")
                .ToList();

            _estado.RegistrarEjecucion(afectados.Count, detalle);

            _logger.LogInformation(
                "Tarea de desactivación de menús ejecutada: {Cantidad} menú(s) desactivado(s).",
                afectados.Count);
        }
    }
}
