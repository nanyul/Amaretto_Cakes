using Amaretto.Infraestructure.Models;

namespace Amaretto.Infraestructure.Repository.Interfaces
{
    public interface IRepositoryPedido
    {
        Task<Pedido> CrearAsync(Pedido pedido);

        /// <summary>
        /// Historial de pedidos ordenado de más reciente a más antiguo.
        /// Cuando <paramref name="idCliente"/> viene con valor solo devuelve los
        /// pedidos de ese cliente; los demás parámetros son filtros opcionales.
        /// </summary>
        Task<ICollection<Pedido>> ListarHistorialAsync(int? idCliente, DateTime? fechaDesde, DateTime? fechaHasta, string? estado);

        /// <summary>
        /// Pedido con todo lo necesario para la pantalla de detalle:
        /// cliente, encargado, líneas (producto/combo/personalización) y pago.
        /// </summary>
        Task<Pedido?> FindByIdAsync(int idPedido);

        /// <summary>
        /// Estados que existen realmente en la tabla, para poblar el filtro.
        /// </summary>
        Task<ICollection<string>> ListarEstadosAsync();
    }
}
