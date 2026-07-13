using System;
using ServianOps_Backend.Application.DTOs.Base;

namespace ServianOps_Backend.Application.DTOs.Role
{
    public class RoleDto : BaseAuditDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
