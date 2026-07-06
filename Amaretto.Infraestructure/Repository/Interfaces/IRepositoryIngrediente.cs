using Amaretto.Infraestructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Amaretto.Infraestructure.Repository.Interfaces
{
    public interface IRepositoryIngrediente
    {
        Task<ICollection<Ingrediente>> ListAsync();
        Task<Ingrediente?> FindByIdAsync(int id);
    }
}
