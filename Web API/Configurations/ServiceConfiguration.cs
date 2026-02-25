using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Web_API.Models;

namespace Web_API.Configurations
{
    public class ServiceConfiguration : IEntityTypeConfiguration<Service>
    {
        public void Configure(EntityTypeBuilder<Service> builder)
        {
            builder.Property(s => s.Name)
                .IsRequired()
                .HasMaxLength(150);

            builder.Property(s => s.Description)
                .HasMaxLength(500);

            builder.Property(s => s.Price)
                .IsRequired()
                .HasDefaultValue(0);
        }
    }
}