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
    public record MenuDetalleProductoDTO
    {
    [DisplayName("Identificador Menu Detalle Producto")]
    public int IdMenuDetalleProducto { get; set; }

    [DisplayName("Menu Producto")]
    public int IdMenuProducto { get; set; }

    [DisplayName("Producto")]
    public string IdProducto { get; set; } = null!;

    [ValidateNever]
    public virtual ProductoDTO IdProductoNavigation { get; set; } = null!;

    [ValidateNever]
    public virtual MenuProductoDTO IdMenuProductoNavigation { get; set; } = null!;
    }
}
