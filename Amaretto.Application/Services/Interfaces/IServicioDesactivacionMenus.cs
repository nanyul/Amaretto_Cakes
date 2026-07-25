using System.Threading;
using System.Threading.Tasks;

namespace Amaretto.Application.Services.Interfaces
{
    public interface IServicioDesactivacionMenus
    {
        Task<bool> DebeEjecutarseAsync();
        Task EjecutarTareaAsync(CancellationToken cancellationToken);
    }
}
