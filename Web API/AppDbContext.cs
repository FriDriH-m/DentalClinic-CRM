using Microsoft.EntityFrameworkCore;

namespace Web_API
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Models.Clinic> Clinics { get; set; }
        public DbSet<Models.Employee> Employees { get; set; }
        public DbSet<Models.Client> Clients { get; set; }
        public DbSet<Models.Material> Materials { get; set; }
        public DbSet<Models.Appointment> Appointments { get; set; }
        public DbSet<Models.Service> Services { get; set; }
        public DbSet<Models.AppointmentMaterial> AppointmentMaterial { get; set; }
        public DbSet<Models.AppointmentService> AppointmentService { get; set; }
        public DbSet<Models.Check> Checks { get; set; }
        public DbSet<Models.Bonuse> Bonuses { get; set; }
        public DbSet<Models.DoctorCategorySkill> DoctorCategorySkills { get; set; }
        public DbSet<Models.ClinicEmployee> ClinicEmployees { get; set; }
        public DbSet<Models.DoctorMaterialAccess> DoctorMaterialAccesses { get; set; }
        public DbSet<Models.ServiceMaterials> ServiceMaterials { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        }
    }
}
