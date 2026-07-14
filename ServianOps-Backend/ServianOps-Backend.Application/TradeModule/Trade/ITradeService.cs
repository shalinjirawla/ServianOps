using System.Collections.Generic;
using System.Threading.Tasks;
using ServianOps_Backend.Application.Common.DTOs;
using ServianOps_Backend.Application.TradeModule.Trade.TradeDto;

namespace ServianOps_Backend.Application.TradeModule.Trade
{
    public interface ITradeService
    {
        Task<StandardResponse<TradeDetailDto>> CreateTrade(CreateTradeDto dto);
        Task<StandardResponse<TradeDetailDto>> UpdateTrade(long id, UpdateTradeDto dto);
        Task<StandardResponse<TradeDetailDto>> GetTradeById(long id);
        Task<StandardResponse<PagedResultDto<TradeListDto>>> GetAllTrades(TradeFilterDto filter);
        Task<StandardResponse<IReadOnlyList<TradeLookupDto>>> GetTradeLookup();
        Task<StandardResponse<bool>> DeleteTrade(long id);
    }
}
