using Amaretto.Infraestructure.Models;

namespace Amaretto.Infraestructure.Repository.Interfaces
{
    public interface IRepositoryPedido
    {
        Task<Pedido> CrearAsync(Pedido pedido);
    }
}