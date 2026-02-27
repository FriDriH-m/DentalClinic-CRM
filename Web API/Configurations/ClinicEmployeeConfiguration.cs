using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Web_API.Models;

namespace Web_API.Configurations
{
    public class ClinicEmployeeConfiguration : IEntityTypeConfiguration<ClinicEmployee>
    {
        public void Configure(EntityTypeBuilder<ClinicEmployee> builder)
        {
            builder.HasIndex(ce => ce.EmployeeId);
            builder.HasIndex(ce => ce.ClinicId);

            builder.HasIndex(ce => new { ce.EmployeeId, ce.ClinicId });

            builder.HasOne(ce => ce.Employee)
                .WithMany(e => e.ClinicEmployees)
                .HasForeignKey(ce => ce.EmployeeId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(ce => ce.Clinic)
                .WithMany(c => c.ClinicEmployees)
                .HasForeignKey(ce => ce.ClinicId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}