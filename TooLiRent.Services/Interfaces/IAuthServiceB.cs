using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TooLiRent.Services.DTOs.LoginDTOs;
using TooLiRent.Services.DTOs.RegisterDTOs;
using TooLiRent.Services.DTOs.UsersDTOs;

namespace TooLiRent.Services.Interfaces
{
    public interface IAuthServiceB
    {
        Task<(string AccessToken, string RefreshToken)?> LoginAsync(LoginDto dto);
        Task<(string AccessToken, string RefreshToken)?> RefreshAsync(string refreshToken);
        Task<(bool Succeeded, IEnumerable<string> Errors)> RegisterMemberAsync(RegisterDto dto);
        Task<(bool Succeeded, IEnumerable<string> Errors)> RegisterAdminAsync(RegisterDto dto);
        Task<(bool Succeeded, IEnumerable<string> Errors)> DeleteUserAsync(string email);
        Task<IReadOnlyList<UserListDto>> GetAdminsAsync();
        Task<IReadOnlyList<UserListDto>> GetMembersAsync();
        Task<(bool Succeeded, IEnumerable<string> Errors)> DeactivateUserAsync(string email);
        Task<(bool Succeeded, IEnumerable<string> Errors)> ActivateUserAsync(string email);
    }
}
