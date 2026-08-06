using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Amaretto.Application.DTOs
{
    public class PedidoRegistroViewModel
    {
        public ResumenPedidoDTO Resumen { get; set; } = new();
        public UsuarioDTO UsuarioActual { get; set; } = null!;
        public bool EsEncargado { get; set; }
        public List<UsuarioDTO> Clientes { get; set; } = new(); // solo si EsEncargado
        public string MetodoEntrega { get; set; } = "Domicilio";
    }
}
