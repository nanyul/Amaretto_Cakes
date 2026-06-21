using System;
using System.Collections.Generic;

namespace Amaretto.Infraestructure.Models;

public partial class Ingrediente
{
    public int IdIngrediente { get; set; }

    public string Nombre { get; set; } = null!;

    public bool Estado { get; set; }

    public virtual ICollection<ProductoIngrediente> ProductoIngrediente { get; set; } = new List<ProductoIngrediente>();
}
