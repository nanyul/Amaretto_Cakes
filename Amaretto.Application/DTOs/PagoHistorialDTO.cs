using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Amaretto.Application.DTOs
{
    public class PagoHistorialDTO
    {
        public string MetodoPago { get; set; } = null!;
        public string? TipoTarjeta { get; set; }
        public string? UltimosDigitos { get; set; }
        public string? NombreTitular { get; set; }
        public decimal? MontoRecibido { get; set; }
        public decimal? Vuelto { get; set; }
        public DateTime FechaPago { get; set; }
        public string Estado { get; set; } = null!;
    }
}
