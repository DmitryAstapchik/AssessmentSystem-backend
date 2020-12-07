using System.Data.Entity.Migrations;

namespace AssessmentSystem.Data.Access.Migrations
{
    public partial class AddModuleVersionId : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TestAssemblyInfoes", "ModuleVersionId", c => c.Guid(nullable: false));
        }

        public override void Down()
        {
            DropColumn("dbo.TestAssemblyInfoes", "ModuleVersionId");
        }
    }
}
