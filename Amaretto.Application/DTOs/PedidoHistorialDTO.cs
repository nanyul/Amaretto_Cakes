using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Amaretto.Application.DTOs
{
    public class PedidoHistorialDTO
    {
        public int IdPedido { get; set; }
        public DateTime FechaPedido { get; set; }
        public string Estado { get; set; } = null!;
        public string MetodoEntrega { get; set; } = null!;
        public string NombreCliente { get; set; } = null!;
        public string? NombreEncargado { get; set; }
        public int CantidadArticulos { get; set; }
        public decimal Total { get; set; }
    }
}
