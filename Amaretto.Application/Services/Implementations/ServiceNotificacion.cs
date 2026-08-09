using Amaretto.Application.Documents;
using Amaretto.Application.DTOs;
using Amaretto.Application.Services.Interfaces;
using Amaretto.Infraestructure.Models;
using Amaretto.Infraestructure.Repository.Interfaces;
using Libreria.Application.Config;
using Microsoft.Extensions.Options;
using QuestPDF.Fluent;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Amaretto.Application.Services.Implementations
{
    /// <summary>
    /// Notificación de pedidos. El comprobante se envía al cliente a través del
    /// servicio de correo SMTP configurado en appsettings y la notificación
    /// queda guardada para mostrarla en la campana del encabezado.
    /// </summary>
    public class ServiceNotificacion : IServiceNotificacion
    {
        private readonly IRepositoryNotificacion _repoNotificacion;
        private readonly IServiceUsuarioActual _usuarioActual;
        private readonly SmtpConfiguration _smtp;

        private static readonly CultureInfo Cultura = new("es-CR");

        public ServiceNotificacion(
            IRepositoryNotificacion repoNotificacion,
            IServiceUsuarioActual usuarioActual,
            IOptions<AppConfig> appConfig)
        {
            _repoNotificacion = repoNotificacion;
            _usuarioActual = usuarioActual;
            _smtp = appConfig.Value.SmtpConfiguration;
        }

        public async Task<ResultadoNotificacionDTO> NotificarPedidoRegistradoAsync(PedidoDetalleCompletoDTO pedido)
        {
            var titulo = $"Pedido #{pedido.IdPedido} registrado";
            var mensaje = $"Tu pedido por {Moneda(pedido.Total)} quedó registrado el " +
                          $"{pedido.FechaPedido.ToString("dd/MM/yyyy hh:mm tt", Cultura)}. Estado: {pedido.Estado}.";

            var resultado = EnviarCorreo(pedido, titulo);

            await _repoNotificacion.CrearAsync(new Notificacion
            {
                IdUsuario = pedido.IdCliente,
                IdPedido = pedido.IdPedido,
                Titulo = titulo,
                Mensaje = Recortar(mensaje, 500)!,
                Tipo = "Pedido",
                Leida = false,
                FechaCreacion = DateTime.Now,
                CorreoEnviado = resultado.CorreoEnviado,
                DetalleEnvio = Recortar(resultado.Detalle, 300)
            });

            return resultado;
        }

        public async Task<ICollection<NotificacionDTO>> ListarMiasAsync(int cantidad = 10)
        {
            if (!_usuarioActual.EstaAutenticado)
                return new List<NotificacionDTO>();

            var lista = await _repoNotificacion.ListarPorUsuarioAsync(_usuarioActual.IdUsuario, cantidad);
            return lista.Select(Mapear).ToList();
        }

        public async Task<int> ContarNoLeidasAsync()
        {
            if (!_usuarioActual.EstaAutenticado) return 0;
            return await _repoNotificacion.ContarNoLeidasAsync(_usuarioActual.IdUsuario);
        }

        public async Task MarcarMiasLeidasAsync()
        {
            if (!_usuarioActual.EstaAutenticado) return;
            await _repoNotificacion.MarcarLeidasAsync(_usuarioActual.IdUsuario);
        }

        public async Task<NotificacionDTO?> ObtenerDePedidoAsync(int idPedido)
        {
            var entidad = await _repoNotificacion.UltimaPorPedidoAsync(idPedido);
            return entidad == null ? null : Mapear(entidad);
        }

        /*  Envío por SMTP  */

        private ResultadoNotificacionDTO EnviarCorreo(PedidoDetalleCompletoDTO pedido, string asunto)
        {
            var resultado = new ResultadoNotificacionDTO { DestinatarioEmail = pedido.EmailCliente };

            // Sin credenciales el pedido igual se registra: se deja constancia del
            // motivo para mostrarlo en el comprobante en vez de romper el flujo.
            if (string.IsNullOrWhiteSpace(_smtp?.UserName) || string.IsNullOrWhiteSpace(_smtp?.Password))
            {
                resultado.CorreoEnviado = false;
                resultado.Detalle = "El servicio de correo no está configurado (SmtpConfiguration sin usuario o contraseña).";
                return resultado;
            }

            if (string.IsNullOrWhiteSpace(pedido.EmailCliente) || !pedido.EmailCliente.Contains('@'))
            {
                resultado.CorreoEnviado = false;
                resultado.Detalle = "El cliente no tiene un correo válido registrado.";
                return resultado;
            }

            try
            {
                // La factura se genera con QuestPDF y viaja como adjunto; el
                // cuerpo del correo solo acompaña con los datos mínimos.
                var pdf = new FacturaPedidoDocument(pedido).GeneratePdf();

                using var cliente = new SmtpClient(_smtp.Server, _smtp.PortNumber)
                {
                    EnableSsl = _smtp.EnableSsl,
                    Credentials = new NetworkCredential(_smtp.UserName, _smtp.Password)
                };

                using var correo = new MailMessage
                {
                    From = new MailAddress(_smtp.UserName, string.IsNullOrWhiteSpace(_smtp.FromName) ? "Amaretto Cakes" : _smtp.FromName),
                    Subject = asunto,
                    Body = CuerpoHtml(pedido),
                    IsBodyHtml = true,
                    BodyEncoding = Encoding.UTF8,
                    SubjectEncoding = Encoding.UTF8
                };
                correo.To.Add(pedido.EmailCliente);

                using var adjunto = new MemoryStream(pdf);
                correo.Attachments.Add(new Attachment(adjunto, NombreArchivo(pedido.IdPedido), "application/pdf"));

                cliente.Send(correo);

                resultado.CorreoEnviado = true;
                resultado.Detalle = $"Factura enviada a {pedido.EmailCliente} como {NombreArchivo(pedido.IdPedido)}.";
            }
            catch (Exception ex)
            {
                resultado.CorreoEnviado = false;
                resultado.Detalle = $"No se pudo enviar el correo: {ex.Message}";
            }

            return resultado;
        }

        private static string NombreArchivo(int idPedido) => $"Factura-Pedido-{idPedido}.pdf";


        /// <summary>
        /// Texto que acompaña al adjunto. El detalle completo va en la factura
        /// PDF, así que aquí solo se repiten los datos de referencia.
        /// </summary>
        /// <summary>
        /// Solo el saludo. Todos los datos del pedido van en la factura adjunta,
        /// así que el cuerpo no repite nada.
        /// </summary>
        private static string CuerpoHtml(PedidoDetalleCompletoDTO p)
        {
            return $@"
<div style=""font-family:Arial,Helvetica,sans-serif;color:#0A3323;"">
    <p>Hola <strong>{Escapar(p.NombreCliente)}</strong>, tu pedido quedó pago correctamente.</p>
</div>";
        }

        /*  Utilidades  */

        private static NotificacionDTO Mapear(Notificacion n) => new()
        {
            IdNotificacion = n.IdNotificacion,
            IdPedido = n.IdPedido,
            Titulo = n.Titulo,
            Mensaje = n.Mensaje,
            Tipo = n.Tipo,
            Leida = n.Leida,
            FechaCreacion = n.FechaCreacion,
            CorreoEnviado = n.CorreoEnviado,
            DetalleEnvio = n.DetalleEnvio
        };

        private static string Moneda(decimal valor) => "₡" + valor.ToString("N2", Cultura);

        private static string? Recortar(string? texto, int maximo) =>
            string.IsNullOrEmpty(texto) || texto.Length <= maximo ? texto : texto[..maximo];

        private static string Escapar(string? texto) =>
            System.Net.WebUtility.HtmlEncode(texto ?? string.Empty);
    }
}
