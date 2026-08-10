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
    public class RepositoryPedido : IRepositoryPedido
    {
        private readonly AmarettoContext _context;

        public RepositoryPedido(AmarettoContext context)
        {
            _context = context;
        }

        public async Task<Pedido> CrearAsync(Pedido pedido)
        {
            // Al agregar el Pedido con sus colecciones de PedidoDetalle y Pago ya
            // pobladas, EF inserta el padre primero y luego los hijos en un solo
            // SaveChanges, respetando el orden de FKs automáticamente.
            _context.Pedido.Add(pedido);
            await _context.SaveChangesAsync();
            return pedido;
        }

        public async Task<ICollection<Pedido>> ListarHistorialAsync(int? idCliente, DateTime? fechaDesde, DateTime? fechaHasta, string? estado)
        {
            var query = _context.Pedido
                .AsNoTracking()
                .Include(p => p.IdUsuarioNavigation)
                .Include(p => p.IdEncargadoNavigation)
                .Include(p => p.PedidoDetalle)
                .AsQueryable();

            if (idCliente.HasValue)
                query = query.Where(p => p.IdUsuario == idCliente.Value);

            if (fechaDesde.HasValue)
                query = query.Where(p => p.FechaPedido >= fechaDesde.Value.Date);

            if (fechaHasta.HasValue)
            {
                var limite = fechaHasta.Value.Date.AddDays(1);
                query = query.Where(p => p.FechaPedido < limite);
            }

            if (!string.IsNullOrWhiteSpace(estado))
                query = query.Where(p => p.Estado == estado);

            return await query
                .OrderByDescending(p => p.FechaPedido)
                .ThenByDescending(p => p.IdPedido)
                .ToListAsync();
        }

        public async Task<Pedido?> FindByIdAsync(int idPedido)
        {
            return await _context.Pedido
                .AsNoTracking()
                .Include(p => p.IdUsuarioNavigation)
                .Include(p => p.IdEncargadoNavigation)
                .Include(p => p.Pago)
                .Include(p => p.PedidoDetalle).ThenInclude(d => d.IdProductoNavigation)
                .Include(p => p.PedidoDetalle).ThenInclude(d => d.IdComboNavigation)
                .Include(p => p.PedidoDetalle).ThenInclude(d => d.PedidoDetallePersonalizacion)
                .FirstOrDefaultAsync(p => p.IdPedido == idPedido);
        }

        public async Task<ICollection<string>> ListarEstadosAsync()
        {
            return await _context.Pedido
                .AsNoTracking()
                .Select(p => p.Estado)
                .Distinct()
                .OrderBy(e => e)
                .ToListAsync();
        }
    }
}
