namespace SLBS.Membership.Domain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Adult",
                c => new
                    {
                        AdultId = c.Int(nullable: false, identity: true),
                        MembershipId = c.Int(nullable: false),
                        FullName = c.String(),
                        Address = c.String(),
                        Phone = c.String(),
                        Email = c.String(),
                        Role = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.AdultId)
                .ForeignKey("dbo.Membership", t => t.MembershipId, cascadeDelete: true)
                .Index(t => t.MembershipId);
            
            CreateTable(
                "dbo.Membership",
                c => new
                    {
                        MembershipId = c.Int(nullable: false, identity: true),
                        MembershipNumber = c.String(nullable: false, maxLength: 60),
                        ContactName = c.String(),
                        PaidUpTo = c.DateTime(),
                    })
                .PrimaryKey(t => t.MembershipId)
                .Index(t => t.MembershipNumber, unique: true);
            
            CreateTable(
                "dbo.Child",
                c => new
                    {
                        ChildId = c.Int(nullable: false, identity: true),
                        MembershipId = c.Int(nullable: false),
                        FullName = c.String(),
                        ClassLevel = c.Int(nullable: false),
                        MediaConsent = c.Boolean(),
                        AmbulanceCover = c.Boolean(),
                    })
                .PrimaryKey(t => t.ChildId)
                .ForeignKey("dbo.Membership", t => t.MembershipId, cascadeDelete: true)
                .Index(t => t.MembershipId);
            
            CreateTable(
                "dbo.MembershipComment",
                c => new
                    {
                        MembershipCommentId = c.Int(nullable: false, identity: true),
                        MembershipId = c.Int(nullable: false),
                        Comment = c.String(),
                        CommentedOn = c.DateTime(nullable: false),
                        CreatedBy = c.String(),
                        CurrentStatus = c.String(),
                        StatusUpdatedOn = c.DateTime(nullable: false),
                        StatusUpdatedBy = c.String(),
                    })
                .PrimaryKey(t => t.MembershipCommentId)
                .ForeignKey("dbo.Membership", t => t.MembershipId, cascadeDelete: true)
                .Index(t => t.MembershipId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Adult", "MembershipId", "dbo.Membership");
            DropForeignKey("dbo.MembershipComment", "MembershipId", "dbo.Membership");
            DropForeignKey("dbo.Child", "MembershipId", "dbo.Membership");
            DropIndex("dbo.MembershipComment", new[] { "MembershipId" });
            DropIndex("dbo.Child", new[] { "MembershipId" });
            DropIndex("dbo.Membership", new[] { "MembershipNumber" });
            DropIndex("dbo.Adult", new[] { "MembershipId" });
            DropTable("dbo.MembershipComment");
            DropTable("dbo.Child");
            DropTable("dbo.Membership");
            DropTable("dbo.Adult");
        }
    }
}
