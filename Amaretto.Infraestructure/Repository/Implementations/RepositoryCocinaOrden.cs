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
    }
}
