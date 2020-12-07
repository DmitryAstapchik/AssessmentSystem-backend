namespace AssessmentSystem.Services.ExerciseManagement
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using AssessmentSystem.Data.Access.Context.Interfaces;
    using AssessmentSystem.Models.ExerciseManagement;
    using AutoMapper;

    /// <summary>
    /// Represents an exercise pool service.
    /// </summary>
    internal class ExercisePoolService : IExercisePoolService
    {
        private readonly IExercisesContext _exerciseContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExercisePoolService"/> class.
        /// </summary>
        /// <param name="exerciseContext">An instance of <see cref="IExercisesContext"/>.</param>
        /// <exception cref="ArgumentNullException">Exception thrown when
        /// <paramref name="exerciseContext"/> is null.</exception>
        public ExercisePoolService(IExercisesContext exerciseContext)
        {
            _exerciseContext = exerciseContext ?? throw new ArgumentNullException(nameof(exerciseContext));
        }

        public IEnumerable<Exercise> GetActiveExerciseSet()
        {
            var entityTasks = _exerciseContext.CandidateTasks.Where(task => !task.IsSoftDeleted).ToArray();

            var modelTasks = Mapper.Map<IEnumerable<AssessmentSystem.Data.Access.ExerciseManagement.Task>,
                IEnumerable<Task>>(entityTasks);

            var result = new List<Exercise>();
            result.AddRange(modelTasks);

            return result;
        }

        public Task GetCandidateTask(int id)
        {
            var task = _exerciseContext.CandidateTasks.FirstOrDefault(t => t.Id == id);

            return task == null ? null
                : Mapper.Map<AssessmentSystem.Data.Access.ExerciseManagement.Task, Task>(task);
        }
    }
}