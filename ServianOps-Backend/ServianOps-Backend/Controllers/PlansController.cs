using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServianOps_Backend.Application.Interfaces;

namespace ServianOps_Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // Should ideally be [Authorize(Roles = "SuperAdmin")]
    public class PlansController : ControllerBase
    {
        private readonly IPlanService _planService;

        public PlansController(IPlanService planService)
        {
            _planService = planService;
        }

        [HttpGet]
        public async Task<IActionResult> GetPlans()
        {
            var plans = await _planService.GetPlansAsync();
            return Ok(plans);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetPlanById(long id)
        {
            var plan = await _planService.GetPlanByIdAsync(id);
            if (plan == null) return NotFound();
            return Ok(plan);
        }

        [HttpPost]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> CreatePlan([FromBody] ServianOps_Backend.Application.DTOs.Plan.CreatePlanDto dto)
        {
            try
            {
                var plan = await _planService.CreatePlanAsync(dto);
                return CreatedAtAction(nameof(GetPlanById), new { id = plan.Id }, plan);
            }
            catch (System.Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> UpdatePlan(long id, [FromBody] ServianOps_Backend.Application.DTOs.Plan.CreatePlanDto dto)
        {
            try
            {
                await _planService.UpdatePlanAsync(id, dto);
                return NoContent();
            }
            catch (System.Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> DeletePlan(long id)
        {
            await _planService.DeletePlanAsync(id);
            return NoContent();
        }
    }
}
