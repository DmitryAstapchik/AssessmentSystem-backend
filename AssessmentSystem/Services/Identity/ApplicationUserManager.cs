using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using AssessmentSystem.Identity;
using AssessmentSystem.Services.Security;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;

namespace AssessmentSystem.Services.Identity
{
    /// <summary>
    /// Represents an application user manager.
    /// </summary>
    public class ApplicationUserManager : UserManager<ApplicationUser>, IApplicationUserManager
    {
        private readonly ApplicationIdentityDbContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationUserManager"/> class.
        /// </summary>
        /// <param name="store">The IUserStore is responsible for commiting changes via the UpdateAsync/CreateAsync methods.</param>
        /// <param name="context"><see cref="DbContext"/> for ASP.NET Identity infrastructure.</param>
        public ApplicationUserManager(IUserStore<ApplicationUser> store, ApplicationIdentityDbContext context)
            : base(store)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        /// <summary>
        /// Creates an instance of <see cref="ApplicationUserManager"/>.
        /// </summary>
        /// <param name="options">Configuration options for a IdentityFactoryMiddleware.</param>
        /// <param name="context">An instace of <see cref="IOwinContext"/></param>
        /// <returns>An instance of <see cref="ApplicationUserManager"/></returns>
        public static ApplicationUserManager Create(IdentityFactoryOptions<ApplicationUserManager> options, IOwinContext context)
        {
            var dbContext = context.Get<ApplicationIdentityDbContext>();
            var manager = new ApplicationUserManager(new UserStore<ApplicationUser>(dbContext), dbContext);

            // Configure validation logic for usernames
            manager.UserValidator = new UserValidator<ApplicationUser>(manager)
            {
                AllowOnlyAlphanumericUserNames = false,
                RequireUniqueEmail = true
            };

            // Configure validation logic for passwords
            manager.PasswordValidator = new PasswordValidator
            {
                RequiredLength = 1,
                RequireNonLetterOrDigit = false,
                RequireDigit = false,
                RequireLowercase = false,
                RequireUppercase = false,
            };

            // Configure user lockout defaults
            manager.UserLockoutEnabledByDefault = true;
            manager.DefaultAccountLockoutTimeSpan = TimeSpan.FromMinutes(5);
            manager.MaxFailedAccessAttemptsBeforeLockout = 5;

            // Register two factor authentication providers. This application uses Phone and Emails as a step of receiving a code for verifying the user
            // You can write your own provider and plug it in here.
            manager.RegisterTwoFactorProvider("Phone Code", new PhoneNumberTokenProvider<ApplicationUser>
            {
                MessageFormat = "Your security code is {0}"
            });
            manager.RegisterTwoFactorProvider("Email Code", new EmailTokenProvider<ApplicationUser>
            {
                Subject = "Security Code",
                BodyFormat = "Your security code is {0}"
            });
            manager.EmailService = new EmailService();
            manager.SmsService = new SmsService();
            var dataProtectionProvider = options.DataProtectionProvider;
            if (dataProtectionProvider != null)
            {
                manager.UserTokenProvider =
                    new DataProtectorTokenProvider<ApplicationUser>(dataProtectionProvider.Create("ASP.NET Identity"));
            }

            return manager;
        }

        /// <summary>
        /// Gets a user by identifier.
        /// </summary>
        /// <param name="userId">Identifier of target user.</param>
        /// <returns>An instance of <see cref="ApplicationUser"/>.</returns>
        public ApplicationUser FindById(string userId)
        {
            return UserManagerExtensions.FindById(this, userId);
        }

        /// <summary>
        /// Adds a user profile.
        /// </summary>
        /// <param name="userId">A user identifier.</param>
        /// <param name="userProfile">A <see cref="UserProfileInfo"/>.</param>
        /// <returns>A <see cref="Task"/>.</returns>
        public async Task AddProfileAsync(Guid userId, UserProfileInfo userProfile)
        {
            if (userProfile == null)
            {
                throw new ArgumentNullException(nameof(userProfile));
            }

            var user = await Users.SingleOrDefaultAsync(u => userId == u.DomainId).ConfigureAwait(false);
            if (user == null)
            {
                throw new UserNotFoundException(userId.ToString());
            }

            var profile = await _context.UserProfiles.SingleOrDefaultAsync(u => user.Id == u.UserId).ConfigureAwait(false);

            if (profile != null)
            {
                _context.UserProfiles.Remove(profile);
            }

            _context.UserProfiles.Add(new ApplicationUserProfile
            {
                UserId = user.Id,
                FirstName = userProfile.FirstName,
                LastName = userProfile.LastName
            });

            await _context.SaveChangesAsync().ConfigureAwait(false);
        }

        /// <summary>
        /// Gets a user profiles.
        /// </summary>
        /// <param name="start">Number of the first element.</param>
        /// <param name="amount">Amount of elements.</param>
        /// <param name="roleName">User role name.</param>
        /// <param name="lastNamePart">Part of a lastname of a user to search users by lastname.</param>
        /// <param name="sortCriteria">Criteria of user sorting.</param>
        /// <param name="isDescending">Determines whether sorting is descending.</param>
        /// <returns>A <see cref="IEnumerable{UserProfileInfo}"/>.</returns>
        public IEnumerable<UserProfileInfo> GetProfiles(
            int start,
            int amount,
            string roleName = null,
            string lastNamePart = null,
            string sortCriteria = nameof(UserProfileInfo.LastName),
            bool isDescending = false)
        {
            var profiles = _context.UserProfiles.Where(profile => !profile.User.IsSoftDeleted);
            if (!string.IsNullOrWhiteSpace(roleName))
            {
                profiles = profiles.Where(p => p.User.Roles.Any(
                    r => r.RoleId.Equals(_context.Roles.FirstOrDefault(s => s.Name.Equals(roleName)).Id)));
            }

            if (!string.IsNullOrWhiteSpace(lastNamePart))
            {
                profiles = profiles.Where(profile => profile.LastName.Contains(lastNamePart));
            }

            profiles = SortProfiles(profiles, sortCriteria, isDescending);
            var result = profiles
                .Skip(start)
                .Take(amount)
                .AsEnumerable();

            return result.Select(MapUserProfile);
        }

        /// <summary>
        /// Gets a user profile.
        /// </summary>
        /// <param name="userId">A user identifier.</param>
        /// <returns>A <see cref="Task{UserProfileInfo}"/>.</returns>
        public async Task<UserProfileInfo> GetProfileAsync(Guid userId)
        {
            var user = await _context.Users.SingleOrDefaultAsync(u => userId == u.DomainId);
            if (user == null || user.IsSoftDeleted)
            {
                throw new UserNotFoundException(userId.ToString());
            }

            var profile = await _context.UserProfiles.SingleOrDefaultAsync(p => user.Id == p.UserId);

            return profile == null ? throw new UserProfileNotFoundException(userId.ToString()) : MapUserProfile(profile);
        }

        /// <summary>
        /// Removes a user profile.
        /// </summary>
        /// <param name="userId">A user identifier.</param>
        /// <returns>A <see cref="Task"/>.</returns>
        public async Task RemoveProfileAsync(Guid userId)
        {
            var user = await Users.SingleOrDefaultAsync(u => userId == u.DomainId);
            if (user == null)
            {
                throw new UserNotFoundException(userId.ToString());
            }

            var profile = await _context.UserProfiles.SingleOrDefaultAsync(p => user.Id == p.UserId);

            if (profile != null)
            {
                _context.UserProfiles.Remove(profile);
                await _context.SaveChangesAsync();
            }
        }

        /// <summary>
        /// Sets a user profile.
        /// </summary>
        /// <param name="userId">A user identifier.</param>
        /// <param name="userProfile">A <see cref="UserProfileInfo"/>.</param>
        /// <returns>A <see cref="Task"/>.</returns>
        public async Task SetProfileAsync(Guid userId, UserProfileInfo userProfile)
        {
            if (userProfile == null)
            {
                throw new ArgumentNullException(nameof(userProfile));
            }

            var user = await Users.SingleOrDefaultAsync(u => userId == u.DomainId);
            if (user == null)
            {
                throw new UserNotFoundException(userId.ToString());
            }

            var profile = await _context.UserProfiles.SingleOrDefaultAsync(p => user.Id == p.UserId);
            if (profile == null)
            {
                throw new UserProfileNotFoundException(userId.ToString());
            }

            profile.FirstName = userProfile.FirstName;
            profile.LastName = userProfile.LastName;

            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Gets a user accounts.
        /// </summary>
        /// <param name="start">Number of the first element.</param>
        /// <param name="amount">Amount of elements.</param>
        /// <param name="roleName">Name of a target user role.</param>
        /// <param name="emailPart">Part of an email of a user to search users by email.</param>
        /// <param name="sortCriteria">Criteria of user sorting.</param>
        /// <param name="isDescending">Determines whether sorting is descending.</param>
        /// <returns>A <see cref="IEnumerable{UserAccountInfo}"/>.</returns>
        public IEnumerable<UserAccountInfo> GetAccounts(
            int start,
            int amount,
            string roleName = null,
            string emailPart = null,
            string sortCriteria = nameof(UserAccountInfo.Email),
            bool isDescending = false)
        {
            var users = _context.Users.Where(user => !user.IsSoftDeleted);
            if (!string.IsNullOrWhiteSpace(roleName))
            {
                users = users.Where(u => u.Roles.Any(r => r.RoleId.Equals(_context.Roles.FirstOrDefault(s => s.Name.Equals(roleName)).Id)));
            }

            if (!string.IsNullOrWhiteSpace(emailPart))
            {
                users = users.Where(user => user.Email.Contains(emailPart));
            }

            users = SortUsers(users, sortCriteria, isDescending);
            var result = users
                .Skip(start)
                .Take(amount)
                .AsEnumerable();

            return result.Select(MapUserAccount);
        }

        /// <summary>
        /// Gets a count of user accounts.
        /// </summary>
        /// <param name="roleName">Name of users role.</param>
        /// <param name="emailPart">Part of an email of a user to search users by email.</param>
        /// <returns>Count of user accounts.</returns>
        public int GetAccountsCount(string roleName = null, string emailPart = null)
        {
            var users = _context.Users.Where(user => !user.IsSoftDeleted);
            if (!string.IsNullOrWhiteSpace(roleName))
            {
                users = users.Where(user => user.Roles.Any(r =>
                    r.RoleId.Equals(_context.Roles.FirstOrDefault(s => s.Name.Equals(roleName)).Id)));
            }

            if (!string.IsNullOrWhiteSpace(emailPart))
            {
                users = users.Where(user => user.Email.Contains(emailPart));
            }

            return users.Count();
        }

        /// <summary>
        /// Gets a user account.
        /// </summary>
        /// <param name="userId">A user identifier.</param>
        /// <returns>A <see cref="Task{UserAccountInfo}"/>.</returns>
        public async Task<UserAccountInfo> GetAccountAsync(string userId)
        {
            if (!Guid.TryParse(userId, out var res) || res == Guid.Empty)
            {
                throw new ArgumentException($@"{nameof(userId)} is not a valid Guid.", nameof(userId));
            }

            var user = await _context.Users.SingleOrDefaultAsync(u => userId == u.Id);
            return user == null || user.IsSoftDeleted ? throw new UserNotFoundException(userId) : MapUserAccount(user);
        }

        /// <summary>
        /// Changes value of IsActive field of user.
        /// </summary>
        /// <param name="userId">A user identifier.</param>
        /// <returns>A <see cref="Task"/>.</returns>
        public async Task ChangeIsActiveAsync(string userId)
        {
            if (!Guid.TryParse(userId, out var guid) || guid == Guid.Empty)
            {
                throw new ArgumentException($@"{nameof(userId)} is not a valid Guid.", nameof(userId));
            }

            var user = await _context.Users.SingleOrDefaultAsync(u => userId == u.Id);
            if (user == null)
            {
                throw new UserNotFoundException(userId);
            }

            if (user.Roles.Any(r =>
                r.RoleId.Equals(_context.Roles.FirstOrDefault(s => s.Name.Equals(GlobalInfo.Admin))?.Id)))
            {
                return;
            }

            user.IsActive = !user.IsActive;
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Gets names of roles of a user.
        /// </summary>
        /// <param name="userEmail">Email address of target user.</param>
        /// <param name="userPassword">Password of target user.</param>
        /// <returns>Names of roles of target user.</returns>
        public IEnumerable<string> GetUserRolesByCredentials(string userEmail, string userPassword)
        {
            if (string.IsNullOrWhiteSpace(userEmail))
            {
                throw new ArgumentException($@"{nameof(userEmail)} can't be empty.", nameof(userEmail));
            }

            if (string.IsNullOrWhiteSpace(userPassword))
            {
                throw new ArgumentException($@"{nameof(userPassword)} can't be empty.", nameof(userPassword));
            }

            var user = _context.Users?.FirstOrDefault(usr => usr.Email.Equals(userEmail, StringComparison.Ordinal));
            var isPasswordValid = PasswordHasher
                .VerifyHashedPassword(user?.PasswordHash ?? string.Empty, userPassword) == PasswordVerificationResult.Success;
            if (!isPasswordValid)
            {
                user = null;
            }

            return GetUserRolesNames(user);
        }

        /// <summary>
        /// Confirms email by admin.
        /// </summary>
        /// <param name="userId">Id of a target user.</param>
        /// <returns>A <see cref="Task"/>.</returns>
        public async Task ConfirmEmailByAdminAsync(Guid userId)
        {
            var user = await FindByIdAsync(userId.ToString());
            if (user == null)
            {
                throw new UserNotFoundException(userId.ToString());
            }

            user.EmailConfirmed = true;
            _context.Entry(user).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        private static UserProfileInfo MapUserProfile(ApplicationUserProfile profile)
        {
            return new UserProfileInfo
            {
                Id = profile.User.DomainId,
                FirstName = profile.FirstName,
                LastName = profile.LastName
            };
        }

        private UserAccountInfo MapUserAccount(ApplicationUser user)
        {
            return new UserAccountInfo
            {
                Id = user.Id,
                ProfileId = user.DomainId,
                Email = user.Email,
                Roles = GetUserRolesNames(user),
                IsActive = user.IsActive,
                IsEmailConfirmed = user.EmailConfirmed
            };
        }

        private IEnumerable<string> GetUserRolesNames(ApplicationUser user)
        {
            if (user == null)
            {
                return new string[] { };
            }

            var userRoles = user.Roles.ToArray();
            var roles = _context.Roles.ToArray();
            return userRoles.Select(userRole => roles.FirstOrDefault(role => role.Id == userRole.RoleId)?.Name).ToArray();
        }

        private IQueryable<ApplicationUser> SortUsers(IQueryable<ApplicationUser> users, string sortCriteria, bool isDescending)
        {
            if (sortCriteria.Equals(nameof(UserAccountInfo.IsEmailConfirmed), StringComparison.OrdinalIgnoreCase))
            {
                users = isDescending
                    ? users.OrderByDescending(u => u.EmailConfirmed)
                    : users.OrderBy(u => u.EmailConfirmed);
            }
            else if (sortCriteria.Equals(nameof(UserAccountInfo.IsActive), StringComparison.OrdinalIgnoreCase))
            {
                users = isDescending
                    ? users.OrderByDescending(u => u.IsActive)
                    : users.OrderBy(u => u.IsActive);
            }
            else
            {
                users = isDescending
                    ? users.OrderByDescending(u => u.Email.ToLower())
                    : users.OrderBy(u => u.Email.ToLower());
            }

            return users;
        }

        private IQueryable<ApplicationUserProfile> SortProfiles(
            IQueryable<ApplicationUserProfile> profiles,
            string sortCriteria,
            bool isDescending)
        {
            if (sortCriteria.Equals(nameof(UserProfileInfo.FirstName), StringComparison.OrdinalIgnoreCase))
            {
                profiles = isDescending
                    ? profiles.OrderByDescending(p => p.FirstName.ToLower())
                    : profiles.OrderBy(p => p.FirstName.ToLower());
            }
            else
            {
                profiles = isDescending
                    ? profiles.OrderByDescending(p => p.LastName.ToLower())
                    : profiles.OrderBy(p => p.LastName.ToLower());
            }

            return profiles;
        }
    }
}
