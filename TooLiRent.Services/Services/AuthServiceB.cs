using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using TooLiRent.Core.Enums;
using TooLiRent.Core.Interfaces;
using TooLiRent.Core.Models;
using TooLiRent.Services.DTOs.LoginDTOs;
using TooLiRent.Services.DTOs.RegisterDTOs;
using TooLiRent.Services.DTOs.UsersDTOs;
using TooLiRent.Services.Interfaces;
using JwtRegisteredClaimNames = Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames;



namespace TooLiRent.Services.Services
{
    public class AuthServiceB : IAuthServiceB
    {
        private const string RefreshTokenProvider = "TooLiRent";
        private const string RefreshTokenName = "RefreshToken";

        private readonly UserManager<IdentityUser> _users;
        private readonly RoleManager<IdentityRole> _roles;
        private readonly IConfiguration _cfg;
        private readonly IUnitOfWork _uow;

        public AuthServiceB(
            UserManager<IdentityUser> users,
            RoleManager<IdentityRole> roles,
            IConfiguration cfg, IUnitOfWork uow)

        {
            _users = users;
            _roles = roles;
            _cfg = cfg;
            _uow = uow;
        }

        public async Task<(string AccessToken, string RefreshToken)?> LoginAsync(LoginDto dto)
        {
            var user = await _users.FindByEmailAsync(dto.Email);
            if (user is null) return null;

            if (await _users.IsLockedOutAsync(user))
                return null;

            var ok = await _users.CheckPasswordAsync(user, dto.Password);
            if (!ok) return null;

            var roles = await _users.GetRolesAsync(user);
            var accessToken = GenerateJwt(user, roles);

            // Skapa refresh token
            var refreshToken = Guid.NewGuid().ToString();

            // Spara refresh token i AspNetUserTokens
            await _users.SetAuthenticationTokenAsync(
                user,
                RefreshTokenProvider,
                RefreshTokenName,
                refreshToken
            );

            return (accessToken, refreshToken);
        }

        public async Task<(string AccessToken, string RefreshToken)?> RefreshAsync(string refreshToken)
        {
            var users = _users.Users.ToList();
            foreach (var user in users)
            {
                var storedToken = await _users.GetAuthenticationTokenAsync(
                    user,
                    RefreshTokenProvider,
                    RefreshTokenName);

                if (storedToken == refreshToken)
                {
                    var roles = await _users.GetRolesAsync(user);
                    var newAccessToken = GenerateJwt(user, roles);
                    var newRefreshToken = Guid.NewGuid().ToString();

                    await _users.SetAuthenticationTokenAsync(
                        user,
                        RefreshTokenProvider,
                        RefreshTokenName,
                        newRefreshToken);

                    return (newAccessToken, newRefreshToken);
                }
            }

            return null;
        }

        public async Task<(bool Succeeded, IEnumerable<string> Errors)> RegisterMemberAsync(RegisterDto dto)
            => await RegisterAsync(dto, "Member");

        public async Task<(bool Succeeded, IEnumerable<string> Errors)> RegisterAdminAsync(RegisterDto dto)
            => await RegisterAsync(dto, "Admin");

        private async Task<(bool Succeeded, IEnumerable<string> Errors)> RegisterAsync(RegisterDto dto, string role)
        {
            var user = new IdentityUser
            {
                UserName = dto.Email,
                Email = dto.Email,
                EmailConfirmed = true
            };

            var result = await _users.CreateAsync(user, dto.Password);
            if (!result.Succeeded)
                return (false, result.Errors.Select(e => e.Description));

            if (!await _roles.RoleExistsAsync(role))
                return (false, new[] { $"Role '{role}' does not exist" });

            var roleResult = await _users.AddToRoleAsync(user, role);
            if (!roleResult.Succeeded)
                return (false, roleResult.Errors.Select(e => e.Description));
            if (string.Equals(role, "Member", StringComparison.OrdinalIgnoreCase))
            {
                var existingCustomer = await _uow.Customers.GetByEmailAsync(dto.Email);
                if (existingCustomer is null)
                {
                    var customer = new Customer
                    {
                        Name = dto.Email.Split('@')[0],   
                        Email = dto.Email,
                        PhoneNumber = string.Empty,        
                        Status = CustomerStatus.Active,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    };

                    await _uow.Customers.AddAsync(customer);
                    await _uow.SaveChangesAsync();
                }
            }
            return (true, Enumerable.Empty<string>());
        }

        private string GenerateJwt(IdentityUser user, IList<string> roles)
        {
            var key = _cfg["Jwt:Key"]!;
            var issuer = _cfg["Jwt:Issuer"];
            var audience = _cfg["Jwt:Audience"];
            var now = DateTime.UtcNow;

            Console.WriteLine($"[AuthService] JWT KEY PREFIX : {key[..8]}");
            Console.WriteLine($"[AuthService] JWT ISSUER    : {issuer}");
            Console.WriteLine($"[AuthService] JWT AUDIENCE  : {audience}");

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                new Claim(JwtRegisteredClaimNames.Email, user.Email ?? string.Empty),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat,
                          new DateTimeOffset(now).ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64),
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Name, user.UserName ?? user.Email ?? string.Empty)
            };

            claims.AddRange(roles.Select(r => new Claim(ClaimTypes.Role, r)));

            var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var creds = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                notBefore: now,
                expires: now.AddHours(1),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);

        }

        public async Task<(bool Succeeded, IEnumerable<string> Errors)> DeleteUserAsync(string email)
        {
            var user = await _users.FindByEmailAsync(email);
            if (user == null)
                return (false, new[] { "User not found" });

            var result = await _users.DeleteAsync(user);
            if (!result.Succeeded)
                return (false, result.Errors.Select(e => e.Description));

            return (true, Enumerable.Empty<string>());
        }

        public async Task<IReadOnlyList<UserListDto>> GetAdminsAsync()
            => await GetUsersInRoleAsync("Admin");

        public async Task<IReadOnlyList<UserListDto>> GetMembersAsync()
            => await GetUsersInRoleAsync("Member");

        private async Task<IReadOnlyList<UserListDto>> GetUsersInRoleAsync(string role)
        {
            var users = await _users.GetUsersInRoleAsync(role); // IList<IdentityUser>
            var list = new List<UserListDto>(users.Count);
            foreach (var u in users)
            {
                var roles = await _users.GetRolesAsync(u);
                list.Add(new UserListDto
                {
                    Id = u.Id,
                    Email = u.Email ?? string.Empty,
                    UserName = u.UserName ?? string.Empty,
                    Roles = roles
                });
            }
            return list;
        }

        public async Task<(bool Succeeded, IEnumerable<string> Errors)> DeactivateUserAsync(string email)
        {
            var user = await _users.FindByEmailAsync(email);
            if (user is null)
                return (false, new[] { "User not found" });

            // Lås Identity-user
            await _users.SetLockoutEnabledAsync(user, true);
            var res = await _users.SetLockoutEndDateAsync(user, DateTimeOffset.MaxValue);

            var customer = await _uow.Customers.GetByEmailAsync(email);
            if (customer is not null)
            {
                customer.Status = CustomerStatus.Inactive; 
                customer.UpdatedAt = DateTime.UtcNow;
                await _uow.SaveChangesAsync();
            }

            return res.Succeeded
                ? (true, Enumerable.Empty<string>())
                : (false, res.Errors.Select(e => e.Description));
        }

        public async Task<(bool Succeeded, IEnumerable<string> Errors)> ActivateUserAsync(string email)
        {
            var user = await _users.FindByEmailAsync(email);
            if (user is null)
                return (false, new[] { "User not found" });

            // Lås upp Identity-user
            await _users.SetLockoutEnabledAsync(user, true);
            var res = await _users.SetLockoutEndDateAsync(user, null);

            var customer = await _uow.Customers.GetByEmailAsync(email);
            if (customer is not null)
            {
                customer.Status = CustomerStatus.Active;
                customer.UpdatedAt = DateTime.UtcNow;
                await _uow.SaveChangesAsync();
            }

            return res.Succeeded
                ? (true, Enumerable.Empty<string>())
                : (false, res.Errors.Select(e => e.Description));
        }
    }
}
