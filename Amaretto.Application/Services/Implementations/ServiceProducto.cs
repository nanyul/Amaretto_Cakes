using Amaretto.Application.DTOs;
using Amaretto.Application.Services.Interfaces;
using Amaretto.Infraestructure.Data;
using Amaretto.Infraestructure.Models;
using Amaretto.Infraestructure.Repository.Interfaces;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Amaretto.Application.Services.Implementations.ServiceProducto;

namespace Amaretto.Application.Services.Implementations
{

public class ServiceProducto : IServiceProducto
    {
        private readonly IRepositoryProducto _repository;
        private readonly IMapper _mapper;

        public ServiceProducto(IRepositoryProducto repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        // LISTADO (cards)
        public async Task<ICollection<ProductoDTO>> ListAsync()
        {
            var list = await _repository.ListAsync();
            return _mapper.Map<ICollection<ProductoDTO>>(list);
        }

        // DETALLE
        public async Task<ProductoDTO> FindByIdAsync(string id)
        {
            var entity = await _repository.FindByIdAsync(id);
            return _mapper.Map<ProductoDTO>(entity);
        }

        //FILTRAR
        public async Task<ICollection<ProductoDTO>> FilterAsync(string? estado, decimal? precioMax, List<int>? categoriaIds, string? ordenarPor)
        {
            var list = await _repository.FilterAsync(estado, precioMax, categoriaIds, ordenarPor);
            return _mapper.Map<ICollection<ProductoDTO>>(list);
        }
    }
}