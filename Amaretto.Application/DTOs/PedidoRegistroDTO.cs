using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Amaretto.Application.DTOs
{
    public class PedidoRegistroDTO
    {
        public int IdCliente { get; set; }
        public int? IdEncargado { get; set; }
        public string MetodoEntrega { get; set; } = null!; // Domocilio o Parallevar
        public string? DireccionEntrega { get; set; }
        public string? Observaciones { get; set; }

        // Pago (viene del modal)
        public string MetodoPago { get; set; } = null!; // "Tarjeta" | "Efectivo"
        public string? TipoTarjeta { get; set; }
        public string? UltimosDigitos { get; set; }
        public string? NombreTitular { get; set; }
        public decimal? MontoRecibido { get; set; }
    }

    public class PedidoResultadoDTO
    {
        public int IdPedido { get; set; }
        public decimal Total { get; set; }
        public decimal? Vuelto { get; set; }

        /// <summary>Estado con el que quedó el pedido tras el pago.</summary>
        public string Estado { get; set; } = null!;

        /// <summary>Resultado del envío del comprobante por correo.</summary>
        public bool CorreoEnviado { get; set; }
        public string? DetalleNotificacion { get; set; }
    }
}