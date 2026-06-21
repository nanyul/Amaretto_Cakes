using System.ComponentModel.DataAnnotations;

namespace Amaretto.Web.ViewModels
{
    public class ViewModelInput
    {

        [Display(Name = "Libro")]
        public int IdLibro { get; set; }

        [Display(Name = "Cantidad")]
        [Range(0, 999999999, ErrorMessage = "Cantidad mínimo es {0}")]
        public int Cantidad { get; set; }
        [Range(0, 999999999, ErrorMessage = "Precio mínimo es {0}")]
        [Display(Name = "Precio")]
        public decimal Precio { get; set; }
    }
}
