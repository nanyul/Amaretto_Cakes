using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Amaretto.Application.DTOs
{
    public record ComboProductoDTO
    {
        [Display(Name = "Identificador Combo")]
        [ValidateNever]
        public string IdCombo { get; set; } = null!;

        [Display(Name = "Identificador Producto")]
        [ValidateNever]
        public string IdProducto { get; set; } = null!;

        
    }
}
