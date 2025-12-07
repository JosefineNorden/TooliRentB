using AutoMapper;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TooLiRent.Core.Enums;
using TooLiRent.Core.Interfaces;
using TooLiRent.Services.DTOs.ToolDTOs;
using TooLiRent.Services.Interfaces;

namespace TooLiRent.Services.Services
{
    public class ToolService : IToolService
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;
        private readonly IValidator<ToolCreateDto> _createValidator;
        private readonly IValidator<ToolUpdateDto> _updateValidator;

        public ToolService(
            IUnitOfWork uow,
            IMapper mapper,
            IValidator<ToolCreateDto> createValidator,
            IValidator<ToolUpdateDto> updateValidator)
        {
            _uow = uow;
            _mapper = mapper;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
        }

        public async Task<IReadOnlyList<ToolDto>> GetAllAsync(CancellationToken ct)
        {
            var items = await _uow.Tools.GetAllToolsAsync(ct);
            return _mapper.Map<IReadOnlyList<ToolDto>>(items);
        }

        public async Task<ToolDto?> GetByIdAsync(int id, CancellationToken ct)
        {
            var entity = await _uow.Tools.GetToolByIdAsync(id, ct);
            return entity is null ? null : _mapper.Map<ToolDto>(entity);
        }

        public async Task<ToolDto> CreateAsync(ToolCreateDto dto, CancellationToken ct)
        {
            await _createValidator.ValidateAndThrowAsync(dto, ct);

            var entity = _mapper.Map<TooLiRent.Core.Models.Tool>(dto);
            await _uow.Tools.AddToolAsync(entity, ct);
            await _uow.SaveChangesAsync(ct);

            return _mapper.Map<ToolDto>(entity);
        }

        public async Task<bool> UpdateAsync(int id, ToolUpdateDto dto, CancellationToken ct)
        {
            await _updateValidator.ValidateAndThrowAsync(dto, ct);

            var entity = await _uow.Tools.GetToolByIdAsync(id, ct);
            if (entity is null) return false;

            _mapper.Map(dto, entity);
            await _uow.Tools.UpdateToolAsync(entity, ct);
            await _uow.SaveChangesAsync(ct);
            return true;
        }

        public async Task<bool> DeleteAsync(int id, CancellationToken ct)
        {
            if (!await _uow.Tools.ExistAsync(id, ct)) return false;
            await _uow.Tools.DeleteToolAsync(id, ct);
            await _uow.SaveChangesAsync(ct);
            return true;
        }

        public Task<bool> IsAvailableAsync(int id, DateTime? from, DateTime? to, CancellationToken ct)
            => _uow.Tools.IsAvailableAsync(id, from, to, ct);

        public async Task<IReadOnlyList<ToolDto>> GetByStatusAsync(ToolStatus status, CancellationToken ct)
        {
            var items = await _uow.Tools.GetByStatusAsync(status, ct);
            return _mapper.Map<IReadOnlyList<ToolDto>>(items);
        }

        public async Task<IReadOnlyList<ToolDto>> FilterAsync(
            string? name, int? categoryId, ToolStatus? status,
            bool? onlyAvailable, DateTime? from, DateTime? to,
            CancellationToken ct)
        {
            var items = await _uow.Tools.FilterToolAsync(name, categoryId, status, onlyAvailable, from, to, ct);
            return _mapper.Map<IReadOnlyList<ToolDto>>(items);
        }

    }
}
