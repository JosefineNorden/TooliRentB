using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TooLiRent.Core.Enums;
using TooLiRent.Services.DTOs.ToolDTOs;

namespace TooLiRent.Services.Interfaces
{
    public interface IToolService
    {
        Task<IReadOnlyList<ToolDto>> GetAllAsync(CancellationToken ct);
        Task<ToolDto?> GetByIdAsync(int id, CancellationToken ct);

        Task<ToolDto> CreateAsync(ToolCreateDto dto, CancellationToken ct);
        Task<bool> UpdateAsync(int id, ToolUpdateDto dto, CancellationToken ct);
        Task<bool> DeleteAsync(int id, CancellationToken ct);

        Task<bool> IsAvailableAsync(int id, DateTime? from, DateTime? to, CancellationToken ct);
        Task<IReadOnlyList<ToolDto>> GetByStatusAsync(ToolStatus status, CancellationToken ct);

        Task<IReadOnlyList<ToolDto>> FilterAsync(
            string? name, int? categoryId, ToolStatus? status,
            bool? onlyAvailable, DateTime? from, DateTime? to,
            CancellationToken ct);
    }
}
