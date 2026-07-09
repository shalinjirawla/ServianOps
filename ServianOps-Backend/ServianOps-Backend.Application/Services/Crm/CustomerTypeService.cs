using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ServianOps_Backend.Application.DTOs.Crm;
using ServianOps_Backend.Application.Interfaces.Crm;
using ServianOps_Backend.Core.Entities.Crm;
using ServianOps_Backend.Core.Interfaces.Repositories.Crm;

namespace ServianOps_Backend.Application.Services.Crm
{
    public class CustomerTypeService : ICustomerTypeService
    {
        private readonly ICustomerTypeRepository _repository;

        public CustomerTypeService(ICustomerTypeRepository repository)
        {
            _repository = repository;
        }

        public async Task<CustomerTypeDto> CreateAsync(CreateCustomerTypeDto dto)
        {
            var entity = new CustomerType
            {
                Name = dto.Name
            };

            await _repository.AddAsync(entity);

            return MapToDto(entity);
        }

        public async Task<CustomerTypeDto> UpdateAsync(long id, UpdateCustomerTypeDto dto)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null) throw new Exception("Customer Type not found.");

            entity.Name = dto.Name;

            await _repository.UpdateAsync(entity);

            return MapToDto(entity);
        }

        public async Task<CustomerTypeDto> GetByIdAsync(long id)
        {
            var entity = await _repository.GetByIdAsync(id);
            return entity == null ? null : MapToDto(entity);
        }

        public async Task<IReadOnlyList<CustomerTypeDto>> GetAllPagedAsync(int pageNumber, int pageSize)
        {
            var entities = await _repository.GetPagedAsync(pageNumber, pageSize);
            return entities.Select(MapToDto).ToList();
        }

        public async Task DeleteAsync(long id)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity != null)
            {
                await _repository.DeleteAsync(entity);
            }
        }

        private static CustomerTypeDto MapToDto(CustomerType entity)
        {
            return new CustomerTypeDto
            {
                Id = entity.Id,
                Name = entity.Name,
                CreationTime = entity.CreationTime,
                IsActive = entity.IsActive
            };
        }
    }
}
