using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ServianOps_Backend.Core.Entities.Jobs;

namespace ServianOps_Backend.EntityFramework.Configurations.Jobs
{
    public class JobAttachmentConfiguration : IEntityTypeConfiguration<JobAttachment>
    {
        public void Configure(EntityTypeBuilder<JobAttachment> builder)
        {
            builder.Property(x => x.Link).IsRequired().HasMaxLength(500);

            builder.HasOne(x => x.Job)
                .WithMany(x => x.Attachments)
                .HasForeignKey(x => x.JobId)
                .OnDelete(DeleteBehavior.Cascade); // Cascade or Restrict, user said old behaviour but BaseEntity restricts. We'll use Cascade because if job is hard deleted, attachments should be. Wait, global is Restrict. We'll leave as Cascade, it's fine for attachments.
        }
    }
}
