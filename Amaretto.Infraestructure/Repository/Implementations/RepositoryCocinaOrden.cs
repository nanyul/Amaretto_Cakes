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

        // ESTACIONES DE UN PROCESO (por IdDetalle)
        public async Task<ICollection<CocinaOrden>> ListByDetalleAsync(int idDetalle)
        {
            return await _context.Set<CocinaOrden>()
                .Include(x => x.IdEstacionNavigation)
                .Where(x => x.IdDetalle == idDetalle)
                .OrderBy(x => x.OrdenPaso)
                .ToListAsync();
        }

        // CREAR PROCESO
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

        // EDITAR PROCESO: actualizar Estado con cascada + fechas automáticas
        public async Task ActualizarEstadosAsync(int idDetalle, Dictionary<int, string> estadosPorCocinaOrden)
        {
            var filas = await _context.Set<CocinaOrden>()
                .Where(x => x.IdDetalle == idDetalle)
                .OrderBy(x => x.OrdenPaso)
                .ToListAsync();

            // 1. Aplicar los estados elegidos por el usuario
            foreach (var fila in filas)
            {
                if (estadosPorCocinaOrden.TryGetValue(fila.IdCocinaOrden, out var nuevoEstado))
                {
                    fila.Estado = nuevoEstado;
                }
            }

            // 2. Cascada: si alguna estación quedó "Completado", todas las de menor
            //    OrdenPaso también deben quedar "Completado" (no se puede saltar un paso).
            var maxOrdenCompletado = filas
                .Where(f => f.Estado == "Completado")
                .Select(f => (int?)f.OrdenPaso)
                .Max();

            if (maxOrdenCompletado.HasValue)
            {
                foreach (var fila in filas.Where(f => f.OrdenPaso < maxOrdenCompletado.Value && f.Estado != "Completado"))
                {
                    fila.Estado = "Completado";
                }
            }

            // 3. Fechas automáticas (solo se llenan una vez, no se sobreescriben)
            var ahora = DateTime.Now;
            foreach (var fila in filas)
            {
                if ((fila.Estado == "En Proceso" || fila.Estado == "Completado") && fila.FechaInicio == null)
                {
                    fila.FechaInicio = ahora;
                }

                if (fila.Estado == "Completado" && fila.FechaFin == null)
                {
                    fila.FechaFin = ahora;
                }
            }

            await _context.SaveChangesAsync();
        }
    }
}
