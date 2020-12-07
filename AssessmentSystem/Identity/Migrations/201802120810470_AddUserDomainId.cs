using System.Data.Entity.Migrations;

namespace AssessmentSystem.Identity.Migrations
{
    /// <summary>
    /// Represents an EF migration to add DomainId column to users table.
    /// </summary>
    public partial class AddUserDomainId : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "DomainId", c => c.Guid(nullable: false));
        }

        public override void Down()
        {
            DropColumn("dbo.AspNetUsers", "DomainId");
        }
    }
}
