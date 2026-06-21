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
    
    public class RepositoryCategoria : IRepositoryCategoria
    {
        private readonly AmarettoContext _context;
        //Alt+Enter
        public RepositoryCategoria(AmarettoContext context)
        {
            _context = context;
        }

        public async Task<Categoria> FindByIdAsync(int id)
        {
            var @object = await _context.Set<Categoria>().FindAsync(id);

            return @object!;
        }

        public async Task<ICollection<Categoria>> ListAsync()
        {
            var collection = await _context.Set<Categoria>().AsNoTracking().ToListAsync();
            return collection;
        }
    }
}
