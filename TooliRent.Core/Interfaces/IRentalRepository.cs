using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TooliRent.Core.Models;

namespace TooliRent.Core.Interfaces
{
    public interface IRentalRepository
    {
        Task<IEnumerable<Rental>> GetAllAsync();
        Task<Rental?> GetByIdAsync(int id);
        Task AddAsync(Rental rental);
        Task UpdateAsync(Rental rental);
        Task DeleteAsync(Rental rental);
        Task<bool> ExistsAsync(int id);
        Task<bool> ExistsForCustomerAsync(int customerId);
        Task<IEnumerable<Rental>> GetByCustomerEmailAsync(string email);
        Task<IEnumerable<Rental>> GetOverdueAsync(DateTime utcNow);
        Task<int> CountOverlappingBookedQuantityAsync(int toolId, DateTime startUtc, DateTime endUtc);
    }
}
