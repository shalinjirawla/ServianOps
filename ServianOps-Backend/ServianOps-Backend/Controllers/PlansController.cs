using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServianOps_Backend.Application.Common.DTOs;
using ServianOps_Backend.Application.PlanModule.Plan;
using ServianOps_Backend.Application.PlanModule.Plan.PlanDto;

namespace ServianOps_Backend.Controllers
{
    [ApiController]
    [Route("api/plan")]
    [Authorize]
    public class PlansController : ControllerBase
    {
        private readonly IPlanService _service;

        public PlansController(IPlanService service)
        {
            _service = service;
        }

        [HttpGet("get-all-plans")]
        [ProducesResponseType(typeof(StandardResponse<PagedResultDto<PlanListDto>>), 200)]
        public async Task<IActionResult> GetAllPlans([FromQuery] PlanFilterDto filter)
        {
            var result = await _service.GetAllPlans(filter);
            return Ok(result);
        }

        [HttpGet("get-plan-lookup")]
        [ProducesResponseType(typeof(StandardResponse<System.Collections.Generic.IReadOnlyList<PlanLookupDto>>), 200)]
        public async Task<IActionResult> GetPlanLookup()
        {
            var result = await _service.GetPlanLookup();
            return Ok(result);
        }

        [HttpGet("get-plan-by-id/{id}")]
        [ProducesResponseType(typeof(StandardResponse<PlanDetailDto>), 200)]
        public async Task<IActionResult> GetPlanById(long id)
        {
            var result = await _service.GetPlanById(id);
            if (!result.Success) return NotFound(result);
            return Ok(result);
        }

        [HttpPost("create-plan")]
        [ProducesResponseType(typeof(StandardResponse<PlanDetailDto>), 201)]
        public async Task<IActionResult> CreatePlan([FromBody] CreatePlanDto dto)
        {
            var result = await _service.CreatePlan(dto);
            return CreatedAtAction(nameof(GetPlanById), new { id = result.Data?.Id ?? 0 }, result);
        }

        [HttpPut("update-plan/{id}")]
        [ProducesResponseType(typeof(StandardResponse<PlanDetailDto>), 200)]
        public async Task<IActionResult> UpdatePlan(long id, [FromBody] UpdatePlanDto dto)
        {
            var result = await _service.UpdatePlan(id, dto);
            if (!result.Success) return BadRequest(result);
            return Ok(result);
        }

        [HttpDelete("delete-plan/{id}")]
        public async Task<IActionResult> DeletePlan(long id)
        {
            var result = await _service.DeletePlan(id);
            if (!result.Success) return BadRequest(result);
            return NoContent();
        }
    }
}
