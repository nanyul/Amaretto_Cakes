using System;
using System.Collections.Generic;

namespace Amaretto.Infraestructure.Models;

public partial class PedidoDetallePersonalizacion
{
    public int IdPersonalizacion { get; set; }

    public int IdDetalle { get; set; }

    public string Tamano { get; set; } = null!;

    public int MedidaCm { get; set; }

    public string SaborBizcocho { get; set; } = null!;

    public string TipoRelleno { get; set; } = null!;

    public string TipoDecoracion { get; set; } = null!;

    public string? DecoracionDetalle { get; set; }

    public string? RutaImagenReferencia { get; set; }

    public string Dedicatoria { get; set; } = null!;

    public decimal PrecioExtra { get; set; }

    public virtual PedidoDetalle IdDetalleNavigation { get; set; } = null!;
}
