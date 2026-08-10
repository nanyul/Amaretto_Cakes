using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Amaretto.Application.DTOs
{
    public class PedidoHistorialViewModel
    {
        public UsuarioDTO UsuarioActual { get; set; } = null!;

        public bool EsAdminOEncargado { get; set; }

        public List<PedidoHistorialDTO> Pedidos { get; set; } = new();

        // Filtros
        public DateTime? FechaDesde { get; set; }
        public DateTime? FechaHasta { get; set; }
        public string? Estado { get; set; }

        public List<string> EstadosDisponibles { get; set; } = new();

        public bool HayFiltrosAplicados =>
            FechaDesde.HasValue || FechaHasta.HasValue || !string.IsNullOrWhiteSpace(Estado);
    }
}
