using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Amaretto.Application.DTOs
{
    public class PersonalizacionDTO
    {
        // Usados al recibir el formulario
        [Required] public string IdProducto { get; set; } = null!;
        public int Cantidad { get; set; } = 1;

        // Comunes: formulario y almacenamiento en carrito
        [Required] public string Tamano { get; set; } = null!;
        public int MedidaCm { get; set; }
        [Required] public string SaborBizcocho { get; set; } = null!;
        [Required] public string TipoRelleno { get; set; } = null!;
        [Required] public string TipoDecoracion { get; set; } = null!;
        public string? DecoracionDetalle { get; set; }
        public string? RutaImagenReferencia { get; set; }
        [Required, StringLength(300, MinimumLength = 1)]
        public string Dedicatoria { get; set; } = null!;

        // Se completa en el servidor, no viene del formulario
        public decimal PrecioExtra { get; set; }
    }
}
