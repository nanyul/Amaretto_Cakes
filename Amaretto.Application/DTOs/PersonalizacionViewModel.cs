using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Amaretto.Application.DTOs
{
    public class PersonalizacionViewModel
    {
        public string IdProducto { get; set; } = null!;
        public string NombreProducto { get; set; } = null!;
        public string? Descripcion { get; set; }
        public string? Imagen { get; set; }
        public decimal PrecioBase { get; set; }
        public int Cantidad { get; set; }
        public List<string> Tematicas { get; set; } = new();
    }
}
