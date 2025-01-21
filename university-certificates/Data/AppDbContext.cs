using Microsoft.EntityFrameworkCore;

namespace UniversityCertificates.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure your entity mappings here
            // modelBuilder.Entity<YourEntity>().ToTable("YourEntities");
        }
    }
}