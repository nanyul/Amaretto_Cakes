using System;
using System.Collections.Generic;

namespace Amaretto.Infraestructure.Models;

public partial class Pago
{
    public int IdPago { get; set; }

    public int IdPedido { get; set; }

    public string MetodoPago { get; set; } = null!;

    public string? TipoTarjeta { get; set; }

    public string? UltimosDigitos { get; set; }

    public string? NombreTitular { get; set; }

    public decimal? MontoRecibido { get; set; }

    public decimal? Vuelto { get; set; }

    public DateTime FechaPago { get; set; }

    public string Estado { get; set; } = null!;

    public virtual Pedido IdPedidoNavigation { get; set; } = null!;
}
