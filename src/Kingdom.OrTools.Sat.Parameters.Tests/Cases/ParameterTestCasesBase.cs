using System;
using System.Collections.Generic;
using System.Linq;

namespace Kingdom.OrTools.Sat.Parameters
{
    using static Double;
    using static Names;
    using static TestCaseDescriptor;
    using Xunit.Theoretically;

    // TODO: TBD: potentially fleshing this out a bit more for purposes of supporting both individual and collection oriented test cases...

    // TODO: TBD: fill in some gaps here when we determine what the shape and composition of the parameter unit test should be...
    internal abstract class ParameterTestCasesBase : TestCasesBase
    {
        private static Type VerifyTypeIsEnum<T>()
        {
            var type = typeof(T);
            if (!type.IsEnum)
            {
                throw new InvalidOperationException($"Type `{type.FullName}´ is not an enum.");
            }

            return type;
        }

        protected static IEnumerable<T> GetEnumValues<T>()
            where T : struct
        {
            foreach (T x in Enum.GetValues(VerifyTypeIsEnum<T>()))
            {
                yield return x;
            }
        }

        private static IEnumerable<TestCaseDescriptor> _descriptors;

        /// <summary>
        /// Gets the <see cref="TestCaseDescriptor"/> Range for Internal use. This is provided as
        /// a huge convenience with common things such as expected rendering neatly stitched away
        /// in the <see cref="TestCaseDescriptor"/> classes.
        /// </summary>
        internal static IEnumerable<TestCaseDescriptor> Descriptors
        {
            get
            {
                TestCaseDescriptor Make<T, TParameter>(T value, Func<T, TParameter> create = null, string parameterName = null)
                    where T : IComparable
                    where TParameter : IParameter<T>, new()
                    => Create<T, TParameter>((create ?? (x => new TParameter {Value = x})).Invoke(value), parameterName);

                IEnumerable<TestCaseDescriptor> MakeRange<T, TParameter>(IEnumerable<T> values, Func<T, TParameter> create = null, string parameterName = null)
                    where T : IComparable
                    where TParameter : IParameter<T>, new()
                {
                    foreach (var value in values)
                    {
                        yield return Make(value, create, parameterName);
                    }
                }

                TestCaseDescriptor MakeRepeated<T, TParameter>(IEnumerable<T> values, Func<IEnumerable<T>, TParameter> create, string parameterName = null)
                    where T : IComparable
                    where TParameter : IRepeatedParameter<T>, new()
                    => CreateRepeated<T, TParameter>(create.Invoke(values), parameterName);

                // ReSharper disable PossibleMultipleEnumeration
                IEnumerable<TestCaseDescriptor> GetAll()
                {
                    var boolValues = GetRange(default, true).ToArray();
                    var intValues = GetRange(default, 1, 2).ToArray();
                    var longValues = GetRange<long>(default, 1, 2).ToArray();
                    var doubleValues = GetRange(default, 1, 2, PositiveInfinity, NegativeInfinity, NaN).ToArray();
                    var weekdayValues = GetEnumValues<AnnotatedWeekday>().ToArray();
                    var monthValues = GetEnumValues<Month>().ToArray();

                    foreach (var x in MakeRange(boolValues, x => new BooleanParameter(x)))
                    {
                        yield return x;
                    }

                    foreach (var x in MakeRange(boolValues, x => new AnnotatedBooleanParameter(x), annotated_boolean))
                    {
                        yield return x;
                    }

                    yield return MakeRepeated(boolValues, x => new BooleanRepeatedParameter(x.First(), x.Skip(1).ToArray()));
                    yield return MakeRepeated(boolValues, x => new AnnotatedBooleanRepeatedParameter(x.First(), x.Skip(1).ToArray()), annotated_boolean_repeated);

                    foreach (var x in MakeRange(intValues, x => new IntegerParameter(x)))
                    {
                        yield return x;
                    }

                    foreach (var x in MakeRange(intValues, x => new AnnotatedIntegerParameter(x), annotated_integer))
                    {
                        yield return x;
                    }

                    yield return MakeRepeated(intValues, x => new IntegerRepeatedParameter(x.First(), x.Skip(1).ToArray()));
                    yield return MakeRepeated(intValues, x => new AnnotatedIntegerRepeatedParameter(x.First(), x.Skip(1).ToArray()), annotated_integer_repeated);

                    foreach (var x in MakeRange(longValues, x => new LongParameter(x)))
                    {
                        yield return x;
                    }

                    foreach (var x in MakeRange(longValues, x => new AnnotatedLongParameter(x), annotated_long))
                    {
                        yield return x;
                    }

                    yield return MakeRepeated(longValues, x => new LongRepeatedParameter(x.First(), x.Skip(1).ToArray()));
                    yield return MakeRepeated(longValues, x => new AnnotatedLongRepeatedParameter(x.First(), x.Skip(1).ToArray()), annotated_long_repeated);

                    foreach (var x in MakeRange(doubleValues, x => new DoubleParameter(x)))
                    {
                        yield return x;
                    }

                    foreach (var x in MakeRange(doubleValues, x => new AnnotatedDoubleParameter(x), annotated_double))
                    {
                        yield return x;
                    }

                    yield return MakeRepeated(doubleValues, x => new DoubleRepeatedParameter(x.First(), x.Skip(1).ToArray()));
                    yield return MakeRepeated(doubleValues, x => new AnnotatedDoubleRepeatedParameter(x.First(), x.Skip(1).ToArray()), annotated_double_repeated);

                    foreach (var x in MakeRange(monthValues, x => new MonthParameter(x)))
                    {
                        yield return x;
                    }

                    foreach (var x in MakeRange(monthValues, x => new AnnotatedMonthParameter(x), annotated_month))
                    {
                        yield return x;
                    }

                    yield return MakeRepeated(monthValues, x => new MonthRepeatedParameter(x.First(), x.Skip(1).ToArray()));
                    yield return MakeRepeated(monthValues, x => new AnnotatedMonthRepeatedParameter(x.First(), x.Skip(1).ToArray()), annotated_month_repeated);

                    foreach (var x in MakeRange(weekdayValues, x => new WeekdayParameter(x)))
                    {
                        yield return x;
                    }

                    foreach (var x in MakeRange(weekdayValues, x => new AnnotatedWeekdayParameter(x), annotated_weekday))
                    {
                        yield return x;
                    }

                    yield return MakeRepeated(weekdayValues, x => new WeekdayRepeatedParameter(x.First(), x.Skip(1).ToArray()));
                    yield return MakeRepeated(weekdayValues, x => new AnnotatedWeekdayRepeatedParameter(x.First(), x.Skip(1).ToArray()), annotated_weekday_repeated);
                }
                // ReSharper restore PossibleMultipleEnumeration

                return _descriptors ?? (_descriptors = GetAll().ToArray());
            }
        }
    }
}
