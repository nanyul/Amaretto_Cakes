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
    public record ProductoDTO
    {
        [Display(Name = "Identificador Producto")]
        [ValidateNever]
        public string IdProducto { get; set; } = null!;

        public int IdCategoria { get; set; }

        [Display(Name = "Nombre Producto")]
        [Required(ErrorMessage = "{0} es un dato requerido")]
        public string Nombre { get; set; } = null!;

        public string? Descripcion { get; set; }

        public decimal Precio { get; set; }

        public bool EsPersonalizable { get; set; }

        public bool Estado { get; set; }

        public string? Imagen1 { get; set; }

        public string? Imagen2 { get; set; }

        [Display(Name = "Categoría")]
        [ValidateNever]
       public virtual CategoriaDTO IdCategoriaNavigation { get; set; } = null!;

        // public virtual List<MenuDetalleProductoDTO> MenuDetalleProducto { get; set; } = new List<MenuDetalleProductoDTO>();

        //// public virtual List<PedidoDetalleDTO> PedidoDetalle { get; set; } = new List<PedidoDetalle>();

        public virtual List<ProductoIngredienteDTO> ProductoIngrediente { get; set; } = new List<ProductoIngredienteDTO>();

        // public virtual List<ComboProductoDTO> ComboProductos { get; set; } = new();
    }
}
