using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TooliRent.Core.Enums;
using TooliRent.Core.Models;

namespace TooliRent.Core.Interfaces
{
    public interface IToolRepository
    {
        Task<List<Tool>> GetAllToolsAsync(CancellationToken ct);
        Task<Tool?> GetToolByIdAsync(int id, CancellationToken ct);
        Task AddToolAsync(Tool tool, CancellationToken ct);
        Task UpdateToolAsync(Tool tool, CancellationToken ct);
        Task DeleteToolAsync(int id, CancellationToken ct);
        Task<bool> ExistAsync(int id, CancellationToken ct);

        //Status och tillgänglighet
        Task<IReadOnlyList<Tool>> GetByStatusAsync(ToolStatus status, CancellationToken ct);
        Task<bool> IsAvailableAsync(int toolId, DateTime? from, DateTime? to, CancellationToken ct);
        Task<IReadOnlyList<Tool>> GetAvailableToolsAsync(DateTime? from, DateTime? to, CancellationToken ct);

        //Filtering
        Task<IReadOnlyList<Tool>> FilterToolAsync(
            string? name,
            int? categoryId,
            ToolStatus? status,
            bool? onlyAvailable,
            DateTime? from,
            DateTime? to,
            CancellationToken ct);
    }
}
