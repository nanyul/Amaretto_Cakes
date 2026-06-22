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
    public class ProductoIngredienteProfile : Profile
    {
        public ProductoIngredienteProfile()
        {
            CreateMap<ProductoIngrediente, ProductoIngredienteDTO>().ReverseMap();
        }
    }
}
