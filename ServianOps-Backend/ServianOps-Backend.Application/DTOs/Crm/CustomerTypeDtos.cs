using System;

namespace ServianOps_Backend.Application.DTOs.Crm
{
    public class CustomerTypeDto
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public DateTime CreationTime { get; set; }
        public bool IsActive { get; set; }
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
