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
    public class ServiceMenuCombo : IServiceMenuCombo
    {
        private readonly IRepositoryMenuCombo _repository;
        private readonly IMapper _mapper;

        public ServiceMenuCombo(
            IRepositoryMenuCombo repository,
            IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<ICollection<MenuComboDTO>> ListAsync()
        {
            var list = await _repository.ListAsync();

            return _mapper.Map<ICollection<MenuComboDTO>>(list);
        }

        public async Task<MenuComboDTO> FindByIdAsync(int id)
        {
            var entity = await _repository.FindByIdAsync(id);

            return _mapper.Map<MenuComboDTO>(entity);
        }

        public async Task<MenuComboDTO?> ObtenerMenuDisponibleAsync()
        {
            var entity = await _repository.ObtenerMenuDisponibleAsync();

            return _mapper.Map<MenuComboDTO>(entity);
        }
    }
}
