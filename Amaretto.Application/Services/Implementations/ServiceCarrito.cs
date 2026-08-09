using Amaretto.Application.DTOs;
using Amaretto.Application.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace Amaretto.Application.Services.Implementations
{
    public class ServiceCarrito : IServiceCarrito
    {
        private const string SessionKey = "Carrito";
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ServiceCarrito(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        private ISession Session => _httpContextAccessor.HttpContext!.Session;

        public List<CarritoItem> ObtenerCarrito()
        {
            var json = Session.GetString(SessionKey);
            if (string.IsNullOrEmpty(json)) return new List<CarritoItem>();
            return JsonSerializer.Deserialize<List<CarritoItem>>(json) ?? new List<CarritoItem>();
        }

        private void Guardar(List<CarritoItem> carrito)
        {
            Session.SetString(SessionKey, JsonSerializer.Serialize(carrito));
        }

        public void Agregar(CarritoItem item)
        {
            var carrito = ObtenerCarrito();
            var existente = carrito.FirstOrDefault(i => i.IdItem == item.IdItem && i.Tipo == item.Tipo && i.Personalizacion == null);

            if (existente != null)
                existente.Cantidad += item.Cantidad;
            else
                carrito.Add(item);

            Guardar(carrito);
        }

        public void AgregarPersonalizado(CarritoItem item)
        {
            var carrito = ObtenerCarrito();
            carrito.Add(item); // nunca se fusiona: cada pastel personalizado es único
            Guardar(carrito);
        }

        public void ActualizarCantidad(string idItem, string tipo, int cantidad, string? lineaId = null)
        {
            var carrito = ObtenerCarrito();
            var existente = lineaId != null
                ? carrito.FirstOrDefault(i => i.LineaId == lineaId)
                : carrito.FirstOrDefault(i => i.IdItem == idItem && i.Tipo == tipo);

            if (existente == null) return;

            if (cantidad <= 0)
                carrito.Remove(existente);
            else
                existente.Cantidad = cantidad;

            Guardar(carrito);
        }

        public void Eliminar(string idItem, string tipo, string? lineaId = null)
        {
            var carrito = ObtenerCarrito();
            if (lineaId != null)
                carrito.RemoveAll(i => i.LineaId == lineaId);
            else
                carrito.RemoveAll(i => i.IdItem == idItem && i.Tipo == tipo);

            Guardar(carrito);
        }

        public void Vaciar() => Session.Remove(SessionKey);

        public int ObtenerCantidadTotal() => ObtenerCarrito().Sum(i => i.Cantidad);

        public void ActualizarObservaciones(string idItem, string tipo, string? observaciones, string? lineaId = null)
        {
            var carrito = ObtenerCarrito();
            var existente = lineaId != null
                ? carrito.FirstOrDefault(i => i.LineaId == lineaId)
                : carrito.FirstOrDefault(i => i.IdItem == idItem && i.Tipo == tipo);

            if (existente == null) return;
            existente.Observaciones = observaciones;
            Guardar(carrito);
        }
    }
}