using Amaretto.Application.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Amaretto.Application.Services.Interfaces
{
    public interface IServiceNotificacion
    {
        /// <summary>
        /// Notifica al cliente que su pedido quedó registrado: envía el
        /// comprobante por correo y deja la notificación guardada para la
        /// campana del encabezado.
        /// </summary>
        Task<ResultadoNotificacionDTO> NotificarPedidoRegistradoAsync(PedidoDetalleCompletoDTO pedido);

        /// <summary>
        /// Notificaciones del usuario en sesión para el panel del encabezado.
        /// </summary>
        Task<ICollection<NotificacionDTO>> ListarMiasAsync(int cantidad = 10);

        Task<int> ContarNoLeidasAsync();

        Task MarcarMiasLeidasAsync();

        Task<NotificacionDTO?> ObtenerDePedidoAsync(int idPedido);
    }
}
