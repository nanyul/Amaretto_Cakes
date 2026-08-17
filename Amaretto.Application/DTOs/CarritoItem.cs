namespace Amaretto.Application.DTOs;

public class CarritoItem
{
    public string LineaId { get; set; } = Guid.NewGuid().ToString("N");
    public string IdItem { get; set; } = null!;
    public string Tipo { get; set; } = null!;
    public string Nombre { get; set; } = null!;
    public decimal Precio { get; set; }
    public int Cantidad { get; set; }
    public string? Imagen { get; set; }
    public string? Observaciones { get; set; }
    public PersonalizacionDTO? Personalizacion { get; set; }
}
