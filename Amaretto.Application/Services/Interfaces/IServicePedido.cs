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

        Task<PedidoHistorialViewModel> ObtenerHistorialAsync(DateTime? fechaDesde, DateTime? fechaHasta, string? estado);

        Task<PedidoDetalleCompletoDTO?> ObtenerDetalleAsync(int idPedido);

        Task<ReporteViewModel> ObtenerReporteAsync(string? cliente, DateTime? fechaDesde, DateTime? fechaHasta, string? estado);
    }
}
