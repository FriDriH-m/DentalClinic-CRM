using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Web_API.Models;

namespace Web_API.Configurations
{
    public class EmployeeConfiguration : IEntityTypeConfiguration<Employee>
    {
        public void Configure(EntityTypeBuilder<Employee> builder)
        {
            builder.Property(c => c.FirstName)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(c => c.SecondName)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(c => c.PhoneNumber)
                .IsRequired()
                .HasMaxLength(20);

            builder.Property(c => c.Position)
                .IsRequired()
                .HasMaxLength(30);

            builder.Property(c => c.Info)
                .HasMaxLength(300);

            builder.HasIndex(e => e.PhoneNumber);
            builder.HasIndex(e => e.Position);
            builder.HasIndex(e => new { e.FirstName, e.SecondName });
        }
    }
}
