using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Amaretto.Application.DTOs
{
    public class PedidoLineaHistorialDTO
    {
        public int IdDetalle { get; set; }
        public string Tipo { get; set; } = null!; // producto o combo
        public string Nombre { get; set; } = null!;
        public string? Imagen { get; set; }
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
        public decimal Subtotal { get; set; }
        public decimal Iva { get; set; }
        public decimal Total { get; set; }
        public string? Observaciones { get; set; }
        public bool Personalizado { get; set; }
        public PersonalizacionDTO? Personalizacion { get; set; }
    }
}
