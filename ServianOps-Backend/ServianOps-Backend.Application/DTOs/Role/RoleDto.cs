using System;

namespace ServianOps_Backend.Application.DTOs.Role
{
    public class RoleDto
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
    }
}
