using System;
using System.ComponentModel.DataAnnotations.Schema;
using ServianOps_Backend.Core.Entities.Identity;

namespace ServianOps_Backend.Core.Entities.Base
{
    public abstract class BaseEntity : IAuditEntity
    {
        public long Id { get; set; }
        public DateTime CreationTime { get; set; } = DateTime.UtcNow;
        public long? CreatorUserId { get; set; }
        [NotMapped]
        public virtual User? CreatorUser { get; set; }
        public DateTime? LastModificationTime { get; set; }
        public long? LastModifierUserId { get; set; }
        [NotMapped]
        public virtual User? LastModifierUser { get; set; }
        
        public bool IsDeleted { get; set; } = false;
        
        public long? DeletedBy { get; set; }
        [NotMapped]
        public virtual User? DeleterUser { get; set; }
        
        public DateTime? DeletedDate { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
