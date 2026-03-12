using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Web_API.Models;

namespace Web_API.Configurations
{
    public class AppointmentMaterialConfiguration : IEntityTypeConfiguration<AppointmentMaterial>
    {
        public void Configure(EntityTypeBuilder<AppointmentMaterial> builder)
        {
            builder.HasKey(am => new { am.AppointmentId, am.MaterialId });

            builder.HasIndex(am => am.AppointmentId);
            builder.HasIndex(am => am.MaterialId);

            builder.Property(am => am.Quantity)
                .HasDefaultValue(1);

            builder.Property(am => am.Price)
                .HasDefaultValue(0);

            builder.HasOne(am => am.Appointment)
                .WithMany(a => a.AppointmentMaterials)
                .HasForeignKey(am => am.AppointmentId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(am => am.Material)
                .WithMany(m => m.AppointmentMaterials)
                .HasForeignKey(am => am.MaterialId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}