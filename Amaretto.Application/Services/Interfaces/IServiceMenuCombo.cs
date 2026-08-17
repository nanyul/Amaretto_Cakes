using Amaretto.Application.DTOs;
using Amaretto.Infraestructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Amaretto.Application.Services.Interfaces
{
    public interface IServiceMenuCombo
    {
        Task<ICollection<MenuComboDTO>> ListAsync();

        Task<MenuComboDTO> FindByIdAsync(int id);

        Task<MenuComboDTO?> ObtenerMenuDisponibleAsync();

        Task<int> AddAsync(MenuComboDTO dto, string[] selectedCombos);

        Task UpdateAsync(int id, MenuComboDTO dto, string[] selectedCombos);
    }
}
