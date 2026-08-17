using Amaretto.Application.DTOs;
using Amaretto.Infraestructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Amaretto.Application.Services.Interfaces
{
    public interface IServiceMenuProducto
    {
        Task<ICollection<MenuProductoDTO>> ListAsync();

        Task<MenuProductoDTO> FindByIdAsync(int id);

        Task<MenuProductoDTO?> ObtenerMenuDisponibleAsync();

        Task<int> AddAsync(MenuProductoDTO dto, string[] selectedProductos);

        Task UpdateAsync(int id, MenuProductoDTO dto, string[] selectedProductos);
    }
}
