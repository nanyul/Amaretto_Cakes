using Amaretto.Infraestructure.Data;
using Amaretto.Infraestructure.Models;
using Amaretto.Infraestructure.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Amaretto.Infraestructure.Repository.Implementations
{
    public class RepositoryNotificacion : IRepositoryNotificacion
    {
        private readonly AmarettoContext _context;

        public RepositoryNotificacion(AmarettoContext context)
        {
            _context = context;
        }

        public async Task<Notificacion> CrearAsync(Notificacion notificacion)
        {
            _context.Notificacion.Add(notificacion);
            await _context.SaveChangesAsync();
            return notificacion;
        }

        public async Task<ICollection<Notificacion>> ListarPorUsuarioAsync(int idUsuario, int cantidad)
        {
            return await _context.Notificacion
                .AsNoTracking()
                .Where(n => n.IdUsuario == idUsuario)
                .OrderByDescending(n => n.FechaCreacion)
                .ThenByDescending(n => n.IdNotificacion)
                .Take(cantidad)
                .ToListAsync();
        }

        public async Task<int> ContarNoLeidasAsync(int idUsuario)
        {
            return await _context.Notificacion
                .AsNoTracking()
                .CountAsync(n => n.IdUsuario == idUsuario && !n.Leida);
        }

        public async Task MarcarLeidasAsync(int idUsuario)
        {
            var pendientes = await _context.Notificacion
                .Where(n => n.IdUsuario == idUsuario && !n.Leida)
                .ToListAsync();

            if (!pendientes.Any()) return;

            foreach (var n in pendientes)
                n.Leida = true;

            await _context.SaveChangesAsync();
        }

        public async Task<Notificacion?> UltimaPorPedidoAsync(int idPedido)
        {
            return await _context.Notificacion
                .AsNoTracking()
                .Where(n => n.IdPedido == idPedido)
                .OrderByDescending(n => n.IdNotificacion)
                .FirstOrDefaultAsync();
        }
    }
}
