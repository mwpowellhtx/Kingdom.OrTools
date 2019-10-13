using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Kingdom.OrTools.Sat.Parameters
{
    using Xunit;
    using Xunit.Abstractions;
    using static String;

    public class OrToolsSatParametersTests : OrToolsSatParametersTestFixtureBase
    {
        public OrToolsSatParametersTests(ITestOutputHelper outputHelper)
            : base(outputHelper)
        {
        }

        /// <summary>
        /// Gets the <see cref="IParameter"/> Type.
        /// </summary>
        private static Type ParameterType { get; } = typeof(IParameter);

        // ReSharper disable once UnusedMember.Local
        /// <summary>
        /// Gets the <see cref="IRepeatedParameter"/> Type.
        /// </summary>
        private static Type RepeatedParameterType { get; } = typeof(IRepeatedParameter);

        /// <summary>
        /// Returns the Parameter Types deriving from the <paramref name="expectedType"/>.
        /// </summary>
        /// <param name="expectedType"></param>
        /// <returns></returns>
        private static IEnumerable<Type> GetParameterTypes(Type expectedType) =>
            ParametersAssembly.GetTypes().Where(expectedType.IsAssignableFrom);

        /// <summary>
        /// Gets the entire set of Parameter Types.
        /// </summary>
        /// <see cref="ParameterType"/>
        private static IEnumerable<Type> ParameterTypes => GetParameterTypes(ParameterType).AssertNotNull().AssertNotEmpty();

        /// <summary>
        /// Gets the entire set of Parameter Types.
        /// </summary>
        /// <see cref="ParameterType"/>
        private static IEnumerable<Type> RepeatedParameterTypes => GetParameterTypes(RepeatedParameterType).AssertNotNull().AssertNotEmpty();

        /// <summary>
        /// Do some verification of the internal bits, without which the
        /// <see cref="TheoryAttribute"/> unit tests will fail to yield test cases in most
        /// instances.
        /// </summary>
        [Fact]
        public void Should_Have_ParameterTypes() => ParameterTypes.AssertNotNull();

        /// <summary>
        /// Do some verification of the internal bits, without which the
        /// <see cref="TheoryAttribute"/> unit tests will fail to yield test cases in most
        /// instances.
        /// </summary>
        [Fact]
        public void Should_Have_RepeatedParameterTypes() => RepeatedParameterTypes.AssertNotNull();

        private static IEnumerable<object[]> _overallParameterCounts;

        /// <summary>
        /// Gets the arguments informing the <see cref="Assembly_Has_Parameter_Types"/> test cases.
        /// </summary>
        public static IEnumerable<object[]> OverallParameterCounts
        {
            get
            {
                IEnumerable<object[]> GetAll()
                {
                    // See ParameterCriteria for detailed delta observations.
                    // 126: IParameter
                    // 2: IRepeatedParameter

                    yield return GetRange<object>(typeof(IParameter), 126).ToArray();
                    yield return GetRange<object>(typeof(IRepeatedParameter), 2).ToArray();
                }

                return _overallParameterCounts ?? (_overallParameterCounts = GetAll().ToArray());
            }
        }

        /// <summary>
        /// Performs an Overall verification of the Parameter Types.
        /// </summary>
        /// <param name="expectedParameterType"></param>
        /// <param name="expectedCount"></param>
        [Theory, MemberData(nameof(OverallParameterCounts))]
        public void Assembly_Has_Parameter_Types(Type expectedParameterType, int expectedCount) => GetParameterTypes(
            expectedParameterType).AssertNotNull().AssertNotEmpty().AssertEqual(expectedCount, x => x.Count());

        private static IEnumerable<object[]> _parameterCriteria;

        /// <summary>
        /// Gets the arguments informing the <see cref="Derived_Parameter_Matches"/> test cases.
        /// </summary>
        public static IEnumerable<object[]> ParameterCriteria
        {
            get
            {
                IEnumerable<object[]> GetAll()
                {
                    IEnumerable<object> GetOne<T>(int expectedCount)
                    {
                        var parameterType = typeof(Parameter<T>);
                        Func<Type, bool> criteria = parameterType.IsAssignableFrom;
                        yield return typeof(T);
                        yield return criteria;
                        yield return expectedCount;
                    }

                    // Awaiting an up to date commit merge:
                    // https://github.com/google/or-tools/blob/stable/ortools/sat/sat_parameters.proto
                    // https://github.com/google/or-tools/commits/stable/ortools/sat/sat_parameters.proto
                    // Next Tag delta 149 from 139, which jives with the +10 overall Parameters.
                    // Pending a more official delta report once they've committed and/or merged their code.
                    // 7.4 release / https://groups.google.com/forum/#!topic/or-tools-discuss/3O45yhSF4Uk

                    // 58: Parameter<bool>
                    // 26: Parameter<int>
                    // 24: Parameter<double>
                    // 7: Parameter<long>
                    // 124: total

                    yield return GetOne<bool>(58).ToArray();
                    yield return GetOne<int>(26).ToArray();
                    yield return GetOne<double>(24).ToArray();
                    yield return GetOne<long>(7).ToArray();

                    IEnumerable<object> GetOneEnum<TParameter>(int expectedCount, Type baseParameterType)
                        where TParameter : IParameter
                    {
                        var parameterType = typeof(TParameter);

                        bool CriteriaBaseTypeIsEnumGeneric(Type baseType)
                        {
                            bool IsSpecifiedGenericType(Type genericType)
                            {
                                var result = baseType == baseParameterType.MakeGenericType(genericType);
                                return result;
                            }

                            // TODO: TBD: It should be assignable to both Parameter<> (at the Collection level) ...
                            // TODO: TBD: as well as RepeatedParameter<> (at the Parameter level) ...
                            var genericArgs = baseType.GetGenericArguments();
                            var isGenericType = baseType.IsGenericType;
                            var isAssignableFrom = parameterType.IsAssignableFrom(baseType);
                            var isEnumGenericType = genericArgs.Length == 1 && genericArgs[0].IsEnum;

                            return isGenericType && isAssignableFrom && isEnumGenericType &&
                                   IsSpecifiedGenericType(genericArgs[0]);
                        }

                        bool VerifyBaseType(Type parentType)
                            => parentType?.BaseType != null
                               && parentType.Namespace == parameterType.Namespace
                               && CriteriaBaseTypeIsEnumGeneric(parentType.BaseType);

                        Func<Type, bool> criteria = VerifyBaseType;
                        yield return typeof(ICollection<Enum>);
                        yield return criteria;
                        yield return expectedCount;
                    }

                    // 9: Parameter<`enum´>
                    // 2: RepeatedParameter<`enum´>

                    // More like 11, composed of 9 Parameters +2 Repeated, which also derives from IParameter.
                    yield return GetOneEnum<IParameter>(9, typeof(Parameter<>)).ToArray();
                    yield return GetOneEnum<IRepeatedParameter>(2, typeof(RepeatedParameter<>)).ToArray();
                }

                return _parameterCriteria ?? (_parameterCriteria = GetAll().ToArray());
            }
        }

        /// <summary>
        /// Verifies that the Derived <see cref="Parameter"/> Types Match the <paramref name="criteria"/>.
        /// </summary>
        /// <param name="_">A furnished Type which further informs the nature of the Test Case.</param>
        /// <param name="criteria">Anonymous Criteria used in order to filter the Parameter Types.</param>
        /// <param name="expectedCount">The Expected number of Parameter Types matching the Criteria.</param>
        [Theory, MemberData(nameof(ParameterCriteria))]
        public void Derived_Parameter_Matches(Type _, Func<Type, bool> criteria, int expectedCount)
        {
            // We do not care what the Type was, per se, but the Cases should furnish something.
            _.AssertNotNull();

            bool TryReportParameterTypes(IEnumerable<Type> reportedTypes)
            {
                OutputHelper.WriteLine(
                    $"Reporting {expectedCount} expected "
                    // ReSharper disable once PossibleNullReferenceException
                    + $"types: {Join(", ", reportedTypes.Select(x => $"`{x.FullName} : {x.BaseType.FullName}´"))}"
                );
                return true;
            }

            var parameterTypes = ParameterTypes.ToArray();

            try
            {
                parameterTypes.Where(criteria.AssertNotNull()).AssertEqual(expectedCount, x => x.Count())
                    .AssertTrue(TryReportParameterTypes);
            }
            catch (Exception ex)
            {
                OutputHelper.WriteLine("Failed to verify derived Parameter matches.");
                parameterTypes.AssertTrue(TryReportParameterTypes);
                throw;
            }
        }

        private static IEnumerable<object[]> _parameterCreationTestCases;

        /* We can reference the type names properly because we are landing in a separate workspace
         * altogether. This does not work in the original CG context for whatever reason. After
         * some discussions in the Gitter forums, it is apparently also a fairly deep tooling
         * known issue. This being the case, this is the best possible workaround we can do is
         * to establish yet another solution and workspace in which to conduct the necessary unit
         * and integration tests verifying the results we expect.
         * https://gitter.im/dotnet/csharplang
         * https://gitter.im/Microsoft/VisualStudio */

        /// <summary>
        /// Gets the ParameterCreationTestCases.
        /// </summary>
        /// <see cref="RestartAlgorithm"/>
        /// <see cref="RestartAlgorithmsRepeatedParameter"/>
        public static IEnumerable<object[]> ParameterCreationTestCases
        {
            get
            {
                // ReSharper disable once CommentTypo
                /* In this we have built-in two de-facto smoke tests in the sense that these types
                 * are sourced by our internal Parameters Code Generation. In other words, yes, we
                 * need to perform the additional verification, but we have an immediate feedback
                 * that code generation has indeed occurred and at least generated an enumeration
                 * and a parameter, before having run any unit tests whatsoever. */

                // Specifically, RestartAlgorithmsRepeatedParameter and RestartAlgorithm.
                IEnumerable<object[]> GetAll()
                {
                    bool IsParameterTypeEligible(Type type)
                        => type != null
                           && type.Name != nameof(RestartAlgorithmsRepeatedParameter);

                    var restartAlgorithmType = ParametersAssembly.GetType(
                        $"{typeof(OrToolsSatParametersTests).Namespace}.{nameof(RestartAlgorithm)}"
                    );

                    var restartAlgorithmArrayType = restartAlgorithmType.MakeArrayType();

                    IEnumerable<Type> GetCtorTypes(Type type)
                    {
                        if (!IsParameterTypeEligible(type))
                        {
                            yield return restartAlgorithmType;
                            yield return restartAlgorithmArrayType;
                        }
                    }

                    IEnumerable<object> GetCtorArgs(Type type)
                    {
                        if (!IsParameterTypeEligible(type))
                        {
                            const int i = 0;
                            yield return Enum.GetValues(restartAlgorithmType).GetValue(i);
                            yield return Array.CreateInstance(restartAlgorithmType, 0);
                        }
                    }

                    foreach (var parameterType in ParameterTypes)
                    {
                        var types = GetCtorTypes(parameterType).ToArray();
                        yield return GetRange<object>(
                            parameterType
                            , types
                            , GetCtorArgs(parameterType).ToArray()).ToArray();
                    }
                }

                return _parameterCreationTestCases ?? (_parameterCreationTestCases = GetAll().ToArray());
            }
        }

        /// <summary>
        /// Verifies that the <paramref name="parameterType"/> Constructor Given
        /// <paramref name="argumentTypes"/> and <paramref name="args"/> themselves
        /// functions properly. That is, that we properly identify the corresponding
        /// <see cref="ConstructorInfo"/> and that it yields a valid <see cref="IParameter"/>
        /// instance.
        /// </summary>
        /// <param name="parameterType"></param>
        /// <param name="argumentTypes"></param>
        /// <param name="args"></param>
        [Theory, MemberData(nameof(ParameterCreationTestCases))]
        public void Given_Parameters_Ctor_Works(Type parameterType, Type[] argumentTypes, object[] args)
        {
            /* TODO: TBD: So far so good, but the DefaultRestartAlgorithmsRepeatedParameter repeated RestartAlgorithm parameter is proving a bit difficult...
             1. cannot directly use the types emitted, even though they are there.
             2. the DefaultRestartAlgorithmsRepeatedParameter type in particular is a corner case in its present form, required first parameter, optional set of params values thereafter...
             3. may consider a default ctor, or at least a single value ctor...
             4. at the same time introduce a third ctor taking IEnumerable<RestartAlgorithm> values...
             5. then if there are any differences we can isolate those test cases downstream...
             6a. might also consider how better to `publish´ the Kingdom.OrTools.Sat.Core assembly...
             6b. ...as a direct ProjectReference? ...or as yet another internally delivered PackageReference? */

            // TODO: TBD: SO! ... with all the above considerations...
            // TODO: TBD: we have a satisfactory workaround to the immediate issue...
            // TODO: TBD: notwithstanding cleaning some code up...
            // TODO: TBD: might consider receiving only the args, and inferring the types out...
            // TODO: TBD: should also reflect the Ctor out in the body of the test, since the idea is we can to verify the ctor during the test...
            // TODO: TBD: IOW, we are not there to receive a ctor, but to verify we can find it successfully, invoke it, etc, with the expected `args´.

            var ctor = parameterType.GetConstructor(argumentTypes).AssertNotNull();
            args = args.AssertNotNull();
            var instance = ctor.Invoke(args).AssertNotNull().AssertIsAssignableFrom<IParameter>();
            instance.AssertIsType(parameterType);
        }

        private static IEnumerable<object[]> _parameterDecoratedTestCases;

        public static IEnumerable<object[]> ParameterDecoratedTestCases
        {
            get
            {
                IEnumerable<object[]> GetAll()
                {
                    foreach (var x in ParameterTypes)
                    {
                        yield return GetRange<object>(x).ToArray();
                    }
                }

                return _parameterDecoratedTestCases ?? (_parameterDecoratedTestCases = GetAll().ToArray());
            }
        }

        private static string GetParameterDecoratedName(Type parameterType)
        {
            parameterType = parameterType.AssertNotNull().AssertTrue(x => x.IsClass && !x.IsAbstract);
            var attribute = parameterType.GetCustomAttribute<ParameterNameAttribute>().AssertNotNull();

            // TODO: TBD: we could get fancier here, like we are not expecting whitespace ...
            // TODO: TBD: perhaps only expecting in a character set compatible with the Protocol Buffer specification, that sort of thing...
            // TODO: TBD: but this will do for the time being...
            return attribute.Name.AssertNotNull().AssertNotEmpty();
        }

        [Theory, MemberData(nameof(ParameterDecoratedTestCases))]
        public void Parameter_Decorated_Correctly(Type parameterType)
        {
            // Much if not all of the actual verification occurs in the subsequent method.
            var actualName = GetParameterDecoratedName(parameterType);

            // Then we may simply report the result.
            OutputHelper.WriteLine($"Parameter type `{parameterType.FullName}´ named `{actualName}´.");
        }

        [Fact]
        public void Parameters_Distinctly_Decorated()
        {
            var names = ParameterTypes.Select(GetParameterDecoratedName).AssertNotNull().AssertNotEmpty().ToArray();
            names.Distinct().AssertEqual(names);
        }
    }
}
