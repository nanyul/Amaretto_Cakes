using Amaretto.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Amaretto.Application.Services.Interfaces
{
    public interface IServiceIngrediente
    {
        Task<ICollection<IngredienteDTO>> ListAsync();
        Task<IngredienteDTO> FindByIdAsync(int id);
    }
}
