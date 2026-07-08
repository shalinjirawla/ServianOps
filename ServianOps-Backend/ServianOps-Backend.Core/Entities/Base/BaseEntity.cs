using System;

namespace ServianOps_Backend.Core.Entities.Base
{
    public abstract class BaseEntity : IAuditEntity
    {
        public long Id { get; set; }
        public DateTime CreationTime { get; set; } = DateTime.UtcNow;
        public long? CreatorUserId { get; set; }
        public DateTime? LastModificationTime { get; set; }
        public long? LastModifierUserId { get; set; }
        public bool IsDeleted { get; set; } = false;
        public long? DeletedBy { get; set; }
        public DateTime? DeletedDate { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
