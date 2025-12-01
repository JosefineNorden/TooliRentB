using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TooliRent.Core.Interfaces
{
    public interface IUnitOfWork
    {
        IToolRepository Tools { get; }
        ICustomerRepository Customers { get; }
        IRentalRepository Rentals { get; }
        Task SaveChangesAsync(CancellationToken ct);
        Task SaveChangesAsync();
    }
}
