using System;
using System.Collections.Generic;

namespace Amaretto.Infraestructure.Models;

public partial class PedidoDetalle
{
    public int IdDetalle { get; set; }

    public int IdPedido { get; set; }

    public string? IdProducto { get; set; }

    public string? IdCombo { get; set; }

    public int Cantidad { get; set; }

    public decimal PrecioUnitario { get; set; }

    public decimal Subtotal { get; set; }

    public string? Observaciones { get; set; }

    public decimal Iva { get; set; }

    public decimal Total { get; set; }

    public bool Personalizado { get; set; }

    public virtual ICollection<CocinaOrden> CocinaOrden { get; set; } = new List<CocinaOrden>();

    public virtual Combo? IdComboNavigation { get; set; }

    public virtual Pedido IdPedidoNavigation { get; set; } = null!;

    public virtual Producto? IdProductoNavigation { get; set; }

    public virtual PedidoDetallePersonalizacion? PedidoDetallePersonalizacion { get; set; }
}
