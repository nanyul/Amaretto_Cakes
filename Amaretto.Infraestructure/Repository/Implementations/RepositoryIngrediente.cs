using Amaretto.Infraestructure.Data;
using Amaretto.Infraestructure.Models;
using Amaretto.Infraestructure.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Amaretto.Infraestructure.Repository.Implementations
{
    public class RepositoryIngrediente : IRepositoryIngrediente
    {
        private readonly AmarettoContext _context;
        public RepositoryIngrediente(AmarettoContext context)
        {
            _context = context;
        }

        public async Task<ICollection<Ingrediente>> ListAsync()
        {
            var collection = await _context.Set<Ingrediente>()
                .OrderBy(x => x.Nombre)
                .AsNoTracking()
                .ToListAsync();
            return collection;
        }

        public async Task<Ingrediente> FindByIdAsync(int id)
        {
            var @object = await _context.Set<Ingrediente>()
                .Where(x => x.IdIngrediente == id)
                .FirstOrDefaultAsync();
            return @object!;
        }
    }
}