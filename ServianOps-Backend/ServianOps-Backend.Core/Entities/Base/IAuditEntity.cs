using System;

namespace ServianOps_Backend.Core.Entities.Base
{
    public interface IAuditEntity
    {
        DateTime CreationTime { get; set; }
        long? CreatorUserId { get; set; }
        DateTime? LastModificationTime { get; set; }
        long? LastModifierUserId { get; set; }
        bool IsDeleted { get; set; }
        long? DeletedBy { get; set; }
        DateTime? DeletedDate { get; set; }
        bool IsActive { get; set; }
    }
}
