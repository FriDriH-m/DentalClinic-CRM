using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Web_API.Models;

namespace Web_API.Configurations
{
    public class ChecksConfiguration : IEntityTypeConfiguration<Check>
    {
        public void Configure(EntityTypeBuilder<Check> builder)
        {
            builder.Property(c => c.TotalPrice)
                .HasDefaultValue(0);

            builder.Property(c => c.Discount)
                .HasDefaultValue(0);

            builder.HasIndex(c => c.ClientId);
            builder.HasIndex(c => c.AppointmentId);
            builder.HasIndex(c => c.Date);
        }
    }
}
