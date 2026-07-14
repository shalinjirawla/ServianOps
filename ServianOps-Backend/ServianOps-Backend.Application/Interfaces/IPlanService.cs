using System.Collections.Generic;
using System.Threading.Tasks;
using ServianOps_Backend.Application.DTOs.Plan;

namespace ServianOps_Backend.Application.Interfaces
{
    public interface IPlanService
    {
        Task<PlanDto> GetPlanByIdAsync(long id);
        Task<ServianOps_Backend.Application.DTOs.Shared.PagedResponseDto<PlanDto>> GetPlansAsync(PlanFilterDto filter);
        Task<PlanDto> CreatePlanAsync(CreatePlanDto dto);
        Task UpdatePlanAsync(long id, CreatePlanDto dto);
        Task DeletePlanAsync(long id);
    }
}
