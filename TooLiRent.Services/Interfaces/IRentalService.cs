using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TooLiRent.Services.DTOs.RentalDTOs;

namespace TooLiRent.Services.Interfaces
{
    public interface IRentalService
    {
        Task<IEnumerable<RentalDto>> GetAllAsync(); // Admin i controller

        Task<RentalDto?> GetByIdAsync(int id, string? requesterEmail, bool isAdmin);

        Task<RentalDto> CreateAsync(RentalCreateDto dto, string? requesterEmail, bool isAdmin);

        Task<RentalDto?> UpdateAsync(RentalUpdateDto dto, string? requesterEmail, bool isAdmin);
        Task<RentalDto?> PickUpAsync(int id, string? requesterEmail, bool isAdmin);

        Task<RentalDto?> ReturnAsync(int id, string? requesterEmail, bool isAdmin);
        Task<IEnumerable<RentalDto>> GetMyAsync(string email);
        Task<IEnumerable<RentalDto>> GetOverdueAsync();
        Task<bool> CancelAsync(int id, string? requesterEmail, bool isAdmin);
        Task<bool> DeleteAsync(int id); // Admin i controller
    }
}
