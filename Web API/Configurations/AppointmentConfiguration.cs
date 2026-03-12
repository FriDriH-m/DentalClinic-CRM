using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Web_API.Models;

namespace Web_API.Configurations
{
    public class AppointmentConfiguration : IEntityTypeConfiguration<Appointment>
    {
        public void Configure(EntityTypeBuilder<Appointment> builder)
        {
            builder.HasIndex(a => a.EmployeeId);
            builder.HasIndex(a => a.ClientId);
            builder.HasIndex(a => a.ClinicId);
            builder.HasIndex(a => a.Date);

            builder.Property(a => a.Status)
                .HasDefaultValue(AppointmentStatus.Pending);

            builder.Property(a => a.TotalPrice)
                .HasDefaultValue(0);

            builder.Property(a => a.IsClosed)
                .HasDefaultValue(false);

            builder
                .HasOne(a => a.Employee)
                .WithMany(e => e.Appointments)
                .HasForeignKey(a => a.EmployeeId)
                .OnDelete(DeleteBehavior.Restrict);

            builder
                .HasOne(a => a.Client)
                .WithMany(c => c.Appointments)
                .HasForeignKey(a => a.ClientId)
                .OnDelete(DeleteBehavior.Restrict);

            builder
                .HasOne(a => a.Clinic)
                .WithMany() 
                .HasForeignKey(a => a.ClinicId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}