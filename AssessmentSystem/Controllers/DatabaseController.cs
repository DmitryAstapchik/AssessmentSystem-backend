#if DEBUG
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Web.Http;
using System.Web.Http.Description;
using AssessmentSystem.Data.Access.Context;
using AssessmentSystem.Data.Access.ExerciseExecutor;
using AssessmentSystem.Data.Access.ExerciseManagement;
using AssessmentSystem.Data.Access.UserManagement;
using AssessmentSystem.Identity;
using AssessmentSystem.Services.Identity;
using AssessmentSystem.Services.Security;
using Microsoft.AspNet.Identity;
using Swashbuckle.Swagger.Annotations;

namespace AssessmentSystem.Controllers
{
    /// <summary>
    /// Represents controller that is responsible for seeding and unseeding a database.
    /// </summary>
    [Authorize(Roles = GlobalInfo.Admin)]
    [RoutePrefix("api/v1/database")]
    public class DatabaseController : ApiController
    {
        private readonly ApplicationDbContext _appDbContext;
        private readonly ApplicationUserManager _userManager;
        private readonly ApplicationIdentityDbContext _identityDbContext;
        private readonly string _codeTemplatesPath = @"C:\Users\dmitriy\source\repos\AssessmentSystem\AssessmentSystem.Examples";

        /// <summary>
        /// Initializes a new instance of the <see cref="DatabaseController"/> class.
        /// </summary>
        /// <param name="appDbContext">Instance of <see cref="ApplicationDbContext"/>.</param>
        /// <param name="userManager">Instance of <see cref="ApplicationUserManager"/>.</param>
        /// <param name="identityDbContext">Instance of <see cref="ApplicationIdentityDbContext"/>.</param>
        public DatabaseController(
            ApplicationDbContext appDbContext,
            ApplicationUserManager userManager,
            ApplicationIdentityDbContext identityDbContext)
        {
            _appDbContext = appDbContext ?? throw new ArgumentNullException(nameof(appDbContext));
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _identityDbContext = identityDbContext ?? throw new ArgumentNullException(nameof(identityDbContext));
        }

        /// <summary>
        /// Seeds a database with data.
        /// </summary>
        /// <returns>Status of database seeding.</returns>
        [HttpPost]
        [Route("seed/all")]
        [ResponseType(typeof(string))]
        [SwaggerResponse(HttpStatusCode.InternalServerError, Description = "Internal server error")]
        public IHttpActionResult SeedAll()
        {
            AddCandidates();
            AddTasks();
            AddTests();
            AddTaskResults();
            AddInvites();

            return Ok("Database was successfully seeded.");
        }

        /// <summary>
        /// Seeds a database with candidates.
        /// </summary>
        /// <param name="amount">The amount of candidates to seed.</param>
        /// <returns>Status of database seeding.</returns>
        /// <remarks>By default, an amount is 60.</remarks>
        // POST /api/v1/database/seed/candidates?amount=60
        [HttpPost]
        [Route("seed/candidates")]
        [SwaggerResponse(HttpStatusCode.InternalServerError, Description = "Internal server error")]
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "Parameter values are not correct")]
        public IHttpActionResult SeedCandidates([FromUri] int amount = 60)
        {
            try
            {
                if (amount < 1)
                {
                    return BadRequest($"{nameof(amount)} must be greater than 0.");
                }

                AddCandidates(amount);
            }
            catch (Exception)
            {
                return InternalServerError();
            }

            return Ok("Database was successfully seeded.");
        }

        /// <summary>
        /// Seeds a database with invites.
        /// </summary>
        /// <param name="amount">The amount of invites to seed.</param>
        /// <returns>Status of database seeding.</returns>
        /// <remarks>By default, an amount is 60.</remarks>
        // POST /api/v1/database/seed/invites?amount=60
        [HttpPost]
        [Route("seed/invites")]
        [SwaggerResponse(HttpStatusCode.InternalServerError, Description = "Internal server error")]
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "Parameter values are not correct")]
        public IHttpActionResult SeedInvites(int amount = 60)
        {
            try
            {
                if (amount < 1)
                {
                    return BadRequest($"{nameof(amount)} must be greater than 0.");
                }

                AddInvites(amount);
            }
            catch (Exception)
            {
                return InternalServerError();
            }

            return Ok("Database was successfully seeded.");
        }

        /// <summary>
        /// Seeds a database with candidate tasks.
        /// </summary>
        /// <returns>Status of database seeding.</returns>
        // POST /api/v1/database/seed/tasks
        [HttpPost]
        [Route("seed/tasks")]
        [SwaggerResponse(HttpStatusCode.NoContent, Description = "Database was successfully seeded.")]
        [SwaggerResponse(HttpStatusCode.InternalServerError, Description = "Internal server error")]
        public IHttpActionResult SeedCandidateTasks()
        {
            AddTasks();
            return StatusCode(HttpStatusCode.NoContent);
        }

        /// <summary>
        /// Seeds a database with candidate tests.
        /// </summary>
        /// <returns>Status of database seeding.</returns>
        // POST /api/v1/database/seed/tests
        [HttpPost]
        [Route("seed/tests")]
        [SwaggerResponse(HttpStatusCode.NoContent, Description = "Database was successfully seeded.")]
        [SwaggerResponse(HttpStatusCode.InternalServerError, Description = "Internal server error")]
        public IHttpActionResult SeedCandidateTests()
        {
            AddTests();
            return StatusCode(HttpStatusCode.NoContent);
        }

        /// <summary>
        /// Seeds a database with candidate tasks results.
        /// </summary>
        /// <returns>Status of database seeding.</returns>
        /// <remarks>Note: candidates and candidate tasks must be already seeded.</remarks>
        // POST /api/v1/database/seed/taskResults
        [HttpPost]
        [Route("seed/taskResults")]
        [SwaggerResponse(HttpStatusCode.InternalServerError, Description = "Internal server error")]
        public IHttpActionResult SeedTaskResults()
        {
            AddTaskResults();
            return Ok("Database was successfully seeded.");
        }

        /// <summary>
        /// Removes any data from database.
        /// </summary>
        /// <returns>Status of database unseeding.</returns>
        [HttpDelete]
        [Route("unseed/all")]
        [ResponseType(typeof(IHttpActionResult))]
        [SwaggerResponse(HttpStatusCode.NoContent, Description = "Database was successfully seeded.")]
        [SwaggerResponse(HttpStatusCode.InternalServerError, Description = "Internal server error")]
        public IHttpActionResult UnseedAll()
        {
            CleanCandidates();
            CleanTasks();
            CleanTests();
            CleanTaskResults();
            CleanInvites();

            return StatusCode(HttpStatusCode.NoContent);
        }

        /// <summary>
        /// Removes candidates from database.
        /// </summary>
        /// <returns>Status of database unseeding.</returns>
        [HttpDelete]
        [Route("unseed/candidates")]
        [SwaggerResponse(HttpStatusCode.InternalServerError, Description = "Internal server error")]
        public IHttpActionResult UnseedCandidates()
        {
            try
            {
                CleanCandidates();
            }
            catch (Exception)
            {
                return InternalServerError();
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        /// <summary>
        /// Removes invites from database.
        /// </summary>
        /// <returns>Status of database unseeding.</returns>
        [HttpDelete]
        [Route("unseed/invites")]
        [SwaggerResponse(HttpStatusCode.InternalServerError, Description = "Internal server error")]
        public IHttpActionResult UnseedInvites()
        {
            try
            {
                CleanInvites();
            }
            catch (Exception)
            {
                return InternalServerError();
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        /// <summary>
        /// Removes candidate tasks from database.
        /// </summary>
        /// <returns>Status of database unseeding.</returns>
        [HttpDelete]
        [Route("unseed/tasks")]
        [SwaggerResponse(HttpStatusCode.NoContent, Description = "Tasks were successfully deleted.")]
        [SwaggerResponse(HttpStatusCode.InternalServerError, Description = "Internal server error")]
        public IHttpActionResult UnseedCandidateTasks()
        {
            CleanTasks();
            return StatusCode(HttpStatusCode.NoContent);
        }

        /// <summary>
        /// Removes candidate tests from database.
        /// </summary>
        /// <returns>Status of database unseeding.</returns>
        [HttpDelete]
        [Route("unseed/tests")]
        [SwaggerResponse(HttpStatusCode.NoContent, Description = "Tests were successfully deleted.")]
        [SwaggerResponse(HttpStatusCode.InternalServerError, Description = "Internal server error")]
        public IHttpActionResult UnseedCandidateTests()
        {
            CleanTests();
            return StatusCode(HttpStatusCode.NoContent);
        }

        /// <summary>
        /// Removes candidate task results from database.
        /// </summary>
        /// <returns>Status of database unseeding.</returns>
        [HttpDelete]
        [Route("unseed/taskResults")]
        [SwaggerResponse(HttpStatusCode.InternalServerError, Description = "Internal server error")]
        public IHttpActionResult UnseedCandidateTaskResults()
        {
            CleanTaskResults();
            return StatusCode(HttpStatusCode.NoContent);
        }

        private void AddCandidates(int amount = 60)
        {
            var testCandidates = new Bogus.Faker<ApplicationUser>()
                .RuleFor(i => i.Email, f => f.Internet.ExampleEmail())
                .RuleFor(i => i.UserName, (f, i) => i.Email)
                .RuleFor(i => i.DomainId, Guid.NewGuid)
                .RuleFor(i => i.IsActive, true)
                .RuleFor(i => i.EmailConfirmed, true);

            var userProfiles = new Bogus.Faker<UserProfileInfo>()
                .RuleFor(i => i.FirstName, f => f.Name.FirstName())
                .RuleFor(i => i.LastName, f => f.Name.LastName())
                .Generate(amount);

            var candidateRole = _identityDbContext.Roles.FirstOrDefault(role =>
                role.Name.Equals(GlobalInfo.Candidate, StringComparison.Ordinal));

            int j = 0;
            foreach (var candidate in testCandidates.Generate(amount))
            {
                if (_userManager.CreateAsync(candidate, "1").Result == IdentityResult.Success)
                {
                    _userManager.AddToRoleAsync(candidate.Id, candidateRole.Name).Wait();
                    _userManager.AddProfileAsync(candidate.DomainId, userProfiles[j++]).Wait();
                }
            }

            _identityDbContext.SaveChanges();
            _appDbContext.SaveChanges();
        }

        private void AddInvites(int amount = 60)
        {
            var roles = new[] { GlobalInfo.Coach, GlobalInfo.Manager };
            var testInvites = new Bogus.Faker<Invite>()
                .RuleFor(i => i.Email, f => f.Internet.ExampleEmail())
                .RuleFor(i => i.RoleName, f => f.PickRandom(roles))
                .RuleFor(i => i.Token, Guid.NewGuid)
                .RuleFor(i => i.ExpiredDate, f => f.PickRandomParam(f.Date.Past(), f.Date.Recent(), f.Date.Soon(2)))
                .RuleFor(i => i.UserId, f => f.PickRandomParam(null, Guid.NewGuid().ToString()));

            _appDbContext.Invites.AddRange(testInvites.Generate(amount));
            _appDbContext.SaveChanges();
        }

        private void AddTasks()
        {
            string codeFilePath = Path.GetFullPath($"{_codeTemplatesPath}/Messenger.cs");
            string codeTemplate = File.ReadAllText(codeFilePath);
            var messenger = new Task
            {
                Name = "Messenger",
                TestClass = _appDbContext.TestClasses.FirstOrDefault(_ =>
                    _.Name.Equals("AssessmentSystem.ExerciseVerification.StackAndQueueTest", StringComparison.Ordinal)),
                TestMethod = _appDbContext.TestMethods.FirstOrDefault(_ =>
                    _.Name.Equals("TestQueueAndStack", StringComparison.Ordinal)),
                Subject = "Data structures",
                MaximumScore = 10,
                Description = "Implement simple messenger.",
                CodeTemplate = codeTemplate
            };

            messenger.Tips.Add(new TaskTip { Text = "Programming language is C#." });
            messenger.Tips.Add(new TaskTip { Text = "The implementation is based on the stack and queue." });
            messenger.Tips.Add(new TaskTip { Text = "There are built-in classes for the stack and queue." });

            _appDbContext.CandidateTasks.Add(messenger);

            codeFilePath = Path.GetFullPath($"{_codeTemplatesPath}/LinkedList.cs");
            codeTemplate = File.ReadAllText(codeFilePath);
            var linkedList = new Task
            {
                Name = "Linked list",
                CodeTemplate = codeTemplate,
                Description = "Implement standard doubly-linked list.",
                MaximumScore = 10,
                Subject = "Data structures",
                TestClass = _appDbContext.TestClasses.FirstOrDefault(_ =>
                    _.Name.Equals("AssessmentSystem.ExerciseVerification.LinkedListTest", StringComparison.Ordinal))
            };

            linkedList.Tips.Add(new TaskTip { Text = "Programming language is C#." });
            linkedList.Tips.Add(new TaskTip { Text = "https://en.wikipedia.org/wiki/Linked_list" });

            _appDbContext.CandidateTasks.Add(linkedList);

            codeFilePath = Path.GetFullPath($"{_codeTemplatesPath}/Calculator.cs");
            codeTemplate = File.ReadAllText(codeFilePath);
            var calculator = new Task
            {
                Name = "Calculator",
                CodeTemplate = codeTemplate,
                Description = "Implement simple calculator.",
                MaximumScore = 5,
                Subject = "Example",
                TestClass = _appDbContext.TestClasses.FirstOrDefault(c =>
                    c.Name.Equals("AssessmentSystem.ExerciseVerification.ExampleForUserTest", StringComparison.Ordinal))
            };

            calculator.Tips.Add(new TaskTip { Text = "Programming language is C#." });
            calculator.Tips.Add(new TaskTip { Text = "Sum = '+' Subst = '-' Divide = '/'" });

            _appDbContext.CandidateTasks.Add(calculator);

            _appDbContext.SaveChanges();
        }

        private void AddTests()
        {
            var test1 = new Test()
            {
                Description = "Тест по основам си-шарп",
                MaximumScore = 10,
                Name = "Простой тест по си-шарп",
                Subject = "C#",
                TimeMinutes = null
            };

            var questions = new TestQuestion[10]
            {
                new TestQuestion
                {
                    Text = "Какой тип переменной используется в коде: int a = 5",
                    AnswerVariants = new TestAnswerVariant[4]
                    {
                        new TestAnswerVariant
                        {
                            Text = "Знаковое 32-бит целое",
                            IsCorrect = true
                        },
                        new TestAnswerVariant
                        {
                            Text = "Знаковое 64-бит целое",
                            IsCorrect = false
                        },
                        new TestAnswerVariant
                        {
                            Text = "Знаковое 8-бит целое",
                            IsCorrect = false
                        },
                        new TestAnswerVariant
                        {
                            Text = "1 байт",
                            IsCorrect = false
                        },
                    }
                },
                new TestQuestion
                {
                    Text = "Что делает оператор «%»",
                    AnswerVariants = new TestAnswerVariant[4]
                    {
                        new TestAnswerVariant
                        {
                            Text = "Возвращает остаток от деления",
                            IsCorrect = true
                        },
                        new TestAnswerVariant
                        {
                            Text = "Возвращает процент от суммы",
                            IsCorrect = false
                        },
                        new TestAnswerVariant
                        {
                            Text = "Возвращает тригонометрическую функцию",
                            IsCorrect = false
                        },
                        new TestAnswerVariant
                        {
                            Text = "Ни чего из выше перечисленного",
                            IsCorrect = false
                        },
                    }
                },
                new TestQuestion
                {
                    Text = "Что сделает программа выполнив следующий код: Console.WriteLine(«Hello, World!»)",
                    AnswerVariants = new TestAnswerVariant[4]
                    {
                        new TestAnswerVariant
                        {
                            Text = "Напишет Hello, World!",
                            IsCorrect = false
                        },
                         new TestAnswerVariant
                        {
                            Text = "Удалит все значения с Hello, World!",
                            IsCorrect = false
                        },
                          new TestAnswerVariant
                        {
                            Text = "Напишет на новой строчке Hello, World!",
                            IsCorrect = true
                        },
                           new TestAnswerVariant
                        {
                            Text = "Вырежет слово Hello, World! из всего текста",
                            IsCorrect = false
                        }
                    }
                },
                new TestQuestion
                {
                    Text = "Как сделать инкрементацию числа",
                    AnswerVariants = new TestAnswerVariant[4]
                    {
                        new TestAnswerVariant
                        {
                            Text = "++",
                            IsCorrect = true
                        },
                        new TestAnswerVariant
                        {
                            Text = "—",
                            IsCorrect = false
                        },
                        new TestAnswerVariant
                        {
                            Text = "%%",
                            IsCorrect = false
                        },
                        new TestAnswerVariant
                        {
                            Text = "!=",
                            IsCorrect = false
                        },
                    }
                },
                new TestQuestion
                {
                    Text = "Как сделать декрементация числа",
                    AnswerVariants = new TestAnswerVariant[4]
                    {
                        new TestAnswerVariant
                        {
                            Text = "—",
                            IsCorrect = true
                        },
                        new TestAnswerVariant
                        {
                            Text = "%%",
                            IsCorrect = false
                        },
                        new TestAnswerVariant
                        {
                            Text = "!=",
                            IsCorrect = false
                        },
                        new TestAnswerVariant
                        {
                            Text = "++",
                            IsCorrect = false
                        },
                    }
                },
                new TestQuestion
                {
                    Text = "Как найти квадратный корень из числа x",
                    AnswerVariants = new TestAnswerVariant[4]
                    {
                        new TestAnswerVariant
                        {
                            Text = "Math.Sqrt(x)",
                            IsCorrect = true
                        },
                        new TestAnswerVariant
                        {
                            Text = "Summ.Koren(x)",
                            IsCorrect = false
                        },
                        new TestAnswerVariant
                        {
                            Text = "Arifmetic.sqrt",
                            IsCorrect = false
                        },
                        new TestAnswerVariant
                        {
                            Text = "Sqrt(x)",
                            IsCorrect = false
                        },
                    }
                },
                new TestQuestion
                {
                    Text = "Обозначение оператора «НЕ»",
                    AnswerVariants = new TestAnswerVariant[4]
                    {
                        new TestAnswerVariant
                        {
                            Text = "!",
                            IsCorrect = true
                        },
                        new TestAnswerVariant
                        {
                            Text = "Not",
                            IsCorrect = false
                        },
                        new TestAnswerVariant
                        {
                            Text = "No",
                            IsCorrect = false
                        },
                        new TestAnswerVariant
                        {
                            Text = "!=",
                            IsCorrect = false
                        },
                    }
                },
                new TestQuestion
                {
                    Text = "Обозначение оператора «ИЛИ»",
                    AnswerVariants = new TestAnswerVariant[4]
                    {
                        new TestAnswerVariant
                        {
                            Text = "||",
                            IsCorrect = true
                        },
                        new TestAnswerVariant
                        {
                            Text = "!",
                            IsCorrect = false
                        },
                        new TestAnswerVariant
                        {
                            Text = "Or",
                            IsCorrect = false
                        },
                        new TestAnswerVariant
                        {
                            Text = "!=",
                            IsCorrect = false
                        },
                    }
                },
                new TestQuestion
                {
                    Text = "Обозначение оператора «И»",
                    AnswerVariants = new TestAnswerVariant[4]
                    {
                        new TestAnswerVariant
                        {
                            Text = "&&",
                            IsCorrect = true
                        },
                        new TestAnswerVariant
                        {
                            Text = "and",
                            IsCorrect = false
                        },
                        new TestAnswerVariant
                        {
                            Text = "&",
                            IsCorrect = false
                        },
                        new TestAnswerVariant
                        {
                            Text = "Все перечисленные",
                            IsCorrect = false
                        },
                    }
                },
                new TestQuestion
                {
                    Text = "Как называется оператор «?:»",
                    AnswerVariants = new TestAnswerVariant[4]
                    {
                        new TestAnswerVariant
                        {
                            Text = "Тернарный оператор",
                            IsCorrect = true
                        },
                        new TestAnswerVariant
                        {
                            Text = "Вопросительный",
                            IsCorrect = false
                        },
                        new TestAnswerVariant
                        {
                            Text = "Прямой оператор",
                            IsCorrect = false
                        },
                        new TestAnswerVariant
                        {
                            Text = "Территориальный оператор",
                            IsCorrect = false
                        },
                    }
                },
            };

            test1.Questions = questions;

            _appDbContext.CandidateTests.Add(test1);
            _appDbContext.SaveChanges();
        }

        private void AddTaskResults()
        {
            var candidates = _identityDbContext.Users
               .Where(u => u.Roles
                   .Any(r => r.RoleId == _identityDbContext.Roles
                       .FirstOrDefault(s => s.Name == GlobalInfo.Candidate).Id));

            if (!candidates.Any())
            {
                throw new InvalidOperationException("No candidates exist.");
            }

            if (!_appDbContext.CandidateTasks.Any())
            {
                throw new InvalidOperationException("No tasks exist.");
            }

            foreach (var candidate in candidates)
            {
                int i = 0;
                var taskResults = new Bogus.Faker<TaskResult>()
                .RuleFor(r => r.CreatorId, candidate.DomainId)
                .RuleFor(r => r.ModifierId, candidate.DomainId)
                .RuleFor(r => r.CandidateExercise, f => _appDbContext.CandidateTasks.ToArray().ElementAt(i++))
                .RuleFor(r => r.UsedTipsNumber, (f, r) => f.Random.Number(((Task)r.CandidateExercise).Tips.Count()))
                .RuleFor(r => r.IsCompleted, true)
                .RuleFor(r => r.Score, (f, r) => r.CandidateExercise.MaximumScore - r.UsedTipsNumber)
                .RuleFor(r => r.Code, (f, r) => ((Task)r.CandidateExercise).CodeTemplate)
                .Generate(_appDbContext.CandidateTasks.Count());
                _appDbContext.CandidateTaskResults.AddRange(taskResults);
            }

            _appDbContext.SaveChanges();
        }

        private void CleanCandidates()
        {
            foreach (var user in _identityDbContext.Users
                .Where(u => u.Roles
                    .Any(r => r.RoleId.Equals(
                        _identityDbContext.Roles
                            .FirstOrDefault(s => s.Name
                                .Equals(GlobalInfo.Candidate, StringComparison.Ordinal)).Id, StringComparison.Ordinal))))
            {
                _identityDbContext.UserProfiles.Remove(_identityDbContext.UserProfiles.Find(user.Id));
                _identityDbContext.Users.Remove(user);
            }

            _identityDbContext.SaveChanges();
        }

        private void CleanTaskResults()
        {
            // _appDbContext.CandidateTaskTips.RemoveRange(_appDbContext.CandidateTaskTips);
            _appDbContext.CandidateTaskResults.RemoveRange(_appDbContext.CandidateTaskResults);
            //_appDbContext.Database.ExecuteSqlCommand("DBCC CHECKIDENT('dbo.TaskTips', RESEED, 0)");
            _appDbContext.Database.ExecuteSqlCommand("DBCC CHECKIDENT('dbo.ExerciseResults', RESEED, 0)");

            _appDbContext.SaveChanges();
        }

        private void CleanInvites()
        {
            _appDbContext.Invites.RemoveRange(_appDbContext.Invites);
            _appDbContext.Database.ExecuteSqlCommand("DBCC CHECKIDENT('dbo.Invites', RESEED, 0)");

            _appDbContext.SaveChanges();
        }

        private void CleanTasks()
        {
            _appDbContext.CandidateTasks.RemoveRange(_appDbContext.CandidateTasks);
            _appDbContext.Database.ExecuteSqlCommand("DBCC CHECKIDENT('dbo.Exercises', RESEED, 0)");

            _appDbContext.SaveChanges();
        }

        private void CleanTests()
        {
            _appDbContext.CandidateTests.RemoveRange(_appDbContext.CandidateTests);
            _appDbContext.Database.ExecuteSqlCommand("DBCC CHECKIDENT('dbo.Exercises', RESEED, 0)");
            _appDbContext.Database.ExecuteSqlCommand("DBCC CHECKIDENT('dbo.TestQuestions', RESEED, 0)");
            _appDbContext.Database.ExecuteSqlCommand("DBCC CHECKIDENT('dbo.TestAnswerVariants', RESEED, 0)");

            _appDbContext.SaveChanges();
        }
    }
}
#endif