using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServianOps_Backend.Application.Common.DTOs;
using ServianOps_Backend.Application.TenantModule.Tenant;
using ServianOps_Backend.Application.TenantModule.Tenant.TenantDto;

namespace ServianOps_Backend.Controllers
{
    [ApiController]
    [Route("api/tenant")]
    [Authorize]
    public class TenantsController : ControllerBase
    {
        private readonly ITenantService _service;

        public TenantsController(ITenantService service)
        {
            _service = service;
        }

        [HttpGet("get-all-tenants")]
        [ProducesResponseType(typeof(StandardResponse<PagedResultDto<TenantListDto>>), 200)]
        public async Task<IActionResult> GetAllTenants([FromQuery] TenantFilterDto filter)
        {
            var result = await _service.GetAllTenants(filter);
            return Ok(result);
        }

        [HttpGet("get-tenant-lookup")]
        [ProducesResponseType(typeof(StandardResponse<System.Collections.Generic.IReadOnlyList<TenantLookupDto>>), 200)]
        public async Task<IActionResult> GetTenantLookup()
        {
            var result = await _service.GetTenantLookup();
            return Ok(result);
        }

        [HttpGet("get-tenant-by-id/{id}")]
        [ProducesResponseType(typeof(StandardResponse<TenantDetailDto>), 200)]
        public async Task<IActionResult> GetTenantById(long id)
        {
            var result = await _service.GetTenantById(id);
            if (!result.Success) return NotFound(result);
            return Ok(result);
        }

        [HttpPost("create-tenant")]
        [ProducesResponseType(typeof(StandardResponse<TenantDetailDto>), 201)]
        public async Task<IActionResult> CreateTenant([FromBody] CreateTenantDto dto)
        {
            var result = await _service.CreateTenant(dto);
            return CreatedAtAction(nameof(GetTenantById), new { id = result.Data?.Id ?? 0 }, result);
        }

        [HttpPut("update-tenant/{id}")]
        [ProducesResponseType(typeof(StandardResponse<TenantDetailDto>), 200)]
        public async Task<IActionResult> UpdateTenant(long id, [FromBody] UpdateTenantDto dto)
        {
            var result = await _service.UpdateTenant(id, dto);
            if (!result.Success) return BadRequest(result);
            return Ok(result);
        }

        [HttpDelete("delete-tenant/{id}")]
        public async Task<IActionResult> DeleteTenant(long id)
        {
            var result = await _service.DeleteTenant(id);
            if (!result.Success) return BadRequest(result);
            return NoContent();
        }
        
        [HttpPost("setup-default")]
        [AllowAnonymous]
        public async Task<IActionResult> SetupDefaultTenant()
        {
            var result = await _service.SetupDefaultTenant();
            return Ok(result);
        }
    }
}
