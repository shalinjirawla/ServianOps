using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServianOps_Backend.Application.DTOs.User;
using ServianOps_Backend.Application.Interfaces;
using ServianOps_Backend.Core.Interfaces;

namespace ServianOps_Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ICurrentTenant _currentTenant;

        public UsersController(IUserService userService, ICurrentTenant currentTenant)
        {
            _userService = userService;
            _currentTenant = currentTenant;
        }

        [HttpPost("search")]
        public async Task<IActionResult> GetUsers([FromBody] UserFilterDto filter)
        {
            if (User.IsInRole("SuperAdmin"))
            {
                var admins = await _userService.GetAdministratorsPagedAsync(filter);
                return Ok(admins);
            }

            var users = await _userService.GetUsersPagedAsync(filter);
            return Ok(users);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(long id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null) return NotFound();
            return Ok(user);
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserDto dto)
        {
            try
            {
                // Ensure a tenant id is present (usually extracted via Middleware)
                if (!_currentTenant.TenantId.HasValue) return Forbid("No Tenant context found.");
                
                var user = await _userService.CreateUserAsync(dto, _currentTenant.TenantId.Value);
                return CreatedAtAction(nameof(GetUserById), new { id = user.Id }, user);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(long id, [FromBody] CreateUserDto dto)
        {
            try
            {
                await _userService.UpdateUserAsync(id, dto);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(long id)
        {
            await _userService.DeleteUserAsync(id);
            return NoContent();
        }

        [HttpPatch("{id}/toggle-status")]
        public async Task<IActionResult> ToggleUserStatus(long id)
        {
            await _userService.ToggleUserStatusAsync(id);
            return NoContent();
        }
    }
}
