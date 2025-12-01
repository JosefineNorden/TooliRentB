using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TooliRent.Core.Interfaces;
using TooliRent.Core.Models;
using TooliRent.Infrastructure.Data;

namespace TooLiRent.Infrastructure.Repositories
{
    public class RentalRepository : IRentalRepository
    {
        private readonly TooLiRentBDbContext _context;

        public RentalRepository(TooLiRentBDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Rental>> GetAllAsync()
        {
            return await _context.Rentals
            .Include(r => r.Customer)
            .Include(r => r.RentalDetails)
            .ThenInclude(rd => rd.Tool)
            .ToListAsync();
        }

        public async Task<Rental?> GetByIdAsync(int id)
        {
            return await _context.Rentals
                .Include(r => r.Customer)
                .Include(r => r.RentalDetails)
                .ThenInclude(rd => rd.Tool)
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task AddAsync(Rental rental)
        {
            await _context.Rentals.AddAsync(rental);
        }

        public Task UpdateAsync(Rental rental)
        {
            _context.Rentals.Update(rental);
            return Task.CompletedTask;
        }

        public async Task DeleteAsync(Rental rental)
        {
            // Ladda in rental med dess detaljer
            var rentalWithDetails = await _context.Rentals
                .Include(r => r.RentalDetails)
                .FirstOrDefaultAsync(r => r.Id == rental.Id);

            if (rentalWithDetails == null) return;

            // Ta bort alla detaljer först
            if (rentalWithDetails.RentalDetails.Any())
            {
                _context.RentalDetails.RemoveRange(rentalWithDetails.RentalDetails);
            }

            // Ta bort själva rental
            _context.Rentals.Remove(rentalWithDetails);
        }

        // Hämta alla uthyrningar för en viss kund via e-post
        public async Task<IEnumerable<Rental>> GetByCustomerEmailAsync(string email)
        {
            var e = email.Trim().ToLower();

            return await _context.Rentals
                .Include(r => r.Customer)
                .Include(r => r.RentalDetails).ThenInclude(d => d.Tool)
                .Where(r => r.Customer != null &&
                            r.Customer.Email != null &&
                            r.Customer.Email.ToLower() == e)
                .OrderByDescending(r => r.StartDate)
                .ToListAsync();
        }

        // Hämta alla förfallna uthyrningar (inte återlämnade och EndDate har passerat)
        public async Task<IEnumerable<Rental>> GetOverdueAsync(DateTime utcNow)
        {
            return await _context.Rentals
                .Include(r => r.Customer)
                .Include(r => r.RentalDetails).ThenInclude(d => d.Tool)
                .Where(r => !r.IsReturned && r.EndDate < utcNow)
                .OrderBy(r => r.EndDate)
                .ToListAsync();
        }

        public async Task<int> CountOverlappingBookedQuantityAsync(int toolId, DateTime startUtc, DateTime endUtc)
        {
            // Overlap: existing.Start < new.End && new.Start < existing.End
            var q =
                from d in _context.RentalDetails
                join r in _context.Rentals on d.RentalId equals r.Id
                where d.ToolId == toolId
                   && !r.IsReturned
                   && r.StartDate < endUtc
                   && startUtc < r.EndDate
                select d.Quantity;

            return await q.SumAsync();
        }


        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Rentals.AnyAsync(r => r.Id == id);
        }

        //Kollar om en kund har några uthyrningar
        public async Task<bool> ExistsForCustomerAsync(int customerId)
        {
            return await _context.Rentals.AnyAsync(r => r.CustomerId == customerId);
        }
    }
}

