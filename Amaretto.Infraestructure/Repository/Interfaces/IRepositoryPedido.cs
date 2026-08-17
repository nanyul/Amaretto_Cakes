using Amaretto.Infraestructure.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Amaretto.Infraestructure.Repository.Interfaces
{
    public interface IRepositoryPedido
    {
        Task<Pedido> CrearAsync(Pedido pedido);

        Task<ICollection<Pedido>> ListarHistorialAsync(int? idCliente, DateTime? fechaDesde, DateTime? fechaHasta, string? estado, string? cliente = null);

        Task<Pedido?> FindByIdAsync(int idPedido);

        Task<ICollection<string>> ListarEstadosAsync();

        Task<List<(string Nombre, int Cantidad)>> ObtenerTop3ItemsAsync(DateTime fechaDesde, DateTime fechaHasta);

        Task<Dictionary<string, int>> ObtenerPedidosPorEstadoAsync(DateTime fechaDesde, DateTime fechaHasta);
    }
}
