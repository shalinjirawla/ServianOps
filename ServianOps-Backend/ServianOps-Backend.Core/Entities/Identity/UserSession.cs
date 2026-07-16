using System;
using System.ComponentModel.DataAnnotations.Schema;
using ServianOps_Backend.Core.Entities.Base;

namespace ServianOps_Backend.Core.Entities.Identity
{
    public class UserSession : BaseEntity, IMayHaveTenant
    {
        public long UserId { get; set; }
        public virtual User User { get; set; }

        public long? TenantId { get; set; }

        public Guid Jti { get; set; }
        
        public string RefreshTokenHash { get; set; }
        
        public DateTime RefreshTokenExpiry { get; set; }
        public DateTime AccessTokenExpiry { get; set; }
        
        public string? DeviceName { get; set; }
        public string? Browser { get; set; }
        public string? OperatingSystem { get; set; }
        public string? IPAddress { get; set; }
        public string? UserAgent { get; set; }
        
        public DateTime LastActivity { get; set; }
        
        public bool IsRevoked { get; set; } = false;
        public DateTime? RevokedAt { get; set; }
        public long? RevokedBy { get; set; }
        
        [NotMapped]
        public virtual User? RevokedByUser { get; set; }
        
        public string? Reason { get; set; }
    }
}
