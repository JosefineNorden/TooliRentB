using AutoMapper;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TooliRent.Core.Enums;
using TooliRent.Core.Interfaces;
using TooliRent.Core.Models;
using TooLiRent.Services.DTOs.RentalDTOs;
using TooLiRent.Services.Interfaces;

namespace TooLiRent.Services.Services
{
    public class RentalService : IRentalService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IValidator<RentalCreateDto> _createValidator;
        private readonly IValidator<RentalUpdateDto> _updateValidator;


        public RentalService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IValidator<RentalCreateDto> createValidator,
            IValidator<RentalUpdateDto> updateValidator)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
        }

        public async Task<IEnumerable<RentalDto>> GetAllAsync()
        {
            var rentals = await _unitOfWork.Rentals.GetAllAsync();
            return _mapper.Map<IEnumerable<RentalDto>>(rentals);
        }

        public async Task<RentalDto?> GetByIdAsync(int id, string? requesterEmail, bool isAdmin)
        {
            var rental = await _unitOfWork.Rentals.GetByIdAsync(id);
            if (rental is null) return null;

            if (!isAdmin && !IsOwner(rental, requesterEmail))
                throw new UnauthorizedAccessException("Not owner of this rental.");

            return _mapper.Map<RentalDto>(rental);
        }

        public async Task<RentalDto> CreateAsync(RentalCreateDto dto, string? requesterEmail, bool isAdmin)
        {
            // Validering av DTO
            var validationResult = await _createValidator.ValidateAsync(dto);
            if (!validationResult.IsValid)
                throw new ValidationException(validationResult.Errors);

            if (dto.StartDate >= dto.EndDate)
                throw new InvalidOperationException("StartDate måste vara före EndDate.");

            var startUtc = dto.StartDate.Kind == DateTimeKind.Utc
                ? dto.StartDate
                : dto.StartDate.ToUniversalTime();

            var endUtc = dto.EndDate.Kind == DateTimeKind.Utc
                ? dto.EndDate
                : dto.EndDate.ToUniversalTime();

            if (startUtc < DateTime.UtcNow)
                throw new InvalidOperationException("StartDate måste ligga i framtiden.");

            // Verktyg som ska hyras
            var toolIds = dto.ToolIds?.ToList() ?? new List<int>();
            if (toolIds.Count == 0)
                throw new InvalidOperationException("Minst ett verktyg måste väljas.");

            // Grupp per toolId (ifall samma verktyg valts flera gånger)
            var groups = toolIds.GroupBy(id => id);
            foreach (var g in groups)
            {
                var toolId = g.Key;
                var needed = g.Count();

                var tool = await _unitOfWork.Tools.GetToolByIdAsync(toolId, CancellationToken.None);
                if (tool == null)
                    throw new InvalidOperationException($"Verktyg med ID {toolId} finns inte.");

                if (tool.Status == ToolStatus.Broken)
                    throw new InvalidOperationException($"Verktyget '{tool.Name}' är trasigt och kan inte bokas.");

                // Hur många exemplar är redan bokade (ej återlämnade) som överlappar perioden?
                var overlaps = await _unitOfWork.Rentals
                    .CountOverlappingBookedQuantityAsync(toolId, startUtc, endUtc);

                var remaining = tool.Stock - overlaps;
                if (remaining < needed)
                {
                    throw new InvalidOperationException(
                        $"Verktyget '{tool.Name}' är fullbokat för perioden. Ledigt: {Math.Max(0, remaining)}, önskat: {needed}.");
                }
            }

            // --- Hitta/skap kund ---
            int customerId;
            if (isAdmin && dto.CustomerId > 0)
            {
                var customerEntity = await _unitOfWork.Customers.GetByIdAsync(dto.CustomerId);
                if (customerEntity is null)
                    throw new InvalidOperationException($"Customer with ID {dto.CustomerId} not found.");

                if (customerEntity.Status != CustomerStatus.Active)
                    throw new InvalidOperationException("Customer is deactivated and cannot make rentals.");

                customerId = customerEntity.Id;
            }
            else
            {
                if (string.IsNullOrWhiteSpace(requesterEmail))
                    throw new UnauthorizedAccessException("Missing requester email.");

                var customer = await _unitOfWork.Customers.GetByEmailAsync(requesterEmail);
                if (customer is null)
                {
                    customer = new Customer
                    {
                        Name = requesterEmail.Split('@')[0],
                        Email = requesterEmail,
                        PhoneNumber = string.Empty,
                        Status = CustomerStatus.Active,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    };
                    await _unitOfWork.Customers.AddAsync(customer);
                    await _unitOfWork.SaveChangesAsync();
                }
                else
                {
                    if (customer.Status != CustomerStatus.Active)
                        throw new InvalidOperationException("Your account is deactivated and cannot make rentals.");
                }

                customerId = customer.Id;
            }

            // --- Skapa själva uthyrningen ---
            var rental = new Rental
            {
                CustomerId = customerId,
                StartDate = startUtc,
                EndDate = endUtc,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
                // IsReturned = false (default)
            };

            // Lägg till detaljer (1 st per toolId här)
            foreach (var toolId in toolIds)
            {
                var tool = await _unitOfWork.Tools.GetToolByIdAsync(toolId, CancellationToken.None);
                if (tool == null)
                    throw new InvalidOperationException($"Tool with ID {toolId} not found.");

                rental.RentalDetails.Add(new RentalDetail
                {
                    ToolId = tool.Id,
                    Quantity = 1,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                });
            }

            await _unitOfWork.Rentals.AddAsync(rental);
            await _unitOfWork.SaveChangesAsync();

            var created = await _unitOfWork.Rentals.GetByIdAsync(rental.Id);
            return _mapper.Map<RentalDto>(created);
        }

        public async Task<RentalDto?> UpdateAsync(RentalUpdateDto dto, string? requesterEmail, bool isAdmin)
        {
            var validationResult = await _updateValidator.ValidateAsync(dto);
            if (!validationResult.IsValid)
                throw new ValidationException(validationResult.Errors);

            var rental = await _unitOfWork.Rentals.GetByIdAsync(dto.Id);
            if (rental == null) return null;

            if (!isAdmin && !IsOwner(rental, requesterEmail))
                throw new UnauthorizedAccessException("Not owner of this rental.");

            rental.StartDate = dto.StartDate;
            rental.EndDate = dto.EndDate;
            rental.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.SaveChangesAsync();
            var updated = await _unitOfWork.Rentals.GetByIdAsync(rental.Id);
            return _mapper.Map<RentalDto>(updated);
        }

        public async Task<RentalDto?> PickUpAsync(int id, string? requesterEmail, bool isAdmin)
        {
            var rental = await _unitOfWork.Rentals.GetByIdAsync(id);
            if (rental == null) return null;

            // Endast ägaren eller admin
            if (!isAdmin && !IsOwner(rental, requesterEmail))
                throw new UnauthorizedAccessException("Not owner of this rental.");

            // Redan avslutad?
            if (rental.IsReturned)
                throw new InvalidOperationException("Uthyrningen är redan avslutad.");

            foreach (var d in rental.RentalDetails)
            {
                var tool = d.Tool;
                if (tool == null) continue;

                var qty = d.Quantity > 0 ? d.Quantity : 1;

                if (tool.Status == ToolStatus.Broken)
                    throw new InvalidOperationException($"Verktyget '{tool.Name}' är trasigt och kan inte hämtas ut.");

                // Ev. säkerhetskoll så vi inte plockar ut mer än fysiskt lager
                if (tool.Stock < qty)
                    throw new InvalidOperationException(
                        $"Inte tillräckligt i lager av '{tool.Name}' för att hämta ut ({tool.Stock} < {qty}).");

                // OBS: Vi ändrar INTE tool.Status här,
                // och vi ändrar egentligen inte Stock heller om du vill låta
                // tillgänglighet styras av bokningar+IsReturned i Rental.

                tool.Stock -= qty;
                if (tool.Stock < 0) tool.Stock = 0;
            }

            await _unitOfWork.SaveChangesAsync();

            var refreshed = await _unitOfWork.Rentals.GetByIdAsync(rental.Id);
            return _mapper.Map<RentalDto>(refreshed);
        }

        public async Task<RentalDto?> ReturnAsync(int id, string? requesterEmail, bool isAdmin)
        {
            var rental = await _unitOfWork.Rentals.GetByIdAsync(id);
            if (rental == null) return null;

            // Endast ägaren eller admin
            if (!isAdmin && !IsOwner(rental, requesterEmail))
                throw new UnauthorizedAccessException("Not owner of this rental.");

            if (rental.IsReturned)
                throw new InvalidOperationException("Uthyrningen är redan återlämnad.");

            foreach (var d in rental.RentalDetails)
            {
                var tool = d.Tool;
                if (tool == null) continue;

                var qty = d.Quantity > 0 ? d.Quantity : 1;

                // Om du justerar stock vid pickup kan du här öka tillbaka:
                // tool.Stock += qty;

                // Om verktyget inte är markerat som trasigt, säkerställ att det står som Available
                if (tool.Status != ToolStatus.Broken)
                    tool.Status = ToolStatus.Available;
            }

            rental.IsReturned = true;

            await _unitOfWork.SaveChangesAsync();

            var refreshed = await _unitOfWork.Rentals.GetByIdAsync(rental.Id);
            return _mapper.Map<RentalDto>(refreshed);
        }

        public async Task<IEnumerable<RentalDto>> GetMyAsync(string email)
        {
            var rentals = await _unitOfWork.Rentals.GetByCustomerEmailAsync(email);
            return _mapper.Map<IEnumerable<RentalDto>>(rentals);
        }

        public async Task<IEnumerable<RentalDto>> GetOverdueAsync()
        {
            var now = DateTime.UtcNow;
            var rentals = await _unitOfWork.Rentals.GetOverdueAsync(now);
            return rentals.Select(r =>
            {
                var dto = _mapper.Map<RentalDto>(r);
                dto.IsLate = !r.IsReturned &&
                             (r.EndDate.Kind == DateTimeKind.Utc
                                 ? r.EndDate
                                 : r.EndDate.ToUniversalTime()) < now;
                return dto;
            }).ToList();
        }

        public async Task<bool> CancelAsync(int id, string? requesterEmail, bool isAdmin)
        {
            var rental = await _unitOfWork.Rentals.GetByIdAsync(id);
            if (rental is null) return false;

            if (!isAdmin && !IsOwner(rental, requesterEmail))
                throw new UnauthorizedAccessException();

            var startUtc = rental.StartDate.Kind == DateTimeKind.Utc
                ? rental.StartDate
                : rental.StartDate.ToUniversalTime();

            if (DateTime.UtcNow >= startUtc)
                throw new InvalidOperationException("Bokningen kan inte avbokas efter start.");

            await _unitOfWork.Rentals.DeleteAsync(rental);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var rental = await _unitOfWork.Rentals.GetByIdAsync(id);
            if (rental == null) return false;

            await _unitOfWork.Rentals.DeleteAsync(rental);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        private static bool IsOwner(Rental rental, string? email)
            => !string.IsNullOrWhiteSpace(email)
               && string.Equals(rental.Customer?.Email, email, StringComparison.OrdinalIgnoreCase);
    }
}


