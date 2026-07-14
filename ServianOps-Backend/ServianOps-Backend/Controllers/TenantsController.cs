using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServianOps_Backend.Application.Interfaces;
using ServianOps_Backend.Core.Interfaces;
namespace ServianOps_Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class TenantsController : ControllerBase
    {
        private readonly ITenantService _tenantService;
        private readonly ICurrentTenant _currentTenant;

        public TenantsController(ITenantService tenantService, ICurrentTenant currentTenant)
        {
            _tenantService = tenantService;
            _currentTenant = currentTenant;
        }

        // Host endpoint to list all tenants
        [HttpPost("search")]
        public async Task<IActionResult> GetTenants([FromBody] ServianOps_Backend.Application.DTOs.Tenant.TenantFilterDto filter)
        {
            var tenants = await _tenantService.GetTenantsPagedAsync(filter);
            return Ok(tenants);
        }

        // Endpoint for the current tenant to view their own info
        [HttpGet("me")]
        public async Task<IActionResult> GetCurrentTenant()
        {
            // Note: Normally we'd get Tenant by ID using a GetTenantByIdAsync method, but for demo:// Since we don't have GetTenantByIdAsync on the interface right now, we can fetch all and filter // OR ideally add it to the ITenantService. // In a real scenario, this would use a proper GetById.// For now just returning the
             return Ok(new { CurrentTenantId = _currentTenant.TenantId });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetTenantById(long id)
        {
            var tenant = await _tenantService.GetTenantByIdAsync(id);
            if (tenant == null)
            {
                return NotFound();
            }
            return Ok(tenant);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTenant(long id, [FromBody] ServianOps_Backend.Application.DTOs.Tenant.CreateTenantDto dto)
        {
            try
            {
                await _tenantService.UpdateTenantAsync(id, dto);
                return NoContent();
            }
            catch (System.Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTenant(long id)
        {
            try
            {
                await _tenantService.DeleteTenantAsync(id);
                return NoContent();
            }
            catch (System.Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}
