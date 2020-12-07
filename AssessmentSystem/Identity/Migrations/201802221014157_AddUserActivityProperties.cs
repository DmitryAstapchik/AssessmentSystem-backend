using System.Data.Entity.Migrations;

namespace AssessmentSystem.Identity.Migrations
{
    /// <summary>
    /// Represents a database migration.
    /// </summary>
    public partial class AddUserActivityProperties : DbMigration
    {
        /// <summary>
        /// Operations to be performed during the upgrade process.
        /// </summary>
        /// <remarks>Represents an ALTER database operation.</remarks>
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "IsActive", c => c.Boolean(nullable: false));
            AddColumn("dbo.AspNetUsers", "IsSoftDeleted", c => c.Boolean(nullable: false));
        }

        /// <summary>
        /// Operations to be performed during the downgrade process.
        /// </summary>
        /// <remarks>Represents an ALTER database operation.</remarks>
        public override void Down()
        {
            DropColumn("dbo.AspNetUsers", "IsSoftDeleted");
            DropColumn("dbo.AspNetUsers", "IsActive");
        }
    }
}
