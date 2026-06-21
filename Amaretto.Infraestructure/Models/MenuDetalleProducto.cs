using System;
using System.Collections.Generic;

namespace Amaretto.Infraestructure.Models;

public partial class MenuDetalleProducto
{
    public int IdMenuDetalleProducto { get; set; }

    public int IdMenuProducto { get; set; }

    public string? IdProducto { get; set; }

    public virtual MenuProducto IdMenuProductoNavigation { get; set; } = null!;

    public virtual Producto? IdProductoNavigation { get; set; }
}
