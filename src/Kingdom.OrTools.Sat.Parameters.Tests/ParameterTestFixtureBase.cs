using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Kingdom.OrTools.Sat.Parameters
{
    using Xunit;
    using Xunit.Abstractions;
    using static String;
    using static ParameterValueRenderingOptions;

    public abstract class ParameterTestFixtureBase : TestFixtureBase
    {
        protected ParameterTestFixtureBase(ITestOutputHelper outputHelper)
            : base(outputHelper)
        {
        }

        /// <summary>
        /// Verifies the <paramref name="parameter"/> in the <paramref name="expectedValue"/>,
        /// <paramref name="valueType"/> and that it <paramref name="rendered"/> correctly.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="parameter"></param>
        /// <param name="expectedValue"></param>
        /// <param name="valueType"></param>
        /// <param name="rendered"></param>
        private static void VerifyParameter<T>(IParameter<T> parameter, T expectedValue, Type valueType, string rendered)
            => parameter.AssertNotNull()
                .AssertEqual(valueType.AssertNotNull(), x => x.ValueType)
                .AssertEqual(expectedValue, x => x.Value)
                .ToString().AssertEqual(rendered);

        /// <summary>
        /// Verifies the <paramref name="parameter"/> in the <paramref name="expectedValue"/>
        /// and <paramref name="precision"/>, <paramref name="valueType"/> and that it
        /// <paramref name="rendered"/> correctly.
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="expectedValue"></param>
        /// <param name="precision"></param>
        /// <param name="valueType"></param>
        /// <param name="rendered"></param>
        private static void VerifyParameter(IParameter<double> parameter, double expectedValue, int precision, Type valueType, string rendered)
            => parameter.AssertNotNull()
                .AssertEqual(valueType.AssertNotNull(), x => x.ValueType)
                .AssertEqual(expectedValue, precision, x => x.Value)
                .ToString().AssertEqual(rendered);

        // ReSharper disable PossibleMultipleEnumeration
        /// <summary>
        /// Verifies the <paramref name="parameter"/> in the <paramref name="expectedValues"/>
        /// and <paramref name="itemType"/> and that it <paramref name="rendered"/> correctly.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="parameter"></param>
        /// <param name="expectedValues"></param>
        /// <param name="itemType"></param>
        /// <param name="rendered"></param>
        private static void VerifyRepeatedParameter<T>(IRepeatedParameter<T> parameter, IEnumerable<T> expectedValues, Type itemType, string rendered)
            => parameter.AssertNotNull()
                .AssertEqual(itemType.AssertNotNull(), x => x.ItemType)
                .AssertEqual(rendered.AssertNotNull().AssertNotEmpty(), x => x.ToString())
                .AssertEqual(expectedValues.Count(), x => x.Value.Count)
                .Value.Zip(expectedValues, (x, y) => new {X = x, Y = y})
                .ToList().ForEach(zipped => zipped.X.AssertEqual(zipped.Y));

        /// <summary>
        /// Verifies the <paramref name="parameter"/> in the <paramref name="expectedValues"/>
        /// and <paramref name="precision"/>, <paramref name="itemType"/> and that it
        /// <paramref name="rendered"/> correctly.
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="expectedValues"></param>
        /// <param name="precision"></param>
        /// <param name="itemType"></param>
        /// <param name="rendered"></param>
        private static void VerifyRepeatedParameter(IRepeatedParameter<double> parameter, IEnumerable<double> expectedValues, int precision, Type itemType, string rendered)
            => parameter.AssertNotNull()
                .AssertEqual(itemType.AssertNotNull(), x => x.ItemType)
                .AssertEqual(rendered.AssertNotNull().AssertNotEmpty(), x => x.ToString())
                .AssertEqual(expectedValues.Count(), x => x.Value.Count)
                .Value.Zip(expectedValues, (x, y) => new {X = x, Y = y})
                .ToList().ForEach(zipped => zipped.X.AssertEqual(zipped.Y, precision));
        // ReSharper restore PossibleMultipleEnumeration

        /// <summary>
        /// Verifies that Each Parameter Renders Correctly.
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="valueOrItemType"></param>
        /// <param name="value"></param>
        /// <param name="rendered"></param>
        /// <param name="ordinal"></param>
        /// <param name="precision"></param>
        /// <exception cref="InvalidOperationException">Thrown when <paramref name="parameter"/>
        /// and <paramref name="value"/> cannot be reconciled.</exception>
        [Theory, ClassData(typeof(IndividualParameterTestCases))]
        public void Each_Parameter_Renders_Correctly(IParameter parameter, Type valueOrItemType, object value, string rendered, long ordinal, int? precision)
        {
            const int defaultPrecision = 0;

            valueOrItemType.AssertNotNull();
            value.AssertNotNull();
            ordinal.AssertTrue(x => x > 0L);
            rendered.AssertNotNull().AssertNotEmpty();

            switch (parameter.AssertNotNull().AssertEqual(ordinal, x => x.Ordinal))
            {
                case IParameter<bool> boolParam when value is bool boolValue:
                    VerifyParameter(boolParam, boolValue, valueOrItemType, rendered);
                    break;

                case IParameter<int> intParam when value is int intValue:
                    VerifyParameter(intParam, intValue, valueOrItemType, rendered);
                    break;

                case IParameter<long> longParam when value is long longValue:
                    VerifyParameter(longParam, longValue, valueOrItemType, rendered);
                    break;

                case IParameter<Month> monthParam when value is Month monthValue:
                    VerifyParameter(monthParam, monthValue, valueOrItemType, rendered);
                    break;

                case IParameter<AnnotatedWeekday> weekdayParam when value is AnnotatedWeekday weekdayValue:
                    VerifyParameter(weekdayParam, weekdayValue, valueOrItemType, rendered);
                    break;

                case IParameter<double> doubleParam when value is double doubleValue:
                    VerifyParameter(doubleParam, doubleValue, precision.AssertNotNull() ?? defaultPrecision, valueOrItemType, rendered);
                    break;

                case IRepeatedParameter<bool> boolParam when value is IEnumerable<bool> boolValues:
                    VerifyRepeatedParameter(boolParam, FilterValues(boolValues), valueOrItemType, rendered);
                    break;

                case IRepeatedParameter<int> intParam when value is IEnumerable<int> intValues:
                    VerifyRepeatedParameter(intParam, FilterValues(intValues), valueOrItemType, rendered);
                    break;

                case IRepeatedParameter<long> longParam when value is IEnumerable<long> longValues:
                    VerifyRepeatedParameter(longParam, FilterValues(longValues), valueOrItemType, rendered);
                    break;

                case IRepeatedParameter<Month> monthParam when value is IEnumerable<Month> monthValues:
                    VerifyRepeatedParameter(monthParam, FilterValues(monthValues), valueOrItemType, rendered);
                    break;

                case IRepeatedParameter<AnnotatedWeekday> weekdayParam when value is IEnumerable<AnnotatedWeekday> weekdayValues:
                    VerifyRepeatedParameter(weekdayParam, FilterValues(weekdayValues), valueOrItemType, rendered);
                    break;

                case IRepeatedParameter<double> doubleParam when value is IEnumerable<double> doubleValues:
                    VerifyRepeatedParameter(doubleParam, FilterValues(doubleValues), precision.AssertNotNull() ?? defaultPrecision, valueOrItemType, rendered);
                    break;

                default:

                    string message = null;

                    // Borderline repeating ourselves here, but it is not terrible.
                    string RenderValue(object x) => x is bool
                        ? $"{x}".ToLower()
                        : x is double y
                            ? $"{y:R}"
                            : $"{x}";

                    if (value is IEnumerable enumerableValue)
                    {
                        var renderedValue = Join(", ", Enumerate(enumerableValue).Select(RenderValue));
                        message = $"Repeated `{parameter.GetType().FullName}´ ({valueOrItemType.FullName}) with"
                                  + $" `{nameof(value)}´ [{renderedValue}] is an unexpected combination.";
                    }

                    string GetSingleParameterMessage()
                    {
                        return $"Single `{parameter.GetType().FullName}´ ({valueOrItemType.FullName}) with"
                               + $" `{nameof(value)}´ ({RenderValue(value)}) is an unexpected combination.";
                    }

                    throw new InvalidOperationException(message ?? GetSingleParameterMessage());
            }
        }

        // TODO: TBD: could potentially see this refactored into a more general library / API at some point...
        // ReSharper disable once CommentTypo
        // TODO: TBD: Perhaps the Collections libraries? Or even Combinatorics?
        /// <summary>
        /// This is not necessarily a Parameters test, apart from the Slicing strategy
        /// being borne out of the effort.
        /// </summary>
        [Fact]
        public void Try_Slicing_Descriptor_Subsets()
        {
            var original = ParameterTestCasesBase.Descriptors.AssertNotNull().AssertNotEmpty().ToArray();

            IEnumerable<TestCaseDescriptor> descriptors = original.ToArray();

            // ReSharper disable once RedundantEmptyObjectOrCollectionInitializer
            var slices = new List<IEnumerable<TestCaseDescriptor>> { };

            // Permutations are definitely overkill, this is perfectly adequate.
            while (descriptors.TrySliceOverCollection(7, out var slice, out descriptors))
            {
                slices.Add(slice);
            }

            // We should have All of the Original Elements accounted for.
            slices.AssertNotEmpty().AssertEqual(original.Length, x => x.Sum(y => y.Count()));

            // Which we may further verify in actual.
            slices.SelectMany(x => x.Select(y => y.Id)).OrderBy(y => y).AssertEqual(original.Select(y => y.Id).OrderBy(y => y));
        }

        // TODO: TBD: did some refactoring in the test cases...
        // TODO: TBD: circle around on this one after rinsing that one out...
        // ReSharper disable PossibleMultipleEnumeration
        /// <summary>
        /// Verifies that the <paramref name="collection"/> is <paramref name="rendered"/>
        /// correctly and that it reflects the <paramref name="expected"/>
        /// <see cref="IParameter"/> elements.
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="expected"></param>
        /// <param name="rendered"></param>
        [Theory, ClassData(typeof(ParameterCollectionRenderingTestCases))]
        public void Parameter_Collection_Renders_Correctly(IParameterCollection collection, IEnumerable<IParameter> expected, string rendered)
        {
            collection.AssertNotNull().AssertEqual(expected.AssertNotNull().Count(), x => x.Count);
            collection.Zip(expected, (x, y) => new {x, y}).ToList().ForEach(zipped => zipped.x.AssertSame(zipped.y));
            collection.ToString().AssertNotNull().AssertEqual(rendered.AssertNotNull());
        }
        // ReSharper restore PossibleMultipleEnumeration
    }
}
