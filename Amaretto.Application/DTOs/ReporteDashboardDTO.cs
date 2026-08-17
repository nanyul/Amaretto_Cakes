using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Amaretto.Application.DTOs
{
    public class ReporteDashboardDTO
    {
        public DateTime FechaDesde { get; set; }
        public DateTime FechaHasta { get; set; }
        public List<TopItemDTO> TopItems { get; set; } = new();
        public List<EstadoCantidadDTO> PedidosPorEstado { get; set; } = new();
    }

    public class TopItemDTO
    {
        public string Nombre { get; set; } = null!;
        public int Cantidad { get; set; }
    }

    public class EstadoCantidadDTO
    {
        public string Estado { get; set; } = null!;
        public int Cantidad { get; set; }
    }
}