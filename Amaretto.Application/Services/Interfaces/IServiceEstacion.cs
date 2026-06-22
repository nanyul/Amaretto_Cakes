using Amaretto.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Amaretto.Application.Services.Interfaces
{
    public interface IServiceEstacion
    {
        Task<ICollection<EstacionCocinaDTO>> ListAsync();
        Task<EstacionCocinaDTO> FindByIdAsync(string id);
    }
}
