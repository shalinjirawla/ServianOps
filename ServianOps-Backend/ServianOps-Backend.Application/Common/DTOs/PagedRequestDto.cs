using System.Collections.Generic;

namespace ServianOps_Backend.Application.Common.DTOs
{
    public class PagedRequestDto
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string SortBy { get; set; }
        public string SortDirection { get; set; } = "asc";
    }
}
