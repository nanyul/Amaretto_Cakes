using Amaretto.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Amaretto.Application.Services.Interfaces
{
    public interface IServiceUsuario
    {
        Task<ICollection<UsuarioDTO>> ListAsync();
        Task<UsuarioDTO> FindByIdAsync(int id);
        Task<UsuarioLoginResultDTO> LoginAsync(string email, string password);
        Task<UsuarioLoginResultDTO> RegisterAsync(RegisterDTO dto);
        Task<UsuarioDTO?> FindByEmailAsync(string email);
        Task<ICollection<UsuarioDTO>> BuscarClientesAsync(string termino);
    }
}
