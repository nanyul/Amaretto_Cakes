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

public class ServiceEstacion : IServiceEstacion
    {
        private readonly IRepositoryEstacion _repository;
        private readonly IMapper _mapper;

        public ServiceEstacion(IRepositoryEstacion repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        // LISTADO (cards)
        public async Task<ICollection<EstacionCocinaDTO>> ListAsync()
        {
            var list = await _repository.ListAsync();
            return _mapper.Map<ICollection<EstacionCocinaDTO>>(list);
        }

        // DETALLE
        public async Task<EstacionCocinaDTO> FindByIdAsync(string id)
        {
            var entity = await _repository.FindByIdAsync(id);
            return _mapper.Map<EstacionCocinaDTO>(entity);
        }
    }
}