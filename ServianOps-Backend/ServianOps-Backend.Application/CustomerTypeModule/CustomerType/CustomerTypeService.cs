using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ServianOps_Backend.Application.Common.DTOs;
using ServianOps_Backend.Application.CustomerTypeModule.CustomerType.CustomerTypeDto;
using ServianOps_Backend.Core.Interfaces.Repositories;

namespace ServianOps_Backend.Application.CustomerTypeModule.CustomerType
{
    public class CustomerTypeService : ICustomerTypeService
    {
        private readonly IGenericRepository<Core.Entities.Crm.CustomerType> _repository;
        private readonly IMapper _mapper;

        public CustomerTypeService(IGenericRepository<Core.Entities.Crm.CustomerType> repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<StandardResponse<CustomerTypeDetailDto>> CreateCustomerType(CreateCustomerTypeDto dto)
        {
            var entity = _mapper.Map<Core.Entities.Crm.CustomerType>(dto);
            await _repository.AddAsync(entity);
            return StandardResponse<CustomerTypeDetailDto>.Ok(_mapper.Map<CustomerTypeDetailDto>(entity));
        }

        public async Task<StandardResponse<CustomerTypeDetailDto>> UpdateCustomerType(long id, UpdateCustomerTypeDto dto)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null) return StandardResponse<CustomerTypeDetailDto>.Error("Customer Type not found.");

            _mapper.Map(dto, entity);
            await _repository.UpdateAsync(entity);

            return StandardResponse<CustomerTypeDetailDto>.Ok(_mapper.Map<CustomerTypeDetailDto>(entity));
        }

        public async Task<StandardResponse<CustomerTypeDetailDto>> GetCustomerTypeById(long id)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null) return StandardResponse<CustomerTypeDetailDto>.Error("Customer Type not found.");
            return StandardResponse<CustomerTypeDetailDto>.Ok(_mapper.Map<CustomerTypeDetailDto>(entity));
        }

        public async Task<StandardResponse<PagedResultDto<CustomerTypeListDto>>> GetAllCustomerTypes(CustomerTypeFilterDto filter)
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

            var totalCount = await query.CountAsync();
            var items = await query
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToListAsync();

            var result = new PagedResultDto<CustomerTypeListDto>(
                _mapper.Map<IReadOnlyList<CustomerTypeListDto>>(items),
                totalCount,
                filter.PageNumber,
                filter.PageSize);

            return StandardResponse<PagedResultDto<CustomerTypeListDto>>.Ok(result);
        }

        public async Task<StandardResponse<IReadOnlyList<CustomerTypeLookupDto>>> GetCustomerTypeLookup()
        {
            var items = await _repository.GetAllAsync();
            var result = items.Select(c => new CustomerTypeLookupDto
            {
                Id = c.Id,
                Name = c.Name
            }).OrderBy(x => x.Name).ToList();

            return StandardResponse<IReadOnlyList<CustomerTypeLookupDto>>.Ok(result);
        }

        public async Task<StandardResponse<bool>> DeleteCustomerType(long id)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity != null)
            {
                await _repository.DeleteAsync(entity);
                return StandardResponse<bool>.Ok(true, "Customer Type deleted.");
            }
            return StandardResponse<bool>.Error("Customer Type not found.");
        }
    }
}
