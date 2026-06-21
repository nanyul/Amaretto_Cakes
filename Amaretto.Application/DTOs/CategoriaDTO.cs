using Amaretto.Application.DTOs;
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
    public record CategoriaDTO
    {
        [Display(Name = "Identificador Categoria")]
        [ValidateNever]
        public int IdCategoria { get; set; }

        [Display(Name = "Nombre Categoría")]
        [Required(ErrorMessage = "{0} es un dato requerido")]
        public string Nombre { get; set; } = null!;

        //public virtual List<ProductoIngredienteDTO> IdProducto { get; set; } = null!;
        public virtual List<ProductoDTO> Producto { get; set; } = new List<ProductoDTO>();
    }
}
