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
        private readonly AutoMapper.IMapper _mapper;

        public CustomerTypeService(ICustomerTypeRepository repository, AutoMapper.IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<CustomerTypeDto> CreateAsync(CreateCustomerTypeDto dto)
        {
            var entity = _mapper.Map<CustomerType>(dto);
            await _repository.AddAsync(entity);
            return _mapper.Map<CustomerTypeDto>(entity);
        }

        public async Task<CustomerTypeDto> UpdateAsync(long id, UpdateCustomerTypeDto dto)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null) throw new Exception("Customer Type not found.");

            _mapper.Map(dto, entity);
            await _repository.UpdateAsync(entity);

            return _mapper.Map<CustomerTypeDto>(entity);
        }

        public async Task<CustomerTypeDto> GetByIdAsync(long id)
        {
            var entity = await _repository.GetByIdAsync(id);
            return entity == null ? null : _mapper.Map<CustomerTypeDto>(entity);
        }

        public async Task<ServianOps_Backend.Application.DTOs.Shared.PagedResponseDto<CustomerTypeDto>> GetAllPagedAsync(CustomerTypeFilterDto filter)
        {
            var query = _repository.GetQueryable();

            if (filter.IsActive.HasValue)
            {
                query = query.Where(c => c.IsActive == filter.IsActive.Value);
            }

            if (!string.IsNullOrWhiteSpace(filter.Search))
            {
                query = query.Where(c => c.Name.Contains(filter.Search));
            }

            var totalCount = await Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.CountAsync(query);
            var items = await Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.ToListAsync(
                query.Skip((filter.PageNumber - 1) * filter.PageSize).Take(filter.PageSize)
            );

            return new ServianOps_Backend.Application.DTOs.Shared.PagedResponseDto<CustomerTypeDto>
            {
                TotalCount = totalCount,
                PageNumber = filter.PageNumber,
                PageSize = filter.PageSize,
                Items = _mapper.Map<List<CustomerTypeDto>>(items)
            };
        }

        public async Task DeleteAsync(long id)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity != null)
            {
                await _repository.DeleteAsync(entity);
            }
        }
    }
}
