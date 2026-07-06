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

        //CREAR
        public async Task<string> AddAsync(Producto entity, int[] selectedIngredientes)
        {
            // Generar un nuevo IdProducto único
            entity.IdProducto = await GetNextIdProductoAsync();
            if(selectedIngredientes != null && selectedIngredientes.Any())
            {
                foreach(var idIngrediente in selectedIngredientes)
                {
                     entity.ProductoIngrediente.Add(new ProductoIngrediente
                    {
                        IdProducto = entity.IdProducto,
                        IdIngrediente = idIngrediente,
                        Cantidad = 1 // Asignar una cantidad predeterminada
                     });
                }
            }
            
            await _context.Set<Producto>().AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity.IdProducto;
        }

        // ACTUALIZAR 
        public async Task UpdateAsync(Producto entity, int[] selectedIngredientes)
        {
            // Asegurar que la relación con Categoria se mantenga
            var categoria = await _context.Set<Categoria>().FindAsync(entity.IdCategoria);
            entity.IdCategoriaNavigation = categoria!;

            // Relación muchos a muchos con ingredientes: limpiar y reasignar
            entity.ProductoIngrediente.Clear();
            if (selectedIngredientes != null)
            {
                foreach (var idIngrediente in selectedIngredientes)
                {
                    entity.ProductoIngrediente.Add(new ProductoIngrediente
                    {
                        IdProducto = entity.IdProducto,
                        IdIngrediente = idIngrediente,
                        Cantidad = 1
                    });
                }
            }

            await _context.SaveChangesAsync();
        }

        // Genera el siguiente consecutivo tipo PROD010, PROD011, ...
        private async Task<string> GetNextIdProductoAsync()
        {
            var ids = await _context.Set<Producto>()
                .Select(x => x.IdProducto)
                .Where(x => x.StartsWith("PROD"))
                .ToListAsync();

            int max = 0;
            foreach (var id in ids)
            {
                if (int.TryParse(id.Replace("PROD", ""), out int numero) && numero > max)
                {
                    max = numero;
                }
            }
            int siguiente = max + 1;
            return $"PROD{siguiente:D3}";
        }
    }
}
