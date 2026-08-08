using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Amaretto.Application.DTOs
{
    public class PedidoHistorialViewModel
    {
        /// <summary>
        /// Usuario identificado en la sesión: de él dependen los pedidos que se
        /// listan y si la barra de filtros se muestra o no.
        /// </summary>
        public UsuarioDTO UsuarioActual { get; set; } = null!;

        /// <summary>
        /// True para Administrador y Encargado: ven todos los pedidos y filtran.
        /// False para el resto: solo ven los pedidos propios.
        /// </summary>
        public bool EsGestor { get; set; }

        public List<PedidoHistorialDTO> Pedidos { get; set; } = new();

        // Filtros aplicados (solo se usan cuando EsGestor es true)
        public DateTime? FechaDesde { get; set; }
        public DateTime? FechaHasta { get; set; }
        public string? Estado { get; set; }

        public List<string> EstadosDisponibles { get; set; } = new();

        public bool HayFiltrosAplicados =>
            FechaDesde.HasValue || FechaHasta.HasValue || !string.IsNullOrWhiteSpace(Estado);
    }
}
