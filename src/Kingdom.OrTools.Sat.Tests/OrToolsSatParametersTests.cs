using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Kingdom.OrTools.Sat
{
    using Parameters;
    using Xunit;
    using Xunit.Abstractions;

    public class OrToolsSatParametersTests : TestFixtureBase
    {
        public OrToolsSatParametersTests(ITestOutputHelper outputHelper)
            : base(outputHelper)
        {
        }

        private Type ParameterType { get; } = typeof(IParameter);

        private Type RepeatedParameterType { get; } = typeof(IRepeatedParameter);

        private Assembly OrProblemSolverAssembly { get; } = typeof(IOrProblemSolver).Assembly;

        [Fact]
        public void Problem_Solver_Assembly_Is_Valid() => OrProblemSolverAssembly.AssertNotNull();

        private IEnumerable<Type> GetParameterTypes(Type expectedType) => OrProblemSolverAssembly.GetTypes().Where(expectedType.IsAssignableFrom);

        [Theory
         , InlineData(typeof(IParameter))
         , InlineData(typeof(IRepeatedParameter))]
        public void Assembly_Has_Parameter_Types(Type expectedParameterType) => GetParameterTypes(expectedParameterType).AssertNotNull().AssertNotEmpty();
    }
}
