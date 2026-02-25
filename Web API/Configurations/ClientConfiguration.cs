using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Web_API.Models;

namespace Web_API.Configurations
{
    public class ClientConfiguration : IEntityTypeConfiguration<Client>
    {
        public void Configure(EntityTypeBuilder<Client> builder)
        {
            builder
                .HasIndex(c => c.Email)
                .IsUnique();

            builder
                .HasIndex(c => c.PhoneNumber)
                .IsUnique();

            builder.Property(c => c.FirstName)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(c => c.SecondName)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(c => c.PhoneNumber)
                .IsRequired()
                .HasMaxLength(20);

            builder.Property(c => c.Email)
                .IsRequired()
                .HasMaxLength(30);

            builder.Property(c => c.Info)
                .HasMaxLength(300);

            builder.Property(c => c.MoneySpent)
                .HasDefaultValue(0);

            builder.Property(c => c.Bonuses)
                .HasDefaultValue(0);

            builder.Property(c => c.Status)
                .HasDefaultValue(ClientStatus.Regular);
        }
    }
}
