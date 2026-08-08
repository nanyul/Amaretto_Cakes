using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Amaretto.Application.DTOs
{
    public class NotificacionDTO
    {
        public int IdNotificacion { get; set; }
        public int? IdPedido { get; set; }
        public string Titulo { get; set; } = null!;
        public string Mensaje { get; set; } = null!;
        public string Tipo { get; set; } = null!;
        public bool Leida { get; set; }
        public DateTime FechaCreacion { get; set; }
        public bool CorreoEnviado { get; set; }
        public string? DetalleEnvio { get; set; }
    }

    /// <summary>
    /// Resultado del intento de notificación: sirve para que el comprobante
    /// diga si el correo de confirmación salió y, si no, por qué.
    /// </summary>
    public class ResultadoNotificacionDTO
    {
        public bool CorreoEnviado { get; set; }
        public string? Detalle { get; set; }
        public string? DestinatarioEmail { get; set; }
    }
}
