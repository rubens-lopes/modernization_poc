using Microsoft.EntityFrameworkCore;

using ModernizationPoC.Shared;

namespace ModernizationPoC.Modern.DAL;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) 
    : DbContext(options)
{
    public DbSet<ModernizationToggle> Toggles { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ModernizationToggle>()
            .ToTable("modernization_toggle")
            .HasKey(t => t.Id);

        base.OnModelCreating(modelBuilder);
    }
}