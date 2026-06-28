using Amaretto.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Amaretto.Application.Services.Interfaces
{
    public interface IServiceProducto
    {
        Task<ICollection<ProductoDTO>> ListAsync();
        Task<ProductoDTO> FindByIdAsync(string id);
        Task<ICollection<ProductoDTO>> FilterAsync(string? estado, decimal? precioMax, List<int>? categoriaIds, string? ordenarPor);
  
    }
}
