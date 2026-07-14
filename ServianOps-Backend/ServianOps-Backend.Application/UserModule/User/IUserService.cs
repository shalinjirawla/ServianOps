using System.Collections.Generic;
using System.Threading.Tasks;
using ServianOps_Backend.Application.Common.DTOs;
using ServianOps_Backend.Application.UserModule.User.UserDto;

namespace ServianOps_Backend.Application.UserModule.User
{
    public interface IUserService
    {
        Task<StandardResponse<UserDetailDto>> CreateUser(CreateUserDto dto, long? tenantId);
        Task<StandardResponse<UserDetailDto>> UpdateUser(long id, UpdateUserDto dto);
        Task<StandardResponse<UserDetailDto>> GetUserById(long id);
        Task<StandardResponse<UserDetailDto>> GetUserByEmail(string email);
        Task<StandardResponse<UserDetailDto>> GetUserByEmailAndTenantId(string email, long? tenantId);
        Task<StandardResponse<bool>> ValidateCredentials(string email, string password, long? tenantId);
        Task<StandardResponse<string>> GetUserRoleName(long userId);
        Task<StandardResponse<PagedResultDto<UserListDto>>> GetAllUsers(UserFilterDto filter);
        Task<StandardResponse<PagedResultDto<UserListDto>>> GetAdministrators(UserFilterDto filter);
        Task<StandardResponse<IReadOnlyList<UserListDto>>> GetTenantAdministrators(long tenantId);
        Task<StandardResponse<bool>> ToggleUserStatus(long id);
        Task<StandardResponse<bool>> DeleteUser(long id);
    }
}
