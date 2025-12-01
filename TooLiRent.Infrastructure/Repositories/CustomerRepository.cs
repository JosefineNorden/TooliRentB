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
    public class CustomerRepository : ICustomerRepository
    {
        private readonly TooLiRentBDbContext _context;

        public CustomerRepository(TooLiRentBDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Customer>> GetAllAsync()
        {
            return await _context.Customers.ToListAsync();
        }

        public async Task<Customer?> GetByIdAsync(int id)
        {
            return await _context.Customers.FindAsync(id);
        }

        public async Task AddAsync(Customer customer)
        {
            await _context.Customers.AddAsync(customer);
        }

        public Task UpdateAsync(Customer customer)
        {
            _context.Customers.Update(customer);
            return Task.CompletedTask;
        }

        public Task DeleteAsync(Customer customer)
        {
            _context.Customers.Remove(customer);
            return Task.CompletedTask;
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Customers.AnyAsync(c => c.Id == id);
        }

        public Task<IEnumerable<Customer>> GetAvailableAsync()
        {
            // Placeholder 
            return Task.FromResult(Enumerable.Empty<Customer>() as IEnumerable<Customer>);
        }

        public Task<IEnumerable<Customer>> FilterAsync(string? category, string? status, bool? onlyAvailable)
        {
            // Placeholder 
            return Task.FromResult(Enumerable.Empty<Customer>() as IEnumerable<Customer>);
        }

        public async Task<Customer?> GetByEmailAsync(string email)
        {
            return await _context.Customers.FirstOrDefaultAsync(c => c.Email == email);
        }
    }
}
