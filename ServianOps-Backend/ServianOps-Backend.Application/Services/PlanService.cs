using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ServianOps_Backend.Application.DTOs.Plan;
using ServianOps_Backend.Core.Entities.Saas;
using ServianOps_Backend.Core.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace ServianOps_Backend.Application.Services
{
    public class PlanService : Interfaces.IPlanService
    {
        private readonly IPlanRepository _planRepository;
        private readonly AutoMapper.IMapper _mapper;

        public PlanService(IPlanRepository planRepository, AutoMapper.IMapper mapper)
        {
            _planRepository = planRepository;
            _mapper = mapper;
        }

        public async Task<PlanDto> GetPlanByIdAsync(long id)
        {
            var plan = await _planRepository.GetByIdAsync(id);
            return plan == null ? null : _mapper.Map<PlanDto>(plan);
        }

        public async Task<ServianOps_Backend.Application.DTOs.Shared.PagedResponseDto<PlanDto>> GetPlansAsync(PlanFilterDto filter)
        {
            var query = _planRepository.GetQueryable();

            if (!string.IsNullOrWhiteSpace(filter.Search))
            {
                query = query.Where(p => p.PlanName.Contains(filter.Search));
            }

            if (filter.IsActive.HasValue)
            {
                query = query.Where(p => p.IsActive == filter.IsActive.Value);
            }

            var totalCount = await query.CountAsync();
            var items = await query
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToListAsync();

            return new ServianOps_Backend.Application.DTOs.Shared.PagedResponseDto<PlanDto>
            {
                TotalCount = totalCount,
                PageNumber = filter.PageNumber,
                PageSize = filter.PageSize,
                Items = _mapper.Map<List<PlanDto>>(items)
            };
        }

        public async Task<PlanDto> CreatePlanAsync(CreatePlanDto dto)
        {
            var plan = _mapper.Map<Plan>(dto);
            await _planRepository.AddAsync(plan);
            return _mapper.Map<PlanDto>(plan);
        }

        public async Task UpdatePlanAsync(long id, CreatePlanDto dto)
        {
            var plan = await _planRepository.GetByIdAsync(id);
            if (plan == null) throw new System.Exception("Plan not found");

            _mapper.Map(dto, plan);
            await _planRepository.UpdateAsync(plan);
        }

        public async Task DeletePlanAsync(long id)
        {
            var plan = await _planRepository.GetByIdAsync(id);
            if (plan != null)
            {
                await _planRepository.DeleteAsync(plan);
            }
        }
    }
}
