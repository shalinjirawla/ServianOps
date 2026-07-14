using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ServianOps_Backend.Application.Common.DTOs;
using ServianOps_Backend.Application.PlanModule.Plan.PlanDto;
using ServianOps_Backend.Core.Interfaces.Repositories;

namespace ServianOps_Backend.Application.PlanModule.Plan
{
    public class PlanService : IPlanService
    {
        private readonly IGenericRepository<Core.Entities.Saas.Plan> _repository;
        private readonly IMapper _mapper;

        public PlanService(IGenericRepository<Core.Entities.Saas.Plan> repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<StandardResponse<PlanDetailDto>> CreatePlan(CreatePlanDto dto)
        {
            var entity = _mapper.Map<Core.Entities.Saas.Plan>(dto);
            await _repository.AddAsync(entity);
            return StandardResponse<PlanDetailDto>.Ok(_mapper.Map<PlanDetailDto>(entity));
        }

        public async Task<StandardResponse<PlanDetailDto>> UpdatePlan(long id, UpdatePlanDto dto)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null) return StandardResponse<PlanDetailDto>.Error("Plan not found.");

            _mapper.Map(dto, entity);
            await _repository.UpdateAsync(entity);

            return StandardResponse<PlanDetailDto>.Ok(_mapper.Map<PlanDetailDto>(entity));
        }

        public async Task<StandardResponse<PlanDetailDto>> GetPlanById(long id)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null) return StandardResponse<PlanDetailDto>.Error("Plan not found.");
            return StandardResponse<PlanDetailDto>.Ok(_mapper.Map<PlanDetailDto>(entity));
        }

        public async Task<StandardResponse<PagedResultDto<PlanListDto>>> GetAllPlans(PlanFilterDto filter)
        {
            var query = _repository.GetQueryable();

            if (filter.IsActive.HasValue)
            {
                query = query.Where(p => p.IsActive == filter.IsActive.Value);
            }

            if (!string.IsNullOrWhiteSpace(filter.Search))
            {
                query = query.Where(p => p.PlanName.Contains(filter.Search));
            }

            var totalCount = await query.CountAsync();
            var items = await query
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToListAsync();

            var result = new PagedResultDto<PlanListDto>(
                _mapper.Map<IReadOnlyList<PlanListDto>>(items),
                totalCount,
                filter.PageNumber,
                filter.PageSize);

            return StandardResponse<PagedResultDto<PlanListDto>>.Ok(result);
        }

        public async Task<StandardResponse<IReadOnlyList<PlanLookupDto>>> GetPlanLookup()
        {
            var items = await _repository.GetAllAsync();
            var result = items.Select(p => new PlanLookupDto
            {
                Id = p.Id,
                PlanName = p.PlanName
            }).OrderBy(x => x.PlanName).ToList();

            return StandardResponse<IReadOnlyList<PlanLookupDto>>.Ok(result);
        }

        public async Task<StandardResponse<bool>> DeletePlan(long id)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity != null)
            {
                await _repository.DeleteAsync(entity);
                return StandardResponse<bool>.Ok(true, "Plan deleted.");
            }
            return StandardResponse<bool>.Error("Plan not found.");
        }
    }
}
