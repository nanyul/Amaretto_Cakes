using System;
using System.Collections.Generic;

namespace Amaretto.Infraestructure.Models;

public partial class PedidoDetallePersonalizacion
{
    public int IdPersonalizacion { get; set; }

    public int IdDetalle { get; set; }

    public int? IdPersonalizacionTamano { get; set; }

    public int? IdPersonalizacionSabor { get; set; }

    public int? IdPersonalizacionRelleno { get; set; }

    public int? IdPersonalizacionDecoracion { get; set; }

    public string? RutaImagenReferencia { get; set; }

    public string? Mensaje { get; set; }

    public int? TiempoEst { get; set; }

    public decimal PrecioExtra { get; set; }

    public int? MedidaCm { get; set; }

    public string? Imagen1 { get; set; }

    public string? Imagen2 { get; set; }

    public virtual PedidoDetalle IdDetalleNavigation { get; set; } = null!;
}
