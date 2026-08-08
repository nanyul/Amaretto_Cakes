using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Amaretto.Application.DTOs
{
    public class PedidoDetalleLineaDTO
    {
        /// <summary>
        /// Identifica la línea dentro del carrito. Es lo que distingue dos
        /// pasteles personalizados del mismo producto, que comparten IdItem.
        /// </summary>
        public string LineaId { get; set; } = null!;

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
