using ServianOps_Backend.Application.DTOs.Shared;

namespace ServianOps_Backend.Application.DTOs.Crm
{
    public class CustomerFilterDto : PagedRequestDto
    {
        public bool? IsActive { get; set; }
        public long? CustomerTypeId { get; set; }
    }
}
