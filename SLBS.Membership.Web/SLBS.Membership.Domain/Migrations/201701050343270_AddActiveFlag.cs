namespace SLBS.Membership.Domain.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class AddActiveFlag : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Adult", "IsActive", c => c.Boolean(nullable: false, defaultValue:true));
            AddColumn("dbo.Membership", "IsActive", c => c.Boolean(nullable: false, defaultValue: true));
            AddColumn("dbo.Child", "IsActive", c => c.Boolean(nullable: false, defaultValue: true));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Child", "IsActive");
            DropColumn("dbo.Membership", "IsActive");
            DropColumn("dbo.Adult", "IsActive");
        }
    }
}
