using Amaretto.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Amaretto.Application.Services.Interfaces
{
    public interface IServiceCombo
    {
        Task<ICollection<ComboDTO>> ListAsync();
        Task<ComboDTO> FindByIdAsync(string id);
        Task<ICollection<ComboDTO>> FilterAsync(string? estado, decimal? precioMax, List<int>? categoriaIds, string? ordenarPor);
        Task<string> AddAsync(ComboDTO dto, string[] selectedProductos);
        Task UpdateAsync(string id, ComboDTO dto, string[] selectedProductos);

    }
}
