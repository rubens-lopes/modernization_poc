using System.Data.Entity.Migrations;

namespace ModernizationPoC.Legacy.Migrations
{
    internal sealed class Configuration : DbMigrationsConfiguration<DAL.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }
    } 
}