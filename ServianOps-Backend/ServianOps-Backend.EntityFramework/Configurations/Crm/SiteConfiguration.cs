using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ServianOps_Backend.Core.Entities.Crm;

namespace ServianOps_Backend.EntityFramework.Configurations.Crm
{
    public class SiteConfiguration : IEntityTypeConfiguration<Site>
    {
        public void Configure(EntityTypeBuilder<Site> builder)
        {
            builder.Property(x => x.SiteName).IsRequired().HasMaxLength(255);
            builder.Property(x => x.CompanyName).HasMaxLength(255);
            builder.Property(x => x.Area).HasMaxLength(150);
            builder.Property(x => x.City).HasMaxLength(150);
            builder.Property(x => x.CountryOrState).HasMaxLength(150);
            builder.Property(x => x.PostCode).HasMaxLength(50);
            builder.Property(x => x.MobileNumber).HasMaxLength(50);
            
            builder.Property(x => x.AccessDetails).HasMaxLength(500);
            builder.Property(x => x.ParkingInformation).HasMaxLength(500);
            builder.Property(x => x.KeysOrCode).HasMaxLength(255);
            builder.Property(x => x.SiteNotes).HasMaxLength(2000);

            builder.HasOne(x => x.Customer)
                .WithMany(x => x.Sites)
                .HasForeignKey(x => x.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.AccountManager)
                .WithMany(u => u.ManagedSites)
                .HasForeignKey(x => x.AccountManagerId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
