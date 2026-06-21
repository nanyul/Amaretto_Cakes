using Amaretto.Infraestructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Amaretto.Infraestructure.Repository.Interfaces
{
    public interface IRepositoryMenuProducto
    {
        Task<ICollection<MenuProducto>> ListAsync();

        Task<MenuProducto> FindByIdAsync(int id);

        Task<MenuProducto?> ObtenerMenuDisponibleAsync();
    }
}
