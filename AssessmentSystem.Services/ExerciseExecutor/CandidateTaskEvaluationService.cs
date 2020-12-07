namespace AssessmentSystem.Services.ExerciseExecutor
{
    using System;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using AssessmentSystem.Data.Access.Context.Interfaces;
    using AssessmentSystem.Models.ExerciseExecutor;
    using AssessmentSystem.Services.Exceptions;
    using AutoMapper;
    using TaskResult = AssessmentSystem.Data.Access.ExerciseExecutor.TaskResult;

    /// <summary>
    /// Represents a candidate task evaluation service.
    /// </summary>
    internal class CandidateTaskEvaluationService : ICandidateTaskEvaluationService
    {
        /// <summary>
        /// Store for the string pattern of unwanted namespaces in candidate code.
        /// </summary>
        private const string Pattern = "(using System\\.(Diagnostics|IO)(\\.\\w+)*; *)";

        private readonly IExercisesContext _context;

        private readonly ICandidateTaskRunner _taskRunner;

        /// <summary>
        /// Initializes a new instance of the <see cref="CandidateTaskEvaluationService"/> class.
        /// </summary>
        /// <param name="taskRunner">An instance of <see cref="ICandidateTaskRunner"/>.</param>
        /// <param name="context">An instance of <see cref="IExercisesContext"/></param>
        /// <exception cref="ArgumentNullException">Exception thrown when
        /// <paramref name="taskRunner"/> or <paramref name="context"/> is null.</exception>
        public CandidateTaskEvaluationService(ICandidateTaskRunner taskRunner, IExercisesContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _taskRunner = taskRunner ?? throw new ArgumentNullException(nameof(taskRunner));
        }

        /// <summary>
        /// Validates a candidate's task.
        /// </summary>
        /// <param name="taskId">An id of a task made by a candidate.</param>
        /// <param name="template">A candidate's code.</param>
        /// <param name="usedTipsNumber">A number of tips used by a candidate.</param>
        /// <param name="userId">An id of a candidate who made a task.</param>
        /// <returns>A <see cref="System.Threading.Tasks.Task"/> representing the asynchronous operation.</returns>
        public async Task<TaskRunResult> ValidateAsync(int taskId, string template, int usedTipsNumber, Guid userId)
        {
            VerifyArguments(taskId, ref template, userId);

            if (IsTimerOut(taskId, userId))
            {
                throw new TimerOutException();
            }

            var task = _context.CandidateTasks.FirstOrDefault(i => i.Id == taskId);
            var result = _taskRunner.Run(Mapper.Map<Models.ExerciseManagement.Task>(task), template);

            if (result.Success)
            {
                var candidatesResult = CreateCandidateTaskResult(template, taskId, userId, usedTipsNumber, endDate: DateTime.UtcNow);
                result.Score = candidatesResult.Score = task.MaximumScore - usedTipsNumber;
                candidatesResult.IsCompleted = true;
                await UpdateCandidateTaskResultAsync(candidatesResult);
            }

            return result;
        }

        /// <summary>
        /// Uploads a number of tips used by a candidate.
        /// </summary>
        /// <param name="taskId">An id of a task.</param>
        /// <param name="usedTipsNumber">A number of used tips.</param>
        /// <param name="userId">An id of a candidate.</param>
        /// <returns>A <see cref="System.Threading.Tasks.Task"/> representing the asynchronous operation.</returns>
        public System.Threading.Tasks.Task UploadTipsNumber(int taskId, int usedTipsNumber, Guid userId)
        {
            string cadidateCode = string.Empty;
            TaskResult result = CreateCandidateTaskResult(cadidateCode, taskId, userId, usedTipsNumber);
            result.Score = 0;
            return UpdateCandidateTaskResultAsync(result);
        }

        /// <summary>
        /// Updates task result.
        /// </summary>
        /// <param name="currentTaskResult">A candidates' current result.</param>
        /// <returns>A <see cref="System.Threading.Tasks.Task"/> representing the asynchronous operation.</returns>
        public System.Threading.Tasks.Task UpdateCandidateTaskResultAsync(TaskResult currentTaskResult)
        {
            if (currentTaskResult == null)
            {
                throw new ArgumentNullException(nameof(currentTaskResult));
            }

            TaskResult previousResult = _context.CandidateTaskResults.FirstOrDefault(result => result.ModifierId == currentTaskResult.ModifierId && result.CandidateExerciseId == currentTaskResult.CandidateExerciseId);

            if (previousResult == null)
            {
                return SaveCandidateTaskResultAsync(currentTaskResult);
            }

            UpdateResult(previousResult, currentTaskResult);

            return _context.SaveChangesAsync();
        }

        /// <summary>
        /// Create task result.
        /// </summary>
        /// <param name="taskId">An id of a task.</param>
        /// <param name="startDate">Task start date.</param>
        /// <param name="userId">An id of a candidate.</param>
        /// <returns>A <see cref="System.Threading.Tasks.Task"/> representing the asynchronous operation.</returns>
        public System.Threading.Tasks.Task CreateCandidateTaskResultAsync(int taskId, DateTime startDate, Guid userId)
        {
            TaskResult currentTaskResult = CreateCandidateTaskResult(null, taskId, userId, 0, startDate);

            TaskResult previousResult = _context.CandidateTaskResults.FirstOrDefault(result => result.ModifierId == currentTaskResult.ModifierId && result.CandidateExerciseId == currentTaskResult.CandidateExerciseId);

            if (previousResult != null)
            {
                throw new TaskResultAlreadyExistsException();
            }

            return SaveCandidateTaskResultAsync(currentTaskResult);
        }

        private static void UpdateResult(TaskResult previousResult, TaskResult currentTaskResult)
        {
            previousResult.Code = currentTaskResult.Code;
            previousResult.CandidateExerciseId = currentTaskResult.CandidateExerciseId;
            previousResult.CreatorId = currentTaskResult.CreatorId;
            previousResult.ModifierId = currentTaskResult.ModifierId;
            previousResult.UsedTipsNumber = currentTaskResult.UsedTipsNumber;
            previousResult.Score = currentTaskResult.Score;
            previousResult.IsCompleted = currentTaskResult.IsCompleted;
            previousResult.EndDate = currentTaskResult.EndDate;
        }

        /// <summary>
        /// Creates an instance of <see cref="TaskResult"/>
        /// </summary>
        /// <param name="candidateCode">A code of a candidate.</param>
        /// <param name="taskId">An id of a task made by a candidate.</param>
        /// <param name="userId">An id of a candidate.</param>
        /// <param name="usedTipsNumber">A number of tips used by a candidate.</param>
        /// <param name="startDate">Task start date.</param>
        /// <param name="endDate">Task end date.</param>
        /// <returns>A <see cref="System.Threading.Tasks.Task"/> representing the asynchronous operation.</returns>
        private static TaskResult CreateCandidateTaskResult(
            string candidateCode, int taskId, Guid userId, int usedTipsNumber, DateTime? startDate = null, DateTime? endDate = null)
        {
            var candidatesResult = new TaskResult
            {
                Code = candidateCode,
                CandidateExerciseId = taskId,
                CreatorId = userId,
                ModifierId = userId,
                UsedTipsNumber = usedTipsNumber,
                Score = 0,
                IsCompleted = false,
                StartDate = startDate,
                EndDate = endDate
            };

            return candidatesResult;
        }

        private System.Threading.Tasks.Task SaveCandidateTaskResultAsync(TaskResult candidatesResult)
        {
            _context.CandidateTaskResults.Add(candidatesResult);
            return _context.SaveChangesAsync();
        }

        private void VerifyArguments(int taskId, ref string template, Guid userId)
        {
            if (taskId < 1)
            {
                throw new ArgumentException($"{nameof(taskId)} must be greater than 1.", nameof(taskId));
            }

            if (string.IsNullOrEmpty(template))
            {
                throw new ArgumentNullException(nameof(template));
            }

            template = CheckCodeForUnwantedNamespaces(template);

            if (userId == Guid.Empty)
            {
                throw new ArgumentException($"{nameof(userId)} is an empty guid.", nameof(userId));
            }
        }

        /// <summary>
        /// Checks whether candidate code contains unwanted namespaces and deletes them.
        /// </summary>
        /// <param name="template">Candidate's code.</param>
        /// <returns>A candidate's code without unwanted imports.</returns>
        private string CheckCodeForUnwantedNamespaces(string template)
        {
            var replacement = string.Empty;
            var rgx = new Regex(Pattern);
            return rgx.Replace(template, replacement);
        }

        /// <summary>
        /// Checks whether timer out.
        /// </summary>
        /// <param name="taskId">Task id.</param>
        /// <param name="userId">User id.</param>
        /// <returns>Boolean representation or timer out.</returns>
        private bool IsTimerOut(int taskId, Guid userId)
        {
            var previousResult = _context.CandidateTaskResults.FirstOrDefault(result => result.ModifierId == userId && result.CandidateExerciseId == taskId);
            var candidateTask = _context.CandidateTasks.FirstOrDefault(task => task.Id == taskId);

            if (candidateTask == null || previousResult == null)
            {
                return false;
            }

            if (!(previousResult is TaskResult taskResult) || taskResult.StartDate == null)
            {
                return false;
            }

            return (DateTime.UtcNow - taskResult.StartDate.Value).TotalSeconds > candidateTask.TimeMinutes * 60;
        }
    }
}
