using System;

namespace ServianOps_Backend.Application.Common.DTOs
{
    public abstract class BaseAuditDto
    {
        public long Id { get; set; }
        public DateTime CreationTime { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? LastModificationTime { get; set; }
        public string? ModifiedBy { get; set; }
        public bool IsActive { get; set; }
    }
}
