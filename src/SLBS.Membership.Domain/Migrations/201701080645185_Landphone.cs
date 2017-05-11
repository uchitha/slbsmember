namespace SLBS.Membership.Domain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Landphone : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Adult", "MobilePhone", c => c.String());
            AddColumn("dbo.Adult", "LandPhone", c => c.String());
            DropColumn("dbo.Adult", "Phone");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Adult", "Phone", c => c.String());
            DropColumn("dbo.Adult", "LandPhone");
            DropColumn("dbo.Adult", "MobilePhone");
        }
    }
}
