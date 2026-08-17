using Amaretto.Infraestructure.Models;

namespace Amaretto.Infraestructure.Repository.Interfaces
{
    public interface IRepositoryPedido
    {
        Task<Pedido> CrearAsync(Pedido pedido);

        Task<ICollection<Pedido>> ListarHistorialAsync(int? idCliente, DateTime? fechaDesde, DateTime? fechaHasta, string? estado, string? cliente = null);

        Task<Pedido?> FindByIdAsync(int idPedido);

        Task<ICollection<string>> ListarEstadosAsync();
    }
}
