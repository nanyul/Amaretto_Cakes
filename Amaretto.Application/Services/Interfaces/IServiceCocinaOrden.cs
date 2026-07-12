using Amaretto.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Amaretto.Application.Services.Interfaces
{
    public interface IServiceCocinaOrden
    {
        Task<ICollection<CocinaOrdenDTO>> ListAsync();
        Task<CocinaOrdenDTO> FindByIdAsync(string id);
        Task<ICollection<CocinaOrdenDTO>> ListByDetalleAsync(int idDetalle);
        Task AddAsync(int idDetalle, List<(int IdEstacion, int OrdenPaso)> estaciones);
        Task UpdateEstadoAsync(int idDetalle, Dictionary<int, string> estadosPorCocinaOrden);
    }
}
