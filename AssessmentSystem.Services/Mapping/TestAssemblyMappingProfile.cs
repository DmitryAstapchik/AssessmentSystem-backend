using AutoMapper;
using EntityAssembliesInfo = AssessmentSystem.Data.Access.ExerciseExecutor.TestAssemblyInfo;
using EntityClassInfo = AssessmentSystem.Data.Access.ExerciseExecutor.TestClassInfo;
using EntityMethodInfo = AssessmentSystem.Data.Access.ExerciseExecutor.TestMethodInfo;
using ModelAssemliesInfo = AssessmentSystem.Models.ExerciseExecutor.TestAssemblyInfo;
using ModelClassInfo = AssessmentSystem.Models.ExerciseExecutor.TestClassInfo;
using ModelMethodInfo = AssessmentSystem.Models.ExerciseExecutor.TestMethodInfo;

namespace AssessmentSystem.Services.Mapping
{
    /// <summary>
    /// Represents a mapping profile for assemblies.
    /// </summary>
    internal class TestAssemblyMappingProfile : Profile
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TestAssemblyMappingProfile"/> class.
        /// </summary>
        public TestAssemblyMappingProfile()
        {
            CreateMap<EntityAssembliesInfo, ModelAssemliesInfo>();
            CreateMap<ModelAssemliesInfo, EntityAssembliesInfo>();

            CreateMap<EntityClassInfo, ModelClassInfo>();
            CreateMap<ModelClassInfo, EntityClassInfo>();

            CreateMap<EntityMethodInfo, ModelMethodInfo>();
            CreateMap<ModelMethodInfo, EntityMethodInfo>();
        }
    }
}
