using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServianOps_Backend.Application.Common.DTOs;
using ServianOps_Backend.Application.UserModule.User;
using ServianOps_Backend.Application.UserModule.User.UserDto;
using ServianOps_Backend.Infrastructure.Authentication;

namespace ServianOps_Backend.Controllers
{
    [ApiController]
    [Route("api/user")]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _service;
        public UsersController(IUserService service)
        {
            _service = service;
        }

        [HttpGet("get-all-users")]
        [ProducesResponseType(typeof(StandardResponse<PagedResultDto<UserListDto>>), 200)]
        public async Task<IActionResult> GetAllUsers([FromQuery] UserFilterDto filter)
        {
            var result = await _service.GetAllUsers(filter);
            return Ok(result);
        }

        [HttpGet("get-administrators")]
        [ProducesResponseType(typeof(StandardResponse<PagedResultDto<UserListDto>>), 200)]
        public async Task<IActionResult> GetAdministrators([FromQuery] UserFilterDto filter)
        {
            var result = await _service.GetAdministrators(filter);
            return Ok(result);
        }

        [HttpGet("get-tenant-administrators/{tenantId}")]
        [ProducesResponseType(typeof(StandardResponse<System.Collections.Generic.IReadOnlyList<UserListDto>>), 200)]
        public async Task<IActionResult> GetTenantAdministrators(long tenantId)
        {
            var result = await _service.GetTenantAdministrators(tenantId);
            return Ok(result);
        }

        [HttpGet("get-user-by-id/{id}")]
        [ProducesResponseType(typeof(StandardResponse<UserDetailDto>), 200)]
        public async Task<IActionResult> GetUserById(long id)
        {
            var result = await _service.GetUserById(id);
            if (!result.Success) return NotFound(result);
            return Ok(result);
        }

        [HttpPost("create-user")]
        [ProducesResponseType(typeof(StandardResponse<UserDetailDto>), 201)]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserDto dto)
        {
            var result = await _service.CreateUser(dto, dto.TenantId);
            if (!result.Success) return BadRequest(result);
            return CreatedAtAction(nameof(GetUserById), new { id = result.Data?.Id ?? 0 }, result);
        }

        [HttpPut("update-user/{id}")]
        [ProducesResponseType(typeof(StandardResponse<UserDetailDto>), 200)]
        public async Task<IActionResult> UpdateUser(long id, [FromBody] UpdateUserDto dto)
        {
            var result = await _service.UpdateUser(id, dto);
            if (!result.Success) return BadRequest(result);
            return Ok(result);
        }

        [HttpPut("toggle-user-status/{id}")]
        [ProducesResponseType(typeof(StandardResponse<bool>), 200)]
        public async Task<IActionResult> ToggleUserStatus(long id)
        {
            var result = await _service.ToggleUserStatus(id);
            if (!result.Success) return BadRequest(result);
            return Ok(result);
        }

        [HttpDelete("delete-user/{id}")]
        public async Task<IActionResult> DeleteUser(long id)
        {
            var result = await _service.DeleteUser(id);
            if (!result.Success) return BadRequest(result);
            return NoContent();
        }
    }
}
