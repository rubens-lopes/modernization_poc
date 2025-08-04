using System.Data.Entity;

using ModernizationPoC.Shared;

namespace ModernizationPoC.Legacy.DAL
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext() : base("DefaultConnection")
        {
            Database.SetInitializer(new CreateDatabaseIfNotExists<ApplicationDbContext>());
        }

        public DbSet<ModernizationToggle> Toggles { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ModernizationToggle>()
                .ToTable("modernization_toggle")
                .HasKey(t => t.Id);

            base.OnModelCreating(modelBuilder);
        }
    }
}