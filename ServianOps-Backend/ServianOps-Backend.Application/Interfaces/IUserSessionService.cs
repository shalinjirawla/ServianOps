using System;
using System.Threading.Tasks;
using ServianOps_Backend.Core.Entities.Identity;

namespace ServianOps_Backend.Application.Interfaces
{
    public interface IUserSessionService
    {
        Task<UserSession> CreateSessionAsync(long userId, long? tenantId, Guid jti, string refreshToken, DateTime accessTokenExpiry, DateTime refreshTokenExpiry, string? ipAddress, string? userAgent);
        Task<UserSession?> GetActiveSessionByRefreshTokenAsync(string refreshToken);
        Task<bool> IsSessionActiveAsync(Guid jti);
        Task RevokeSessionAsync(Guid jti, string reason);
        Task RevokeAllSessionsForUserAsync(long userId, string reason);
        Task RevokeSessionByRefreshTokenAsync(string refreshToken, string reason);
    }
}
