namespace SLBS.Membership.Domain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddBlockEmails : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Membership", "BlockEmails", c => c.Boolean());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Membership", "BlockEmails");
        }
    }
}
