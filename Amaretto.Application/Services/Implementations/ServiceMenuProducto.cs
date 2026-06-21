using Amaretto.Application.DTOs;
using Amaretto.Application.Services.Interfaces;
using Amaretto.Infraestructure.Repository.Interfaces;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Amaretto.Application.Services.Implementations
{
    public class ServiceMenuProducto : IServiceMenuProducto
    {
        private readonly IRepositoryMenuProducto _repository;
        private readonly IMapper _mapper;

        public ServiceMenuProducto(
            IRepositoryMenuProducto repository,
            IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<ICollection<MenuProductoDTO>> ListAsync()
        {
            var list = await _repository.ListAsync();

            return _mapper.Map<ICollection<MenuProductoDTO>>(list);
        }

        public async Task<MenuProductoDTO> FindByIdAsync(int id)
        {
            var entity = await _repository.FindByIdAsync(id);

            return _mapper.Map<MenuProductoDTO>(entity);
        }

        public async Task<MenuProductoDTO?> ObtenerMenuDisponibleAsync()
        {
            var entity = await _repository.ObtenerMenuDisponibleAsync();

            return _mapper.Map<MenuProductoDTO>(entity);
        }
    }
}
