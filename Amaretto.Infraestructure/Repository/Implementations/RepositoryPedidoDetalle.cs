using Amaretto.Infraestructure.Data;
using Amaretto.Infraestructure.Models;
using Amaretto.Infraestructure.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Amaretto.Infraestructure.Repository.Implementations
{
    public class RepositoryPedidoDetalle : IRepositoryPedidoDetalle
    {
        private readonly AmarettoContext _context;

        public RepositoryPedidoDetalle(AmarettoContext context)
        {
            _context = context;
        }

        public async Task<ICollection<PedidoDetalle>> ListDisponiblesAsync()
        {
            return await _context.Set<PedidoDetalle>()
                .Include(x => x.IdProductoNavigation)
                .Include(x => x.IdComboNavigation)
                .Where(x => !x.CocinaOrden.Any())
                .OrderByDescending(x => x.IdDetalle)
                .ToListAsync();
        }

        public async Task<PedidoDetalle?> FindByIdAsync(int id)
        {
            return await _context.Set<PedidoDetalle>()
                .Include(x => x.IdProductoNavigation)
                .Include(x => x.IdComboNavigation)
                .FirstOrDefaultAsync(x => x.IdDetalle == id);
        }
    }
}
