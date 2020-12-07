namespace AssessmentSystem.Services.Mapping
{
    using System.Linq;
    using AssessmentSystem.Data.Access.ExerciseManagement;
    using AssessmentSystem.Models.ExerciseManagement;
    using AutoMapper;
    using EntityTask = AssessmentSystem.Data.Access.ExerciseManagement.Task;
    using EntityTaskResult = AssessmentSystem.Data.Access.ExerciseExecutor.TaskResult;
    using ModelTask = AssessmentSystem.Models.ExerciseManagement.Task;
    using ModelTaskResult = AssessmentSystem.Models.ExerciseExecutor.CandidateTaskResult;
    using TestAnswerVariantEntity = Data.Access.ExerciseManagement.TestAnswerVariant;
    using TestAnswerVariantModel = Models.ExerciseManagement.TestAnswerVariant;
    using TestEntity = Data.Access.ExerciseManagement.Test;
    using TestModel = Models.ExerciseManagement.Test;
    using TestQuestionEntity = Data.Access.ExerciseManagement.TestQuestion;
    using TestQuestionModel = Models.ExerciseManagement.TestQuestion;

    /// <summary>
    /// Represents a mapping profile for exercises.
    /// </summary>
    public class CandidateExerciseMappingProfile : Profile
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CandidateExerciseMappingProfile"/> class.
        /// </summary>
        public CandidateExerciseMappingProfile()
        {
            CreateMap<EntityTask, ModelTask>()
                .ForMember(taskInfo => taskInfo.Tips, opt => opt.MapFrom(
                    task => task.Tips.Select(_ => _.Text).ToArray()));

            CreateMap<ModelTask, EntityTask>()
                .ForMember(taskInfo => taskInfo.Tips, opt => opt.MapFrom(
                    task => task.Tips.Select(tip => new TaskTip
                {
                    Text = tip,
                    CandidateTaskId = task.Id
                })));

            CreateMap<EntityTaskResult, ModelTaskResult>();
            CreateMap<ModelTaskResult, EntityTaskResult>();

            CreateMap<EntityTask, TaskInfo>()
                .ForMember(taskInfo => taskInfo.AssemblyName, opt => opt.MapFrom(task => task.TestClass.AssemblyInfo.AssemblyName))
                .ForMember(taskInfo => taskInfo.TestClassName, opt => opt.MapFrom(task => task.TestClass.Name))
                .ForMember(taskInfo => taskInfo.TestMethodName, opt => opt.MapFrom(task => task.TestMethod.Name))
                .ForMember(taskInfo => taskInfo.Tips, opt => opt.MapFrom(task => task.Tips.Select(_ => _.Text).ToArray()));

            CreateMap<TestModel, TestEntity>();
            CreateMap<TestEntity, TestModel>();

            CreateMap<TestQuestionEntity, TestQuestionModel>();
            CreateMap<TestQuestionModel, TestQuestionEntity>();

            CreateMap<TestAnswerVariantEntity, TestAnswerVariantModel>();
            CreateMap<TestAnswerVariantModel, TestAnswerVariantEntity>();
        }
    }
}
