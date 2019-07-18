using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Kingdom.OrTools.Sat.Parameters
{
    using Xunit;
    using Xunit.Abstractions;
    using static String;

    public abstract class ParameterTestFixtureBase : TestFixtureBase
    {
        protected ParameterTestFixtureBase(ITestOutputHelper outputHelper)
            : base(outputHelper)
        {
        }

        private static void VerifyParameter<T>(IParameter<T> parameter, T expectedValue, Type valueType, string rendered)
            => parameter.AssertNotNull()
                .AssertEqual(valueType.AssertNotNull(), x => x.ValueType)
                .AssertEqual(expectedValue, x => x.Value)
                .ToString().AssertEqual(
                    rendered.AssertNotNull().AssertNotEmpty()
                );

        private static void VerifyParameter(IParameter<double> parameter, double expectedValue, int precision, Type valueType, string rendered)
            => parameter.AssertNotNull()
                .AssertEqual(valueType.AssertNotNull(), x => x.ValueType)
                .AssertEqual(expectedValue, precision, x => x.Value)
                .ToString().AssertEqual(
                    rendered.AssertNotNull().AssertNotEmpty()
                );

        // ReSharper disable PossibleMultipleEnumeration
        private static void VerifyRepeatedParameter<T>(IRepeatedParameter<T> parameter, IEnumerable<T> expectedValues, Type itemType, string rendered)
            => parameter.AssertNotNull()
                .AssertEqual(itemType.AssertNotNull(), x => x.ItemType)
                .AssertEqual(rendered.AssertNotNull().AssertNotEmpty(), x => x.ToString())
                .AssertEqual(expectedValues.Count(), x => x.Value.Count)
                .Value.Zip(expectedValues, (x, y) => new {X = x, Y = y})
                .ToList().ForEach(zipped => zipped.X.AssertEqual(zipped.Y));

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
        /// <param name="precision"></param>
        /// <exception cref="InvalidOperationException">Thrown when <paramref name="parameter"/>
        /// and <paramref name="value"/> cannot be reconciled.</exception>
        [Theory, ClassData(typeof(IndividualParameterTestCases))]
        public void Each_Parameter_Renders_Correctly(IParameter parameter, Type valueOrItemType, object value, string rendered, int? precision)
        {
            const int defaultPrecision = 0;

            // Enumerate the Weakly Typed Values.
            IEnumerable<object> Enumerate(IEnumerable values)
            {
                foreach (var x in values)
                {
                    yield return x;
                }
            }

            switch (parameter)
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
                    VerifyRepeatedParameter(boolParam, boolValues, valueOrItemType, rendered);
                    break;

                case IRepeatedParameter<int> intParam when value is IEnumerable<int> intValues:
                    VerifyRepeatedParameter(intParam, intValues, valueOrItemType, rendered);
                    break;

                case IRepeatedParameter<long> longParam when value is IEnumerable<long> longValues:
                    VerifyRepeatedParameter(longParam, longValues, valueOrItemType, rendered);
                    break;

                case IRepeatedParameter<Month> monthParam when value is IEnumerable<Month> monthValues:
                    VerifyRepeatedParameter(monthParam, monthValues, valueOrItemType, rendered);
                    break;

                case IRepeatedParameter<AnnotatedWeekday> weekdayParam when value is IEnumerable<AnnotatedWeekday> weekdayValues:
                    VerifyRepeatedParameter(weekdayParam, weekdayValues, valueOrItemType, rendered);
                    break;

                case IRepeatedParameter<double> doubleParam when value is IEnumerable<double> doubleValues:
                    VerifyRepeatedParameter(doubleParam, doubleValues, precision.AssertNotNull() ?? defaultPrecision, valueOrItemType, rendered);
                    break;

                default:

                    string message = null;

                    // ReSharper disable once InvertIf
                    if (value is IEnumerable enumerableValue)
                    {
                        var renderedValue = Join(", ", Enumerate(enumerableValue).Select(x => $"{x}"));
                        message = $"Repeated `{nameof(parameter)}´ ({valueOrItemType.FullName}) with"
                                  + $" `{nameof(value)}´ [{renderedValue}] is an unexpected combination.";
                    }

                    throw new InvalidOperationException(
                        message ?? $"Single `{nameof(parameter)}´ ({valueOrItemType.FullName}) with"
                        + $" `{nameof(value)}´ ({value}) is an unexpected combination.");
            }
        }

        /// <summary>
        /// Verifies that Each Repeated Parameter Renders Correctly.
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="itemType"></param>
        /// <param name="values"></param>
        /// <param name="expected"></param>
        [Theory(Skip = "We may not need these tests after all..."), ClassData(typeof(IndividualRepeatedParameterTestCases))]
        public void Each_Repeated_Parameter_Renders_Correctly(IRepeatedParameter parameter, Type itemType, object[] values, string expected)
        {
            parameter.AssertNotNull()
                // TODO: TBD: need to probe for the Collection of Items...
                .AssertEqual(values, p => p.WeaklyTypedValue)
                .AssertEqual(itemType, p => p.ItemType)
                .ToString().AssertEqual(expected.AssertNotNull().AssertNotEmpty());
        }
    }
}
