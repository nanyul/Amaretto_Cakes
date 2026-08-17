using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Amaretto.Application.DTOs
{
    public class RegisterDTO
    {
        public string NombreCompleto { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string ConfirmPassword { get; set; } = null!;
        public string? Telefono { get; set; }
        public string? Direccion { get; set; }
        public string? Sexo { get; set; }
    }
}