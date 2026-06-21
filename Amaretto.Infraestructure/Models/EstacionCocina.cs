using System;
using System.Collections.Generic;

namespace Amaretto.Infraestructure.Models;

public partial class EstacionCocina
{
    public int IdEstacion { get; set; }

    public string Nombre { get; set; } = null!;

    public string? Descripcion { get; set; }

    public bool Estado { get; set; }

    public virtual ICollection<CocinaOrden> CocinaOrden { get; set; } = new List<CocinaOrden>();
}
