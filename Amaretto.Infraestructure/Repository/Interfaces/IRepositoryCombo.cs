using Amaretto.Infraestructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Amaretto.Infraestructure.Repository.Interfaces
{
    public interface IRepositoryCombo
    {
        Task<ICollection<Combo>> ListAsync();
        Task<Combo> FindByIdAsync(string id);
        Task<ICollection<Combo>> FilterAsync(string? estado, decimal? precioMax, List<int>? categoriaIds, string? ordenarPor);


    }
}
