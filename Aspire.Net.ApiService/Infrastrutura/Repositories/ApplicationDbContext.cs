using Int.Database.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Int.Database.Infrastrutura.Repositories
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Product> Products { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Aplicar todas as configurações
            modelBuilder.ApplyConfiguration(new ProductConfiguration());
        }
    }
}
