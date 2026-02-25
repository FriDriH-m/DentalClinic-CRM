using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Web_API.Models;

namespace Web_API.Configurations
{
    public class ClinicConfiguration : IEntityTypeConfiguration<Clinic>
    {
        public void Configure(EntityTypeBuilder<Clinic> builder)
        {
            builder.Property(c => c.Location)
                .IsRequired()
                .HasMaxLength(150);

            builder.Property(c => c.PostalCode)
                .IsRequired()
                .HasMaxLength(20);

            builder.Property(c => c.PhoneNumber)
                .IsRequired()
                .HasMaxLength(20);
        }
    }
}