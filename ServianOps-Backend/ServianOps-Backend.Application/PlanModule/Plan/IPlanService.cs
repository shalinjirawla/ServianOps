using System.Collections.Generic;
using System.Threading.Tasks;
using ServianOps_Backend.Application.Common.DTOs;
using ServianOps_Backend.Application.PlanModule.Plan.PlanDto;

namespace ServianOps_Backend.Application.PlanModule.Plan
{
    public interface IPlanService
    {
        Task<StandardResponse<PlanDetailDto>> CreatePlan(CreatePlanDto dto);
        Task<StandardResponse<PlanDetailDto>> UpdatePlan(long id, UpdatePlanDto dto);
        Task<StandardResponse<PlanDetailDto>> GetPlanById(long id);
        Task<StandardResponse<PagedResultDto<PlanListDto>>> GetAllPlans(PlanFilterDto filter);
        Task<StandardResponse<IReadOnlyList<PlanLookupDto>>> GetPlanLookup();
        Task<StandardResponse<bool>> DeletePlan(long id);
    }
}
