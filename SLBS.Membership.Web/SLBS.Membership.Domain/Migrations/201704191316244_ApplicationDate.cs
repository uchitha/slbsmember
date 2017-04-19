namespace SLBS.Membership.Domain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ApplicationDate : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Membership", "ApplicationDate", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Membership", "ApplicationDate");
        }
    }
}
