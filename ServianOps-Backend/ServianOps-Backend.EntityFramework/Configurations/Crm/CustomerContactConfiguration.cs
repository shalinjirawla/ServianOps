using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ServianOps_Backend.Core.Entities.Crm;

namespace ServianOps_Backend.EntityFramework.Configurations.Crm
{
    public class CustomerContactConfiguration : IEntityTypeConfiguration<CustomerContact>
    {
        public void Configure(EntityTypeBuilder<CustomerContact> builder)
        {
            builder.Property(x => x.FirstName).IsRequired().HasMaxLength(100);
            builder.Property(x => x.LastName).HasMaxLength(100);
            builder.Property(x => x.MobileNumber).HasMaxLength(50);
            builder.Property(x => x.Email).HasMaxLength(150);

            builder.HasOne(x => x.Customer)
                .WithMany(x => x.CustomerContacts)
                .HasForeignKey(x => x.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
