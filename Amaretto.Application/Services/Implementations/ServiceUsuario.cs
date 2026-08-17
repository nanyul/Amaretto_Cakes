using AutoMapper;
using Amaretto.Application.DTOs;
using Amaretto.Application.Services.Interfaces;
using Amaretto.Infraestructure.Models;
using Amaretto.Infraestructure.Repository.Interfaces;
using Libreria.Application.Utils;
using Libreria.Application.Config;
using Microsoft.Extensions.Options;
using System.Collections.ObjectModel;

namespace Amaretto.Application.Services.Implementations;

public class UsuarioService : IServiceUsuario
{
    private readonly IRepositoryUsuario _repository;
    private readonly IMapper _mapper;
    private readonly string _cryptoSecret;

    public UsuarioService(IRepositoryUsuario repository, IMapper mapper, IOptions<AppConfig> appConfig)
    {
        _repository = repository;
        _mapper = mapper;
        _cryptoSecret = appConfig.Value.Crypto.Secret;
    }


    public async Task<ICollection<UsuarioDTO>> ListAsync()
    {
        var list = await _repository.ListAsync();
        return _mapper.Map<ICollection<UsuarioDTO>>(list);
    }

    public async Task<UsuarioDTO> FindByIdAsync(int id)
    {
        var entity = await _repository.FindByIdAsync(id);
        return _mapper.Map<UsuarioDTO>(entity);
    }

    public async Task<UsuarioDTO?> FindByEmailAsync(string email)
    {
        var entity = await _repository.FindByEmailAsync(email);
        return entity != null ? _mapper.Map<UsuarioDTO>(entity) : null;
    }

    public async Task<UsuarioLoginResultDTO> LoginAsync(string email, string password)
    {
        var usuario = await _repository.FindByEmailAsync(email);
        
        if (usuario == null)
        {
            return new UsuarioLoginResultDTO 
            { 
                Success = false, 
                Message = "Usuario no encontrado" 
            };
        }

        if (!usuario.Estado)
        {
            return new UsuarioLoginResultDTO 
            { 
                Success = false, 
                Message = "Usuario inactivo" 
            };
        }

        // Verificar password usando Cryptography (AES decrypt)
        string decryptedPassword;
        try 
        {
            decryptedPassword = Cryptography.Decrypt(usuario.Password, _cryptoSecret);
        }
        catch
        {
            return new UsuarioLoginResultDTO 
            { 
                Success = false, 
                Message = "Error al verificar credenciales" 
            };
        }

        if (decryptedPassword != password)
        {
            return new UsuarioLoginResultDTO 
            { 
                Success = false, 
                Message = "Contraseña incorrecta" 
            };
        }

        var usuarioDTO = _mapper.Map<UsuarioDTO>(usuario);
        return new UsuarioLoginResultDTO 
        { 
            Success = true, 
            Message = "Login exitoso",
            UsuarioDTO = usuarioDTO
        };
    }

    public async Task<UsuarioLoginResultDTO> RegisterAsync(RegisterDTO dto)
    {
        // Validaciones
        if (await _repository.ExistsByEmailAsync(dto.Email))
        {
            return new UsuarioLoginResultDTO 
            { 
                Success = false, 
                Message = "El email ya está registrado" 
            };
        }

        if (dto.Password.Length < 6)
        {
            return new UsuarioLoginResultDTO 
            { 
                Success = false, 
                Message = "La contraseña debe tener al menos 6 caracteres" 
            };
        }

        if (dto.Password != dto.ConfirmPassword)
        {
            return new UsuarioLoginResultDTO 
            { 
                Success = false, 
                Message = "Las contraseñas no coinciden" 
            };
        }

        // Crear entidad usuario
        var usuario = _mapper.Map<Usuario>(dto);
        usuario.Password = Cryptography.Encrypt(dto.Password, _cryptoSecret);
        usuario.IdRol = 2; // Cliente por defecto
        usuario.Estado = true;

        var created = await _repository.AddAsync(usuario);
        
        // Recargar con rol para mapear NombreRol
        var createdWithRol = await _repository.FindByIdAsync(created.IdUsuario);
        var usuarioDTO = _mapper.Map<UsuarioDTO>(createdWithRol);

        return new UsuarioLoginResultDTO 
        { 
            Success = true, 
            Message = "Registro exitoso",
            UsuarioDTO = usuarioDTO
        };
    }

    public async Task<ICollection<UsuarioDTO>> BuscarClientesAsync(string termino)
    {
        if (string.IsNullOrWhiteSpace(termino) || termino.Length < 2)
            return new List<UsuarioDTO>();

        var clientes = await _repository.BuscarClientesAsync(termino);
        return _mapper.Map<ICollection<UsuarioDTO>>(clientes);
    }

    /*  Mantenimiento de usuarios  */

    private const int LargoMinimoPassword = 6;

    public async Task<ICollection<RolDTO>> ListarRolesAsync()
    {
        var roles = await _repository.ListarRolesAsync();
        return roles
            .Select(r => new RolDTO { IdRol = r.IdRol, Descripcion = r.Descripcion })
            .ToList();
    }

    public async Task<UsuarioMantenimientoDTO?> ObtenerParaEdicionAsync(int id)
    {
        var entity = await _repository.FindByIdAsync(id);
        if (entity == null) return null;

        return new UsuarioMantenimientoDTO
        {
            IdUsuario = entity.IdUsuario,
            NombreCompleto = entity.NombreCompleto,
            Email = entity.Email,
            IdRol = entity.IdRol,
            Telefono = entity.Telefono,
            Direccion = entity.Direccion,
            Sexo = entity.Sexo,
            Estado = entity.Estado
        };
    }

    public async Task<UsuarioLoginResultDTO> CrearAsync(UsuarioMantenimientoDTO dto)
    {
        var email = dto.Email.Trim();

        if (await _repository.ExistsByEmailAsync(email))
            return Fallo("El correo ya está registrado por otro usuario");

        var errorPassword = ValidarPassword(dto.Password, dto.ConfirmPassword, obligatoria: true);
        if (errorPassword != null)
            return Fallo(errorPassword);

        var usuario = new Usuario
        {
            NombreCompleto = dto.NombreCompleto.Trim(),
            Email = email,
            IdRol = dto.IdRol,
            Telefono = Limpiar(dto.Telefono),
            Direccion = Limpiar(dto.Direccion),
            Sexo = Limpiar(dto.Sexo),
            Estado = dto.Estado,
            Password = Cryptography.Encrypt(dto.Password!, _cryptoSecret)
        };

        var creado = await _repository.AddAsync(usuario);
        var conRol = await _repository.FindByIdAsync(creado.IdUsuario);

        return new UsuarioLoginResultDTO
        {
            Success = true,
            Message = "Usuario creado",
            UsuarioDTO = _mapper.Map<UsuarioDTO>(conRol)
        };
    }

    public async Task<UsuarioLoginResultDTO> ActualizarAsync(UsuarioMantenimientoDTO dto)
    {
        var usuario = await _repository.FindByIdAsync(dto.IdUsuario);
        if (usuario == null)
            return Fallo("El usuario no existe");

        var email = dto.Email.Trim();

        if (await _repository.ExistsByEmailAsync(email, dto.IdUsuario))
            return Fallo("El correo ya está registrado por otro usuario");

        var cambiaPassword = !string.IsNullOrWhiteSpace(dto.Password);
        if (cambiaPassword)
        {
            var errorPassword = ValidarPassword(dto.Password, dto.ConfirmPassword, obligatoria: true);
            if (errorPassword != null)
                return Fallo(errorPassword);
        }

        usuario.NombreCompleto = dto.NombreCompleto.Trim();
        usuario.Email = email;
        usuario.IdRol = dto.IdRol;
        usuario.Telefono = Limpiar(dto.Telefono);
        usuario.Direccion = Limpiar(dto.Direccion);
        usuario.Sexo = Limpiar(dto.Sexo);
        usuario.Estado = dto.Estado;

        if (cambiaPassword)
            usuario.Password = Cryptography.Encrypt(dto.Password!, _cryptoSecret);

        await _repository.UpdateAsync(usuario);

        return new UsuarioLoginResultDTO
        {
            Success = true,
            Message = cambiaPassword ? "Usuario actualizado y contraseña cambiada" : "Usuario actualizado",
            UsuarioDTO = _mapper.Map<UsuarioDTO>(usuario)
        };
    }

    private static string? ValidarPassword(string? password, string? confirmacion, bool obligatoria)
    {
        if (string.IsNullOrWhiteSpace(password))
            return obligatoria ? "La contraseña es un dato requerido" : null;

        if (password.Length < LargoMinimoPassword)
            return $"La contraseña debe tener al menos {LargoMinimoPassword} caracteres";

        if (password != confirmacion)
            return "Las contraseñas no coinciden";

        return null;
    }

    private static UsuarioLoginResultDTO Fallo(string mensaje) =>
        new() { Success = false, Message = mensaje };

    private static string? Limpiar(string? valor) =>
        string.IsNullOrWhiteSpace(valor) ? null : valor.Trim();
}
