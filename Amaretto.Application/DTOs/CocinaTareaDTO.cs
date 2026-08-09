using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Amaretto.Application.DTOs
{

    public class CocinaTareaDTO
    {
        public int IdCocinaOrden { get; set; }
        public int IdPedido { get; set; }
        public int OrdenPaso { get; set; }
        public string Estado { get; set; } = null!;

        public string Producto { get; set; } = null!;
        public string? Imagen { get; set; }
        public int Cantidad { get; set; }
        public string? Observaciones { get; set; }
        public bool Personalizado { get; set; }

        public DateTime FechaPedido { get; set; }
        public DateTime? FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }

        public bool Bloqueada { get; set; }
        public string? EsperandoA { get; set; }
    }

    public class CocinaEstacionViewModel
    {
        public int IdEstacion { get; set; }
        public string Nombre { get; set; } = null!;
        public string? Descripcion { get; set; }
        public List<CocinaTareaDTO> Tareas { get; set; } = new();

        public int Pendientes => Tareas.Count(t => t.Estado == "Pendiente");
        public int EnProceso => Tareas.Count(t => t.Estado == "En Proceso");
        public int Completadas => Tareas.Count(t => t.Estado == "Completado");
    }
}
