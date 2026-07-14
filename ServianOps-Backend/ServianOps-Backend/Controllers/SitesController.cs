using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServianOps_Backend.Application.Common.DTOs;
using ServianOps_Backend.Application.SiteModule.Site;
using ServianOps_Backend.Application.SiteModule.Site.SiteDto;

namespace ServianOps_Backend.Controllers
{
    [ApiController]
    [Route("api/site")]
    [Authorize]
    public class SitesController : ControllerBase
    {
        private readonly ISiteService _siteService;

        public SitesController(ISiteService siteService)
        {
            _siteService = siteService;
        }

        [HttpGet("get-all-sites")]
        [ProducesResponseType(typeof(StandardResponse<PagedResultDto<SiteListDto>>), 200)]
        public async Task<IActionResult> GetAllSites([FromQuery] SiteFilterDto filter)
        {
            var result = await _siteService.GetAllSites(filter);
            return Ok(result);
        }

        [HttpGet("get-site-lookup")]
        [ProducesResponseType(typeof(StandardResponse<IReadOnlyList<SiteLookupDto>>), 200)]
        public async Task<IActionResult> GetSiteLookup([FromQuery] long? customerId)
        {
            var result = await _siteService.GetSiteLookup(customerId);
            return Ok(result);
        }

        [HttpGet("get-site-by-id/{id}")]
        [ProducesResponseType(typeof(StandardResponse<SiteDetailDto>), 200)]
        public async Task<IActionResult> GetSiteById(long id)
        {
            var result = await _siteService.GetSiteById(id);
            if (!result.Success) return NotFound(result);
            return Ok(result);
        }

        [HttpPost("create-site")]
        [ProducesResponseType(typeof(StandardResponse<SiteDetailDto>), 201)]
        public async Task<IActionResult> CreateSite([FromBody] CreateSiteDto dto)
        {
            var result = await _siteService.CreateSite(dto);
            return CreatedAtAction(nameof(GetSiteById), new { id = result.Data?.Id ?? 0 }, result);
        }

        [HttpPut("update-site/{id}")]
        [ProducesResponseType(typeof(StandardResponse<SiteDetailDto>), 200)]
        public async Task<IActionResult> UpdateSite(long id, [FromBody] UpdateSiteDto dto)
        {
            var result = await _siteService.UpdateSite(id, dto);
            if (!result.Success) return BadRequest(result);
            return Ok(result);
        }

        [HttpDelete("delete-site/{id}")]
        public async Task<IActionResult> DeleteSite(long id)
        {
            var result = await _siteService.DeleteSite(id);
            if (!result.Success) return BadRequest(result);
            return NoContent();
        }
    }
}
