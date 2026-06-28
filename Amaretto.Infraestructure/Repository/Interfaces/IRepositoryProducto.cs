using Amaretto.Infraestructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Amaretto.Infraestructure.Repository.Interfaces
{
    public interface IRepositoryProducto
    {
        Task<ICollection<Producto>> ListAsync();
        Task<Producto?> FindByIdAsync(string id);
        Task<ICollection<Producto>> FilterAsync(string? estado, decimal? precioMax, List<int>? categoriaIds, string? ordenarPor);
    }
}
