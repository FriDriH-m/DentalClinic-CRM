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
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
    public class ServiceMaterialsConfiguration : IEntityTypeConfiguration<ServiceMaterials>
    {
        public void Configure(EntityTypeBuilder<ServiceMaterials> builder)
        {
            builder
                .HasOne(sm => sm.Service)
                .WithMany(s => s.Materials)
                .HasForeignKey(sm => sm.ServiceId)
                .OnDelete(DeleteBehavior.Cascade); 

            builder
                .HasOne(sm => sm.Material)
                .WithMany(m => m.Services)
                .HasForeignKey(sm => sm.MaterialId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
    public class DoctorMaterialAccessConfiguration : IEntityTypeConfiguration<DoctorMaterialAccess>
    {
        public void Configure(EntityTypeBuilder<DoctorMaterialAccess> builder)
        {
            builder
                .HasOne(dma => dma.Employee)
                .WithMany(e => e.MaterialsAccess)
                .HasForeignKey(dma => dma.EmployeeId)
                .OnDelete(DeleteBehavior.Cascade);

            builder
                .HasOne(dma => dma.Material)
                .WithMany(m => m.DoctorsAccess)
                .HasForeignKey(dma => dma.MaterialId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}