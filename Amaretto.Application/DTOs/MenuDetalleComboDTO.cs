using Amaretto.Infraestructure.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Amaretto.Application.DTOs
{
    public record MenuDetalleComboDTO
    {
        [DisplayName("Identificador Menu Detalle Combo")]
        [Required(ErrorMessage = "{0} es un dato requerido")]
        public int IdMenuDetalleCombo { get; set; }

        [DisplayName("Menu Combo")]
        [Required(ErrorMessage = "{0} es un dato requerido")]
        public int IdMenuCombo { get; set; }
        
        [DisplayName("Combo")]
        public string IdCombo { get; set; } = null!;

        [DisplayName("Combo")]
        [ValidateNever]
        public virtual ComboDTO IdComboNavigation { get; set; } = null!;

        [DisplayName("Menu Combo")]
        [ValidateNever]
        public virtual MenuComboDTO IdMenuComboNavigation { get; set; } = null!;
    }
}
