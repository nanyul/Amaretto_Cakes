using Amaretto.Infraestructure.Data;
using Amaretto.Infraestructure.Models;
using Amaretto.Infraestructure.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Amaretto.Infraestructure.Repository.Implementations
{

    public class RepositoryCocinaOrden : IRepositoryCocinaOrden
    {
        private readonly AmarettoContext _context;

        public RepositoryCocinaOrden(AmarettoContext context)
        {
            _context = context;
        }

        // LISTADO
        public async Task<ICollection<CocinaOrden>> ListAsync()
        {
            var collection = await _context.Set<CocinaOrden>()
                .Include(x => x.IdEstacionNavigation)
                .Include(x => x.IdDetalleNavigation)
                    .ThenInclude(x => x.IdProductoNavigation)
                    .ThenInclude(x => x.IdCombo)
                .ToListAsync();

            return collection;
        }

        // DETALLE
        public async Task<CocinaOrden> FindByIdAsync(string id)
        {
            var @object = await _context.Set<CocinaOrden>()
                .Include(x => x.IdEstacionNavigation)
                .Include(x => x.IdDetalleNavigation)
                    .ThenInclude(x => x.IdProductoNavigation)
                    .ThenInclude(x => x.IdCombo)
                .FirstOrDefaultAsync(x => x.IdCocinaOrden == int.Parse(id));

            return @object!;
        }

        public async Task<ICollection<CocinaOrden>> ListByDetalleAsync(int idDetalle)
        {
            return await _context.Set<CocinaOrden>()
                .Include(x => x.IdEstacionNavigation)
                .Include(x => x.IdDetalleNavigation)
                .ThenInclude(x => x.IdProductoNavigation)
                .ThenInclude(x => x.IdCombo)
                .Where(x => x.IdDetalle == idDetalle)
                .OrderBy(x => x.OrdenPaso)
                .ToListAsync();
        }

        public async Task AddRangoAsync(int idDetalle, List<CocinaOrdenEstacionInput> estaciones)
        {
            foreach (var e in estaciones)
            {
                _context.Set<CocinaOrden>().Add(new CocinaOrden
                {
                    IdDetalle = idDetalle,
                    IdEstacion = e.IdEstacion,
                    OrdenPaso = e.OrdenPaso,
                    Estado = "Pendiente"
                });
            }

            await _context.SaveChangesAsync();
        }
 
        public async Task ActualizarEstadosAsync(int idDetalle, List<CocinaOrdenUpdateInput> filasExistentes, List<CocinaOrdenEstacionInput> filasNuevas)
        {
            var registros = await _context.Set<CocinaOrden>()
                .Include(x => x.IdEstacionNavigation)
                .Where(x => x.IdDetalle == idDetalle)
                .ToListAsync();

            var inputPorId = filasExistentes.ToDictionary(f => f.IdCocinaOrden);

            foreach (var registro in registros)
            {
                if (inputPorId.TryGetValue(registro.IdCocinaOrden, out var input))
                {
                    registro.OrdenPaso = input.OrdenPaso;
                }
            }

            var nuevosRegistros = filasNuevas.Select(f => new CocinaOrden
            {
                IdDetalle = idDetalle,
                IdEstacion = f.IdEstacion,
                OrdenPaso = f.OrdenPaso,
                Estado = "Pendiente"
            }).ToList();

            _context.Set<CocinaOrden>().AddRange(nuevosRegistros);

            var todos = registros.Concat(nuevosRegistros).OrderBy(r => r.OrdenPaso).ToList();

            string EstadoEfectivo(CocinaOrden r) =>
                inputPorId.TryGetValue(r.IdCocinaOrden, out var i) ? i.Estado : r.Estado;

            foreach (var registro in todos)
            {
                var estadoDeseado = EstadoEfectivo(registro);

                if (estadoDeseado == "Completado" || estadoDeseado == "En Proceso")
                {
                    var hayAnteriorNoCompletada = todos
                        .Where(r => r.OrdenPaso < registro.OrdenPaso)
                        .Any(r => EstadoEfectivo(r) != "Completado");

                    if (hayAnteriorNoCompletada)
                    {
                        var nombreEstacion = registro.IdEstacionNavigation?.Nombre ?? "(nueva)";
                        throw new InvalidOperationException(
                            $"No se puede avanzar la estación \"{nombreEstacion}\" (paso {registro.OrdenPaso}) porque una estación anterior aún no está Completada.");
                    }
                }
            }

            //Aplicar los estados 
            foreach (var registro in registros)
            {
                if (inputPorId.TryGetValue(registro.IdCocinaOrden, out var input))
                {
                    registro.Estado = input.Estado;
                }
            }

           
            var ahora = DateTime.Now;
            foreach (var registro in todos)
            {
                if ((registro.Estado == "En Proceso" || registro.Estado == "Completado") && registro.FechaInicio == null)
                    registro.FechaInicio = ahora;

                if (registro.Estado == "Completado" && registro.FechaFin == null)
                    registro.FechaFin = ahora;
            }

            await _context.SaveChangesAsync();
        }

        /*  Panel de estación  */

        private static string EstadoListo(string metodoEntrega)
        {
            var metodo = metodoEntrega ?? string.Empty;

            var esRetiro = metodo.Contains("Retiro", StringComparison.OrdinalIgnoreCase)
                        || metodo.Contains("Recogida", StringComparison.OrdinalIgnoreCase);

            return esRetiro ? "Listo para retirar" : "Listo para enviar";
        }

        public async Task<ICollection<CocinaOrden>> ListarPorEstacionAsync(int idEstacion)
        {
            return await _context.Set<CocinaOrden>()
                .AsNoTracking()
                .Include(x => x.IdEstacionNavigation)
                .Include(x => x.IdDetalleNavigation).ThenInclude(d => d.IdProductoNavigation)
                .Include(x => x.IdDetalleNavigation).ThenInclude(d => d.IdComboNavigation)
                .Include(x => x.IdDetalleNavigation).ThenInclude(d => d.IdPedidoNavigation)
                .Where(x => x.IdEstacion == idEstacion)
                .OrderBy(x => x.IdDetalleNavigation.IdPedidoNavigation.FechaPedido)
                .ToListAsync();
        }

        public async Task<ICollection<CocinaOrden>> ListarPasosAsync(IEnumerable<int> idsDetalle)
        {
            var ids = idsDetalle.Distinct().ToList();

            return await _context.Set<CocinaOrden>()
                .AsNoTracking()
                .Include(x => x.IdEstacionNavigation)
                .Where(x => ids.Contains(x.IdDetalle))
                .OrderBy(x => x.OrdenPaso)
                .ToListAsync();
        }

        public async Task<CocinaAvanceResultado> AvanzarAsync(int idCocinaOrden, string nuevoEstado)
        {
            if (nuevoEstado != "En Proceso" && nuevoEstado != "Completado")
                throw new InvalidOperationException("Estado no válido para la estación.");

            var tarea = await _context.Set<CocinaOrden>()
                .Include(x => x.IdEstacionNavigation)
                .Include(x => x.IdDetalleNavigation)
                    .ThenInclude(d => d.IdPedidoNavigation)
                    .ThenInclude(p => p.IdUsuarioNavigation)
                .FirstOrDefaultAsync(x => x.IdCocinaOrden == idCocinaOrden);

            if (tarea == null)
                throw new InvalidOperationException("La tarea de cocina no existe.");

            var pasos = await _context.Set<CocinaOrden>()
                .Include(x => x.IdEstacionNavigation)
                .Where(x => x.IdDetalle == tarea.IdDetalle)
                .ToListAsync();

            var anteriorPendiente = pasos
                .Where(h => h.OrdenPaso < tarea.OrdenPaso && h.Estado != "Completado")
                .OrderBy(h => h.OrdenPaso)
                .FirstOrDefault();

            if (anteriorPendiente != null)
                throw new InvalidOperationException(
                    $"Primero debe completarse \"{anteriorPendiente.IdEstacionNavigation.Nombre}\".");

            var ahora = DateTime.Now;
            tarea.Estado = nuevoEstado;

            if (tarea.FechaInicio == null)
                tarea.FechaInicio = ahora;

            tarea.FechaFin = nuevoEstado == "Completado" ? ahora : null;

            var pedido = tarea.IdDetalleNavigation.IdPedidoNavigation;
            var estadoAnterior = pedido.Estado;

            var pasosDelPedido = await _context.Set<CocinaOrden>()
                .Where(x => x.IdDetalleNavigation.IdPedido == pedido.IdPedido)
                .ToListAsync();

            foreach (var p in pasosDelPedido.Where(p => p.IdCocinaOrden == tarea.IdCocinaOrden))
                p.Estado = nuevoEstado;

            if (pasosDelPedido.Any())
            {
                if (pasosDelPedido.All(p => p.Estado == "Completado"))
                    pedido.Estado = EstadoListo(pedido.MetodoEntrega);
                else if (pasosDelPedido.Any(p => p.Estado == "En Proceso" || p.Estado == "Completado"))
                    pedido.Estado = "En Preparacion";
            }

            await _context.SaveChangesAsync();

            return new CocinaAvanceResultado(
                pedido.IdPedido,
                pedido.IdUsuario,
                pedido.IdUsuarioNavigation?.NombreCompleto ?? "",
                pedido.IdUsuarioNavigation?.Email ?? "",
                pedido.Estado,
                pedido.Estado != estadoAnterior);
        }
    }
}
