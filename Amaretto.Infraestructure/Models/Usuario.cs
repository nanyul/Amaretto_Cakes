using System;
using System.Collections.Generic;

namespace Amaretto.Infraestructure.Models;

public partial class Usuario
{
    public int IdUsuario { get; set; }

    public int IdRol { get; set; }

    public string Email { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string? Telefono { get; set; }

    public string? Sexo { get; set; }

    public string? Direccion { get; set; }

    public bool Estado { get; set; }

    public string NombreCompleto { get; set; } = null!;

    public virtual Rol IdRolNavigation { get; set; } = null!;

    public virtual ICollection<Pedido> PedidoIdEncargadoNavigation { get; set; } = new List<Pedido>();

    public virtual ICollection<Pedido> PedidoIdUsuarioNavigation { get; set; } = new List<Pedido>();
}
