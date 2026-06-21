using Amaretto.Application.DTOs;
using System.ComponentModel.DataAnnotations;

namespace Amaretto.Web.ViewModels
{
    public class ViewModelMenuDisponible
    {

        public string NombreMenu { get; set; }

        public DateOnly FechaInicio { get; set; }

        public DateOnly FechaFin { get; set; }

        public List<ViewModelCategoriaMenu> Categorias { get; set; }
                = new();

        public List<ComboDTO> Combos { get; set; }
                = new();
    }
}
