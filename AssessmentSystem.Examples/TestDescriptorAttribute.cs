using System;

namespace AssessmentSystem.Examples
{
    [AttributeUsage(AttributeTargets.Class)]
    public class TestDescriptorAttribute : Attribute
    {
        public TestDescriptorAttribute(Type descriptorSource)
        {
            DescriptorSource = descriptorSource;
        }

        public Type DescriptorSource { get; set; }
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class TestCaseDescriptorAttribute : Attribute
    {
        public TestCaseDescriptorAttribute(Type descriptorSource)
        {
            DescriptorSource = descriptorSource;
        }

        public Type DescriptorSource { get; set; }
    }

    public interface IDescriptorSource
    {

    }
}