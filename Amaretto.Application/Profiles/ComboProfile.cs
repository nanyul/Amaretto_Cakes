using Amaretto.Application.DTOs;
using Amaretto.Infraestructure.Models;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Amaretto.Application.Profiles
{
    public class ComboProfile : Profile
    {
        public ComboProfile()
        {
            CreateMap<ComboDTO, Combo>().ReverseMap();
            CreateMap<Combo, ComboDTO>()
                .ForMember(dest => dest.Producto, orig => orig.MapFrom(o => o.IdProducto));
        }

    }
}
