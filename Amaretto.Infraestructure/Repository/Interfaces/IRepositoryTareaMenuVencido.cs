using System.Collections.Generic;
using System.Threading.Tasks;

namespace Amaretto.Infraestructure.Repository.Interfaces
{
    public interface IRepositoryTareaMenuVencido
    {

        Task<bool> ExistenMenusVencidosActivosAsync();
        Task<List<(string Tipo, string Nombre, DateOnly FechaFin)>> DesactivarMenusVencidosAsync();
    }
}
