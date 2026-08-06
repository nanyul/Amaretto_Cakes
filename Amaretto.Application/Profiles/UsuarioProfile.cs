using AutoMapper;
using Amaretto.Infraestructure.Models;
using Amaretto.Application.DTOs;

namespace Amaretto.Application.Profiles;

public class UsuarioProfile : Profile
{
    public UsuarioProfile()
    {
        CreateMap<Usuario, UsuarioDTO>().ForMember(dest => dest.NombreRol,opt => opt.MapFrom(src => src.IdRolNavigation.Descripcion));

    }
}