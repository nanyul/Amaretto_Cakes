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
    public class RepositoryMenuProducto : IRepositoryMenuProducto
    {
        private readonly AmarettoContext _context;
        //Alt+Enter
        public RepositoryMenuProducto(AmarettoContext context)
        {
            _context = context;
        }
        public async Task<MenuProducto?> ObtenerMenuDisponibleAsync()
        {
            var fechaActual = DateOnly.FromDateTime(DateTime.Now);
            return await _context.MenuProducto
                .Include(m => m.MenuDetalleProducto)
                    .ThenInclude(d => d.IdProductoNavigation)
                        .ThenInclude(p => p.IdCategoriaNavigation)
                .FirstOrDefaultAsync(m =>
                    m.Estado &&
                    fechaActual >= m.FechaInicio &&
                    fechaActual <= m.FechaFin);
        }
        public async Task<MenuProducto> FindByIdAsync(int id)
        {
            var menu = await _context.MenuProducto
                .Include(m => m.MenuDetalleProducto)
                    .ThenInclude(d => d.IdProductoNavigation)
                        .ThenInclude(p => p.IdCategoriaNavigation)
                .FirstOrDefaultAsync(m => m.IdMenuProducto == id);
            if (menu == null)
                throw new Exception("Menú no encontrado");
            return menu;
        }
        public async Task<ICollection<MenuProducto>> ListAsync()
        {
            return await _context.Set<MenuProducto>()
                                 .Include(m => m.MenuDetalleProducto)
                                     .ThenInclude(d => d.IdProductoNavigation)
                                         .ThenInclude(p => p.IdCategoriaNavigation)
                                 .OrderByDescending(x => x.FechaInicio)
                                 .ToListAsync();
        }

        // CREAR
        public async Task<int> AddAsync(MenuProducto entity, string[] selectedProductos)
        {
            if (selectedProductos != null && selectedProductos.Any())
            {
                foreach (var idProducto in selectedProductos)
                {
                    entity.MenuDetalleProducto.Add(new MenuDetalleProducto
                    {
                        IdProducto = idProducto
                    });
                }
            }

            await _context.Set<MenuProducto>().AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity.IdMenuProducto;
        }

        // ACTUALIZAR
        public async Task UpdateAsync(MenuProducto entity, string[] selectedProductos)
        {

            var existentes = await _context.Set<MenuDetalleProducto>()
                .Where(x => x.IdMenuProducto == entity.IdMenuProducto)
                .ToListAsync();

            _context.Set<MenuDetalleProducto>().RemoveRange(existentes);

            if (selectedProductos != null)
            {
                foreach (var idProducto in selectedProductos)
                {
                    _context.Set<MenuDetalleProducto>().Add(new MenuDetalleProducto
                    {
                        IdMenuProducto = entity.IdMenuProducto,
                        IdProducto = idProducto
                    });
                }
            }

            await _context.SaveChangesAsync();
        }
    }
}