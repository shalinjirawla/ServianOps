using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ServianOps_Backend.Core.Entities.Jobs;

namespace ServianOps_Backend.EntityFramework.Configurations.Jobs
{
    public class TradeConfiguration : IEntityTypeConfiguration<Trade>
    {
        public void Configure(EntityTypeBuilder<Trade> builder)
        {
            builder.Property(x => x.Name).IsRequired().HasMaxLength(150);

            // Unique Name per Tenant
            builder.HasIndex(x => new { x.Name, x.TenantId }).IsUnique();
        }
    }
}
