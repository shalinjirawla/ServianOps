using ServianOps_Backend.Application.Common.DTOs;

namespace ServianOps_Backend.Application.CustomerModule.Customer.CustomerDto
{
    public class CustomerFilterDto : FilterDto
    {
        public long? CustomerTypeId { get; set; }
        public long? AccountManagerId { get; set; }
    }
}
