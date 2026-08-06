using Amaretto.Infraestructure.Data;
using Amaretto.Infraestructure.Models;
using Amaretto.Infraestructure.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Amaretto.Infraestructure.Repositories;

public class RepositoryUsuario : IRepositoryUsuario
{
    private readonly AmarettoContext _context;

    public RepositoryUsuario(AmarettoContext context)
    {
        _context = context;
    }

    public async Task<ICollection<Usuario>> ListAsync()
    {
        return await _context.Usuario
            .Include(u => u.IdRolNavigation)
            .ToListAsync();
    }

    public async Task<Usuario?> FindByIdAsync(int id)
    {
        return await _context.Usuario
            .Include(u => u.IdRolNavigation)
            .FirstOrDefaultAsync(u => u.IdUsuario == id);
    }

    public async Task<List<Usuario>> ObtenerPorRolAsync(string rol)
    {
        return await _context.Usuario
            .Include(u => u.IdRolNavigation)
            .Where(u => u.IdRolNavigation.Descripcion == rol && u.Estado)
            .OrderBy(u => u.NombreCompleto)
            .ToListAsync();
    }
}