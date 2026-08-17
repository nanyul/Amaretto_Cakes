using Amaretto.Application.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Amaretto.Web.Scheduling
{

    public class DesactivacionMenusBackgroundService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<DesactivacionMenusBackgroundService> _logger;

        private readonly TimeSpan _intervalo = TimeSpan.FromSeconds(15);

        public DesactivacionMenusBackgroundService(
            IServiceScopeFactory scopeFactory,
            ILogger<DesactivacionMenusBackgroundService> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using var timer = new PeriodicTimer(_intervalo);

            while (await timer.WaitForNextTickAsync(stoppingToken))
            {
                _logger.LogInformation("Programador: verificando menús vencidos.");

                using var scope = _scopeFactory.CreateScope();
                var servicio = scope.ServiceProvider
                    .GetRequiredService<IServicioDesactivacionMenus>();

                if (await servicio.DebeEjecutarseAsync())
                {
                    await servicio.EjecutarTareaAsync(stoppingToken);
                }
                else
                {
                    _logger.LogInformation("Programador: no hay menús vencidos por desactivar.");
                }
            }
        }
    }
}
