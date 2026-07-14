using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
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
        private readonly AutoMapper.IMapper _mapper;

        public UserService(IUserRepository userRepository, IPasswordHasher passwordHasher, AutoMapper.IMapper mapper)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _mapper = mapper;
        }

        public async Task<UserDto> CreateUserAsync(CreateUserDto dto, long? tenantId)
        {
            var existingUser = await _userRepository.GetByEmailAsync(dto.Email);
            if (existingUser != null)
            {
                throw new Exception($"Email '{dto.Email}' is already registered for this tenant.");
            }

            var hash = _passwordHasher.HashPassword(dto.Password, out var salt);

            var user = _mapper.Map<User>(dto);
            user.TenantId = tenantId;
            user.PasswordHash = hash;
            user.PasswordSalt = salt;
            user.IsActive = true;
            user.ProfileImage = string.Empty;

            await _userRepository.AddAsync(user);

            var createdUser = await _userRepository.GetQueryable().Include(u => u.Tenant).FirstOrDefaultAsync(u => u.Id == user.Id);
            return _mapper.Map<UserDto>(createdUser);
        }

        public async Task<UserDto> GetUserByEmailAsync(string email)
        {
            var user = await _userRepository.GetQueryable().Include(u => u.Tenant).FirstOrDefaultAsync(u => u.Email == email);
            return user == null ? null : _mapper.Map<UserDto>(user);
        }

        public async Task<UserDto> GetUserByEmailAndTenantIdAsync(string email, long? tenantId)
        {
            var user = await _userRepository.GetQueryable().Include(u => u.Tenant).FirstOrDefaultAsync(u => u.Email == email && u.TenantId == tenantId);
            return user == null ? null : _mapper.Map<UserDto>(user);
        }

        public async Task<bool> ValidateCredentialsAsync(string email, string password, long? tenantId)
        {
            var user = await _userRepository.GetByEmailAndTenantIdAsync(email, tenantId);
            if (user == null) return false;
            
            return _passwordHasher.VerifyPassword(password, user.PasswordHash, user.PasswordSalt);
        }

        public async Task<UserDto> GetUserByIdAsync(long id)
        {
            var user = await _userRepository.GetQueryable().Include(u => u.Tenant).FirstOrDefaultAsync(u => u.Id == id);
            return user == null ? null : _mapper.Map<UserDto>(user);
        }

        public async Task<string> GetUserRoleNameAsync(long userId)
        {
            return await _userRepository.GetUserRoleNameAsync(userId);
        }

        public async Task<ServianOps_Backend.Application.DTOs.Shared.PagedResponseDto<UserDto>> GetUsersPagedAsync(UserFilterDto filter)
        {
            var query = _userRepository.GetQueryable();

            if (filter.TenantId.HasValue)
            {
                query = query.Where(u => u.TenantId == filter.TenantId.Value);
            }

            if (filter.IsActive.HasValue)
            {
                query = query.Where(u => u.IsActive == filter.IsActive.Value);
            }

            if (!string.IsNullOrWhiteSpace(filter.Search))
            {
                query = query.Where(u => u.FirstName.Contains(filter.Search) || u.LastName.Contains(filter.Search) || u.Email.Contains(filter.Search));
            }

            var totalCount = await query.CountAsync();
            var items = await query
                .Include(u => u.Tenant)
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToListAsync();

            return new ServianOps_Backend.Application.DTOs.Shared.PagedResponseDto<UserDto>
            {
                TotalCount = totalCount,
                PageNumber = filter.PageNumber,
                PageSize = filter.PageSize,
                Items = _mapper.Map<List<UserDto>>(items)
            };
        }

        public async Task<ServianOps_Backend.Application.DTOs.Shared.PagedResponseDto<UserDto>> GetAdministratorsPagedAsync(UserFilterDto filter)
        {
            var query = _userRepository.GetQueryable()
                .Where(u => u.UserRoles.Any(ur => ur.Role.Name == "Administrator"));

            if (filter.IsActive.HasValue)
            {
                query = query.Where(u => u.IsActive == filter.IsActive.Value);
            }

            if (!string.IsNullOrWhiteSpace(filter.Search))
            {
                query = query.Where(u => u.FirstName.Contains(filter.Search) || u.LastName.Contains(filter.Search) || u.Email.Contains(filter.Search));
            }

            var totalCount = await query.CountAsync();
            var items = await query
                .Include(u => u.Tenant)
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToListAsync();

            return new ServianOps_Backend.Application.DTOs.Shared.PagedResponseDto<UserDto>
            {
                TotalCount = totalCount,
                PageNumber = filter.PageNumber,
                PageSize = filter.PageSize,
                Items = _mapper.Map<List<UserDto>>(items)
            };
        }

        public async Task<IReadOnlyList<UserDto>> GetTenantAdministratorsAsync(long tenantId)
        {
            var users = await _userRepository.GetTenantAdministratorsAsync(tenantId);
            return _mapper.Map<List<UserDto>>(users);
        }

        public async Task UpdateUserAsync(long id, CreateUserDto dto)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null) throw new Exception("User not found");

            _mapper.Map(dto, user);
            await _userRepository.UpdateAsync(user);
        }

        public async Task DeleteUserAsync(long id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user != null)
            {
                await _userRepository.DeleteAsync(user);
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
    }
}
