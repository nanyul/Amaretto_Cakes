using Amaretto.Infraestructure.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Amaretto.Infraestructure.Repository.Interfaces
{
    public interface IRepositoryPedidoDetalle
    {
        Task<ICollection<PedidoDetalle>> ListDisponiblesAsync();

        Task<PedidoDetalle?> FindByIdAsync(int id);
    }
}
