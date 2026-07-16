using Amaretto.Application.DTOs;
using Amaretto.Application.Services.Interfaces;
using Amaretto.Infraestructure.Repository.Interfaces;
using AutoMapper;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Amaretto.Application.Services.Implementations
{
    public class ServicePedidoDetalle : IServicePedidoDetalle
    {
        private readonly IRepositoryPedidoDetalle _repository;
        private readonly IMapper _mapper;

        public ServicePedidoDetalle(IRepositoryPedidoDetalle repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<ICollection<PedidoDetalleDTO>> ListDisponiblesAsync()
        {
            var list = await _repository.ListDisponiblesAsync();
            return _mapper.Map<ICollection<PedidoDetalleDTO>>(list);
        }

        public async Task<PedidoDetalleDTO?> FindByIdAsync(int id)
        {
            var entity = await _repository.FindByIdAsync(id);
            return entity == null ? null : _mapper.Map<PedidoDetalleDTO>(entity);
        }
    }
}
