using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TooLiRent.Core.Enums;
using TooLiRent.Core.Interfaces;
using TooLiRent.Services.DTOs.AdminDTOs;
using TooLiRent.Services.DTOs.ToolDTOs;
using TooLiRent.Services.Interfaces;

namespace TooLiRent.Services.Services
{
    public class AdminSummaryService : IAdminSummaryService
    {
        private readonly IUnitOfWork _uow;

        public AdminSummaryService(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<AdminSummaryDto> GetSummaryAsync()
        {
            var nowUtc = DateTime.UtcNow;

            // Hämta alla verktyg och alla uthyrningar
            var tools = await _uow.Tools.GetAllToolsAsync(CancellationToken.None);
            var rentals = await _uow.Rentals.GetAllAsync();

            // --- Verktygssiffror ---

            var toolsTotal = tools.Count;
            var toolsBroken = tools.Count(t => t.Status == ToolStatus.Broken);

            // Aktiva uthyrningar just nu (inte returnerade, och tidsmässigt pågående)
            var activeRentalsNow = rentals.Where(r =>
            {
                var start = r.StartDate.Kind == DateTimeKind.Utc
                    ? r.StartDate
                    : r.StartDate.ToUniversalTime();

                var end = r.EndDate.Kind == DateTimeKind.Utc
                    ? r.EndDate
                    : r.EndDate.ToUniversalTime();

                return !r.IsReturned && start <= nowUtc && end > nowUtc;
            });

            // Alla verktyg som är med i en aktiv uthyrning just nu
            var rentedToolIds = activeRentalsNow
                .SelectMany(r => r.RentalDetails)
                .Select(d => d.ToolId)
                .Distinct()
                .ToHashSet();

            var toolsRented = tools.Count(t => rentedToolIds.Contains(t.Id));

            // Tillgängliga = inte trasiga, inte uthyrda just nu
            var toolsAvailable = tools.Count(t =>
                t.Status != ToolStatus.Broken &&
                !rentedToolIds.Contains(t.Id));

            // --- Uthyrningssiffror ---

            var rentalsTotal = rentals.Count();
            var activeRentals = rentals.Count(r => !r.IsReturned);

            var overdueCount = rentals.Count(r =>
            {
                var endUtc = r.EndDate.Kind == DateTimeKind.Utc
                    ? r.EndDate
                    : r.EndDate.ToUniversalTime();

                return !r.IsReturned && endUtc < nowUtc;
            });

            return new AdminSummaryDto
            {
                ToolsTotal = toolsTotal,
                ToolsAvailable = toolsAvailable,
                ToolsRented = toolsRented,
                ToolsBroken = toolsBroken,
                RentalsTotal = rentalsTotal,
                ActiveRentals = activeRentals,
                OverdueCount = overdueCount
            };
        }

        public async Task<IEnumerable<TopToolDto>> GetTopToolsAsync(
            DateTime? from = null,
            DateTime? to = null,
            int take = 3)
        {
            var rentals = await _uow.Rentals.GetAllAsync();

            var fromUtc = from.HasValue
                ? (from.Value.Kind == DateTimeKind.Utc ? from.Value : from.Value.ToUniversalTime())
                : DateTime.MinValue;

            var toUtc = to.HasValue
                ? (to.Value.Kind == DateTimeKind.Utc ? to.Value : to.Value.ToUniversalTime())
                : DateTime.MaxValue;

            // Uthyrningar som överlappar tidsfönstret
            var window = rentals.Where(r =>
            {
                var start = r.StartDate.Kind == DateTimeKind.Utc ? r.StartDate : r.StartDate.ToUniversalTime();
                var end = r.EndDate.Kind == DateTimeKind.Utc ? r.EndDate : r.EndDate.ToUniversalTime();
                return start < toUtc && end >= fromUtc;
            });

            var top = window
                .SelectMany(r => r.RentalDetails)
                .GroupBy(d => d.ToolId)
                .Select(g => new
                {
                    ToolId = g.Key,
                    Units = g.Sum(x => x.Quantity),
                    Rentals = g.Count(),
                    ToolName = g.First().Tool?.Name ?? $"Tool {g.Key}"
                })
                .OrderByDescending(x => x.Units)
                .ThenByDescending(x => x.Rentals)
                .Take(take)
                .Select(x => new TopToolDto
                {
                    ToolId = x.ToolId,
                    ToolName = x.ToolName,
                    UnitsRented = x.Units,
                    RentalsCount = x.Rentals
                })
                .ToList();

            return top;
        }
    }
}
