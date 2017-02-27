namespace SLBS.Membership.Domain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChildComments : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ChildComment",
                c => new
                    {
                        ChildCommentId = c.Int(nullable: false, identity: true),
                        ChildId = c.Int(nullable: false),
                        Comment = c.String(),
                        CreatedOn = c.DateTime(nullable: false, defaultValue: DateTime.Now),
                        CreatedBy = c.String(),
                        ModifiedOn = c.DateTime(),
                        ModifiedBy = c.String(),
                        Membership_MembershipId = c.Int(),
                    })
                .PrimaryKey(t => t.ChildCommentId)
                .ForeignKey("dbo.Child", t => t.ChildId, cascadeDelete: true)
                .ForeignKey("dbo.Membership", t => t.Membership_MembershipId)
                .Index(t => t.ChildId)
                .Index(t => t.Membership_MembershipId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ChildComment", "Membership_MembershipId", "dbo.Membership");
            DropForeignKey("dbo.ChildComment", "ChildId", "dbo.Child");
            DropIndex("dbo.ChildComment", new[] { "Membership_MembershipId" });
            DropIndex("dbo.ChildComment", new[] { "ChildId" });
            DropTable("dbo.ChildComment");
        }
    }
}
