using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ServianOps_Backend.Application.DTOs.Plan;
using ServianOps_Backend.Core.Entities.Saas;
using ServianOps_Backend.Core.Interfaces.Repositories;

namespace ServianOps_Backend.Application.Services
{
    public class PlanService : Interfaces.IPlanService
    {
        private readonly IPlanRepository _planRepository;

        public PlanService(IPlanRepository planRepository)
        {
            _planRepository = planRepository;
        }

        public async Task<PlanDto> GetPlanByIdAsync(long id)
        {
            var plan = await _planRepository.GetByIdAsync(id);
            return plan == null ? null : MapToDto(plan);
        }

        public async Task<IReadOnlyList<PlanDto>> GetPlansAsync()
        {
            var plans = await _planRepository.GetAllAsync();
            return plans.Select(MapToDto).ToList();
        }

        private PlanDto MapToDto(Plan plan)
        {
            return new PlanDto
            {
                Id = plan.Id,
                PlanName = plan.PlanName,
                MaxUsers = plan.MaxUsers,
                MaxProjects = plan.MaxProjects,
                MaxStorageGB = plan.MaxStorageGB,
                Price = plan.Price,
                BillingCycle = plan.BillingCycle,
                IsTrialAvailable = plan.IsTrialAvailable,
                TrialDays = plan.TrialDays,
                IsActive = plan.IsActive
            };
        }

        public async Task<PlanDto> CreatePlanAsync(CreatePlanDto dto)
        {
            var plan = new Plan
            {
                PlanName = dto.PlanName,
                MaxUsers = dto.MaxUsers,
                MaxProjects = dto.MaxProjects,
                MaxStorageGB = dto.MaxStorageGB,
                Price = dto.Price,
                BillingCycle = dto.BillingCycle,
                IsTrialAvailable = dto.IsTrialAvailable,
                TrialDays = dto.TrialDays,
                IsActive = dto.IsActive
            };

            await _planRepository.AddAsync(plan);
            return MapToDto(plan);
        }

        public async Task UpdatePlanAsync(long id, CreatePlanDto dto)
        {
            var plan = await _planRepository.GetByIdAsync(id);
            if (plan == null) throw new System.Exception("Plan not found");

            plan.PlanName = dto.PlanName;
            plan.MaxUsers = dto.MaxUsers;
            plan.MaxProjects = dto.MaxProjects;
            plan.MaxStorageGB = dto.MaxStorageGB;
            plan.Price = dto.Price;
            plan.BillingCycle = dto.BillingCycle;
            plan.IsTrialAvailable = dto.IsTrialAvailable;
            plan.TrialDays = dto.TrialDays;
            plan.IsActive = dto.IsActive;

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
