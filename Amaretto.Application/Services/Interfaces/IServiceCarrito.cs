using Amaretto.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Amaretto.Application.Services.Interfaces
{
    public interface IServiceCarrito
    {
        List<CarritoItem> ObtenerCarrito();
        void Agregar(CarritoItem item);
        void AgregarPersonalizado(CarritoItem item);
        void ActualizarCantidad(string idItem, string tipo, int cantidad, string? lineaId = null);
        void Eliminar(string idItem, string tipo, string? lineaId = null);
        void ActualizarObservaciones(string idItem, string tipo, string? observaciones);
        void Vaciar();
        int ObtenerCantidadTotal();
    }
}
