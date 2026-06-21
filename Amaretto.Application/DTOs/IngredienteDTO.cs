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
    public record IngredienteDTO
    {
        public int IdIngrediente { get; set; }

        public string Nombre { get; set; } = null!;

        public bool Estado { get; set; }

        public virtual List<ProductoIngredienteDTO> ProductoIngrediente { get; set; } = new List<ProductoIngredienteDTO>();
    }
}

