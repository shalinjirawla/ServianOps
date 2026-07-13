using System;
using ServianOps_Backend.Application.DTOs.Shared;

namespace ServianOps_Backend.Application.DTOs.Base
{
    public abstract class BaseAuditDto
    {
        public long Id { get; set; }
        public DateTime CreationTime { get; set; }
        public UserSummaryDto? CreatedByUser { get; set; }
        public DateTime? LastModificationTime { get; set; }
        public UserSummaryDto? ModifiedByUser { get; set; }
        public bool IsActive { get; set; }
    }
}
