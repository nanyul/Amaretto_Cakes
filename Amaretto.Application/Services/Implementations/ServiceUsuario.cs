
using AutoMapper;
using Amaretto.Application.DTOs;
using Amaretto.Infraestructure.Repositories;
using Amaretto.Application.Services.Interfaces;
using Amaretto.Infraestructure.Repository.Interfaces;
using System.Collections.ObjectModel;

namespace Amaretto.Application.Services;

public class UsuarioService : IServiceUsuario
{
    private readonly IRepositoryUsuario _repository;
    private readonly IMapper _mapper;

    public UsuarioService(IRepositoryUsuario repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
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
}