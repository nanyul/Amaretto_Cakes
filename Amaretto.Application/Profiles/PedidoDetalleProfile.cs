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
    public class PedidoDetalleProfile : Profile
    {
        public PedidoDetalleProfile()
        {
            CreateMap<PedidoDetalle, PedidoDetalleDTO>()
                .ForMember(dest => dest.NombreProductoOCombo, opt => opt.MapFrom(src =>
                    src.IdProductoNavigation != null ? src.IdProductoNavigation.Nombre :
                    src.IdComboNavigation != null ? src.IdComboNavigation.Nombre :
                    string.Empty))
                .ReverseMap();

        }
    }
}
