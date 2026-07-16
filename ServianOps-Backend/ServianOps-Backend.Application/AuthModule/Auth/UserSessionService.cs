using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ServianOps_Backend.Application.Interfaces;
using ServianOps_Backend.Core.Entities.Identity;
using ServianOps_Backend.Core.Interfaces;
using ServianOps_Backend.Core.Interfaces.Repositories;

namespace ServianOps_Backend.Application.AuthModule.Auth
{
    public class UserSessionService : IUserSessionService
    {
        private readonly IGenericRepository<UserSession> _sessionRepository;

        public UserSessionService(IGenericRepository<UserSession> sessionRepository)
        {
            _sessionRepository = sessionRepository;
        }

        public async Task<UserSession> CreateSessionAsync(long userId, long? tenantId, Guid jti, string refreshToken, DateTime accessTokenExpiry, DateTime refreshTokenExpiry, string? ipAddress, string? userAgent)
        {
            var session = new UserSession
            {
                UserId = userId,
                TenantId = tenantId,
                Jti = jti,
                RefreshTokenHash = HashToken(refreshToken),
                AccessTokenExpiry = accessTokenExpiry,
                RefreshTokenExpiry = refreshTokenExpiry,
                IPAddress = ipAddress,
                UserAgent = userAgent,
                LastActivity = DateTime.UtcNow,
                IsRevoked = false
            };

            await _sessionRepository.AddAsync(session);
            return session;
        }

        public async Task<UserSession?> GetActiveSessionByRefreshTokenAsync(string refreshToken)
        {
            var hash = HashToken(refreshToken);
            return await _sessionRepository.GetQueryable()
                .Where(s => s.RefreshTokenHash == hash && !s.IsRevoked)
                .FirstOrDefaultAsync();
        }

        public async Task<bool> IsSessionActiveAsync(Guid jti)
        {
            var session = await _sessionRepository.GetQueryable()
                .Where(s => s.Jti == jti)
                .FirstOrDefaultAsync();
            
            return session != null && !session.IsRevoked && session.AccessTokenExpiry > DateTime.UtcNow;
        }

        public async Task RevokeSessionAsync(Guid jti, string reason)
        {
            var session = await _sessionRepository.GetQueryable()
                .Where(s => s.Jti == jti && !s.IsRevoked)
                .FirstOrDefaultAsync();
            
            if (session != null)
            {
                session.IsRevoked = true;
                session.RevokedAt = DateTime.UtcNow;
                session.Reason = reason;
                await _sessionRepository.UpdateAsync(session);
            }
        }

        public async Task RevokeAllSessionsForUserAsync(long userId, string reason)
        {
            var activeSessions = await _sessionRepository.GetQueryable()
                .Where(s => s.UserId == userId && !s.IsRevoked)
                .ToListAsync();

            foreach (var session in activeSessions)
            {
                session.IsRevoked = true;
                session.RevokedAt = DateTime.UtcNow;
                session.Reason = reason;
                await _sessionRepository.UpdateAsync(session);
            }
        }

        public async Task RevokeSessionByRefreshTokenAsync(string refreshToken, string reason)
        {
            var hash = HashToken(refreshToken);
            var session = await _sessionRepository.GetQueryable()
                .Where(s => s.RefreshTokenHash == hash && !s.IsRevoked)
                .FirstOrDefaultAsync();

            if (session != null)
            {
                session.IsRevoked = true;
                session.RevokedAt = DateTime.UtcNow;
                session.Reason = reason;
                await _sessionRepository.UpdateAsync(session);
            }
        }

        private static string HashToken(string token)
        {
            using var sha256 = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(token);
            var hash = sha256.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }
    }
}
