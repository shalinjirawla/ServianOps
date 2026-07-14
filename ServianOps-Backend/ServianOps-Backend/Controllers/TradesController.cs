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
    public class TradesController : ControllerBase
    {
        private readonly ITradeService _tradeService;

        public TradesController(ITradeService tradeService)
        {
            _tradeService = tradeService;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateTradeDto dto)
        {
            var result = await _tradeService.CreateAsync(dto);
            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(long id, [FromBody] UpdateTradeDto dto)
        {
            var result = await _tradeService.UpdateAsync(id, dto);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(long id)
        {
            var result = await _tradeService.GetByIdAsync(id);
            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpPost("search")]
        public async Task<IActionResult> GetAll([FromBody] ServianOps_Backend.Application.DTOs.Jobs.TradeFilterDto filter)
        {
            var result = await _tradeService.GetAllPagedAsync(filter);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(long id)
        {
            await _tradeService.DeleteAsync(id);
            return NoContent();
        }
    }
}
