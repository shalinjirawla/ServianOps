using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ServianOps_Backend.Application.DTOs.Jobs;
using ServianOps_Backend.Application.Interfaces.Jobs;
using ServianOps_Backend.Core.Entities.Jobs;
using ServianOps_Backend.Core.Interfaces.Repositories.Jobs;

namespace ServianOps_Backend.Application.Services.Jobs
{
    public class TradeService : ITradeService
    {
        private readonly ITradeRepository _repository;
        private readonly AutoMapper.IMapper _mapper;

        public TradeService(ITradeRepository repository, AutoMapper.IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<TradeDto> CreateAsync(CreateTradeDto dto)
        {
            var trade = _mapper.Map<Trade>(dto);
            await _repository.AddAsync(trade);
            return _mapper.Map<TradeDto>(trade);
        }

        public async Task<TradeDto> UpdateAsync(long id, UpdateTradeDto dto)
        {
            var trade = await _repository.GetByIdAsync(id);
            if (trade == null) throw new Exception("Trade not found.");

            _mapper.Map(dto, trade);
            await _repository.UpdateAsync(trade);

            return _mapper.Map<TradeDto>(trade);
        }

        public async Task<TradeDto> GetByIdAsync(long id)
        {
            var trade = await _repository.GetByIdAsync(id);
            return trade == null ? null : _mapper.Map<TradeDto>(trade);
        }

        public async Task<ServianOps_Backend.Application.DTOs.Shared.PagedResponseDto<TradeListDto>> GetAllPagedAsync(TradeFilterDto filter)
        {
            var query = _repository.GetQueryable();

            if (filter.IsActive.HasValue)
            {
                query = query.Where(t => t.IsActive == filter.IsActive.Value);
            }

            if (!string.IsNullOrWhiteSpace(filter.Search))
            {
                query = query.Where(t => t.Name.Contains(filter.Search));
            }

            var totalCount = await Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.CountAsync(query);
            var items = await Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.ToListAsync(
                query.OrderBy(t => t.Name).Skip((filter.PageNumber - 1) * filter.PageSize).Take(filter.PageSize)
            );

            return new ServianOps_Backend.Application.DTOs.Shared.PagedResponseDto<TradeListDto>
            {
                TotalCount = totalCount,
                PageNumber = filter.PageNumber,
                PageSize = filter.PageSize,
                Items = _mapper.Map<List<TradeListDto>>(items)
            };
        }

        public async Task DeleteAsync(long id)
        {
            var trade = await _repository.GetByIdAsync(id);
            if (trade != null)
            {
                await _repository.DeleteAsync(trade);
            }
        }
    }
}
