using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServianOps_Backend.Application.DTOs.Jobs;
using ServianOps_Backend.Application.Interfaces.Jobs;

namespace ServianOps_Backend.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class JobsController : ControllerBase
    {
        private readonly IJobService _jobService;

        public JobsController(IJobService jobService)
        {
            _jobService = jobService;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromForm] CreateJobDto dto)
        {
            var result = await _jobService.CreateAsync(dto);
            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(long id, [FromForm] UpdateJobDto dto)
        {
            var result = await _jobService.UpdateAsync(id, dto);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(long id)
        {
            var result = await _jobService.GetByIdAsync(id);
            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpPost("search")]
        public async Task<IActionResult> GetAll([FromBody] ServianOps_Backend.Application.DTOs.Jobs.JobFilterDto filter)
        {
            var jobs = await _jobService.GetAllPagedAsync(filter);
            return Ok(jobs);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(long id)
        {
            await _jobService.DeleteAsync(id);
            return NoContent();
        }

        [HttpDelete("{jobId}/attachments/{attachmentId}")]
        public async Task<IActionResult> DeleteAttachment(long jobId, long attachmentId)
        {
            await _jobService.DeleteAttachmentAsync(jobId, attachmentId);
            return NoContent();
        }
    }
}
