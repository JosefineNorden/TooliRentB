using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TooliRent.Core.Enums;
using TooliRent.Core.Interfaces;
using TooliRent.Core.Models;
using TooliRent.Infrastructure.Data;

namespace TooLiRent.Infrastructure.Repositories
{
    public class ToolRepository : IToolRepository
    {
        private readonly TooLiRentBDbContext _context;

        public ToolRepository(TooLiRentBDbContext context)
        {
            _context = context;
        }

        // --- CRUD Operations ---
     
        public async Task AddToolAsync(Tool tool, CancellationToken ct)
        {
            await _context.Tools.AddAsync(tool, ct);
        }
        public async Task UpdateToolAsync(Tool tool, CancellationToken ct)
        {
            _context.Tools.Update(tool);
            await Task.CompletedTask;
        }

        public async Task DeleteToolAsync(int id, CancellationToken ct)
        {
            var entity = await _context.Tools.FirstOrDefaultAsync(t => t.Id == id, ct);
            if (entity is null) return;
            _context.Tools.Remove(entity);
        }

        public Task<bool> ExistAsync(int id, CancellationToken ct)
            => _context.Tools.AnyAsync(t => t.Id == id, ct);
        public Task<List<Tool>> GetAllToolsAsync(CancellationToken ct)
            => _context.Tools.AsNoTracking().ToListAsync(ct);
        public Task<Tool?> GetToolByIdAsync(int id, CancellationToken ct)
            => _context.Tools.AsNoTracking().FirstOrDefaultAsync(t => t.Id == id, ct);

        // --- Status ---

        public Task<IReadOnlyList<Tool>> GetByStatusAsync(ToolStatus status, CancellationToken ct)
            => _context.Tools.AsNoTracking()
                .Where(t => t.Status == status)
                .ToListAsync(ct)
                .ContinueWith(t => (IReadOnlyList<Tool>)t.Result, ct);

        // --- Availability ---

        public async Task<bool> IsAvailableAsync(int toolId, DateTime? from, DateTime? to, CancellationToken ct)
        {
            var tool = await _context.Tools.AsNoTracking()
                .FirstOrDefaultAsync(t => t.Id == toolId, ct);
            if (tool is null) return false;
            if (tool.Status != ToolStatus.Available) return false;

            var start = from ?? DateTime.UtcNow;
            var end = to ?? start;

            var rentedQty = await _context.RentalDetails
                .Where(rd => rd.ToolId == toolId
                && rd.Rental.IsReturned == false
                && rd.Rental.EndDate > start
                && rd.Rental.StartDate < end)
                .SumAsync(rd => (int?)rd.Quantity, ct) ?? 0;

            var remainingQty = tool.Stock - rentedQty;
            return remainingQty > 0;
        }
        public async Task<IReadOnlyList<Tool>> GetAvailableToolsAsync(DateTime? from, DateTime? to, CancellationToken ct)
        {
            var start = from ?? DateTime.UtcNow;
            var end = to ?? start;

            var baseQuery = _context.Tools.AsNoTracking()
                .Where(t => t.Status == ToolStatus.Available);

            // bokningar som överlappar med det angivna datumintervallet
            var bookedPerTool = _context.RentalDetails
                .Where(rd => rd.Rental.IsReturned == false
                && rd.Rental.EndDate > start
                && rd.Rental.StartDate < end)
                .GroupBy(rd => rd.ToolId)
                .Select(g => new { ToolId = g.Key, Qty = g.Sum(x => x.Quantity) });

            var query = from tool in baseQuery
                        join booked in bookedPerTool on tool.Id equals booked.ToolId into gj
                        from b in gj.DefaultIfEmpty()
                        let booked = b != null ? b.Qty : 0
                        where (tool.Stock - booked) > 0
                        select tool;

            var list = await query.ToListAsync(ct);
            return list;
        }

        // --- Filtering ---

        public async Task<IReadOnlyList<Tool>> FilterToolAsync(string? name, int? categoryId, ToolStatus? status, bool? onlyAvailable, DateTime? from, DateTime? to, CancellationToken ct)
        {
            var query = _context.Tools.AsNoTracking().AsQueryable();
            if (!string.IsNullOrWhiteSpace(name))
            {
                var n = name.Trim();
                query = query.Where(t => t.Name.Contains(n));
            }

            if (categoryId.HasValue)
            {
                query = query.Where(t => t.CategoryId == categoryId.Value);
            }
            if (status.HasValue)
            {
                query = query.Where(t => t.Status == status.Value);
            }
            if (onlyAvailable == true)
            {
                var start = from ?? DateTime.UtcNow;
                var end = to ?? start;

                var bookedPerTool = _context.RentalDetails
                    .Where(rd => rd.Rental.IsReturned == false
                                 && rd.Rental.EndDate > start
                                 && rd.Rental.StartDate < end)
                    .GroupBy(rd => rd.ToolId)
                    .Select(g => new { ToolId = g.Key, Qty = g.Sum(x => x.Quantity) });

                query = from t in query.Where(t => t.Status == ToolStatus.Available)
                    join b in bookedPerTool on t.Id equals b.ToolId into gj
                    from b in gj.DefaultIfEmpty()
                    let booked = (b != null ? b.Qty : 0)
                    where (t.Stock - booked) > 0
                    select t;
            }

            return await query.ToListAsync(ct);
        }

    }
}
