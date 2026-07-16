using Amaretto.Application.DTOs;
using Amaretto.Application.Services.Interfaces;
using Amaretto.Infraestructure.Models;
using Amaretto.Infraestructure.Repository.Interfaces;
using AutoMapper;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Amaretto.Application.Services.Implementations
{
    public class ServiceMenuCombo : IServiceMenuCombo
    {
        private readonly IRepositoryMenuCombo _repository;
        private readonly IMapper _mapper;

        public ServiceMenuCombo(IRepositoryMenuCombo repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<MenuComboDTO?> ObtenerMenuDisponibleAsync()
        {
            var entity = await _repository.ObtenerMenuDisponibleAsync();
            return entity == null ? null : _mapper.Map<MenuComboDTO>(entity);
        }

        public async Task<MenuComboDTO> FindByIdAsync(int id)
        {
            var entity = await _repository.FindByIdAsync(id);
            return _mapper.Map<MenuComboDTO>(entity);
        }

        public async Task<ICollection<MenuComboDTO>> ListAsync()
        {
            var list = await _repository.ListAsync();
            return _mapper.Map<ICollection<MenuComboDTO>>(list);
        }

        // CREAR
        public async Task<int> AddAsync(MenuComboDTO dto, string[] selectedCombos)
        {
            var entity = _mapper.Map<MenuCombo>(dto);
            return await _repository.AddAsync(entity, selectedCombos);
        }

        // ACTUALIZAR
        public async Task UpdateAsync(int id, MenuComboDTO dto, string[] selectedCombos)
        {
            var entity = await _repository.FindByIdAsync(id);
            var actualizado = _mapper.Map(dto, entity);
            await _repository.UpdateAsync(actualizado, selectedCombos);
        }
    }
}
