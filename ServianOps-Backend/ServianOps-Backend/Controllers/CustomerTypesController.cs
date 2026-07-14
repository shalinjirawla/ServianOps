using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServianOps_Backend.Application.Common.DTOs;
using ServianOps_Backend.Application.CustomerTypeModule.CustomerType;
using ServianOps_Backend.Application.CustomerTypeModule.CustomerType.CustomerTypeDto;

namespace ServianOps_Backend.Controllers
{
    [ApiController]
    [Route("api/customer-type")]
    [Authorize]
    public class CustomerTypesController : ControllerBase
    {
        private readonly ICustomerTypeService _service;

        public CustomerTypesController(ICustomerTypeService service)
        {
            _service = service;
        }

        [HttpGet("get-all-customer-types")]
        [ProducesResponseType(typeof(StandardResponse<PagedResultDto<CustomerTypeListDto>>), 200)]
        public async Task<IActionResult> GetAllCustomerTypes([FromQuery] CustomerTypeFilterDto filter)
        {
            var result = await _service.GetAllCustomerTypes(filter);
            return Ok(result);
        }

        [HttpGet("get-customer-type-lookup")]
        [ProducesResponseType(typeof(StandardResponse<System.Collections.Generic.IReadOnlyList<CustomerTypeLookupDto>>), 200)]
        public async Task<IActionResult> GetCustomerTypeLookup()
        {
            var result = await _service.GetCustomerTypeLookup();
            return Ok(result);
        }

        [HttpGet("get-customer-type-by-id/{id}")]
        [ProducesResponseType(typeof(StandardResponse<CustomerTypeDetailDto>), 200)]
        public async Task<IActionResult> GetCustomerTypeById(long id)
        {
            var result = await _service.GetCustomerTypeById(id);
            if (!result.Success) return NotFound(result);
            return Ok(result);
        }

        [HttpPost("create-customer-type")]
        [ProducesResponseType(typeof(StandardResponse<CustomerTypeDetailDto>), 201)]
        public async Task<IActionResult> CreateCustomerType([FromBody] CreateCustomerTypeDto dto)
        {
            var result = await _service.CreateCustomerType(dto);
            return CreatedAtAction(nameof(GetCustomerTypeById), new { id = result.Data?.Id ?? 0 }, result);
        }

        [HttpPut("update-customer-type/{id}")]
        [ProducesResponseType(typeof(StandardResponse<CustomerTypeDetailDto>), 200)]
        public async Task<IActionResult> UpdateCustomerType(long id, [FromBody] UpdateCustomerTypeDto dto)
        {
            var result = await _service.UpdateCustomerType(id, dto);
            if (!result.Success) return BadRequest(result);
            return Ok(result);
        }

        [HttpDelete("delete-customer-type/{id}")]
        public async Task<IActionResult> DeleteCustomerType(long id)
        {
            var result = await _service.DeleteCustomerType(id);
            if (!result.Success) return BadRequest(result);
            return NoContent();
        }
    }
}
