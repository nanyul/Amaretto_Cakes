using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Amaretto.Application.DTOs
{
    public class UsuarioLoginResultDTO
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public UsuarioDTO? UsuarioDTO { get; set; }
    }
}