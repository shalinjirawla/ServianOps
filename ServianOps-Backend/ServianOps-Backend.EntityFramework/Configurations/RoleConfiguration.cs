using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ServianOps_Backend.Core.Entities.Identity;

namespace ServianOps_Backend.EntityFramework.Configurations
{
    public class RoleConfiguration : IEntityTypeConfiguration<Role>
    {
        public void Configure(EntityTypeBuilder<Role> builder)
        {
            builder.HasKey(x => x.Id);
            
            builder.Property(x => x.Name).IsRequired().HasMaxLength(100);
            builder.Property(x => x.Description).HasMaxLength(500);
            
            // Role name unique per tenant
            builder.HasIndex(x => new { x.TenantId, x.Name }).IsUnique();
        }
    }
}
