using Amaretto.Infraestructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Amaretto.Infraestructure.Repository.Interfaces
{
    public interface IRepositoryEstacion
    {
        Task<ICollection<EstacionCocina>> ListAsync();
        Task<EstacionCocina?> FindByIdAsync(string id);
    }
}
