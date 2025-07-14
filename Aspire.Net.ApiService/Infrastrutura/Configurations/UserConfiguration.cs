using Aspire.Net.ApiService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Aspire.Net.ApiService.Infrastrutura.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("Users");
            builder.HasKey(e => e.Id);

            builder.Property(e => e.Username)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(e => e.Email)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(e => e.PasswordHash)
                .IsRequired();

            builder.Property(e => e.Role)
                .HasMaxLength(50)
                .HasDefaultValue("User");

            builder.Property(e => e.CreatedAt)
                   .IsRequired();

            builder.Property(e => e.IsActive)
                .HasDefaultValue(true);

            // Índices únicos
            builder.HasIndex(e => e.Username)
                .IsUnique();

            builder.HasIndex(e => e.Email)
                .IsUnique();
        }
    }
}
