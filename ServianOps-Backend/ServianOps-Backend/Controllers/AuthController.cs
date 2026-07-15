using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServianOps_Backend.Application.TenantModule.Tenant;
using ServianOps_Backend.Application.TenantModule.Tenant.TenantDto;
using ServianOps_Backend.Application.Interfaces;
using ServianOps_Backend.Application.AuthModule.Auth.AuthDto;
using ServianOps_Backend.Application.UserModule.User;
using ServianOps_Backend.Application.Common.DTOs;

namespace ServianOps_Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly ITenantService _tenantService;
        private readonly IUserService _userService;
        private readonly IJwtProvider _jwtProvider;

        public AuthController(ITenantService tenantService, IUserService userService, IJwtProvider jwtProvider)
        {
            _tenantService = tenantService;
            _userService = userService;
            _jwtProvider = jwtProvider;
        }

        [HttpPost("register-tenant")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(StandardResponse<long>), 200)]
        public async Task<IActionResult> RegisterTenant([FromBody] CreateTenantDto dto)
        {
            try
            {
                var tenantResult = await _tenantService.CreateTenant(dto);
                if (!tenantResult.Success) return Ok(StandardResponse<long>.Error(tenantResult.Message));
                return Ok(StandardResponse<long>.Ok(tenantResult.Data.Id, "Tenant registered successfully."));
            }
            catch (Exception ex)
            {
                return Ok(StandardResponse<long>.Error(ex.Message));
            }
        }

        [HttpPost("login")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(StandardResponse<AuthResponseDto>), 200)]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            long? tenantId = null;

            // 1. Resolve Tenant if TenancyName is provided
            if (!string.IsNullOrWhiteSpace(dto.TenancyName))
            {
                var tenantResult = await _tenantService.GetByTenancyName(dto.TenancyName);
                if (!tenantResult.Success || tenantResult.Data == null)
                {
                    return Ok(StandardResponse<AuthResponseDto>.Error("Invalid Company Code."));
                }
                tenantId = tenantResult.Data.Id;
            }

            // 2. Validate Credentials
            var validationResult = await _userService.ValidateCredentials(dto.Email, dto.Password, tenantId);
            if (!validationResult.Success || !validationResult.Data)
            {
                return Ok(StandardResponse<AuthResponseDto>.Error("Invalid Email or Password."));
            }

            // 3. Generate Token
            var userResult = await _userService.GetUserByEmailAndTenantId(dto.Email, tenantId);
            var user = userResult.Data;
            
            // 4. Fetch Role
            var role = "User"; 
            if (tenantId == null)
            {
                role = "SuperAdmin";
            }
            else 
            {
                 var fetchedRoleResult = await _userService.GetUserRoleName(user.Id);
                 if (fetchedRoleResult.Success && !string.IsNullOrEmpty(fetchedRoleResult.Data))
                 {
                     role = fetchedRoleResult.Data;
                 }
            }

            var token = _jwtProvider.GenerateToken(user, role);

            var authResponse = new AuthResponseDto
            {
                Token = token,
                UserId = user.Id,
                TenantId = tenantId,
                Email = user.Email,
                Role = role
            };
            return Ok(StandardResponse<AuthResponseDto>.Ok(authResponse));
        }

        [HttpPost("logout")]
        [Authorize]
        [ProducesResponseType(typeof(StandardResponse<bool>), 200)]
        public IActionResult Logout()
        {
            // For pure JWT, logout is handled client-side by destroying the token.
            // If tracking refresh tokens, we would invalidate it in DB here.
            return Ok(StandardResponse<bool>.Ok(true, "Logged out successfully."));
        }

        [HttpPost("refresh")]
        [Authorize]
        [ProducesResponseType(typeof(StandardResponse<AuthResponseDto>), 200)]
        public async Task<IActionResult> Refresh()
        {
            try
            {
                var userIdClaim = User.FindFirst("user_id")?.Value;
                var tenantIdClaim = User.FindFirst("tenant_id")?.Value;

                if (string.IsNullOrEmpty(userIdClaim))
                {
                    return Ok(StandardResponse<AuthResponseDto>.Error("User unauthorized or invalid session."));
                }

                long userId = long.Parse(userIdClaim);
                long? tenantId = string.IsNullOrEmpty(tenantIdClaim) || tenantIdClaim == "null" || tenantIdClaim == ""
                    ? (long?)null 
                    : long.Parse(tenantIdClaim);

                var userResult = await _userService.GetUserById(userId);
                if (!userResult.Success || userResult.Data == null)
                {
                    return Ok(StandardResponse<AuthResponseDto>.Error("User not found or inactive."));
                }

                var user = userResult.Data;

                var role = "User";
                if (tenantId == null)
                {
                    role = "SuperAdmin";
                }
                else
                {
                     var fetchedRoleResult = await _userService.GetUserRoleName(user.Id);
                     if (fetchedRoleResult.Success && !string.IsNullOrEmpty(fetchedRoleResult.Data))
                     {
                         role = fetchedRoleResult.Data;
                     }
                }

                var token = _jwtProvider.GenerateToken(user, role);

                var authResponse = new AuthResponseDto
                {
                    Token = token,
                    UserId = user.Id,
                    TenantId = tenantId,
                    Email = user.Email,
                    Role = role
                };
                return Ok(StandardResponse<AuthResponseDto>.Ok(authResponse, "Token refreshed successfully."));
            }
            catch (Exception ex)
            {
                return Ok(StandardResponse<AuthResponseDto>.Error($"Refresh failed: {ex.Message}"));
            }
        }

        [HttpPost("forgot-password")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(StandardResponse<bool>), 200)]
        public IActionResult ForgotPassword([FromBody] ForgotPasswordDto dto)
        {
            // Placeholder for email sending logic
            return Ok(StandardResponse<bool>.Ok(true, "If the email and company code match, a reset link will be sent."));
        }

        [HttpPost("reset-password")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(StandardResponse<bool>), 200)]
        public IActionResult ResetPassword([FromBody] ResetPasswordDto dto)
        {
            // Placeholder for password reset logic
            return Ok(StandardResponse<bool>.Ok(false, "Password reset functionality is under construction."));
        }
    }
}
