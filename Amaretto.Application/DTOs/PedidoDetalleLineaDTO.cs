using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Amaretto.Application.DTOs
{
    public class PedidoDetalleLineaDTO
    {
        public string IdItem { get; set; } = null!;
        public string Tipo { get; set; } = null!; // producto o combo
        public string Nombre { get; set; } = null!;
        public string? Imagen { get; set; }
        public decimal PrecioUnitario { get; set; }
        public int Cantidad { get; set; }
        public string? Observaciones { get; set; }

        public decimal Subtotal => Math.Round(PrecioUnitario * Cantidad, 2);
        public decimal Iva => Math.Round(Subtotal * 0.13m, 2);
        public decimal Total => Subtotal + Iva;

        public PersonalizacionDTO? Personalizacion { get; set; }
    }
}
