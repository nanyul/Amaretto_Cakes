namespace Amaretto.Application.DTOs;

public class CarritoItem
{
    public string IdItem { get; set; } = null!;   // IdProducto o IdCombo
    public string Tipo { get; set; } = null!;      // "producto" o "combo"
    public string Nombre { get; set; } = null!;
    public decimal Precio { get; set; }
    public int Cantidad { get; set; }
    public string? Imagen { get; set; }
    public string? Observaciones { get; set; }
}
