using Amaretto.Infraestructure.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Amaretto.Infraestructure.Repository.Interfaces
{
    public record CocinaOrdenEstacionInput(int IdEstacion, int OrdenPaso);
    public record CocinaOrdenUpdateInput(int IdCocinaOrden, int OrdenPaso, string Estado);

    public record CocinaAvanceResultado(
        int IdPedido,
        int IdCliente,
        string NombreCliente,
        string EmailCliente,
        string EstadoPedido,
        bool CambioEstado);

    public interface IRepositoryCocinaOrden
    {
        Task<ICollection<CocinaOrden>> ListAsync();
        Task<CocinaOrden> FindByIdAsync(string idProducto);
        Task<ICollection<CocinaOrden>> ListByDetalleAsync(int idDetalle);
        Task AddRangoAsync(int idDetalle, List<CocinaOrdenEstacionInput> estaciones);
        Task ActualizarEstadosAsync(int idDetalle, List<CocinaOrdenUpdateInput> filasExistentes, List<CocinaOrdenEstacionInput> filasNuevas);

        Task<ICollection<CocinaOrden>> ListarPorEstacionAsync(int idEstacion);

        Task<ICollection<CocinaOrden>> ListarPasosAsync(IEnumerable<int> idsDetalle);

        Task<CocinaAvanceResultado> AvanzarAsync(int idCocinaOrden, string nuevoEstado);
    }
}
