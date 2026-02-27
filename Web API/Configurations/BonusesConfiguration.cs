using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.Extensions.Logging.Abstractions;
using Web_API.Models;

namespace Web_API.Configurations
{
    public class BonusesConfiguration : IEntityTypeConfiguration<Bonuse>
    {
        public void Configure(EntityTypeBuilder<Bonuse> builder)
        {
            builder.Property(b => b.Amount)
                .HasDefaultValue(0);


            builder.HasIndex(b => b.ExpiredAt);
            builder.HasIndex(b => b.ClientId);
        }
    }
}
