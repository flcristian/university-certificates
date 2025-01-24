using Microsoft.EntityFrameworkCore;
using UniversityCertificates.Students.Models;

namespace UniversityCertificates.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options) { }

    public DbSet<Student> Students { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Student>().HasKey(s => s.SerialNumber);
    }
}
