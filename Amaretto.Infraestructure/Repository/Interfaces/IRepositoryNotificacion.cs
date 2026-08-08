using Amaretto.Infraestructure.Models;

namespace Amaretto.Infraestructure.Repository.Interfaces
{
    public interface IRepositoryNotificacion
    {
        Task<Notificacion> CrearAsync(Notificacion notificacion);

        /// <summary>
        /// Notificaciones del usuario, de la más reciente a la más antigua.
        /// </summary>
        Task<ICollection<Notificacion>> ListarPorUsuarioAsync(int idUsuario, int cantidad);

        Task<int> ContarNoLeidasAsync(int idUsuario);

        Task MarcarLeidasAsync(int idUsuario);

        /// <summary>
        /// Última notificación asociada a un pedido, para mostrar en el comprobante
        /// si el correo de confirmación salió o no.
        /// </summary>
        Task<Notificacion?> UltimaPorPedidoAsync(int idPedido);
    }
}
