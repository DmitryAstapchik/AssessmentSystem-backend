using System.Data.Entity.Migrations;

namespace AssessmentSystem.Identity.Migrations
{
    /// <summary>
    /// Represents a EF migration: add user profiles.
    /// </summary>
    public partial class AddUserProfiles : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AspNetUserProfiles",
                c => new
                {
                    UserId = c.String(nullable: false, maxLength: 128),
                    FirstName = c.String(),
                    LastName = c.String(),
                })
                .PrimaryKey(t => t.UserId)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .Index(t => t.UserId);
        }

        public override void Down()
        {
            DropForeignKey("dbo.AspNetUserProfiles", "UserId", "dbo.AspNetUsers");
            DropIndex("dbo.AspNetUserProfiles", new[] { "UserId" });
            DropTable("dbo.AspNetUserProfiles");
        }
    }
}
