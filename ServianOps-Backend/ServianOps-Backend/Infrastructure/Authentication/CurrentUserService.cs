using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using ServianOps_Backend.Application.Interfaces;

namespace ServianOps_Backend.Infrastructure.Authentication
{
    public class CurrentUserService : ICurrentUser
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public long? UserId
        {
            get
            {
                var userIdClaim = _httpContextAccessor.HttpContext?.User?.Claims.FirstOrDefault(c => c.Type == "user_id" || c.Type == ClaimTypes.NameIdentifier);
                if (userIdClaim != null && long.TryParse(userIdClaim.Value, out var userId))
                {
                    return userId;
                }
                return null;
            }
        }

        public string Email => _httpContextAccessor.HttpContext?.User?.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email || c.Type == JwtRegisteredClaimNames.Email)?.Value;

        public bool IsAuthenticated => _httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false;
    }
}
