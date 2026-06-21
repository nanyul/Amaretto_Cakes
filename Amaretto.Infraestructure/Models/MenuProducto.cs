using System;
using System.Collections.Generic;

namespace Amaretto.Infraestructure.Models;

public partial class MenuProducto
{
    public int IdMenuProducto { get; set; }

    public string Nombre { get; set; } = null!;

    public string? Descripcion { get; set; }

    public DateOnly FechaInicio { get; set; }

    public DateOnly FechaFin { get; set; }

    public bool Estado { get; set; }

    public virtual ICollection<MenuDetalleProducto> MenuDetalleProducto { get; set; } = new List<MenuDetalleProducto>();
}
