using Microsoft.EntityFrameworkCore;
using UniversityCertificates.Register.Models;
using UniversityCertificates.Students.Models;

namespace UniversityCertificates.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options) { }

    public DbSet<Student> Students { get; set; }
    public DbSet<RegisterEntry> RegisterEntries { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Student>().ToTable("Students");
        modelBuilder.Entity<RegisterEntry>().ToTable("RegisterEntries");

        modelBuilder
            .Entity<RegisterEntry>()
            .HasOne(re => re.Student)
            .WithMany(s => s.RegisterEntries)
            .HasForeignKey(re => re.StudentSerialNumber)
            .HasPrincipalKey(s => s.SerialNumber)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
