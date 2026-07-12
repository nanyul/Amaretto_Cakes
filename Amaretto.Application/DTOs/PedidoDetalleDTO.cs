using Amaretto.Application.DTOs;
using Amaretto.Infraestructure.Models;
using Amaretto.Infraestructure.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Amaretto.Application.DTOs
{
    public record PedidoDetalleDTO
    {
        public int IdDetalle { get; set; }

        public int IdPedido { get; set; }

        public string? IdProducto { get; set; }

        public int Cantidad { get; set; }

        public string NombreProductoOCombo { get; set; } = string.Empty;

        public virtual List<CocinaOrdenDTO> CocinaOrden { get; set; } = new List<CocinaOrdenDTO>();
    }
}
