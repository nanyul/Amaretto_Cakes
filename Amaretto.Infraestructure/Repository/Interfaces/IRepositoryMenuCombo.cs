using Amaretto.Infraestructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Amaretto.Infraestructure.Repository.Interfaces
{
    public interface IRepositoryMenuCombo
    {
        Task<ICollection<MenuCombo>> ListAsync();

        Task<MenuCombo> FindByIdAsync(int id);

        Task<MenuCombo?> ObtenerMenuDisponibleAsync();
    }
}
