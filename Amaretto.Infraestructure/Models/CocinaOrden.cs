using System;
using System.Collections.Generic;

namespace Amaretto.Infraestructure.Models;

public partial class CocinaOrden
{
    public int IdCocinaOrden { get; set; }

    public int IdDetalle { get; set; }

    public int IdEstacion { get; set; }

    public string Estado { get; set; } = null!;

    public int OrdenPaso { get; set; }

    public int IdEstacion { get; set; 

    public DateTime? FechaInicio { get; set; }

    public DateTime? FechaFin { get; set; }

    public virtual PedidoDetalle IdDetalleNavigation { get; set; } = null!;

    public virtual EstacionCocina IdEstacionNavigation { get; set; } = null!;
}
