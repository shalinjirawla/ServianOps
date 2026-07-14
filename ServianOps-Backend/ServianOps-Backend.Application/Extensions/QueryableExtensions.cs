using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ServianOps_Backend.Application.DTOs.Shared;

namespace ServianOps_Backend.Application.Extensions
{
    public static class QueryableExtensions
    {
        public static IQueryable<T> ApplyPagination<T>(this IQueryable<T> query, int pageNumber, int pageSize)
        {
            if (pageNumber <= 0) pageNumber = 1;
            if (pageSize <= 0) pageSize = 10;
            return query.Skip((pageNumber - 1) * pageSize).Take(pageSize);
        }

        public static async Task<PagedResponseDto<TDto>> ToPagedResponseAsync<TEntity, TDto>(
            this IQueryable<TEntity> query,
            int pageNumber,
            int pageSize,
            Func<TEntity, TDto> mapFunc)
        {
            var totalCount = await query.CountAsync();
            var items = await query.ApplyPagination(pageNumber, pageSize).ToListAsync();

            return new PagedResponseDto<TDto>
            {
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize,
                Items = items.Select(mapFunc).ToList()
            };
        }
    }
}
