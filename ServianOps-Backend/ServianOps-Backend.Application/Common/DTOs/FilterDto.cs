using System;

namespace ServianOps_Backend.Application.Common.DTOs
{
    public class FilterDto : PagedRequestDto
    {
        public string Search { get; set; }
        public bool? IsActive { get; set; }
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
    }
}
