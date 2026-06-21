using System;
using System.Collections.Generic;

namespace Amaretto.Infraestructure.Models;

public partial class MenuCombo
{
    public int IdMenuCombo { get; set; }

    public string Nombre { get; set; } = null!;

    public string? Descripcion { get; set; }

    public DateOnly FechaInicio { get; set; }

    public DateOnly FechaFin { get; set; }

    public bool Estado { get; set; }

    public virtual ICollection<MenuDetalleCombo> MenuDetalleCombo { get; set; } = new List<MenuDetalleCombo>();
}
