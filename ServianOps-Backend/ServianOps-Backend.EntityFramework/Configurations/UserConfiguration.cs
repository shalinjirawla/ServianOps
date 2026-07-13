using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ServianOps_Backend.Core.Entities.Identity;

namespace ServianOps_Backend.EntityFramework.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasKey(x => x.Id);
            
            builder.Property(x => x.FirstName).IsRequired().HasMaxLength(100);
            builder.Property(x => x.LastName).IsRequired().HasMaxLength(100);
            builder.Property(x => x.Email).IsRequired().HasMaxLength(150);
            
            // Email must be unique per tenant
            builder.HasIndex(x => new { x.TenantId, x.Email }).IsUnique();
            
            builder.HasOne(x => x.Tenant)
                   .WithMany(t => t.Users)
                   .HasForeignKey(x => x.TenantId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
