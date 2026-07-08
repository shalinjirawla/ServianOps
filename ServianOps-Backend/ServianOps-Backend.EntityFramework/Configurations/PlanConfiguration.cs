using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ServianOps_Backend.Core.Entities.Saas;

namespace ServianOps_Backend.EntityFramework.Configurations
{
    public class PlanConfiguration : IEntityTypeConfiguration<Plan>
    {
        public void Configure(EntityTypeBuilder<Plan> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.PlanName).IsRequired().HasMaxLength(100);
            builder.Property(x => x.BillingCycle).HasMaxLength(20);
        }
    }
}
