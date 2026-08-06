using System;
using System.Collections.Generic;

namespace Amaretto.Infraestructure.Models;

public partial class Combo
{
    public string IdCombo { get; set; } = null!;

    public string Nombre { get; set; } = null!;

    public string? Descripcion { get; set; }

    public decimal Precio { get; set; }

    public bool Estado { get; set; }

    public string? Imagen1 { get; set; }

    public string? Imagen2 { get; set; }

    public virtual ICollection<MenuDetalleCombo> MenuDetalleCombo { get; set; } = new List<MenuDetalleCombo>();

    public virtual ICollection<PedidoDetalle> PedidoDetalle { get; set; } = new List<PedidoDetalle>();

    public virtual ICollection<Producto> IdProducto { get; set; } = new List<Producto>();
}
