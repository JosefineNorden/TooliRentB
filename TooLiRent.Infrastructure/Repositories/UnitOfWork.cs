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
    public class UnitOfWork(TooLiRentBDbContext context) : IUnitOfWork
    {
        private readonly TooLiRentBDbContext _context = context;
        private IToolRepository? _tools;

        public IToolRepository Tools => _tools ??= new ToolRepository(_context);
        public async Task<int> SaveChangesAsync(CancellationToken ct)
        {
            return await _context.SaveChangesAsync(ct);
        }

    }
}
