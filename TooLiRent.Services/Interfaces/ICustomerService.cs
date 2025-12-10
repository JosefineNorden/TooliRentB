using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TooLiRent.Services.DTOs.CustomerDTOs;

namespace TooLiRent.Services.Interfaces
{
    public interface ICustomerService
    {
        Task<IEnumerable<CustomerDto>> GetAllAsync();
        Task<CustomerDto?> GetByIdAsync(int id);
        Task<CustomerDto> CreateAsync(CustomerCreateDto dto);
        Task<bool> UpdateAsync(int id, CustomerUpdateDto dto);
        Task<bool> SetActiveAsync(int id, bool isActive);
        Task<CustomerDto?> GetByEmailAsync(string email);
        Task<bool> DeleteAsync(int id);
    }
}
