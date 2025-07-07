using Aspire.Net.ApiService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Aspire.Net.ApiService.Infrastrutura.Configurations
{
    public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
    {
        public void Configure(EntityTypeBuilder<Payment> builder)
        {
            builder.ToTable("Payments");

            builder.HasKey(p => p.Id);

            builder.Property(p => p.ProductId)
                .IsRequired();
            builder.Property(p => p.TypePayment)
                .IsRequired();
            builder.Property(p => p.Amount)
                .IsRequired()
                .HasColumnType("decimal(18,2)");
            builder.Property(p => p.Details)
                .HasMaxLength(500);
            builder.Property(p => p.AtCreation)
                .IsRequired()
                .HasDefaultValueSql("CURRENT_TIMESTAMP");
            builder.Property(p => p.Status)
                .IsRequired()
                .HasDefaultValue(0);
        }
    }
}
