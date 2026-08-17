using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Amaretto.Application.DTOs
{
    public class ReporteViewModel
    {
        public List<PedidoHistorialDTO> Pedidos { get; set; } = new();

        public string? Cliente { get; set; }
        public DateTime? FechaDesde { get; set; }
        public DateTime? FechaHasta { get; set; }
        public string? Estado { get; set; }

        public List<string> EstadosDisponibles { get; set; } = new();

        public bool HayFiltrosAplicados =>
            !string.IsNullOrWhiteSpace(Cliente) || FechaDesde.HasValue
            || FechaHasta.HasValue || !string.IsNullOrWhiteSpace(Estado);

        /*  Totales del reporte  */

        public int CantidadPedidos => Pedidos.Count;
        public int CantidadArticulos => Pedidos.Sum(p => p.CantidadArticulos);
        public decimal MontoTotal => Pedidos.Sum(p => p.Total);
        public decimal PromedioPorPedido =>
            CantidadPedidos == 0 ? 0m : Math.Round(MontoTotal / CantidadPedidos, 2);
    }
}
