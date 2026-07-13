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
    public class RepositoryMenuCombo : IRepositoryMenuCombo
    {
        private readonly AmarettoContext _context;
        //Alt+Enter
        public RepositoryMenuCombo(AmarettoContext context)
        {
            _context = context;
        }
        public async Task<MenuCombo?> ObtenerMenuDisponibleAsync()
        {
            var fechaActual = DateOnly.FromDateTime(DateTime.Now);
            return await _context.MenuCombo
                .Include(m => m.MenuDetalleCombo)
                    .ThenInclude(d => d.IdComboNavigation)
                .FirstOrDefaultAsync(m =>
                    m.Estado &&
                    fechaActual >= m.FechaInicio &&
                    fechaActual <= m.FechaFin);
        }
        public async Task<MenuCombo> FindByIdAsync(int id)
        {
            var menu = await _context.MenuCombo
                .Include(m => m.MenuDetalleCombo)
                    .ThenInclude(d => d.IdComboNavigation)
                .FirstOrDefaultAsync(m => m.IdMenuCombo == id);
            if (menu == null)
                throw new Exception("Menú no encontrado");
            return menu;
        }
        public async Task<ICollection<MenuCombo>> ListAsync()
        {
            return await _context.Set<MenuCombo>()
                                 .Include(m => m.MenuDetalleCombo)
                                     .ThenInclude(d => d.IdComboNavigation)
                                 .OrderByDescending(x => x.FechaInicio)
                                 .ToListAsync();
        }

        // CREAR
        public async Task<int> AddAsync(MenuCombo entity, string[] selectedCombos)
        {
            if (selectedCombos != null && selectedCombos.Any())
            {
                foreach (var idCombo in selectedCombos)
                {
                    entity.MenuDetalleCombo.Add(new MenuDetalleCombo
                    {
                        IdCombo = idCombo
                    });
                }
            }

            await _context.Set<MenuCombo>().AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity.IdMenuCombo;
        }

        // ACTUALIZAR
        public async Task UpdateAsync(MenuCombo entity, string[] selectedCombos)
        {
            var existentes = await _context.Set<MenuDetalleCombo>()
                .Where(x => x.IdMenuCombo == entity.IdMenuCombo)
                .ToListAsync();

            _context.Set<MenuDetalleCombo>().RemoveRange(existentes);

            if (selectedCombos != null)
            {
                foreach (var idCombo in selectedCombos)
                {
                    _context.Set<MenuDetalleCombo>().Add(new MenuDetalleCombo
                    {
                        IdMenuCombo = entity.IdMenuCombo,
                        IdCombo = idCombo
                    });
                }
            }

            await _context.SaveChangesAsync();
        }
    }
}
