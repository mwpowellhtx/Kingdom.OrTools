using System;

namespace Kingdom.OrTools.Sat.Parameters
{
    using Xunit.Theoretically;

    // TODO: TBD: fill in some gaps here when we determine what the shape and composition of the parameter unit test should be...
    internal abstract class ParameterTestCasesBase : TestCasesBase
    {
        protected static IParameter CreateParameter(Func<IParameter> factory) => factory();

        protected static IParameter CreateParameter<T>(T value, Func<T, IParameter> factory) => factory(value);

        protected static IRepeatedParameter CreateRepeatedParameter<T>(T value, Func<T, IRepeatedParameter> factory) => factory(value);

        protected static IRepeatedParameter CreateRepeatedParameter<T>(T value, Func<T, T[], IRepeatedParameter> factory, params T[] others) => factory(value, others);
    }
}
