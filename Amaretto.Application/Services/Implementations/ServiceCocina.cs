using Amaretto.Application.DTOs;
using Amaretto.Application.Services.Interfaces;
using Amaretto.Infraestructure.Models;
using Amaretto.Infraestructure.Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Amaretto.Application.Services.Implementations
{
    public class ServiceCocina : IServiceCocina
    {
        private readonly IRepositoryCocinaOrden _repoCocina;
        private readonly IRepositoryEstacion _repoEstacion;
        private readonly IRepositoryNotificacion _repoNotificacion;

        public ServiceCocina(
            IRepositoryCocinaOrden repoCocina,
            IRepositoryEstacion repoEstacion,
            IRepositoryNotificacion repoNotificacion)
        {
            _repoCocina = repoCocina;
            _repoEstacion = repoEstacion;
            _repoNotificacion = repoNotificacion;
        }

        public async Task<ICollection<CocinaEstacionViewModel>> ListarEstacionesAsync()
        {
            var estaciones = (await _repoEstacion.ListAsync())
                .Where(e => e.Estado)
                .OrderBy(e => e.IdEstacion)
                .ToList();

            var resultado = new List<CocinaEstacionViewModel>();

            foreach (var estacion in estaciones)
            {
                var vm = await CargarEstacionAsync(estacion);
                resultado.Add(vm);
            }

            return resultado;
        }

        public async Task<CocinaEstacionViewModel?> ObtenerEstacionAsync(int idEstacion)
        {
            var estacion = (await _repoEstacion.ListAsync())
                .FirstOrDefault(e => e.IdEstacion == idEstacion);

            return estacion == null ? null : await CargarEstacionAsync(estacion);
        }

        public async Task<string> AvanzarTareaAsync(int idCocinaOrden, string nuevoEstado)
        {
            var avance = await _repoCocina.AvanzarAsync(idCocinaOrden, nuevoEstado);

            if (!avance.CambioEstado)
                return nuevoEstado == "Completado" ? "Estación completada." : "Estación iniciada.";

            await _repoNotificacion.CrearAsync(new Notificacion
            {
                IdUsuario = avance.IdCliente,
                IdPedido = avance.IdPedido,
                Titulo = $"Pedido #{avance.IdPedido}: {avance.EstadoPedido}",
                Mensaje = MensajeEstado(avance.IdPedido, avance.EstadoPedido),
                Tipo = "Pedido",
                Leida = false,
                FechaCreacion = DateTime.Now,
                CorreoEnviado = false,
                DetalleEnvio = null
            });

            return $"El pedido #{avance.IdPedido} pasó a \"{avance.EstadoPedido}\". Se notificó al cliente.";
        }

        private static string MensajeEstado(int idPedido, string estado) => estado switch
        {
            "En Preparacion" => $"Tu pedido #{idPedido} entró en preparación: ya estamos trabajando en él.",
            "Listo para retirar" => $"Tu pedido #{idPedido} está listo. Podés pasar a retirarlo por la tienda.",
            "Listo para enviar" => $"Tu pedido #{idPedido} está listo y sale hacia tu dirección de entrega.",
            _ => $"Tu pedido #{idPedido} cambió de estado a {estado}."
        };

        private async Task<CocinaEstacionViewModel> CargarEstacionAsync(EstacionCocina estacion)
        {
            var tareas = await _repoCocina.ListarPorEstacionAsync(estacion.IdEstacion);

            var pasos = tareas.Any()
                ? await _repoCocina.ListarPasosAsync(tareas.Select(t => t.IdDetalle))
                : new List<CocinaOrden>();

            var vm = new CocinaEstacionViewModel
            {
                IdEstacion = estacion.IdEstacion,
                Nombre = estacion.Nombre,
                Descripcion = estacion.Descripcion
            };

            foreach (var t in tareas)
            {
                var detalle = t.IdDetalleNavigation;
                var esCombo = detalle.IdCombo != null;

                var anterior = pasos
                    .Where(h => h.IdDetalle == t.IdDetalle
                             && h.OrdenPaso < t.OrdenPaso
                             && h.Estado != "Completado")
                    .OrderBy(h => h.OrdenPaso)
                    .FirstOrDefault();

                vm.Tareas.Add(new CocinaTareaDTO
                {
                    IdCocinaOrden = t.IdCocinaOrden,
                    IdPedido = detalle.IdPedido,
                    OrdenPaso = t.OrdenPaso,
                    Estado = t.Estado,
                    Producto = esCombo
                        ? detalle.IdComboNavigation?.Nombre ?? "Combo"
                        : detalle.IdProductoNavigation?.Nombre ?? "Producto",
                    Imagen = esCombo
                        ? detalle.IdComboNavigation?.Imagen1
                        : detalle.IdProductoNavigation?.Imagen1,
                    Cantidad = detalle.Cantidad,
                    Observaciones = detalle.Observaciones,
                    Personalizado = detalle.Personalizado,
                    FechaPedido = detalle.IdPedidoNavigation?.FechaPedido ?? DateTime.MinValue,
                    FechaInicio = t.FechaInicio,
                    FechaFin = t.FechaFin,
                    Bloqueada = anterior != null,
                    EstacionAnterior = anterior?.IdEstacionNavigation?.Nombre
                });
            }

            return vm;
        }
    }
}
