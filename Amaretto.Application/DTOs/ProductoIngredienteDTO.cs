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
    public record ProductoIngredienteDTO
    {
        [Display(Name = "Identificador Producto")]
        [ValidateNever]
        public string IdProducto { get; set; } = null!;

        [Display(Name = "Identificador Ingrediente")]
        [ValidateNever]
        public int IdIngrediente { get; set; }

        public decimal Cantidad { get; set; }

        public virtual IngredienteDTO IdIngredienteNavigation { get; set; } = null!;
    }
}
