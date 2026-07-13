using Amaretto.Application.DTOs;
using Amaretto.Application.Services.Interfaces;
using Amaretto.Infraestructure.Models;
using Amaretto.Infraestructure.Repository.Interfaces;
using AutoMapper;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Amaretto.Application.Services.Implementations
{
    public class ServiceMenuProducto : IServiceMenuProducto
    {
        private readonly IRepositoryMenuProducto _repository;
        private readonly IMapper _mapper;

        public ServiceMenuProducto(IRepositoryMenuProducto repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<MenuProductoDTO?> ObtenerMenuDisponibleAsync()
        {
            var entity = await _repository.ObtenerMenuDisponibleAsync();
            return entity == null ? null : _mapper.Map<MenuProductoDTO>(entity);
        }

        public async Task<MenuProductoDTO> FindByIdAsync(int id)
        {
            var entity = await _repository.FindByIdAsync(id);
            return _mapper.Map<MenuProductoDTO>(entity);
        }

        public async Task<ICollection<MenuProductoDTO>> ListAsync()
        {
            var list = await _repository.ListAsync();
            return _mapper.Map<ICollection<MenuProductoDTO>>(list);
        }

        // CREAR
        public async Task<int> AddAsync(MenuProductoDTO dto, string[] selectedProductos)
        {
            var entity = _mapper.Map<MenuProducto>(dto);
            return await _repository.AddAsync(entity, selectedProductos);
        }

        // ACTUALIZAR
        public async Task UpdateAsync(int id, MenuProductoDTO dto, string[] selectedProductos)
        {
            var entity = await _repository.FindByIdAsync(id);
            var actualizado = _mapper.Map(dto, entity);
            await _repository.UpdateAsync(actualizado, selectedProductos);
        }
    }
}
