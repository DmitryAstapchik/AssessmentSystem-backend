using System.Data.Entity.Migrations;

namespace AssessmentSystem.Data.Access.Migrations
{
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.CandidateExerciseResults",
                c => new
                {
                    Id = c.Int(nullable: false, identity: true),
                    CandidateExerciseId = c.Int(nullable: false),
                    Score = c.Int(nullable: false),
                    CreatorId = c.Guid(nullable: false),
                    ModifierId = c.Guid(nullable: false),
                    Code = c.String(),
                    UsedTipsNumber = c.Int(),
                    IsCompleted = c.Boolean(),
                    StartDate = c.DateTime(),
                    EndDate = c.DateTime(),
                    Discriminator = c.String(nullable: false, maxLength: 128),
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.CandidateExercises", t => t.CandidateExerciseId, cascadeDelete: true)
                .Index(t => t.CandidateExerciseId);

            CreateTable(
                "dbo.CandidateExercises",
                c => new
                {
                    Id = c.Int(nullable: false, identity: true),
                    Name = c.String(nullable: false),
                    Subject = c.String(nullable: false),
                    Description = c.String(nullable: false),
                    MaximumScore = c.Int(nullable: false),
                    TimeSeconds = c.Long(),
                    IsSoftDeleted = c.Boolean(nullable: false),
                    CodeTemplate = c.String(),
                    TestMethodId = c.Int(),
                    TestClassId = c.Int(),
                    Discriminator = c.String(nullable: false, maxLength: 128),
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.TestClassInfoes", t => t.TestClassId, cascadeDelete: true)
                .ForeignKey("dbo.TestMethodInfoes", t => t.TestMethodId)
                .Index(t => t.TestMethodId)
                .Index(t => t.TestClassId);

            CreateTable(
                "dbo.TestClassInfoes",
                c => new
                {
                    Id = c.Int(nullable: false, identity: true),
                    Name = c.String(),
                    AssemblyInfoId = c.Int(nullable: false),
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.TestAssemblyInfoes", t => t.AssemblyInfoId, cascadeDelete: true)
                .Index(t => t.AssemblyInfoId);

            CreateTable(
                "dbo.TestAssemblyInfoes",
                c => new
                {
                    Id = c.Int(nullable: false, identity: true),
                    AssemblyName = c.String(),
                    Data = c.Binary(),
                    IsSoftDeleted = c.Boolean(nullable: false),
                })
                .PrimaryKey(t => t.Id);

            CreateTable(
                "dbo.TestMethodInfoes",
                c => new
                {
                    Id = c.Int(nullable: false, identity: true),
                    Name = c.String(),
                    ClassInfoId = c.Int(nullable: false),
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.TestClassInfoes", t => t.ClassInfoId, cascadeDelete: true)
                .Index(t => t.ClassInfoId);

            CreateTable(
                "dbo.CandidateTaskTips",
                c => new
                {
                    Id = c.Int(nullable: false, identity: true),
                    Text = c.String(nullable: false),
                    CandidateTaskId = c.Int(nullable: false),
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.CandidateExercises", t => t.CandidateTaskId, cascadeDelete: true)
                .Index(t => t.CandidateTaskId);

            CreateTable(
                "dbo.Invites",
                c => new
                {
                    Id = c.Int(nullable: false, identity: true),
                    Token = c.Guid(nullable: false),
                    Email = c.String(nullable: false),
                    RoleName = c.String(nullable: false),
                    ExpirationDate = c.DateTime(nullable: false),
                    UserId = c.String(),
                })
                .PrimaryKey(t => t.Id);
        }

        public override void Down()
        {
            DropForeignKey("dbo.CandidateExerciseResults", "CandidateExerciseId", "dbo.CandidateExercises");
            DropForeignKey("dbo.CandidateTaskTips", "CandidateTaskId", "dbo.CandidateExercises");
            DropForeignKey("dbo.CandidateExercises", "TestMethodId", "dbo.TestMethodInfoes");
            DropForeignKey("dbo.CandidateExercises", "TestClassId", "dbo.TestClassInfoes");
            DropForeignKey("dbo.TestMethodInfoes", "ClassInfoId", "dbo.TestClassInfoes");
            DropForeignKey("dbo.TestClassInfoes", "AssemblyInfoId", "dbo.TestAssemblyInfoes");
            DropIndex("dbo.CandidateTaskTips", new[] { "CandidateTaskId" });
            DropIndex("dbo.TestMethodInfoes", new[] { "ClassInfoId" });
            DropIndex("dbo.TestClassInfoes", new[] { "AssemblyInfoId" });
            DropIndex("dbo.CandidateExercises", new[] { "TestClassId" });
            DropIndex("dbo.CandidateExercises", new[] { "TestMethodId" });
            DropIndex("dbo.CandidateExerciseResults", new[] { "CandidateExerciseId" });
            DropTable("dbo.Invites");
            DropTable("dbo.CandidateTaskTips");
            DropTable("dbo.TestMethodInfoes");
            DropTable("dbo.TestAssemblyInfoes");
            DropTable("dbo.TestClassInfoes");
            DropTable("dbo.CandidateExercises");
            DropTable("dbo.CandidateExerciseResults");
        }
    }
}
