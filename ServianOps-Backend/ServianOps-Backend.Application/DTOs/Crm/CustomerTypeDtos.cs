using System;
using ServianOps_Backend.Application.DTOs.Base;

namespace ServianOps_Backend.Application.DTOs.Crm
{
    public class CustomerTypeDto : BaseAuditDto
    {
        public string Name { get; set; }
    }

    public class CreateCustomerTypeDto
    {
        public string Name { get; set; }
    }

    public class UpdateCustomerTypeDto
    {
        public string Name { get; set; }
    }
}
