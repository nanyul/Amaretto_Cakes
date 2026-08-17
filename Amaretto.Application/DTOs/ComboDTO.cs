using Amaretto.Infraestructure.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Amaretto.Application.DTOs
{
    public record ComboDTO
    {
        [Display(Name = "Identificador Combo")]
        [ValidateNever]
        public string IdCombo { get; set; } = null!;

        [Display(Name = "Nombre Combo")]
        [Required(ErrorMessage = "{0} es un dato requerido")]
        public string Nombre { get; set; } = null!;


        [Display(Name = "Descripción")]
        [Required(ErrorMessage = "{0} es un dato requerido")]
        public string Descripcion { get; set; } = null!;

        [Display(Name = "Precio")]
        [Range(0, 999999999, ErrorMessage = "El valor mínimo es {0}")]
        [DisplayFormat(DataFormatString = "{0:C0}")]
        [Required(ErrorMessage = "{0} es un dato requerido")]
        public decimal Precio { get; set; }


        public bool Estado { get; set; }


        [Display(Name = "Imagen1 Combo")]
        [Required(ErrorMessage = "{0} es un dato requerido")]
        public string Imagen1 { get; set; } = null!;


        [Display(Name = "Imagen2 Combo")]
        [Required(ErrorMessage = "{0} es un dato requerido")]
        public string Imagen2 { get; set; } = null!;

        [Display(Name = "Productos")]
        [ValidateNever]
        public virtual List<ProductoDTO> Producto { get; set; } = new();
    }
}
