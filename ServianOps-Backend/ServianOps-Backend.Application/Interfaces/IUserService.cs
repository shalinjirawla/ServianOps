using System.Collections.Generic;
using System.Threading.Tasks;
using ServianOps_Backend.Application.DTOs.User;

namespace ServianOps_Backend.Application.Interfaces
{
    public interface IUserService
    {
        Task<UserDto> CreateUserAsync(CreateUserDto dto, long? tenantId);
        Task<UserDto> GetUserByEmailAsync(string email);
        Task<UserDto> GetUserByEmailAndTenantIdAsync(string email, long? tenantId);
        Task<bool> ValidateCredentialsAsync(string email, string password, long? tenantId);
        Task<UserDto> GetUserByIdAsync(long id);
        Task<string> GetUserRoleNameAsync(long userId);
        Task<IReadOnlyList<UserDto>> GetUsersPagedAsync(int pageNumber, int pageSize);
        Task<IReadOnlyList<UserDto>> GetAdministratorsPagedAsync(int pageNumber, int pageSize);
        Task UpdateUserAsync(long id, CreateUserDto dto); // Reusing CreateDto for simplicity, normally UpdateDto
        Task DeleteUserAsync(long id);
        Task ToggleUserStatusAsync(long id);
    }
}
