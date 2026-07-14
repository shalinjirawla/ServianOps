using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServianOps_Backend.Application.Common.DTOs;
using ServianOps_Backend.Application.RoleModule.Role;
using ServianOps_Backend.Application.RoleModule.Role.RoleDto;

namespace ServianOps_Backend.Controllers
{
    [ApiController]
    [Route("api/role")]
    [Authorize]
    public class RolesController : ControllerBase
    {
        private readonly IRoleService _service;

        public RolesController(IRoleService service)
        {
            _service = service;
        }

        [HttpGet("get-all-roles")]
        [ProducesResponseType(typeof(StandardResponse<PagedResultDto<RoleListDto>>), 200)]
        public async Task<IActionResult> GetAllRoles([FromQuery] RoleFilterDto filter)
        {
            var result = await _service.GetAllRoles(filter);
            return Ok(result);
        }

        [HttpGet("get-role-lookup")]
        [ProducesResponseType(typeof(StandardResponse<System.Collections.Generic.IReadOnlyList<RoleLookupDto>>), 200)]
        public async Task<IActionResult> GetRoleLookup()
        {
            var result = await _service.GetRoleLookup();
            return Ok(result);
        }

        [HttpGet("get-role-by-id/{id}")]
        [ProducesResponseType(typeof(StandardResponse<RoleDetailDto>), 200)]
        public async Task<IActionResult> GetRoleById(long id)
        {
            var result = await _service.GetRoleById(id);
            if (!result.Success) return NotFound(result);
            return Ok(result);
        }

        [HttpPost("create-role")]
        [ProducesResponseType(typeof(StandardResponse<RoleDetailDto>), 201)]
        public async Task<IActionResult> CreateRole([FromBody] CreateRoleDto dto)
        {
            var result = await _service.CreateRole(dto);
            return CreatedAtAction(nameof(GetRoleById), new { id = result.Data?.Id ?? 0 }, result);
        }

        [HttpPut("update-role/{id}")]
        [ProducesResponseType(typeof(StandardResponse<RoleDetailDto>), 200)]
        public async Task<IActionResult> UpdateRole(long id, [FromBody] UpdateRoleDto dto)
        {
            var result = await _service.UpdateRole(id, dto);
            if (!result.Success) return BadRequest(result);
            return Ok(result);
        }

        [HttpDelete("delete-role/{id}")]
        public async Task<IActionResult> DeleteRole(long id)
        {
            var result = await _service.DeleteRole(id);
            if (!result.Success) return BadRequest(result);
            return NoContent();
        }
    }
}
