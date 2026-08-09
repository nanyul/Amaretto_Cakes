using Amaretto.Infraestructure.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Amaretto.Infraestructure.Repository.Interfaces
{
    public record CocinaOrdenEstacionInput(int IdEstacion, int OrdenPaso);
    public record CocinaOrdenUpdateInput(int IdCocinaOrden, int OrdenPaso, string Estado);

    /// <summary>
    /// Qué pasó con el pedido al avanzar una estación. Sirve para avisarle al
    /// cliente solo cuando el estado realmente cambió.
    /// </summary>
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

        /// <summary>
        /// Cola de trabajo de una estación, del pedido más viejo al más nuevo.
        /// </summary>
        Task<ICollection<CocinaOrden>> ListarPorEstacionAsync(int idEstacion);

        /// <summary>
        /// Pasos hermanos (mismo producto del mismo pedido) de las tareas dadas,
        /// para resolver la regla de secuencia sin una consulta por fila.
        /// </summary>
        Task<ICollection<CocinaOrden>> ListarHermanosAsync(IEnumerable<int> idsDetalle);

        /// <summary>
        /// Mueve una tarea a "En Proceso" o "Completado", sella las fechas y
        /// recalcula el estado del pedido completo.
        /// </summary>
        Task<CocinaAvanceResultado> AvanzarAsync(int idCocinaOrden, string nuevoEstado);
    }
}