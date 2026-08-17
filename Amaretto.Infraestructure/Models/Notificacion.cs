using System;
using System.Collections.Generic;

namespace Amaretto.Infraestructure.Models;

public partial class Notificacion
{
    public int IdNotificacion { get; set; }

    public int IdUsuario { get; set; }

    public int? IdPedido { get; set; }

    public string Titulo { get; set; } = null!;

    public string Mensaje { get; set; } = null!;

    public string Tipo { get; set; } = null!;

    public bool Leida { get; set; }

    public DateTime FechaCreacion { get; set; }

    public bool CorreoEnviado { get; set; }

    public string? DetalleEnvio { get; set; }

    public virtual Usuario IdUsuarioNavigation { get; set; } = null!;

    public virtual Pedido? IdPedidoNavigation { get; set; }
}
