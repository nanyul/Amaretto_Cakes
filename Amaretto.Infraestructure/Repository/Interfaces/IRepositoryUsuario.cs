using Amaretto.Infraestructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Amaretto.Infraestructure.Repository.Interfaces
{
    public interface IRepositoryUsuario
    {
        Task<ICollection<Usuario>> ListAsync();
        Task<Usuario> FindByIdAsync(int id);
        Task<Usuario?> FindByEmailAsync(string email);
        Task<Usuario> AddAsync(Usuario usuario);
        Task UpdateAsync(Usuario usuario);
        Task<bool> ExistsByEmailAsync(string email);

        Task<bool> ExistsByEmailAsync(string email, int idExcluir);

        Task<List<Usuario>> ObtenerPorRolAsync(string rol);
        Task<List<Usuario>> BuscarClientesAsync(string termino);
        Task<ICollection<Rol>> ListarRolesAsync();
    }
}
