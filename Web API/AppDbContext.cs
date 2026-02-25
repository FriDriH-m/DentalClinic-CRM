using Microsoft.EntityFrameworkCore;

namespace Web_API
{
    public class AppDbContext : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
           
        }

        public DbSet<Models.Clinic> Clinics { get; set; }
        public DbSet<Models.Role> Roles { get; set; }
        public DbSet<Models.Employee> Employees { get; set; }
        public DbSet<Models.Client> Clients { get; set; }
        public DbSet<Models.Material> Materials { get; set; }
        public DbSet<Models.Appointment> Appointments { get; set; }
        public DbSet<Models.Service> AppointmentMaterials { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        }
    }
}
