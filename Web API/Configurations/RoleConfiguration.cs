using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Web_API.Models;

namespace Web_API.Configurations
{
    public class RoleConfiguration : IEntityTypeConfiguration<Role>
    {
        public void Configure(EntityTypeBuilder<Role> builder)
        {
            builder.Property(r => r.Name)
                .IsRequired()
                .HasMaxLength(50);

            // Автоматическое заполнение базовыми ролями
            builder.HasData(
                new Role { Id = 1, Name = "Admin" },
                new Role { Id = 2, Name = "Analyst" },
                new Role { Id = 3, Name = "Doctor" },
                new Role { Id = 4, Name = "Manager" }
            );
        }
    }
}