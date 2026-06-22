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
                   .ThenInclude(cp => cp.IdProducto)
                .Where(x => x.IdCombo == id)
                .FirstOrDefaultAsync();

            return entity!;
        }
    }
}
