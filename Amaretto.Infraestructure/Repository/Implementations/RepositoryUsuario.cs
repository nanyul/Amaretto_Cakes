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

    public async Task<Usuario?> FindByEmailAsync(string email)
    {
        return await _context.Usuario
            .Include(u => u.IdRolNavigation)
            .FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<Usuario> AddAsync(Usuario usuario)
    {
        _context.Usuario.Add(usuario);
        await _context.SaveChangesAsync();
        return usuario;
    }

    public async Task<bool> ExistsByEmailAsync(string email)
    {
        return await _context.Usuario
            .AnyAsync(u => u.Email == email);
    }

    public async Task<List<Usuario>> ObtenerPorRolAsync(string rol)
    {
        return await _context.Usuario
            .Include(u => u.IdRolNavigation)
            .Where(u => u.IdRolNavigation.Descripcion == rol && u.Estado)
            .OrderBy(u => u.NombreCompleto)
            .ToListAsync();
    }

    public async Task<List<Usuario>> BuscarClientesAsync(string termino)
    {
        return await _context.Usuario
            .Include(u => u.IdRolNavigation)
            .Where(u => u.IdRol == 2 && u.Estado && 
                       (u.NombreCompleto.Contains(termino) || u.Email.Contains(termino)))
            .OrderBy(u => u.NombreCompleto)
            .Take(20)
            .ToListAsync();
    }
}