using Aspire.Net.ApiService.Domain.Entities;
using Aspire.Net.ApiService.Infrastrutura.Configurations;
using Microsoft.EntityFrameworkCore;

namespace Aspire.Net.ApiService.Infrastrutura.Repositories
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
    {
        public DbSet<Product> Products { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Aplicar todas as configurações
            modelBuilder.ApplyConfiguration(new ProductConfiguration());
        }
    }
}
