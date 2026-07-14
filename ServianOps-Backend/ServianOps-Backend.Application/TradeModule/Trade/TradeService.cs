using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ServianOps_Backend.Application.Common.DTOs;
using ServianOps_Backend.Application.TradeModule.Trade.TradeDto;
using ServianOps_Backend.Core.Interfaces.Repositories;

namespace ServianOps_Backend.Application.TradeModule.Trade
{
    public class TradeService : ITradeService
    {
        private readonly IGenericRepository<Core.Entities.Jobs.Trade> _repository;
        private readonly IMapper _mapper;

        public TradeService(IGenericRepository<Core.Entities.Jobs.Trade> repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<StandardResponse<TradeDetailDto>> CreateTrade(CreateTradeDto dto)
        {
            var entity = _mapper.Map<Core.Entities.Jobs.Trade>(dto);
            await _repository.AddAsync(entity);
            return StandardResponse<TradeDetailDto>.Ok(_mapper.Map<TradeDetailDto>(entity));
        }

        public async Task<StandardResponse<TradeDetailDto>> UpdateTrade(long id, UpdateTradeDto dto)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null) return StandardResponse<TradeDetailDto>.Error("Trade not found.");

            _mapper.Map(dto, entity);
            await _repository.UpdateAsync(entity);

            return StandardResponse<TradeDetailDto>.Ok(_mapper.Map<TradeDetailDto>(entity));
        }

        public async Task<StandardResponse<TradeDetailDto>> GetTradeById(long id)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null) return StandardResponse<TradeDetailDto>.Error("Trade not found.");
            
            return StandardResponse<TradeDetailDto>.Ok(_mapper.Map<TradeDetailDto>(entity));
        }

        public async Task<StandardResponse<PagedResultDto<TradeListDto>>> GetAllTrades(TradeFilterDto filter)
        {
            var query = _repository.GetQueryable();

            if (!string.IsNullOrWhiteSpace(filter.Search))
            {
                query = query.Where(t => t.Name.Contains(filter.Search));
            }

            var totalCount = await query.CountAsync();
            var items = await query
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToListAsync();

            var result = new PagedResultDto<TradeListDto>(
                _mapper.Map<IReadOnlyList<TradeListDto>>(items),
                totalCount,
                filter.PageNumber,
                filter.PageSize);

            return StandardResponse<PagedResultDto<TradeListDto>>.Ok(result);
        }

        public async Task<StandardResponse<IReadOnlyList<TradeLookupDto>>> GetTradeLookup()
        {
            var items = await _repository.GetAllAsync();
            var result = items.Select(t => new TradeLookupDto
            {
                Id = t.Id,
                Name = t.Name
            }).OrderBy(x => x.Name).ToList();

            return StandardResponse<IReadOnlyList<TradeLookupDto>>.Ok(result);
        }

        public async Task<StandardResponse<bool>> DeleteTrade(long id)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity != null)
            {
                await _repository.DeleteAsync(entity);
                return StandardResponse<bool>.Ok(true, "Trade deleted.");
            }
            return StandardResponse<bool>.Error("Trade not found.");
        }
    }
}
