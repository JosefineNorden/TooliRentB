using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TooliRent.Core.Interfaces;
using TooliRent.Infrastructure.Data;

namespace TooLiRent.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly TooLiRentBDbContext _context;
        private IToolRepository? _tools;
        private ICustomerRepository? _customers;
        private IRentalRepository? _rentals;

        public UnitOfWork(TooLiRentBDbContext context)
        {
            _context = context;
        }

        public IToolRepository Tools => _tools ??= new ToolRepository(_context);
        public ICustomerRepository Customers => _customers ??= new CustomerRepository(_context);
        public IRentalRepository Rentals => _rentals ??= new RentalRepository(_context);

        public Task SaveChangesAsync()
        {
            return _context.SaveChangesAsync();
        }

        public Task SaveChangesAsync(CancellationToken ct)
        {
            return _context.SaveChangesAsync(ct);
        }
    }
}
