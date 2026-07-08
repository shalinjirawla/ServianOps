using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ServianOps_Backend.Core.Entities.Saas;

namespace ServianOps_Backend.EntityFramework.Configurations
{
    public class TenantConfiguration : IEntityTypeConfiguration<Tenant>
    {
        public void Configure(EntityTypeBuilder<Tenant> builder)
        {
            builder.HasKey(x => x.Id);
            
            builder.Property(x => x.CompanyName).IsRequired().HasMaxLength(200);
            builder.Property(x => x.TenancyName).IsRequired().HasMaxLength(50);
            builder.HasIndex(x => x.TenancyName).IsUnique(); // TenancyName unique constraint
            

            builder.HasOne(x => x.Plan)
                   .WithMany()
                   .HasForeignKey(x => x.PlanId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
