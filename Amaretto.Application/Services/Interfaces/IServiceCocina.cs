using Amaretto.Application.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Amaretto.Application.Services.Interfaces
{
    /// <summary>
    /// Panel de estación: lo que ve y hace el personal de cocina.
    /// </summary>
    public interface IServiceCocina
    {
        /// <summary>Estaciones activas con el conteo de trabajo pendiente.</summary>
        Task<ICollection<CocinaEstacionViewModel>> ListarEstacionesAsync();

        /// <summary>Cola de trabajo de una estación. Null si la estación no existe.</summary>
        Task<CocinaEstacionViewModel?> ObtenerEstacionAsync(int idEstacion);

        /// <summary>
        /// Marca una tarea como iniciada o terminada. Propaga el estado al
        /// pedido y, si cambió, deja una notificación para el cliente.
        /// Devuelve el mensaje a mostrarle al cocinero.
        /// </summary>
        Task<string> AvanzarTareaAsync(int idCocinaOrden, string nuevoEstado);
    }
}
