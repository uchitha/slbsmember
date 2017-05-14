namespace SLBS.Membership.Domain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class LastmodificationDate : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Membership", "LastNotificationDate", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Membership", "LastNotificationDate");
        }
    }
}
