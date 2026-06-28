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
    }
}
