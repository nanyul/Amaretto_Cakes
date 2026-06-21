using System;
using System.Collections.Generic;

namespace Amaretto.Infraestructure.Models;

public partial class ProductoIngrediente
{
    public string IdProducto { get; set; } = null!;

    public int IdIngrediente { get; set; }

    public decimal Cantidad { get; set; }

    public virtual Ingrediente IdIngredienteNavigation { get; set; } = null!;

    public virtual Producto IdProductoNavigation { get; set; } = null!;
}
