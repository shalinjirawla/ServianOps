using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServianOps_Backend.Application.Common.DTOs;
using ServianOps_Backend.Application.TradeModule.Trade;
using ServianOps_Backend.Application.TradeModule.Trade.TradeDto;

namespace ServianOps_Backend.Controllers
{
    [ApiController]
    [Route("api/trade")]
    [Authorize]
    public class TradesController : ControllerBase
    {
        private readonly ITradeService _tradeService;

        public TradesController(ITradeService tradeService)
        {
            _tradeService = tradeService;
        }

        [HttpGet("get-all-trades")]
        [ProducesResponseType(typeof(StandardResponse<PagedResultDto<TradeListDto>>), 200)]
        public async Task<IActionResult> GetAllTrades([FromQuery] TradeFilterDto filter)
        {
            var result = await _tradeService.GetAllTrades(filter);
            return Ok(result);
        }

        [HttpGet("get-trade-lookup")]
        [ProducesResponseType(typeof(StandardResponse<IReadOnlyList<TradeLookupDto>>), 200)]
        public async Task<IActionResult> GetTradeLookup()
        {
            var result = await _tradeService.GetTradeLookup();
            return Ok(result);
        }

        [HttpGet("get-trade-by-id/{id}")]
        [ProducesResponseType(typeof(StandardResponse<TradeDetailDto>), 200)]
        public async Task<IActionResult> GetTradeById(long id)
        {
            var result = await _tradeService.GetTradeById(id);
            if (!result.Success) return NotFound(result);
            return Ok(result);
        }

        [HttpPost("create-trade")]
        [ProducesResponseType(typeof(StandardResponse<TradeDetailDto>), 201)]
        public async Task<IActionResult> CreateTrade([FromBody] CreateTradeDto dto)
        {
            var result = await _tradeService.CreateTrade(dto);
            return CreatedAtAction(nameof(GetTradeById), new { id = result.Data?.Id ?? 0 }, result);
        }

        [HttpPut("update-trade/{id}")]
        [ProducesResponseType(typeof(StandardResponse<TradeDetailDto>), 200)]
        public async Task<IActionResult> UpdateTrade(long id, [FromBody] UpdateTradeDto dto)
        {
            var result = await _tradeService.UpdateTrade(id, dto);
            if (!result.Success) return BadRequest(result);
            return Ok(result);
        }

        [HttpDelete("delete-trade/{id}")]
        public async Task<IActionResult> DeleteTrade(long id)
        {
            var result = await _tradeService.DeleteTrade(id);
            if (!result.Success) return BadRequest(result);
            return NoContent();
        }
    }
}
