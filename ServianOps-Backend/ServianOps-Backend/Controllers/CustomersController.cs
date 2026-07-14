using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServianOps_Backend.Application.Common.DTOs;
using ServianOps_Backend.Application.CustomerModule.Customer;
using ServianOps_Backend.Application.CustomerModule.Customer.CustomerDto;

namespace ServianOps_Backend.Controllers
{
    [ApiController]
    [Route("api/customer")]
    [Authorize]
    public class CustomersController : ControllerBase
    {
        private readonly ICustomerService _service;

        public CustomersController(ICustomerService service)
        {
            _service = service;
        }

        [HttpGet("get-all-customers")]
        [ProducesResponseType(typeof(StandardResponse<PagedResultDto<CustomerListDto>>), 200)]
        public async Task<IActionResult> GetAllCustomers([FromQuery] CustomerFilterDto filter)
        {
            var result = await _service.GetAllCustomers(filter);
            return Ok(result);
        }

        [HttpGet("get-customer-lookup")]
        [ProducesResponseType(typeof(StandardResponse<System.Collections.Generic.IReadOnlyList<CustomerLookupDto>>), 200)]
        public async Task<IActionResult> GetCustomerLookup()
        {
            var result = await _service.GetCustomerLookup();
            return Ok(result);
        }

        [HttpGet("get-customer-by-id/{id}")]
        [ProducesResponseType(typeof(StandardResponse<CustomerDetailDto>), 200)]
        public async Task<IActionResult> GetCustomerById(long id)
        {
            var result = await _service.GetCustomerById(id);
            if (!result.Success) return NotFound(result);
            return Ok(result);
        }

        [HttpPost("create-customer")]
        [ProducesResponseType(typeof(StandardResponse<CustomerDetailDto>), 201)]
        public async Task<IActionResult> CreateCustomer([FromBody] CreateCustomerDto dto)
        {
            var result = await _service.CreateCustomer(dto);
            return CreatedAtAction(nameof(GetCustomerById), new { id = result.Data?.Id ?? 0 }, result);
        }

        [HttpPut("update-customer/{id}")]
        [ProducesResponseType(typeof(StandardResponse<CustomerDetailDto>), 200)]
        public async Task<IActionResult> UpdateCustomer(long id, [FromBody] UpdateCustomerDto dto)
        {
            var result = await _service.UpdateCustomer(id, dto);
            if (!result.Success) return BadRequest(result);
            return Ok(result);
        }

        [HttpDelete("delete-customer/{id}")]
        public async Task<IActionResult> DeleteCustomer(long id)
        {
            var result = await _service.DeleteCustomer(id);
            if (!result.Success) return BadRequest(result);
            return NoContent();
        }
    }
}
