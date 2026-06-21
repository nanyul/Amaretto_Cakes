using System;
using System.Collections.Generic;

namespace Amaretto.Infraestructure.Models;

public partial class Pedido
{
    public int IdPedido { get; set; }

    public int IdUsuario { get; set; }

    public string Estado { get; set; } = null!;

    public string MetodoEntrega { get; set; } = null!;

    public string? DireccionEntrega { get; set; }

    public decimal CostoEnvio { get; set; }

    public decimal Subtotal { get; set; }

    public decimal Impuesto { get; set; }

    public decimal Total { get; set; }

    public string? Observaciones { get; set; }

    public DateTime FechaPedido { get; set; }

    public virtual Usuario IdUsuarioNavigation { get; set; } = null!;

    public virtual ICollection<Pago> Pago { get; set; } = new List<Pago>();

    public virtual ICollection<PedidoDetalle> PedidoDetalle { get; set; } = new List<PedidoDetalle>();
}
