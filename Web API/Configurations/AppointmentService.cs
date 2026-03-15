using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Web_API.Models;

namespace Web_API.Configurations
{
    public class AppointmentServiceConfiguration : IEntityTypeConfiguration<AppointmentService>
    {
        public void Configure(EntityTypeBuilder<AppointmentService> builder)
        {
            builder.HasKey(a => a.AppointmentId);

            builder.HasIndex(a => a.AppointmentId);
            builder.HasIndex(a => a.ServiceId);

            builder.HasOne(a => a.Appointment)
                .WithMany(a => a.AppointmentService)
                .HasForeignKey(a => a.AppointmentId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(a => a.Service)
                .WithMany(s => s.AppointmentService)
                .HasForeignKey(s => s.ServiceId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
