using System;
using ServianOps_Backend.Application.UserModule.User.UserDto;

namespace ServianOps_Backend.Application.Interfaces
{
    public interface IJwtProvider
    {
        string GenerateToken(UserDetailDto user, string roleName);
    }
}
