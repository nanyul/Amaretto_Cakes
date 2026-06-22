using Amaretto.Infraestructure.Models;
using Amaretto.Infraestructure.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Amaretto.Application.DTOs
{
    public record CocinaOrdenDTO
    {
        public int IdCocinaOrden { get; set; }

        public int IdDetalle { get; set; }

        public int IdEstacion { get; set; }

        public string Estado { get; set; } = null!;

        public int OrdenPaso { get; set; }

        public DateTime? FechaInicio { get; set; }

        public DateTime? FechaFin { get; set; }

        public virtual EstacionCocinaDTO IdEstacionNavigation { get; set; } = null!;

        public virtual PedidoDetalle IdDetalleNavigation { get; set; } = null!;
    }
}
