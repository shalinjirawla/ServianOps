using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ServianOps_Backend.Application.Common.DTOs;
using ServianOps_Backend.Application.Interfaces;
using ServianOps_Backend.Application.UserModule.User.UserDto;
using ServianOps_Backend.Core.Interfaces.Repositories;
using ServianOps_Backend.Core.Entities.Identity;

namespace ServianOps_Backend.Application.UserModule.User
{
    public class UserService : IUserService
    {
        private readonly IGenericRepository<Core.Entities.Identity.User> _userRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IMapper _mapper;

        public UserService(IGenericRepository<Core.Entities.Identity.User> userRepository, IPasswordHasher passwordHasher, IMapper mapper)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _mapper = mapper;
        }

        public async Task<StandardResponse<UserDetailDto>> CreateUser(CreateUserDto dto, long? tenantId)
        {
            var existingUser = await _userRepository.GetQueryable().FirstOrDefaultAsync(u => u.Email == dto.Email);
            if (existingUser != null)
            {
                return StandardResponse<UserDetailDto>.Error($"Email '{dto.Email}' is already registered for this tenant.");
            }

            var hash = _passwordHasher.HashPassword(dto.Password, out var salt);

            var user = _mapper.Map<Core.Entities.Identity.User>(dto);
            user.TenantId = tenantId;
            user.PasswordHash = hash;
            user.PasswordSalt = salt;
            user.ProfileImage = string.Empty;

            if (dto.RoleIds != null && dto.RoleIds.Any())
            {
                user.UserRoles = dto.RoleIds.Select(roleId => new UserRole { RoleId = roleId }).ToList();
            }

            await _userRepository.AddAsync(user);

            var createdUser = await _userRepository.GetQueryable()
                .Include(u => u.Tenant)
                .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(u => u.Id == user.Id);
                
            return StandardResponse<UserDetailDto>.Ok(_mapper.Map<UserDetailDto>(createdUser));
        }

        public async Task<StandardResponse<UserDetailDto>> UpdateUser(long id, UpdateUserDto dto)
        {
            var user = await _userRepository.GetQueryable()
                .Include(u => u.UserRoles)
                .FirstOrDefaultAsync(u => u.Id == id);
                
            if (user == null) return StandardResponse<UserDetailDto>.Error("User not found.");

            _mapper.Map(dto, user);

            if (dto.RoleIds != null)
            {
                user.UserRoles.Clear();
                foreach (var roleId in dto.RoleIds)
                {
                    user.UserRoles.Add(new UserRole { RoleId = roleId });
                }
            }

            await _userRepository.UpdateAsync(user);

            var updatedUser = await _userRepository.GetQueryable()
                .Include(u => u.Tenant)
                .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(u => u.Id == user.Id);

            return StandardResponse<UserDetailDto>.Ok(_mapper.Map<UserDetailDto>(updatedUser));
        }

        public async Task<StandardResponse<UserDetailDto>> GetUserById(long id)
        {
            var user = await _userRepository.GetQueryable()
                .Include(u => u.Tenant)
                .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(u => u.Id == id);
                
            if (user == null) return StandardResponse<UserDetailDto>.Error("User not found.");
            
            return StandardResponse<UserDetailDto>.Ok(_mapper.Map<UserDetailDto>(user));
        }

        public async Task<StandardResponse<UserDetailDto>> GetUserByEmail(string email)
        {
            var user = await _userRepository.GetQueryable()
                .Include(u => u.Tenant)
                .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(u => u.Email == email);
                
            if (user == null) return StandardResponse<UserDetailDto>.Error("User not found.");
            
            return StandardResponse<UserDetailDto>.Ok(_mapper.Map<UserDetailDto>(user));
        }

        public async Task<StandardResponse<UserDetailDto>> GetUserByEmailAndTenantId(string email, long? tenantId)
        {
            var user = await _userRepository.GetQueryable()
                .Include(u => u.Tenant)
                .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(u => u.Email == email && u.TenantId == tenantId);
                
            if (user == null) return StandardResponse<UserDetailDto>.Error("User not found.");
            
            return StandardResponse<UserDetailDto>.Ok(_mapper.Map<UserDetailDto>(user));
        }

        public async Task<StandardResponse<bool>> ValidateCredentials(string email, string password, long? tenantId)
        {
            var user = await _userRepository.GetQueryable().FirstOrDefaultAsync(u => u.Email == email && u.TenantId == tenantId);
            if (user == null) return StandardResponse<bool>.Ok(false);
            
            var isValid = _passwordHasher.VerifyPassword(password, user.PasswordHash, user.PasswordSalt);
            return StandardResponse<bool>.Ok(isValid);
        }

        public async Task<StandardResponse<string>> GetUserRoleName(long userId)
        {
            var user = await _userRepository.GetQueryable()
                .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(u => u.Id == userId);
            
            var role = user?.UserRoles?.FirstOrDefault()?.Role?.Name ?? "User";
            return StandardResponse<string>.Ok(role);
        }

        public async Task<StandardResponse<PagedResultDto<UserListDto>>> GetAllUsers(UserFilterDto filter)
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

            if (filter.RoleId.HasValue)
            {
                query = query.Where(u => u.UserRoles.Any(ur => ur.RoleId == filter.RoleId.Value));
            }

            if (!string.IsNullOrWhiteSpace(filter.Search))
            {
                query = query.Where(u => u.FirstName.Contains(filter.Search) || u.LastName.Contains(filter.Search) || u.Email.Contains(filter.Search));
            }

            var totalCount = await query.CountAsync();
            var items = await query
                .Include(u => u.Tenant)
                .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToListAsync();

            var result = new PagedResultDto<UserListDto>(
                _mapper.Map<IReadOnlyList<UserListDto>>(items),
                totalCount,
                filter.PageNumber,
                filter.PageSize);

            return StandardResponse<PagedResultDto<UserListDto>>.Ok(result);
        }

        public async Task<StandardResponse<PagedResultDto<UserListDto>>> GetAdministrators(UserFilterDto filter)
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
                .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToListAsync();

            var result = new PagedResultDto<UserListDto>(
                _mapper.Map<IReadOnlyList<UserListDto>>(items),
                totalCount,
                filter.PageNumber,
                filter.PageSize);

            return StandardResponse<PagedResultDto<UserListDto>>.Ok(result);
        }

        public async Task<StandardResponse<IReadOnlyList<UserListDto>>> GetTenantAdministrators(long tenantId)
        {
            var users = await _userRepository.GetQueryable()
                .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                .Where(u => u.TenantId == tenantId && u.UserRoles.Any(ur => ur.Role.Name == "Administrator"))
                .ToListAsync();
                
            return StandardResponse<IReadOnlyList<UserListDto>>.Ok(_mapper.Map<IReadOnlyList<UserListDto>>(users));
        }

        public async Task<StandardResponse<bool>> ToggleUserStatus(long id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user != null)
            {
                user.IsActive = !user.IsActive;
                await _userRepository.UpdateAsync(user);
                return StandardResponse<bool>.Ok(true, "User status toggled.");
            }
            return StandardResponse<bool>.Error("User not found.");
        }

        public async Task<StandardResponse<bool>> DeleteUser(long id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user != null)
            {
                await _userRepository.DeleteAsync(user);
                return StandardResponse<bool>.Ok(true, "User deleted.");
            }
            return StandardResponse<bool>.Error("User not found.");
        }
    }
}
