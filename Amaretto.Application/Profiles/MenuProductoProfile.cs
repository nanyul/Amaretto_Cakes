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
    public class MenuProductoProfile : Profile
    {
        public MenuProductoProfile()
        {
            CreateMap<MenuProducto, MenuProductoDTO>().ReverseMap();

            CreateMap<MenuDetalleProducto, MenuDetalleProductoDTO>().ReverseMap();

            CreateMap<Producto, ProductoDTO>().ReverseMap();

            CreateMap<Categoria, CategoriaDTO>().ReverseMap();

        }

    }
}
