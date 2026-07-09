using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ServianOps_Backend.Core.Entities.Crm;

namespace ServianOps_Backend.EntityFramework.Configurations.Crm
{
    public class CustomerConfiguration : IEntityTypeConfiguration<Customer>
    {
        public void Configure(EntityTypeBuilder<Customer> builder)
        {
            builder.Property(x => x.Name).IsRequired().HasMaxLength(255);
            builder.Property(x => x.CompanyName).HasMaxLength(255);
            builder.Property(x => x.Area).HasMaxLength(150);
            builder.Property(x => x.City).HasMaxLength(150);
            builder.Property(x => x.CountryOrState).HasMaxLength(150);
            builder.Property(x => x.PostCode).HasMaxLength(50);
            builder.Property(x => x.MobileNumber).HasMaxLength(50);
            builder.Property(x => x.AccountNumber).HasMaxLength(100);
            builder.Property(x => x.VatNumber).HasMaxLength(100);

            // Configure relationships
            builder.HasOne(x => x.CustomerType)
                .WithMany(x => x.Customers)
                .HasForeignKey(x => x.CustomerTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.AccountManager)
                .WithMany()
                .HasForeignKey(x => x.AccountManagerId)
                .OnDelete(DeleteBehavior.SetNull); // Or Restrict. Let's use SetNull for nullable user FK.
        }
    }
}
