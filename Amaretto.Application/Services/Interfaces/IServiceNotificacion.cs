using Amaretto.Application.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Amaretto.Application.Services.Interfaces
{
    public interface IServiceNotificacion
    {
        Task<ResultadoNotificacionDTO> NotificarPedidoRegistradoAsync(PedidoDetalleCompletoDTO pedido);

        Task<ICollection<NotificacionDTO>> ListarAsync(int cantidad = 10);

        Task<int> ContarNoLeidasAsync();

        Task MarcarLeidasAsync();

        Task<NotificacionDTO?> ObtenerPorPedidoAsync(int idPedido);
    }
}
