using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Amaretto.Application.DTOs
{
    /// <summary>
    /// Detalle completo de un pedido del historial: encabezado, líneas y pago.
    /// </summary>
    public class PedidoDetalleCompletoDTO
    {
        public int IdPedido { get; set; }
        public DateTime FechaPedido { get; set; }
        public string Estado { get; set; } = null!;
        public string MetodoEntrega { get; set; } = null!;
        public string? DireccionEntrega { get; set; }
        public string? Observaciones { get; set; }

        // Cliente
        public int IdCliente { get; set; }
        public string NombreCliente { get; set; } = null!;
        public string EmailCliente { get; set; } = null!;
        public string? TelefonoCliente { get; set; }

        // Encargado que registró el pedido (null cuando lo hizo el propio cliente)
        public string? NombreEncargado { get; set; }

        // Montos guardados
        public decimal Subtotal { get; set; }
        public decimal Impuesto { get; set; }
        public decimal CostoEnvio { get; set; }
        public decimal Total { get; set; }

        public List<PedidoLineaHistorialDTO> Lineas { get; set; } = new();
        public PagoHistorialDTO? Pago { get; set; }

        /// <summary>
        /// True cuando quien abre el detalle es Administrador o Encargado; la
        /// vista lo usa para mostrar los datos internos (cliente y encargado).
        /// </summary>
        public bool EsGestor { get; set; }
    }
}
