using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Web_API.Models;

namespace Web_API.Configurations
{
    public class AppointmentConfiguration : IEntityTypeConfiguration<Appointment>
    {
        public void Configure(EntityTypeBuilder<Appointment> builder)
        {
            // Статус по умолчанию при создании заявки
            builder.Property(a => a.Status)
                .HasDefaultValue(AppointmentStatus.Pending);

            builder.Property(a => a.TotalPrice)
                .HasDefaultValue(0);

            // Отключаем каскадное удаление при удалении Сотрудника (Врача)
            builder
                .HasOne(a => a.Employee)
                .WithMany(e => e.Appointments)
                .HasForeignKey(a => a.EmployeeId)
                .OnDelete(DeleteBehavior.Restrict);

            // Отключаем каскадное удаление при удалении Пациента
            builder
                .HasOne(a => a.Client)
                .WithMany(c => c.Appointments)
                .HasForeignKey(a => a.ClientId)
                .OnDelete(DeleteBehavior.Restrict);

            // Отключаем каскадное удаление при удалении Клиники
            builder
                .HasOne(a => a.Clinic)
                .WithMany() // Оставляем скобки пустыми! Это значит "много приемов, но без списка в самой клинике"
                .HasForeignKey(a => a.ClinicId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}