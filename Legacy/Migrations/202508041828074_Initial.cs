namespace ModernizationPoC.Legacy.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.modernization_toggle",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        AboutPage = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.modernization_toggle");
        }
    }
}
