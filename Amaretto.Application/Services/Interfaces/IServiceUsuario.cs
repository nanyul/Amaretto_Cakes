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

        /*  Mantenimiento de usuarios  */

        Task<ICollection<RolDTO>> ListarRolesAsync();

        /// <summary>
        /// Carga el usuario en el formato del formulario de edición. La
        /// contraseña nunca se devuelve: el campo llega vacío.
        /// </summary>
        Task<UsuarioMantenimientoDTO?> ObtenerParaEdicionAsync(int id);

        /// <summary>
        /// Crea el usuario encriptando la contraseña antes de guardarla.
        /// </summary>
        Task<UsuarioLoginResultDTO> CrearAsync(UsuarioMantenimientoDTO dto);

        /// <summary>
        /// Actualiza el usuario. Si la contraseña llega vacía se conserva la que
        /// ya tenía; si viene con valor se guarda encriptada.
        /// </summary>
        Task<UsuarioLoginResultDTO> ActualizarAsync(UsuarioMantenimientoDTO dto);
    }
}
