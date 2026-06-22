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
    public class CocinaOrdenProfile : Profile
    {
        public CocinaOrdenProfile()
        {
            CreateMap<CocinaOrden, CocinaOrdenDTO>().ReverseMap();
            CreateMap<PedidoDetalleDTO, PedidoDetalle>().ReverseMap();
            CreateMap<EstacionCocinaDTO, EstacionCocina>().ReverseMap();
            CreateMap<ProductoDTO, Producto>().ReverseMap();
            CreateMap<ComboDTO, Combo>().ReverseMap();
        }
    }
}
