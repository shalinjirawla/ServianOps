using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using ServianOps_Backend.Application.TenantModule.Tenant;
using ServianOps_Backend.Application.TenantModule.Tenant.TenantDto;
using ServianOps_Backend.Application.Interfaces;
using ServianOps_Backend.Application.AuthModule.Auth.AuthDto;
using ServianOps_Backend.Application.UserModule.User;
using ServianOps_Backend.Application.Common.DTOs;
using ServianOps_Backend.Infrastructure.Authentication;

namespace ServianOps_Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly ITenantService _tenantService;
        private readonly IUserService _userService;
        private readonly IJwtProvider _jwtProvider;
        private readonly IUserSessionService _sessionService;
        private readonly JwtSettings _jwtSettings;

        public AuthController(ITenantService tenantService, IUserService userService, IJwtProvider jwtProvider, IUserSessionService sessionService, IOptions<JwtSettings> jwtSettings)
        {
            _tenantService = tenantService;
            _userService = userService;
            _jwtProvider = jwtProvider;
            _sessionService = sessionService;
            _jwtSettings = jwtSettings.Value;
        }

        [HttpPost("login")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(StandardResponse<AuthResponseDto>), 200)]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            long? tenantId = null;

            if (!string.IsNullOrWhiteSpace(dto.TenancyName))
            {
                var tenantResult = await _tenantService.GetByTenancyName(dto.TenancyName);
                if (!tenantResult.Success || tenantResult.Data == null)
                {
                    return Ok(StandardResponse<AuthResponseDto>.Error("Invalid Company Code."));
                }
                tenantId = tenantResult.Data.Id;
            }

            var validationResult = await _userService.ValidateCredentials(dto.Email, dto.Password, tenantId);
            if (!validationResult.Success || !validationResult.Data)
            {
                return Ok(StandardResponse<AuthResponseDto>.Error("Invalid Email or Password."));
            }

            var userResult = await _userService.GetUserByEmailAndTenantId(dto.Email, tenantId);
            var user = userResult.Data;
            
            var role = tenantId == null ? "SuperAdmin" : "User"; 
            if (tenantId != null)
            {
                 var fetchedRoleResult = await _userService.GetUserRoleName(user.Id);
                 if (fetchedRoleResult.Success && !string.IsNullOrEmpty(fetchedRoleResult.Data))
                 {
                     role = fetchedRoleResult.Data;
                 }
            }

            var (accessToken, jti) = _jwtProvider.GenerateToken(user, role);
            
            // Generate Refresh Token
            var refreshTokenBytes = new byte[64];
            using (var rng = System.Security.Cryptography.RandomNumberGenerator.Create())
            {
                rng.GetBytes(refreshTokenBytes);
            }
            var refreshToken = Convert.ToBase64String(refreshTokenBytes);
            
            var accessTokenExpiry = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiryMinutes);
            var refreshTokenExpiry = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpiryDays);

            var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
            var userAgent = Request.Headers["User-Agent"].ToString();

            await _sessionService.CreateSessionAsync(user.Id, tenantId, jti, refreshToken, accessTokenExpiry, refreshTokenExpiry, ipAddress, userAgent);

            SetTokenCookies(accessToken, refreshToken, accessTokenExpiry, refreshTokenExpiry);

            var authResponse = new AuthResponseDto
            {
                UserId = user.Id,
                TenantId = tenantId,
                Email = user.Email,
                Role = role
            };
            
            return Ok(StandardResponse<AuthResponseDto>.Ok(authResponse, "Login successful."));
        }

        [HttpPost("refresh")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(StandardResponse<AuthResponseDto>), 200)]
        public async Task<IActionResult> Refresh()
        {
            var refreshToken = Request.Cookies["RefreshToken"];
            
            if (string.IsNullOrEmpty(refreshToken))
            {
                return Unauthorized(StandardResponse<AuthResponseDto>.Error("No refresh token provided."));
            }

            var session = await _sessionService.GetActiveSessionByRefreshTokenAsync(refreshToken);
            
            if (session == null || session.RefreshTokenExpiry < DateTime.UtcNow)
            {
                // Replay attack detection or expired session
                // We don't know the exact user if session is null (unless we decode the access token)
                // For a replay attack, we would ideally revoke all sessions, but here we just deny access.
                return Unauthorized(StandardResponse<AuthResponseDto>.Error("Invalid or expired refresh token."));
            }

            // Revoke the old session to rotate the refresh token
            await _sessionService.RevokeSessionAsync(session.Jti, "Refreshed");

            // Generate New Tokens
            var userResult = await _userService.GetUserById(session.UserId);
            var user = userResult.Data;
            var role = session.TenantId == null ? "SuperAdmin" : "User";
            if (session.TenantId != null)
            {
                 var fetchedRoleResult = await _userService.GetUserRoleName(user.Id);
                 if (fetchedRoleResult.Success && !string.IsNullOrEmpty(fetchedRoleResult.Data))
                 {
                     role = fetchedRoleResult.Data;
                 }
            }

            var (newAccessToken, newJti) = _jwtProvider.GenerateToken(user, role);
            
            var newRefreshTokenBytes = new byte[64];
            using (var rng = System.Security.Cryptography.RandomNumberGenerator.Create())
            {
                rng.GetBytes(newRefreshTokenBytes);
            }
            var newRefreshToken = Convert.ToBase64String(newRefreshTokenBytes);
            
            var accessTokenExpiry = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiryMinutes);
            var refreshTokenExpiry = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpiryDays);
            var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
            var userAgent = Request.Headers["User-Agent"].ToString();

            await _sessionService.CreateSessionAsync(user.Id, session.TenantId, newJti, newRefreshToken, accessTokenExpiry, refreshTokenExpiry, ipAddress, userAgent);

            SetTokenCookies(newAccessToken, newRefreshToken, accessTokenExpiry, refreshTokenExpiry);

            var authResponse = new AuthResponseDto
            {
                UserId = user.Id,
                TenantId = session.TenantId,
                Email = user.Email,
                Role = role
            };
            
            return Ok(StandardResponse<AuthResponseDto>.Ok(authResponse, "Token refreshed successfully."));
        }

        [HttpPost("logout")]
        [Authorize]
        [ProducesResponseType(typeof(StandardResponse<bool>), 200)]
        public async Task<IActionResult> Logout()
        {
            var jtiClaim = User.FindFirst(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Jti)?.Value;
            if (Guid.TryParse(jtiClaim, out var jti))
            {
                await _sessionService.RevokeSessionAsync(jti, "User Logout");
            }
            
            DeleteTokenCookies();
            return Ok(StandardResponse<bool>.Ok(true, "Logged out successfully."));
        }

        [HttpPost("logout-all")]
        [Authorize]
        [ProducesResponseType(typeof(StandardResponse<bool>), 200)]
        public async Task<IActionResult> LogoutAll()
        {
            var userIdClaim = User.FindFirst("user_id")?.Value;
            if (long.TryParse(userIdClaim, out var userId))
            {
                await _sessionService.RevokeAllSessionsForUserAsync(userId, "Logout All Devices");
            }
            
            DeleteTokenCookies();
            return Ok(StandardResponse<bool>.Ok(true, "Logged out from all devices successfully."));
        }

        [HttpGet("me")]
        [Authorize]
        [ProducesResponseType(typeof(StandardResponse<AuthResponseDto>), 200)]
        public async Task<IActionResult> Me()
        {
            var userIdClaim = User.FindFirst(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub)?.Value 
                              ?? User.FindFirst("user_id")?.Value;
            var tenantIdClaim = User.FindFirst("tenant_id")?.Value;
            var emailClaim = User.FindFirst(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Email)?.Value;
            var roleClaim = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;

            long.TryParse(userIdClaim, out var userId);
            long? tenantId = string.IsNullOrEmpty(tenantIdClaim) ? (long?)null : long.Parse(tenantIdClaim);

            var authResponse = new AuthResponseDto
            {
                UserId = userId,
                TenantId = tenantId,
                Email = emailClaim ?? "",
                Role = roleClaim ?? "User"
            };

            return Ok(StandardResponse<AuthResponseDto>.Ok(authResponse, "User profile retrieved."));
        }

        private void SetTokenCookies(string accessToken, string refreshToken, DateTime accessTokenExpiry, DateTime refreshTokenExpiry)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true, // Must be true for SameSite=None
                SameSite = SameSiteMode.None, // Allows cross-origin (localhost:4200 to localhost:7224) requests to send cookies
                Expires = accessTokenExpiry
            };
            
            Response.Cookies.Append("AccessToken", accessToken, cookieOptions);

            var refreshCookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = refreshTokenExpiry
            };
            
            Response.Cookies.Append("RefreshToken", refreshToken, refreshCookieOptions);
        }

        private void DeleteTokenCookies()
        {
            Response.Cookies.Delete("AccessToken");
            Response.Cookies.Delete("RefreshToken");
        }

        [HttpPost("forgot-password")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(StandardResponse<bool>), 200)]
        public IActionResult ForgotPassword([FromBody] ForgotPasswordDto dto)
        {
            return Ok(StandardResponse<bool>.Ok(true, "If the email and company code match, a reset link will be sent."));
        }

        [HttpPost("reset-password")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(StandardResponse<bool>), 200)]
        public IActionResult ResetPassword([FromBody] ResetPasswordDto dto)
        {
            return Ok(StandardResponse<bool>.Ok(false, "Password reset functionality is under construction."));
        }
    }
}
