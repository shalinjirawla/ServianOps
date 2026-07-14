using AutoMapper;
using ServianOps_Backend.Application.TradeModule.Trade.TradeDto;
using ServianOps_Backend.Core.Entities.Jobs;

namespace ServianOps_Backend.Application.TradeModule.Trade
{
    public class TradeMappingProfile : Profile
    {
        public TradeMappingProfile()
        {
            CreateMap<Core.Entities.Jobs.Trade, TradeDetailDto>();
            CreateMap<Core.Entities.Jobs.Trade, TradeListDto>();
            CreateMap<Core.Entities.Jobs.Trade, TradeLookupDto>();

            CreateMap<CreateTradeDto, Core.Entities.Jobs.Trade>();
            CreateMap<UpdateTradeDto, Core.Entities.Jobs.Trade>();
        }
    }
}
