using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServianOps_Backend.Application.Common.DTOs;
using ServianOps_Backend.Application.JobModule.Job;
using ServianOps_Backend.Application.JobModule.Job.JobDto;

namespace ServianOps_Backend.Controllers
{
    [ApiController]
    [Route("api/job")]
    [Authorize]
    public class JobsController : ControllerBase
    {
        private readonly IJobService _jobService;

        public JobsController(IJobService jobService)
        {
            _jobService = jobService;
        }

        [HttpGet("get-all-jobs")]
        [ProducesResponseType(typeof(StandardResponse<PagedResultDto<JobListDto>>), 200)]
        public async Task<IActionResult> GetAllJobs([FromQuery] JobFilterDto filter)
        {
            var result = await _jobService.GetAllJobs(filter);
            return Ok(result);
        }

        [HttpGet("get-job-by-id/{id}")]
        [ProducesResponseType(typeof(StandardResponse<JobDetailDto>), 200)]
        public async Task<IActionResult> GetJobById(long id)
        {
            var result = await _jobService.GetJobById(id);
            if (!result.Success) return NotFound(result);
            return Ok(result);
        }

        [HttpPost("create-job")]
        [ProducesResponseType(typeof(StandardResponse<JobDetailDto>), 201)]
        public async Task<IActionResult> CreateJob([FromForm] CreateJobDto dto)
        {
            var result = await _jobService.CreateJob(dto);
            return CreatedAtAction(nameof(GetJobById), new { id = result.Data?.Id ?? 0 }, result);
        }

        [HttpPut("update-job/{id}")]
        [ProducesResponseType(typeof(StandardResponse<JobDetailDto>), 200)]
        public async Task<IActionResult> UpdateJob(long id, [FromForm] UpdateJobDto dto)
        {
            var result = await _jobService.UpdateJob(id, dto);
            if (!result.Success) return BadRequest(result);
            return Ok(result);
        }

        [HttpDelete("delete-job/{id}")]
        public async Task<IActionResult> DeleteJob(long id)
        {
            var result = await _jobService.DeleteJob(id);
            if (!result.Success) return BadRequest(result);
            return NoContent();
        }

        [HttpDelete("delete-attachment/{id}/{attachmentId}")]
        public async Task<IActionResult> DeleteAttachment(long id, long attachmentId)
        {
            var result = await _jobService.DeleteAttachment(id, attachmentId);
            if (!result.Success) return BadRequest(result);
            return NoContent();
        }
    }
}
