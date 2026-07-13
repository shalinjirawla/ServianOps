using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ServianOps_Backend.Core.Entities.Jobs;

namespace ServianOps_Backend.EntityFramework.Configurations.Jobs
{
    public class JobConfiguration : IEntityTypeConfiguration<Job>
    {
        public void Configure(EntityTypeBuilder<Job> builder)
        {
            builder.Property(x => x.JobNumber).IsRequired().HasMaxLength(50);
            builder.Property(x => x.Description).IsRequired();
            builder.Property(x => x.PONumber).HasMaxLength(100);

            builder.Property(x => x.Budget).HasColumnType("numeric(18,2)");
            builder.Property(x => x.NTE).HasColumnType("numeric(18,2)");

            // Relationships
            builder.HasOne(x => x.Customer)
                .WithMany(c => c.Jobs)
                .HasForeignKey(x => x.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Site)
                .WithMany(s => s.Jobs)
                .HasForeignKey(x => x.SiteId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Trade)
                .WithMany(x => x.Jobs)
                .HasForeignKey(x => x.TradeId)
                .OnDelete(DeleteBehavior.Restrict);

            // Indexes for faster lookups
            builder.HasIndex(x => x.CustomerId);
            builder.HasIndex(x => x.SiteId);
            builder.HasIndex(x => x.TradeId);
            builder.HasIndex(x => new { x.JobNumber, x.TenantId }).IsUnique();
        }
    }
}
