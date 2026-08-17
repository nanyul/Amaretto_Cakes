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
    public class RepositoryCombo : IRepositoryCombo
    {
        private readonly AmarettoContext _context;

        public RepositoryCombo(AmarettoContext context)
        {
            _context = context;
        }

        public async Task<ICollection<Combo>> ListAsync()
        {
            var collection = await _context.Set<Combo>().ToListAsync();
            return collection;

        }

        public async Task<Combo> FindByIdAsync(string id)
        {
            var entity = await _context.Set<Combo>()
                .Include(x => x.IdProducto)
                .Where(x => x.IdCombo == id)
                .FirstOrDefaultAsync();

            return entity!;
        }

        public async Task<ICollection<Combo>> FilterAsync(string? estado, decimal? precioMax, List<int>? categoriaIds, string? ordenarPor)
        {
            var query = _context.Set<Combo>()
                .Include(x => x.IdProducto)
                    .ThenInclude(p => p.IdCategoriaNavigation)
                .AsNoTracking()
                .AsQueryable();

            if (estado == "disponible") query = query.Where(c => c.Estado == true);
            else if (estado == "inactivo") query = query.Where(c => c.Estado == false);

            if (precioMax.HasValue)
                query = query.Where(c => c.Precio <= precioMax.Value);

            if (categoriaIds != null && categoriaIds.Any())
                query = query.Where(c => c.IdProducto.Any(p => categoriaIds.Contains(p.IdCategoria)));

            query = ordenarPor switch
            {
                "precio_asc" => query.OrderBy(c => c.Precio),
                "precio_desc" => query.OrderByDescending(c => c.Precio),
                "nombre" => query.OrderBy(c => c.Nombre),
                _ => query.OrderBy(c => c.Nombre)
            };

            return await query.ToListAsync();
        }

        // CREAR
        public async Task<string> AddAsync(Combo entity, string[] selectedProductos)
        {
            // Generar un nuevo IdCombo único
            entity.IdCombo = await GetNextIdComboAsync();

            if (selectedProductos != null && selectedProductos.Any())
            {
                var productos = await _context.Set<Producto>()
                    .Where(p => selectedProductos.Contains(p.IdProducto))
                    .ToListAsync();

                foreach (var producto in productos)
                {
                    entity.IdProducto.Add(producto);
                }
            }

            await _context.Set<Combo>().AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity.IdCombo;
        }

        // ACTUALIZAR
        public async Task UpdateAsync(Combo entity, string[] selectedProductos)
        {
            // entity llega ya trackeado (viene de FindByIdAsync sin AsNoTracking + AutoMapper.Map(dto, entity))
            // Relación muchos a muchos con productos: limpiar y reasignar
            entity.IdProducto.Clear();

            if (selectedProductos != null && selectedProductos.Any())
            {
                var productos = await _context.Set<Producto>()
                    .Where(p => selectedProductos.Contains(p.IdProducto))
                    .ToListAsync();

                foreach (var producto in productos)
                {
                    entity.IdProducto.Add(producto);
                }
            }

            await _context.SaveChangesAsync();
        }

        // Genera el siguiente consecutivo tipo COMBO001, COMBO002, ...
        private async Task<string> GetNextIdComboAsync()
        {
            var ids = await _context.Set<Combo>()
                .Select(x => x.IdCombo)
                .Where(x => x.StartsWith("COMBO"))
                .ToListAsync();

            int max = 0;
            foreach (var id in ids)
            {
                if (int.TryParse(id.Replace("COMBO", ""), out int numero) && numero > max)
                {
                    max = numero;
                }
            }
            int siguiente = max + 1;
            return $"COMBO{siguiente:D3}";
        }

    }
}
