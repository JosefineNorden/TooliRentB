using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TooLiRent.Core.Enums;
using TooLiRent.Core.Interfaces;
using TooLiRent.Core.Models;
using TooLiRent.Services.DTOs.CustomerDTOs;
using TooLiRent.Services.Interfaces;

namespace TooLiRent.Services.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly IUnitOfWork _unitOfWork;

        public CustomerService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        private static CustomerDto ToDto(Customer c) => new CustomerDto
        {
            Id = c.Id,
            Name = c.Name,
            Email = c.Email,
            PhoneNumber = c.PhoneNumber
        };

        public async Task<IEnumerable<CustomerDto>> GetAllAsync()
        {
            var customers = await _unitOfWork.Customers.GetAllAsync();
            return customers.Select(ToDto);
        }

        public async Task<CustomerDto?> GetByIdAsync(int id)
        {
            var customer = await _unitOfWork.Customers.GetByIdAsync(id);
            return customer is null ? null : ToDto(customer);
        }

        public async Task<CustomerDto> CreateAsync(CustomerCreateDto dto)
        {
            var customer = new Customer
            {
                Name = dto.Name,
                Email = dto.Email,
                PhoneNumber = dto.PhoneNumber,
                Status = CustomerStatus.Active
            };

            await _unitOfWork.Customers.AddAsync(customer);
            await _unitOfWork.SaveChangesAsync();

            return ToDto(customer);
        }

        public async Task<bool> UpdateAsync(int id, CustomerUpdateDto dto)
        {
            var customer = await _unitOfWork.Customers.GetByIdAsync(id);
            if (customer is null) return false;

            customer.Name = dto.Name;
            customer.Email = dto.Email;
            customer.PhoneNumber = dto.PhoneNumber;

            await _unitOfWork.Customers.UpdateAsync(customer);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        public async Task<bool> SetActiveAsync(int id, bool isActive)
        {
            var customer = await _unitOfWork.Customers.GetByIdAsync(id);
            if (customer is null) return false;

            customer.Status = isActive ? CustomerStatus.Active : CustomerStatus.Inactive;

            await _unitOfWork.Customers.UpdateAsync(customer);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var customer = await _unitOfWork.Customers.GetByIdAsync(id);
            if (customer is null) return false;

            // TODO: Kolla ev. aktiva uthyrningar här och kasta InvalidOperationException vid behov
            await _unitOfWork.Customers.DeleteAsync(customer);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }
    }
}
