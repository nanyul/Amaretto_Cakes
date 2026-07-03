using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Amaretto.Application.DTOs
{
    public class UsuarioDTO
    {
        public int IdUsuario { get; set; }
        public string NombreCompleto { get; set; } = null!;
        public string NombreRol { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string? Telefono { get; set; }
    }
}
