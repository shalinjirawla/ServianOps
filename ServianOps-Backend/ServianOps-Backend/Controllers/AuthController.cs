using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServianOps_Backend.Application.DTOs.Auth;
using ServianOps_Backend.Application.DTOs.Tenant;
using ServianOps_Backend.Application.Interfaces;

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
        public async Task<IActionResult> RegisterTenant([FromBody] CreateTenantDto dto)
        {
            try
            {
                var tenant = await _tenantService.CreateTenantAsync(dto);
                return Ok(new { message = "Tenant registered successfully.", tenantId = tenant.Id });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            long? tenantId = null;

            // 1. Resolve Tenant if TenancyName is provided
            if (!string.IsNullOrWhiteSpace(dto.TenancyName))
            {
                var tenant = await _tenantService.GetByTenancyNameAsync(dto.TenancyName);
                if (tenant == null)
                {
                    return Unauthorized(new { error = "Invalid Company Code." });
                }
                tenantId = tenant.Id;
            }

            // 2. Validate Credentials
            var isValid = await _userService.ValidateCredentialsAsync(dto.Email, dto.Password, tenantId);
            if (!isValid)
            {
                return Unauthorized(new { error = "Invalid Email or Password." });
            }

            // 3. Generate Token
            var user = await _userService.GetUserByEmailAndTenantIdAsync(dto.Email, tenantId);
            
            // 4. Fetch Role
            var role = "User"; 
            if (tenantId == null)
            {
                role = "SuperAdmin";
            }
            else 
            {
                 var fetchedRole = await _userService.GetUserRoleNameAsync(user.Id);
                 if (!string.IsNullOrEmpty(fetchedRole))
                 {
                     role = fetchedRole;
                 }
            }

            var token = _jwtProvider.GenerateToken(user, role);

            return Ok(new AuthResponseDto
            {
                Token = token,
                UserId = user.Id,
                TenantId = tenantId,
                Email = user.Email,
                Role = role
            });
        }

        [HttpPost("logout")]
        [Authorize]
        public IActionResult Logout()
        {
            // For pure JWT, logout is handled client-side by destroying the token.
            // If tracking refresh tokens, we would invalidate it in DB here.
            return Ok(new { message = "Logged out successfully." });
        }

        [HttpPost("forgot-password")]
        [AllowAnonymous]
        public IActionResult ForgotPassword([FromBody] ForgotPasswordDto dto)
        {
            // Placeholder for email sending logic
            return Ok(new { message = "If the email and company code match, a reset link will be sent." });
        }

        [HttpPost("reset-password")]
        [AllowAnonymous]
        public IActionResult ResetPassword([FromBody] ResetPasswordDto dto)
        {
            // Placeholder for password reset logic
            return Ok(new { message = "Password reset functionality is under construction." });
        }
    }
}
