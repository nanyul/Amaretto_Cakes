using Amaretto.Application.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Amaretto.Application.Services.Interfaces
{
    public interface IServiceCocina
    {
        Task<ICollection<CocinaEstacionViewModel>> ListarEstacionesAsync();

        Task<CocinaEstacionViewModel?> ObtenerEstacionAsync(int idEstacion);

        Task<string> AvanzarTareaAsync(int idCocinaOrden, string nuevoEstado);
    }
}
