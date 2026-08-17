using AutoMapper;
using Amaretto.Infraestructure.Models;
using Amaretto.Application.DTOs;

namespace Amaretto.Application.Profiles;

public class UsuarioProfile : Profile
{
    public UsuarioProfile()
    {
        CreateMap<Usuario, UsuarioDTO>()
            .ForMember(dest => dest.NombreRol, opt => opt.MapFrom(src => src.IdRolNavigation.Descripcion))
            .ForMember(dest => dest.IdRol, opt => opt.MapFrom(src => src.IdRol));

        CreateMap<RegisterDTO, Usuario>()
            .ForMember(dest => dest.Password, opt => opt.Ignore())
            .ForMember(dest => dest.IdRol, opt => opt.MapFrom(_ => 2))
            .ForMember(dest => dest.Estado, opt => opt.MapFrom(_ => true));
    }
}