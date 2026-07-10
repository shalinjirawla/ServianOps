using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ServianOps_Backend.Application.DTOs.User;
using ServianOps_Backend.Application.DTOs.Shared;
using ServianOps_Backend.Core.Entities.Identity;
using ServianOps_Backend.Core.Interfaces.Repositories;
using ServianOps_Backend.Application.Interfaces;

namespace ServianOps_Backend.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;

        public UserService(IUserRepository userRepository, IPasswordHasher passwordHasher)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
        }

        public async Task<UserDto> CreateUserAsync(CreateUserDto dto, long? tenantId)
        {
            var existingUser = await _userRepository.GetByEmailAsync(dto.Email);
            if (existingUser != null)
            {
                throw new Exception($"Email '{dto.Email}' is already registered for this tenant.");
            }

            var hash = _passwordHasher.HashPassword(dto.Password, out var salt);

            var user = new User
            {
                TenantId = tenantId,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email,
                Phone = dto.Phone ?? string.Empty,
                ProfileImage = string.Empty,
                PasswordHash = hash,
                PasswordSalt = salt,
                IsActive = true
            };

            await _userRepository.AddAsync(user);

            return MapToDto(user);
        }

        public async Task<UserDto> GetUserByEmailAsync(string email)
        {
            var user = await _userRepository.GetByEmailAsync(email);
            return user == null ? null : MapToDto(user);
        }

        public async Task<UserDto> GetUserByEmailAndTenantIdAsync(string email, long? tenantId)
        {
            var user = await _userRepository.GetByEmailAndTenantIdAsync(email, tenantId);
            return user == null ? null : MapToDto(user);
        }

        public async Task<bool> ValidateCredentialsAsync(string email, string password, long? tenantId)
        {
            var user = await _userRepository.GetByEmailAndTenantIdAsync(email, tenantId);
            if (user == null) return false;
            
            return _passwordHasher.VerifyPassword(password, user.PasswordHash, user.PasswordSalt);
        }

        public async Task<UserDto> GetUserByIdAsync(long id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            return user == null ? null : MapToDto(user);
        }

        public async Task<string> GetUserRoleNameAsync(long userId)
        {
            return await _userRepository.GetUserRoleNameAsync(userId);
        }

        public async Task<IReadOnlyList<UserDto>> GetUsersPagedAsync(int pageNumber, int pageSize)
        {
            var users = await _userRepository.GetPagedAsync(pageNumber, pageSize);
            return users.Select(MapToDto).ToList();
        }

        public async Task<IReadOnlyList<UserDto>> GetAdministratorsPagedAsync(int pageNumber, int pageSize)
        {
            var users = await _userRepository.GetAdministratorsPagedAsync(pageNumber, pageSize);
            return users.Select(MapToDto).ToList();
        }

        public async Task<IReadOnlyList<UserDto>> GetTenantAdministratorsAsync(long tenantId)
        {
            var users = await _userRepository.GetTenantAdministratorsAsync(tenantId);
            return users.Select(MapToDto).ToList();
        }

        public async Task UpdateUserAsync(long id, CreateUserDto dto)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null) throw new Exception("User not found");

            user.FirstName = dto.FirstName;
            user.LastName = dto.LastName;
            user.Phone = dto.Phone;
            // Handle email update + duplicate check here if required, skipped for brevity

            await _userRepository.UpdateAsync(user);
        }

        public async Task DeleteUserAsync(long id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user != null)
            {
                await _userRepository.DeleteAsync(user); // Triggers soft delete in SaveChangesAsync
            }
        }

        public async Task ToggleUserStatusAsync(long id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user != null)
            {
                user.IsActive = !user.IsActive;
                await _userRepository.UpdateAsync(user);
            }
        }

        private UserDto MapToDto(User user)
        {
            return new UserDto
            {
                Id = user.Id,
                TenantId = user.TenantId,
                Tenant = user.Tenant != null ? new TenantSummaryDto 
                {
                    Id = user.Tenant.Id,
                    CompanyName = user.Tenant.CompanyName,
                    TenancyName = user.Tenant.TenancyName
                } : null,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Phone = user.Phone,
                ProfileImage = user.ProfileImage,
                LastLogin = user.LastLogin,
                IsEmailVerified = user.IsEmailVerified,
                IsActive = user.IsActive
            };
        }
    }
}
