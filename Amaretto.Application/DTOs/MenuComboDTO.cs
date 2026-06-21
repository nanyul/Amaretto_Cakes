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
    public record MenuComboDTO
    {
        [Display(Name = "IdMenuDetalleCombo")]
        public int IdMenuCombo { get; set; }

        public string Nombre { get; set; } = null!;

        public string? Descripcion { get; set; }

        public DateOnly FechaInicio { get; set; }

        public DateOnly FechaFin { get; set; }

        public bool Estado { get; set; }

        public virtual List<MenuDetalleComboDTO> MenuDetalleCombo { get; set; } = new List<MenuDetalleComboDTO>();

        public string DiasDisponibles
        {
            get
            {
                if (FechaInicio == default || FechaFin == default)
                    return "No hay fechas definidas";

                var dias = (FechaFin.ToDateTime(TimeOnly.MinValue) -
                            FechaInicio.ToDateTime(TimeOnly.MinValue)).Days + 1;

                return dias == 1
                    ? "1 día"
                    : $"{dias} días";
            }
        }
    }
}
