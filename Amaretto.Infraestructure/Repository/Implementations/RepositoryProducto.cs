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
    
    public class RepositoryProducto : IRepositoryProducto
    {
        private readonly AmarettoContext _context;

        public RepositoryProducto(AmarettoContext context)
        {
            _context = context;
        }

        // LISTADO
        public async Task<ICollection<Producto>> ListAsync()
        {
            var collection = await _context.Set<Producto>()
                .Include(x => x.IdCategoriaNavigation)
                .OrderBy(x => x.Nombre)
                .AsNoTracking()
                .ToListAsync();

            return collection;
        }

        // DETALLE
        public async Task<Producto> FindByIdAsync(string id)
        {
            var @object = await _context.Set<Producto>()
                .Include(x => x.IdCategoriaNavigation)
                .Include(x => x.ProductoIngrediente)
                    .ThenInclude(x => x.IdIngredienteNavigation)
                .Where(x => x.IdProducto == id)
                .FirstOrDefaultAsync();

            return @object!;
        }

        //FILTRO
        public async Task<ICollection<Producto>> FilterAsync(string? estado, decimal? precioMax, List<int>? categoriaIds, string? ordenarPor)
        {
            var query = _context.Set<Producto>()
                .Include(x => x.IdCategoriaNavigation)
                .AsNoTracking()
                .AsQueryable();

            if (estado == "disponible") query = query.Where(p => p.Estado == true);
            else if (estado == "inactivo") query = query.Where(p => p.Estado == false);

            if (precioMax.HasValue)
                query = query.Where(p => p.Precio <= precioMax.Value);

            if (categoriaIds != null && categoriaIds.Any())
                query = query.Where(p => categoriaIds.Contains(p.IdCategoria));

            query = ordenarPor switch
            {
                "precio_asc" => query.OrderBy(p => p.Precio),
                "precio_desc" => query.OrderByDescending(p => p.Precio),
                "nombre" => query.OrderBy(p => p.Nombre),
                _ => query.OrderBy(p => p.Nombre)
            };

            return await query.ToListAsync();
        }
    }
}
