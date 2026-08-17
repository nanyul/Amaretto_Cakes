using Amaretto.Application.DTOs;
using Amaretto.Application.Services.Interfaces;
using Amaretto.Infraestructure.Models;
using Amaretto.Infraestructure.Repository.Interfaces;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Amaretto.Application.Services.Implementations
{
    public class ServiceCombo : IServiceCombo
    {
        private readonly IRepositoryCombo _repository;
        private readonly IMapper _mapper;

        public ServiceCombo(IRepositoryCombo repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<ICollection<ComboDTO>> ListAsync()
        {
            var list = await _repository.ListAsync();
            return _mapper.Map<ICollection<ComboDTO>>(list);
        }

        public async Task<ComboDTO> FindByIdAsync(string id)
        {
            var entity = await _repository.FindByIdAsync(id);
            return _mapper.Map<ComboDTO>(entity);
        }
        public async Task<ICollection<ComboDTO>> FilterAsync(string? estado, decimal? precioMax, List<int>? categoriaIds, string? ordenarPor)
        {
            var list = await _repository.FilterAsync(estado, precioMax, categoriaIds, ordenarPor);
            return _mapper.Map<ICollection<ComboDTO>>(list);
        }

        // CREAR
        public async Task<string> AddAsync(ComboDTO dto, string[] selectedProductos)
        {
            var objectMapped = _mapper.Map<Combo>(dto);
            return await _repository.AddAsync(objectMapped, selectedProductos);
        }

        // ACTUALIZAR
        public async Task UpdateAsync(string id, ComboDTO dto, string[] selectedProductos)
        {
            var @object = await _repository.FindByIdAsync(id);
            var entity = _mapper.Map(dto, @object!);
            await _repository.UpdateAsync(entity, selectedProductos);
        }

    }
}
