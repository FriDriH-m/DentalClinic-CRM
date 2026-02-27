using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Web_API.Models;

namespace Web_API.Configurations
{
    public class MaterialConfiguration : IEntityTypeConfiguration<Material>
    {
        public void Configure(EntityTypeBuilder<Material> builder)
        {
            builder.Property(m => m.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(m => m.Description)
                .HasMaxLength(300);

            builder.Property(m => m.Count)
                .HasDefaultValue(0);

            builder.Property(m => m.Price)
                .HasDefaultValue(0);

            builder.HasIndex(m => m.ClinicId);
            builder.HasIndex(m => m.Name);
        }
    }
}