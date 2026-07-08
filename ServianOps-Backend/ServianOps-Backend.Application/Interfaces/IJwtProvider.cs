using System;
using ServianOps_Backend.Application.DTOs.User;

namespace ServianOps_Backend.Application.Interfaces
{
    public interface IJwtProvider
    {
        string GenerateToken(UserDto user, string roleName);
    }
}
