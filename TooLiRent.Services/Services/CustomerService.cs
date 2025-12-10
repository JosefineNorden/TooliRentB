using AutoMapper;
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
        private readonly IMapper _mapper;

        public CustomerService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
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
            return _mapper.Map<IEnumerable<CustomerDto>>(customers);
        }

        public async Task<CustomerDto?> GetByIdAsync(int id)
        {
            var customer = await _unitOfWork.Customers.GetByIdAsync(id);
            return customer is null ? null : _mapper.Map<CustomerDto>(customer);
        }

        public async Task<CustomerDto> CreateAsync(CustomerCreateDto dto)
        {
            var customer = _mapper.Map<Customer>(dto);
            customer.Status = CustomerStatus.Active;

            await _unitOfWork.Customers.AddAsync(customer);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<CustomerDto>(customer);
        }

        public async Task<bool> UpdateAsync(int id, CustomerUpdateDto dto)
        {
            var customer = await _unitOfWork.Customers.GetByIdAsync(id);
            if (customer is null) return false;

            _mapper.Map(dto, customer); // uppdaterar befintlig entity

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

        public async Task<CustomerDto?> GetByEmailAsync(string email)
        {
            var entity = await _unitOfWork.Customers.GetByEmailAsync(email);
            if (entity is null) return null;

            return _mapper.Map<CustomerDto>(entity);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var customer = await _unitOfWork.Customers.GetByIdAsync(id);
            if (customer is null) return false;

            // TODO: kolla aktiva rentals innan delete
            await _unitOfWork.Customers.DeleteAsync(customer);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }
    }
}
