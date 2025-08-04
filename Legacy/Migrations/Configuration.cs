using System.Data.Entity.Migrations;

using ModernizationPoC.Legacy.DAL;
using ModernizationPoC.Shared;

namespace ModernizationPoC.Legacy.Migrations
{
    internal sealed class Configuration : DbMigrationsConfiguration<ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }
        
        protected override void Seed(ApplicationDbContext context)
        {
            context
                .Toggles
                .AddOrUpdate(new ModernizationToggle { AboutPage = true, Id = 1 });

            context.SaveChanges();
        }
    } 
}