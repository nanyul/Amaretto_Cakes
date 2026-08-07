using Amaretto.Application.DTOs;
using Amaretto.Application.Services.Interfaces;
using System.Collections.Generic;

namespace Amaretto.Application.Services.Implementations
{
    public class ServicePersonalizacion : IServicePersonalizacion
    {
        private static readonly List<string> Tematicas = new()
        {
            "Unicornio Pastel", "Superhéroes", "Princesas", "Flores", "Deportes", "Animales"
        };

        public List<string> ObtenerTematicas() => Tematicas;

        public decimal CalcularPrecioExtra(PersonalizacionDTO dto)
        {
            decimal extra = 0m;

            extra += dto.Tamano switch
            {
                "Pequeño" => 0m,
                "Mediano" => 2500m,
                "Grande" => 5000m,
                _ => 0m
            };

            extra += dto.TipoDecoracion switch
            {
                "Sin decoración especial" => 0m,
                "Temática de catálogo" => 2000m,
                "Imagen de referencia" => 1500m,
                _ => 0m
            };

            return extra;
        }
    }
}