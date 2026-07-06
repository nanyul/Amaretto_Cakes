using Amaretto.Application.DTOs;
using Amaretto.Application.Services.Interfaces;
using Amaretto.Infraestructure.Models;
using Amaretto.Infraestructure.Repository.Interfaces;
using AutoMapper;

namespace Amaretto.Application.Services.Implementations
{
    public class ServiceIngrediente : IServiceIngrediente
    {
        private readonly IRepositoryIngrediente _repository;
        private readonly IMapper _mapper;
        public ServiceIngrediente(IRepositoryIngrediente repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<ICollection<IngredienteDTO>> ListAsync()
        {
            var list = await _repository.ListAsync();
            return _mapper.Map<ICollection<IngredienteDTO>>(list);
        }

        public async Task<IngredienteDTO> FindByIdAsync(int id)
        {
            var entity = await _repository.FindByIdAsync(id);
            return _mapper.Map<IngredienteDTO>(entity);
        }
    }
}