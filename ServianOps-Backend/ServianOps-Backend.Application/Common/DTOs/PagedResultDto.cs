using System.Collections.Generic;

namespace ServianOps_Backend.Application.Common.DTOs
{
    public class PagedResultDto<T>
    {
        public IReadOnlyList<T> Items { get; set; }
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }

        public PagedResultDto(IReadOnlyList<T> items, int totalCount, int pageNumber, int pageSize)
        {
            Items = items;
            TotalCount = totalCount;
            PageNumber = pageNumber;
            PageSize = pageSize;
            TotalPages = pageSize > 0 ? (int)System.Math.Ceiling(totalCount / (double)pageSize) : 0;
        }
    }
}
