using Amaretto.Infraestructure.Data;
using Amaretto.Infraestructure.Models;
using Amaretto.Infraestructure.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Amaretto.Infraestructure.Repository.Implementations
{
    public class RepositoryTareaMenuVencido : IRepositoryTareaMenuVencido
    {
        private readonly AmarettoContext _context;

        public RepositoryTareaMenuVencido(AmarettoContext context)
        {
            _context = context;
        }

        public async Task<bool> ExistenMenusVencidosActivosAsync()
        {
            var hoy = DateOnly.FromDateTime(DateTime.Now);

            var hayProductos = await _context.Set<MenuProducto>()
                .AnyAsync(m => m.Estado && m.FechaFin < hoy);

            var hayCombos = await _context.Set<MenuCombo>()
                .AnyAsync(m => m.Estado && m.FechaFin < hoy);

            return hayProductos || hayCombos;
        }

        public async Task<List<(string Tipo, string Nombre, DateOnly FechaFin)>> DesactivarMenusVencidosAsync()
        {
            var hoy = DateOnly.FromDateTime(DateTime.Now);
            var resultado = new List<(string, string, DateOnly)>();

            var productosVencidos = await _context.Set<MenuProducto>()
                .Where(m => m.Estado && m.FechaFin < hoy)
                .ToListAsync();

            foreach (var m in productosVencidos)
            {
                resultado.Add(("Menú Producto", m.Nombre, m.FechaFin));
                m.Estado = false;
            }

            var combosVencidos = await _context.Set<MenuCombo>()
                .Where(m => m.Estado && m.FechaFin < hoy)
                .ToListAsync();

            foreach (var m in combosVencidos)
            {
                resultado.Add(((string, string, DateOnly))("Menú Combo", m.Nombre, m.FechaFin));
                m.Estado = false;
            }

            await _context.SaveChangesAsync();
            return resultado;
        }
    }
}
