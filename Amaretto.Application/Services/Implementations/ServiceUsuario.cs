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
}