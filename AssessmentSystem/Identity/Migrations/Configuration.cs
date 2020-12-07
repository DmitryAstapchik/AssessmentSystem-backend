using System;
using System.Data.Entity.Migrations;
using AssessmentSystem.Models;
using AssessmentSystem.Services.Identity;
using AssessmentSystem.Services.Security;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace AssessmentSystem.Identity.Migrations
{
    internal sealed class Configuration : DbMigrationsConfiguration<ApplicationIdentityDbContext>
    {
        public Configuration()
        {
            ContextKey = "ApplicationIdentity";
            AutomaticMigrationsEnabled = false;
            AutomaticMigrationDataLossAllowed = false;
            MigrationsDirectory = @"Identity\Migrations";
            MigrationsNamespace = "AssessmentSystem.Identity.Migrations";
        }

        protected override void Seed(ApplicationIdentityDbContext context)
        {
            var userManager = new ApplicationUserManager(new UserStore<ApplicationUser>(context), context);
            var roleManager = new ApplicationRoleManager(new RoleStore<ApplicationRole>(context));

            userManager.PasswordValidator = new PasswordValidator()
            {
                RequireDigit = false,
                RequireLowercase = false,
                RequireNonLetterOrDigit = false,
                RequireUppercase = false,
                RequiredLength = 1
            };

            userManager.UserValidator = new UserValidator<ApplicationUser>(userManager)
            {
                AllowOnlyAlphanumericUserNames = false,
                RequireUniqueEmail = true
            };

            var adminRole = new ApplicationRole(GlobalInfo.Admin);
            var candidateRole = new ApplicationRole(GlobalInfo.Candidate);
            var coachRole = new ApplicationRole(GlobalInfo.Coach);
            var managerRole = new ApplicationRole(GlobalInfo.Manager);

            if (!roleManager.RoleExistsAsync(GlobalInfo.Admin).Result)
            {
                roleManager.CreateAsync(adminRole).Wait();
                roleManager.CreateAsync(candidateRole).Wait();
                roleManager.CreateAsync(coachRole).Wait();
                roleManager.CreateAsync(managerRole).Wait();
            }

            var admin = new ApplicationUser
            {
                Email = "admin@test.com",
                EmailConfirmed = true,
                UserName = "admin@test.com",
                DomainId = new Guid("B4B6780C-F3BE-4554-BFFD-BCFD18E93A94"),
                IsActive = true
            };
            if (userManager.CreateAsync(admin, "1").Result == IdentityResult.Success)
            {
                userManager.AddToRoleAsync(admin.Id, adminRole.Name).Wait();
                userManager.AddProfileAsync(admin.DomainId, new UserProfileInfo
                {
                    FirstName = "Admin",
                    LastName = "Adminov"
                }).Wait();
            }

            var candidate = new ApplicationUser
            {
                Email = "candidate@test.com",
                EmailConfirmed = true,
                UserName = "candidate@test.com",
                DomainId = new Guid("14620737-9BDF-4B4D-B6B9-01CD1EBE69EB"),
                IsActive = true
            };
            if (userManager.CreateAsync(candidate, "1").Result == IdentityResult.Success)
            {
                userManager.AddToRoleAsync(candidate.Id, candidateRole.Name).Wait();
                userManager.AddProfileAsync(candidate.DomainId, new UserProfileInfo
                {
                    FirstName = "Alexander",
                    LastName = "Pachkalov"
                }).Wait();
            }

            var coach = new ApplicationUser
            {
                Email = "coach@test.com",
                EmailConfirmed = true,
                UserName = "coach@test.com",
                DomainId = new Guid("AF789597-4EA1-4478-85E7-5A7D03D47948"),
                IsActive = true
            };
            if (userManager.CreateAsync(coach, "1").Result == IdentityResult.Success)
            {
                userManager.AddToRoleAsync(coach.Id, coachRole.Name).Wait();
                userManager.AddProfileAsync(coach.DomainId, new UserProfileInfo
                {
                    FirstName = "Coach",
                    LastName = "Coachev"
                }).Wait();
            }

            var manager = new ApplicationUser
            {
                Email = "manager@test.com",
                EmailConfirmed = true,
                UserName = "manager@test.com",
                DomainId = new Guid("4eabfa0e-118e-4131-8ec9-1dbd29f68d3a"),
                IsActive = true
            };
            if (userManager.CreateAsync(manager, "1").Result == IdentityResult.Success)
            {
                userManager.AddToRoleAsync(manager.Id, managerRole.Name).Wait();
                userManager.AddProfileAsync(manager.DomainId, new UserProfileInfo
                {
                    FirstName = "Manager",
                    LastName = "Managerov"
                }).Wait();
            }
        }
    }
}