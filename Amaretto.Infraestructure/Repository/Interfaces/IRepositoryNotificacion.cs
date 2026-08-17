using Amaretto.Infraestructure.Models;

namespace Amaretto.Infraestructure.Repository.Interfaces
{
    public interface IRepositoryNotificacion
    {
        Task<Notificacion> CrearAsync(Notificacion notificacion);

        Task<ICollection<Notificacion>> ListarPorUsuarioAsync(int idUsuario, int cantidad);

        Task<int> ContarNoLeidasAsync(int idUsuario);

        Task MarcarLeidasAsync(int idUsuario);

        Task<Notificacion?> BuscarPorPedidoAsync(int idPedido);
    }
}
