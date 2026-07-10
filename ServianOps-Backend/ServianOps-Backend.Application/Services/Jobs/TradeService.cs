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

        public TradeService(ITradeRepository repository)
        {
            _repository = repository;
        }

        public async Task<TradeDto> CreateAsync(CreateTradeDto dto)
        {
            var trade = new Trade
            {
                Name = dto.Name
            };

            await _repository.AddAsync(trade);

            return new TradeDto
            {
                Id = trade.Id,
                Name = trade.Name
            };
        }

        public async Task<TradeDto> UpdateAsync(long id, UpdateTradeDto dto)
        {
            var trade = await _repository.GetByIdAsync(id);
            if (trade == null) throw new Exception("Trade not found.");

            trade.Name = dto.Name;

            await _repository.UpdateAsync(trade);

            return new TradeDto
            {
                Id = trade.Id,
                Name = trade.Name
            };
        }

        public async Task<TradeDto> GetByIdAsync(long id)
        {
            var trade = await _repository.GetByIdAsync(id);
            if (trade == null) return null;

            return new TradeDto
            {
                Id = trade.Id,
                Name = trade.Name
            };
        }

        public async Task<IReadOnlyList<TradeListDto>> GetAllPagedAsync(int pageNumber, int pageSize, string searchTerm)
        {
            var query = await _repository.GetAllAsync();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(x => x.Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            var paged = query
                .OrderBy(x => x.Name)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(x => new TradeListDto
                {
                    Id = x.Id,
                    Name = x.Name
                })
                .ToList();

            return paged;
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
