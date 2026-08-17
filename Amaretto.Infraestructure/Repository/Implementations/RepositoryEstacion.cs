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
    
    public class RepositoryEstacion : IRepositoryEstacion
    {
        private readonly AmarettoContext _context;

        public RepositoryEstacion(AmarettoContext context)
        {
            _context = context;
        }

        // LISTADO
        public async Task<ICollection<EstacionCocina>> ListAsync()
        {
            var collection = await _context.Set<EstacionCocina>()
                .OrderBy(x => x.IdEstacion)
                .AsNoTracking()
                .ToListAsync();

            return collection;
        }

        // DETALLE
        public async Task<EstacionCocina> FindByIdAsync(string id)
        {
            var @object = await _context.Set<EstacionCocina>()
                .Where(x => x.IdEstacion == int.Parse(id))
                .FirstOrDefaultAsync();

            return @object!;
        }
    }
}
