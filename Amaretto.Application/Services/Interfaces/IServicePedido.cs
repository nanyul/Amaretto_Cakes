using Amaretto.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Amaretto.Application.Services.Interfaces
{
    public interface IServicePedido
    {
        Task<PedidoRegistroViewModel> PrepararRegistroAsync(IServiceUsuarioActual usuarioActual);
        ResumenPedidoDTO ObtenerResumen(string metodoEntrega);
        Task<PedidoResultadoDTO> RegistrarPedidoAsync(PedidoRegistroDTO dto);

        /// <summary>
        /// Historial de pedidos del usuario en sesión. El rol decide el alcance:
        /// el cliente ve solo los suyos, el administrador y el encargado ven
        /// todos y pueden filtrar por fecha y por estado.
        /// </summary>
        Task<PedidoHistorialViewModel> ObtenerHistorialAsync(DateTime? fechaDesde, DateTime? fechaHasta, string? estado);

        /// <summary>
        /// Detalle de un pedido del historial. Devuelve null si el pedido no
        /// existe y lanza UnauthorizedAccessException si el usuario en sesión no
        /// tiene permiso para verlo.
        /// </summary>
        Task<PedidoDetalleCompletoDTO?> ObtenerDetalleHistorialAsync(int idPedido);
    }
}
