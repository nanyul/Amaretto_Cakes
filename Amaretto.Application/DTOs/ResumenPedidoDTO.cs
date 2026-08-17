using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Amaretto.Application.DTOs
{
    public class ResumenPedidoDTO
    {
        public List<PedidoDetalleLineaDTO> Lineas { get; set; } = new();
        public decimal Subtotal => Lineas.Sum(l => l.Subtotal);
        public decimal Impuesto => Lineas.Sum(l => l.Iva);
        public decimal CostoEnvio { get; set; }
        public decimal Total => Subtotal + Impuesto + CostoEnvio;
    }
}
