namespace AssessmentSystem.Services.ExerciseManagement
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using AssessmentSystem.Data.Access.Context.Interfaces;
    using AssessmentSystem.Data.Access.ExerciseExecutor;
    using AssessmentSystem.Data.Access.ExerciseManagement;
    using AssessmentSystem.Models.ExerciseManagement;
    using AssessmentSystem.Services.Exceptions;
    using AutoMapper;
    using AnswerEntity = Data.Access.ExerciseManagement.TestAnswerVariant;
    using AnswerModel = Models.ExerciseManagement.TestAnswerVariant;
    using Exercise = AssessmentSystem.Models.ExerciseManagement.Exercise;
    using QuestionEntity = Data.Access.ExerciseManagement.TestQuestion;
    using QuestionModel = Models.ExerciseManagement.TestQuestion;
    using Task = AssessmentSystem.Models.ExerciseManagement.Task;
    using TestEntity = Data.Access.ExerciseManagement.Test;
    using TestModel = Models.ExerciseManagement.Test;

    /// <summary>
    /// Represents an exercise service.
    /// </summary>
    public class ExerciseService : IExerciseService
    {
        private const int ExerciseNotCompleted = -1;
        private const int TestUsedTipsNumber = 0;
        private readonly IExercisesContext _exercisesContext;
        private readonly IExercisePoolService _exercisePoolService;
        private readonly ITestAssemblyContext _assemblyContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExerciseService"/> class.
        /// </summary>
        /// <param name="exerciseContext">An instance of <see cref="IExercisesContext"/>.</param>
        /// <param name="exercisePoolService">An instance of <see cref="IExercisePoolService"/>.</param>
        /// <param name="assemblyContext">An instance of <see cref="ITestAssemblyContext"/>.</param>
        /// <exception cref="ArgumentNullException">Exception thrown when
        /// <paramref name="exerciseContext"/>, <paramref name="exercisePoolService"/>
        /// or <paramref name="assemblyContext"/> is null.</exception>
        public ExerciseService(
            IExercisesContext exerciseContext,
            IExercisePoolService exercisePoolService,
            ITestAssemblyContext assemblyContext)
        {
            _exercisesContext = exerciseContext ?? throw new ArgumentNullException(nameof(exerciseContext));
            _exercisePoolService = exercisePoolService ?? throw new ArgumentNullException(nameof(exercisePoolService));
            _assemblyContext = assemblyContext ?? throw new ArgumentNullException(nameof(assemblyContext));
        }

        public IEnumerable<ExerciseForList> GetCandidateExerciseList(Guid applicationUserId)
        {
            if (applicationUserId == Guid.Empty)
            {
                throw new ArgumentException($"{nameof(applicationUserId)} is an empty guid.", nameof(applicationUserId));
            }

            var exercises = _exercisePoolService.GetActiveExerciseSet();
            var candidateExerciseResults = GetCandidateExerciseResults(applicationUserId);

            var models = exercises.Select(exercise => CreateExerciseForList(exercise, candidateExerciseResults));
            return models.ToArray();
        }

        public IEnumerable<Exercise> GetTestsInfo()
        {
            return _exercisesContext.CandidateTests
                .Where(t => t.IsSoftDeleted == false)
                .AsEnumerable()
                .Cast<Data.Access.ExerciseManagement.Exercise>()
                .Select(e => Mapper.Map<Data.Access.ExerciseManagement.Exercise, Models.ExerciseManagement.Exercise>(e));
        }

        public TestModel GetTest(int testId)
        {
            var test = _exercisesContext.CandidateTests.Single(t => t.Id == testId);
            var questions = _exercisesContext.TestQuestions.Where(q => q.TestId == testId);
            test.Questions = questions.ToArray();
            foreach (var question in test.Questions)
            {
                question.AnswerVariants = _exercisesContext.TestAnswerVariants.Where(v => v.QuestionId == question.Id).ToArray();
            }

            return Mapper.Map<TestEntity, TestModel>(test);
        }

        public async System.Threading.Tasks.Task UpdateTestAsync(int testId, TestModel testModel)
        {
            // update general test info
            var testEntity = _exercisesContext.CandidateTests.Single(t => t.Id == testId);
            testEntity.Description = testModel.Description;
            testEntity.MaximumScore = testModel.MaximumScore;
            testEntity.Name = testModel.Name;
            testEntity.TimeMinutes = testModel.TimeMinutes;
            testEntity.Subject = testModel.Subject;

            // delete test questions that do not exist in model
            foreach (var questionEntity in _exercisesContext.TestQuestions.Where(q => q.TestId == testId))
            {
                if (!testModel.Questions.Any(q => q.Id == questionEntity.Id))
                {
                    _exercisesContext.TestQuestions.Remove(questionEntity);
                }
            }

            foreach (var questionModel in testModel.Questions)
            {
                // update existent questions
                if (_exercisesContext.TestQuestions.Any(q => q.Id == questionModel.Id))
                {
                    _exercisesContext.TestQuestions.Single(q => q.Id == questionModel.Id).Text = questionModel.Text;
                    var variantEntities = _exercisesContext.TestAnswerVariants.Where(v => v.QuestionId == questionModel.Id);
                    foreach (var variantEntity in variantEntities)
                    {
                        var variantModel = questionModel.AnswerVariants.Single(v => v.Id == variantEntity.Id);
                        variantEntity.Text = variantModel.Text;
                        variantEntity.IsCorrect = variantModel.IsCorrect;
                    }
                }

                // add new questions
                else
                {
                    var questionEntity = Mapper.Map<QuestionModel, QuestionEntity>(questionModel);
                    questionEntity.TestId = testId;
                    _exercisesContext.TestQuestions.Add(questionEntity);
                }
            }

            await _exercisesContext.SaveChangesAsync();
        }

        public async System.Threading.Tasks.Task DeleteTestAsync(int testId)
        {
            _exercisesContext.CandidateTests.Remove(_exercisesContext.CandidateTests.Single(t => t.Id == testId));
            foreach (var question in _exercisesContext.TestQuestions.Where(q => q.TestId == testId))
            {
                _exercisesContext.TestQuestions.Remove(question);
                foreach (var variant in _exercisesContext.TestAnswerVariants.Where(v => v.QuestionId == question.Id))
                {
                    _exercisesContext.TestAnswerVariants.Remove(variant);
                }
            }

            await _exercisesContext.SaveChangesAsync();
        }

        public async System.Threading.Tasks.Task AddTestAsync(TestModel test)
        {
            _exercisesContext.CandidateTests.Add(Mapper.Map<TestModel, TestEntity>(test));
            await _exercisesContext.SaveChangesAsync();
        }

        public IEnumerable<Exercise> GetTasksInfo()
        {
            return _exercisesContext.CandidateTasks
                .Where(t => t.IsSoftDeleted == false)
                .AsEnumerable()
                .Cast<Data.Access.ExerciseManagement.Exercise>()
                .Select(e => Mapper.Map<Data.Access.ExerciseManagement.Exercise, Models.ExerciseManagement.Exercise>(e));
        }

        public TaskViewInfo GetCandidateTaskViewInfo(int id, Guid applicationUserId)
        {
            if (applicationUserId == Guid.Empty)
            {
                throw new ArgumentException($"{nameof(applicationUserId)} is an empty guid.", nameof(applicationUserId));
            }

            if (id < 1)
            {
                throw new ArgumentException($"{nameof(id)} is lower than 1.", nameof(id));
            }

            var candidateTask = _exercisePoolService.GetCandidateTask(id);
            var candidateExerciseResults = GetCandidateExerciseResults(applicationUserId);

            return CreateCandidateTask(candidateTask, candidateExerciseResults);
        }

        public IEnumerable<Tuple<string, int>> GetExercisesNamesAndIds()
        {
            var tasks = _exercisesContext.CandidateTasks
                .Where(task => task.IsSoftDeleted == false)
                .Select(task => new { task.Name, task.Id })
                .AsEnumerable()
                .Select(taskInfo => Tuple.Create(taskInfo.Name, taskInfo.Id));

            var tests = _exercisesContext.CandidateTests
                .Where(task => task.IsSoftDeleted == false)
                .Select(test => new { test.Name, test.Id })
                .AsEnumerable()
                .Select(testInfo => Tuple.Create(testInfo.Name, testInfo.Id));

            return tasks.Concat(tests).ToArray();
        }

        public TaskInfo GetCandidateTaskInfo(int id)
        {
            if (id < 1)
            {
                throw new ArgumentException("Id must be greater than or equal to 1.", nameof(id));
            }

            var candidateTask = _exercisesContext.CandidateTasks.FirstOrDefault(task => task.Id == id);
            return candidateTask == null ? null :
                Mapper.Map<AssessmentSystem.Data.Access.ExerciseManagement.Task, TaskInfo>(candidateTask);
        }

        public async Task<int> AddCandidateTaskAsync(TaskInfo candidateTaskInfo)
        {
            if (candidateTaskInfo == null)
            {
                throw new ArgumentNullException(nameof(candidateTaskInfo));
            }

            var candidateTask = CreateCandidateTask(candidateTaskInfo);
            _exercisesContext.CandidateTasks.Add(candidateTask);
            await _exercisesContext.SaveChangesAsync().ConfigureAwait(false);
            return candidateTask.Id;
        }

        public System.Threading.Tasks.Task UpdateCandidateTaskAsync(TaskInfo candidateTaskInfo)
        {
            if (candidateTaskInfo == null)
            {
                throw new ArgumentNullException(nameof(candidateTaskInfo));
            }

            var candidateTask = _exercisesContext.CandidateTasks.FirstOrDefault(
                task => task.Id == candidateTaskInfo.Id);
            if (candidateTask == null)
            {
                throw new TaskNotFoundException($"Task with id = {candidateTaskInfo.Id} not found.");
            }

            UpdateCandidateTask(candidateTask, candidateTaskInfo);
            return _exercisesContext.SaveChangesAsync();
        }

        public System.Threading.Tasks.Task DeleteCandidateExerciseAsync(int id)
        {
            if (id < 1)
            {
                throw new ArgumentException("Id must be greater than or equal to 1.", nameof(id));
            }

            var candidateTask = _exercisesContext.CandidateTasks.FirstOrDefault(task => task.Id == id);
            if (candidateTask != null)
            {
                _exercisesContext.CandidateTasks.Remove(candidateTask);
                return _exercisesContext.SaveChangesAsync();
            }

            var candidateTest = _exercisesContext.CandidateTests.FirstOrDefault(task => task.Id == id);
            if (candidateTest == null)
            {
                throw new ExerciseNotFoundException($"Exercise with id = {id} not found.");
            }

            _exercisesContext.CandidateTests.Remove(candidateTest);
            return _exercisesContext.SaveChangesAsync();
        }

        public System.Threading.Tasks.Task SoftDeleteCandidateExerciseAsync(int id)
        {
            if (id < 1)
            {
                throw new ArgumentException("Id must be greater than or equal to 1.", nameof(id));
            }

            var candidateTask = _exercisesContext.CandidateTasks.FirstOrDefault(
                task => task.Id == id);
            if (candidateTask != null)
            {
                SoftDeleteCandidateExercise(candidateTask);
                return _exercisesContext.SaveChangesAsync();
            }

            var candidateTest = _exercisesContext.CandidateTests.FirstOrDefault(test => test.Id == id);
            if (candidateTest == null)
            {
                throw new ExerciseNotFoundException($"Exercise with id = {id} not found.");
            }

            SoftDeleteCandidateExercise(candidateTest);
            return _exercisesContext.SaveChangesAsync();
        }

        public bool IsResultAssociatedWithTaskExist(int candidateExerciseId)
        {
            var candidateTaskResult = _exercisesContext.CandidateTaskResults.FirstOrDefault(
                result => result.CandidateExerciseId == candidateExerciseId);
            var candidateTestResult = _exercisesContext.CandidateTestResults.FirstOrDefault(
                result => result.CandidateExerciseId == candidateExerciseId);
            return candidateTaskResult != null || candidateTestResult != null;
        }

        private static ExerciseForList CreateExerciseForList(
            Exercise exercise,
            IList<ExerciseResult> candidateExerciseResults)
        {
            return new ExerciseForList
            {
                Id = exercise.Id,
                Name = exercise.Name,
                Subject = exercise.Subject,
                MaximumScore = exercise.MaximumScore,
                CandidateScore = candidateExerciseResults.FirstOrDefault(
                                     result => result.CandidateExerciseId == exercise.Id)?.Score ?? ExerciseNotCompleted,
                UsedTipsNumber = GetExerciseUsedTipsNumber(exercise, candidateExerciseResults),
                IsCompleted = candidateExerciseResults.Any(result => result.CandidateExerciseId == exercise.Id && result.Score != 0),
                ResultId = candidateExerciseResults.FirstOrDefault(
                               result => result.CandidateExerciseId == exercise.Id)?.Id ?? ExerciseNotCompleted,
                IsSoftDeleted = false,
                IsTimeOut = IsTimerOut(exercise, candidateExerciseResults),
                HasTimer = exercise.TimeMinutes.HasValue
            };
        }

        private static TaskViewInfo CreateCandidateTask(
            Task task,
            IList<ExerciseResult> candidateExerciseResults)
        {
            return new TaskViewInfo
            {
                Id = task.Id,
                Name = task.Name,
                Subject = task.Subject,
                Description = task.Description,
                MaximumScore = task.MaximumScore,
                TimeSeconds = task.TimeMinutes,
                CodeTemplate = task.CodeTemplate,
                Tips = task.Tips.ToArray(),
                CandidateScore = candidateExerciseResults.FirstOrDefault(
                                     result => result.CandidateExerciseId == task.Id)?.Score ?? ExerciseNotCompleted,
                UsedTipsNumber = GetExerciseUsedTipsNumber(task, candidateExerciseResults),
                IsCompleted = candidateExerciseResults.Any(result => result.CandidateExerciseId == task.Id && result.Score != 0),
                IsSoftDeleted = false
            };
        }

        private static int GetExerciseUsedTipsNumber(
            Exercise exercise,
            IEnumerable<ExerciseResult> candidateExerciseResults)
        {
            if (exercise.GetType() == typeof(TestModel))
            {
                return TestUsedTipsNumber;
            }

            var exerciseResult = candidateExerciseResults.FirstOrDefault(
                result => result.CandidateExerciseId == exercise.Id);

            if (exerciseResult == null)
            {
                return TestUsedTipsNumber;
            }

            var taskResult = exerciseResult as TaskResult;
            return taskResult?.UsedTipsNumber ?? TestUsedTipsNumber;
        }

        private static bool IsTimerOut(Exercise task, IList<ExerciseResult> candidateExerciseResults)
        {
            if (task == null || candidateExerciseResults == null)
            {
                return false;
            }

            var result = candidateExerciseResults.FirstOrDefault(res => res.CandidateExerciseId == task.Id);

            if (result == null || result.Score != 0 || !(result is TaskResult taskResult)
                || taskResult.StartDate == null)
            {
                return false;
            }

            return (DateTime.UtcNow - taskResult.StartDate.Value).TotalSeconds > task.TimeMinutes * 60;
        }

        private IList<ExerciseResult> GetCandidateExerciseResults(Guid applicationUserId)
        {
            var candidateExerciseResults = new List<ExerciseResult>();

            var candidateTaskResults = _exercisesContext.CandidateTaskResults
                .Where(result => result.CreatorId == applicationUserId)
                .ToArray();

            var candidateTestResults = _exercisesContext.CandidateTestResults
                .Where(result => result.CreatorId == applicationUserId)
                .ToArray();

            candidateExerciseResults.AddRange(candidateTaskResults);
            candidateExerciseResults.AddRange(candidateTestResults);

            return candidateExerciseResults;
        }

        private AssessmentSystem.Data.Access.ExerciseManagement.Task CreateCandidateTask(
            TaskInfo candidateTaskInfo)
        {
            var result = new AssessmentSystem.Data.Access.ExerciseManagement.Task
            {
                CodeTemplate = candidateTaskInfo.CodeTemplate,
                Description = candidateTaskInfo.Description,
                MaximumScore = candidateTaskInfo.MaximumScore,
                TimeMinutes = candidateTaskInfo.TimeMinutes,
                Name = candidateTaskInfo.Name,
                Subject = candidateTaskInfo.Subject,
                IsSoftDeleted = false
            };

            if (candidateTaskInfo.Tips != null)
            {
                result.Tips = candidateTaskInfo.Tips.Select(
                    tipText => new TaskTip { Text = tipText }).ToList();
            }

            SetTestClassAndMethod(result, candidateTaskInfo);

            return result;
        }

        private void UpdateCandidateTask(
            AssessmentSystem.Data.Access.ExerciseManagement.Task candidateTask,
            TaskInfo candidateTaskInfo)
        {
            candidateTask.CodeTemplate = candidateTaskInfo.CodeTemplate;
            candidateTask.Description = candidateTaskInfo.Description;
            candidateTask.MaximumScore = candidateTaskInfo.MaximumScore;
            candidateTask.TimeMinutes = candidateTaskInfo.TimeMinutes;
            candidateTask.Name = candidateTaskInfo.Name;
            candidateTask.Subject = candidateTaskInfo.Subject;

            if (candidateTaskInfo.Tips != null)
            {
                UpdateTips(candidateTask.Tips, candidateTaskInfo.Tips.ToArray());
            }
            else
            {
                UpdateTips(candidateTask.Tips);
            }

            SetTestClassAndMethod(candidateTask, candidateTaskInfo);
        }

        private void SoftDeleteCandidateExercise(
            AssessmentSystem.Data.Access.ExerciseManagement.Exercise candidateExercise)
        {
            candidateExercise.IsSoftDeleted = true;
        }

        private void SetTestClassAndMethod(
            AssessmentSystem.Data.Access.ExerciseManagement.Task candidateTask,
            TaskInfo candidateTaskInfo)
        {
            var testClass = _assemblyContext.TestClassInfo.FirstOrDefault(classInfo =>
                classInfo.Name.Equals(candidateTaskInfo.TestClassName, StringComparison.Ordinal));

            candidateTask.TestClassId = testClass?.Id ?? throw new TestClassInfoNotFoundException();

            if (!string.IsNullOrWhiteSpace(candidateTaskInfo.TestMethodName))
            {
                var testMethod = testClass.TestMethods.FirstOrDefault(method =>
                    method.Name.Equals(candidateTaskInfo.TestMethodName, StringComparison.Ordinal));
                candidateTask.TestMethodId = testMethod?.Id;
            }
        }

        private void UpdateTips(ICollection<TaskTip> oldTips, IReadOnlyList<string> newTips = null)
        {
            int i;
            if (newTips == null)
            {
                i = oldTips.Count;
                while (i > 0)
                {
                    var tip = oldTips.ElementAt(--i);
                    _exercisesContext.CandidateTaskTips.Remove(tip);
                    oldTips.Remove(tip);
                }
            }
            else
            {
                for (i = newTips.Count; i < oldTips.Count; i++)
                {
                    var tip = oldTips.ElementAt(i--);
                    _exercisesContext.CandidateTaskTips.Remove(tip);
                    oldTips.Remove(tip);
                }

                i = 0;
                foreach (var oldTip in oldTips)
                {
                    oldTip.Text = newTips[i++];
                }

                for (i = oldTips.Count; i < newTips.Count; i++)
                {
                    oldTips.Add(new TaskTip { Text = newTips[i] });
                }
            }
        }
    }
}
