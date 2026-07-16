using Amaretto.Application.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Amaretto.Application.Services.Interfaces
{
    public interface IServicePedidoDetalle
    {
        Task<ICollection<PedidoDetalleDTO>> ListDisponiblesAsync();
        Task<PedidoDetalleDTO?> FindByIdAsync(int id);
    }
}
