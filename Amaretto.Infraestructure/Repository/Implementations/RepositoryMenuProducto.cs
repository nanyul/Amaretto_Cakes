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
    }
}