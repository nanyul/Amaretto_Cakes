using System;
using System.Collections.Generic;

namespace Amaretto.Infraestructure.Models;

public partial class Producto
{
    public string IdProducto { get; set; } = null!;

    public int IdCategoria { get; set; }

    public string Nombre { get; set; } = null!;

    public string? Descripcion { get; set; }

    public decimal Precio { get; set; }

    public bool EsPersonalizable { get; set; }

    public bool Estado { get; set; }

    public string? Imagen1 { get; set; }

    public string? Imagen2 { get; set; }

    public virtual Categoria IdCategoriaNavigation { get; set; } = null!;

    public virtual ICollection<MenuDetalleProducto> MenuDetalleProducto { get; set; } = new List<MenuDetalleProducto>();

    public virtual ICollection<PedidoDetalle> PedidoDetalle { get; set; } = new List<PedidoDetalle>();

    public virtual ICollection<ProductoIngrediente> ProductoIngrediente { get; set; } = new List<ProductoIngrediente>();

    public virtual ICollection<Combo> IdCombo { get; set; } = new List<Combo>();
}
