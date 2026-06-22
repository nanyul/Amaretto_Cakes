using Amaretto.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Amaretto.Application.Services.Interfaces
{
    public interface IServiceCocinaOrden
    {
        Task<ICollection<CocinaOrdenDTO>> ListAsync();
        Task<CocinaOrdenDTO> FindByIdAsync(string id);
    }
}
