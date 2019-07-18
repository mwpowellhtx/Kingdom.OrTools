using System;
using System.Collections.Generic;
using System.Linq;

namespace Kingdom.OrTools.Sat.Parameters
{
    using static Double;
    using static Names;
    using static Ordinals;
    using static String;
    using static ParameterValueRenderingOptions;
    using static AnnotatedWeekday;

    internal class IndividualParameterTestCases : ParameterTestCasesBase
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

        private static IEnumerable<T> GetEnumValues<T>()
            where T : struct
        {
            foreach (T x in Enum.GetValues(VerifyTypeIsEnum<T>()))
            {
                yield return x;
            }
        }

        private delegate IParameter<T> CreateParameterCallback<T>(T value);

        private delegate IRepeatedParameter<T> CreateRepeatedParameterCallback<T>(T value, T[] others);

        // ReSharper disable UnusedTypeParameter
        private delegate string RenderParameterValueCallback<TParameter, in T>(T value)
            where TParameter : IParameter<T>;

        private delegate string RenderRepeatedParameterValueCallback<TParameter, in T>(IEnumerable<T> values)
            where TParameter : IRepeatedParameter<T>;
        // ReSharper restore UnusedTypeParameter

        private static RenderParameterValueCallback<TParameter, T> GetDefaultRenderParameterValueCallback<TParameter, T>(Func<T, string> render = null, string parameterName = null)
            where TParameter : IParameter<T>
            => x => $"{parameterName ?? typeof(TParameter).Name}={(render ?? (y => $"{y}")).Invoke(x)}";

        private static RenderRepeatedParameterValueCallback<TParameter, T> GetDefaultRenderRepeatedParameterValueCallback<TParameter, T>(Func<IEnumerable<T>, string> render = null, string parameterName = null)
            where TParameter : IRepeatedParameter<T>
            => x =>
            {
                string RenderBits(IEnumerable<T> bits) => Join(",", FilterValues(bits).Select(y => $"{y}"));
                return $"{parameterName ?? typeof(TParameter).Name}={(render ?? RenderBits).Invoke(x)}";
            };

        private static IEnumerable<object> GetParameterTestCase<TParameter, T>(T value, CreateParameterCallback<T> create
            , RenderParameterValueCallback<TParameter, T> render, int? precision = null)
            where TParameter : IParameter<T>
            => GetRange<object>(create(value)
                , typeof(T), value, render(value), CurrentOrdinal, precision).ToArray();

        // ReSharper disable PossibleMultipleEnumeration
        private static IEnumerable<object> GetRepeatedParameterTestCase<TParameter, T>(IEnumerable<T> values, CreateRepeatedParameterCallback<T> create
            , RenderRepeatedParameterValueCallback<TParameter, T> render, int? precision = null)
            where TParameter : IRepeatedParameter<T>, new()
            => GetRange<object>(values.Any() ? create(values.First(), values.Skip(1).ToArray()) : new TParameter()
                , typeof(T), values, render(values), CurrentOrdinal, precision).ToArray();
        // ReSharper restore PossibleMultipleEnumeration

        private static IEnumerable<object[]> _privateCases;

        private static IEnumerable<object[]> PrivateCases
        {
            get
            {
                string RenderDoubleValue(double x) => ParameterValueRenderingOptions.RenderDoubleValue(x);

                string RenderBoolean(bool x) => $"{x}".ToLower();

                string RenderRepeatedBoolean(IEnumerable<bool> bits) => Join(",", FilterValues(bits).Select(RenderBoolean));

                // We do it this way because we are not here to use the code under test to inform the test case.
                string RenderAnnotatedWeekday(AnnotatedWeekday x)
                {
                    // ReSharper disable once SwitchStatementMissingSomeCases
                    switch (x)
                    {
                        case Monday: return MONDAY;
                        case Tuesday: return TUESDAY;
                        case Wednesday: return WEDNESDAY;
                        case Thursday: return THURSDAY;
                        case Friday: return FRIDAY;
                    }

                    throw new InvalidOperationException($"Unexpected value `{x}´.");
                }

                string RenderAnnotatedEnumValue<T>(T x) => x is AnnotatedWeekday y ? RenderAnnotatedWeekday(y) : $"{x}";

                // ReSharper disable PossibleMultipleEnumeration
                string RenderAnnotatedEnumValues<T>(IEnumerable<T> x) => Join(",", FilterValues(x).Select(RenderAnnotatedEnumValue));
                // ReSharper restore PossibleMultipleEnumeration

                IEnumerable<object[]> GetAllSingularParameters()
                {
                    // Default Boolean is False, correct...
                    yield return GetParameterTestCase(default, _ => new BooleanParameter(), GetDefaultRenderParameterValueCallback<BooleanParameter, bool>(RenderBoolean)).ToArray();
                    yield return GetParameterTestCase(true, y => new BooleanParameter(y), GetDefaultRenderParameterValueCallback<BooleanParameter, bool>(RenderBoolean)).ToArray();

                    yield return GetParameterTestCase(default, _ => new AnnotatedBooleanParameter(), GetDefaultRenderParameterValueCallback<AnnotatedBooleanParameter, bool>(RenderBoolean, annotated_boolean)).ToArray();
                    yield return GetParameterTestCase(true, y => new AnnotatedBooleanParameter(y), GetDefaultRenderParameterValueCallback<AnnotatedBooleanParameter, bool>(RenderBoolean, annotated_boolean)).ToArray();

                    yield return GetParameterTestCase(default, _ => new IntegerParameter(), GetDefaultRenderParameterValueCallback<IntegerParameter, int>()).ToArray();
                    yield return GetParameterTestCase(default, _ => new AnnotatedIntegerParameter(), GetDefaultRenderParameterValueCallback<AnnotatedIntegerParameter, int>(null, annotated_integer)).ToArray();

                    foreach (var x in GetRange(1, 2))
                    {
                        yield return GetParameterTestCase(x, y => new IntegerParameter(y), GetDefaultRenderParameterValueCallback<IntegerParameter, int>()).ToArray();
                        yield return GetParameterTestCase(-x, y => new IntegerParameter(y), GetDefaultRenderParameterValueCallback<IntegerParameter, int>()).ToArray();

                        yield return GetParameterTestCase(x, y => new AnnotatedIntegerParameter(y), GetDefaultRenderParameterValueCallback<AnnotatedIntegerParameter, int>(null, annotated_integer)).ToArray();
                        yield return GetParameterTestCase(-x, y => new AnnotatedIntegerParameter(y), GetDefaultRenderParameterValueCallback<AnnotatedIntegerParameter, int>(null, annotated_integer)).ToArray();
                    }

                    yield return GetParameterTestCase(default, _ => new LongParameter(), GetDefaultRenderParameterValueCallback<LongParameter, long>()).ToArray();
                    yield return GetParameterTestCase(default, _ => new AnnotatedLongParameter(), GetDefaultRenderParameterValueCallback<AnnotatedLongParameter, long>(null, annotated_long)).ToArray();

                    foreach (var x in GetRange<long>(1, 2))
                    {
                        yield return GetParameterTestCase(x, y => new LongParameter(y), GetDefaultRenderParameterValueCallback<LongParameter, long>()).ToArray();
                        yield return GetParameterTestCase(-x, y => new LongParameter(y), GetDefaultRenderParameterValueCallback<LongParameter, long>()).ToArray();

                        yield return GetParameterTestCase(x, y => new AnnotatedLongParameter(y), GetDefaultRenderParameterValueCallback<AnnotatedLongParameter, long>(null, annotated_long)).ToArray();
                        yield return GetParameterTestCase(-x, y => new AnnotatedLongParameter(y), GetDefaultRenderParameterValueCallback<AnnotatedLongParameter, long>(null, annotated_long)).ToArray();
                    }

                    yield return GetParameterTestCase(default, _ => new MonthParameter(), GetDefaultRenderParameterValueCallback<MonthParameter, Month>(RenderAnnotatedEnumValue)).ToArray();
                    yield return GetParameterTestCase(default, _ => new AnnotatedMonthParameter(), GetDefaultRenderParameterValueCallback<AnnotatedMonthParameter, Month>(RenderAnnotatedEnumValue, annotated_month)).ToArray();

                    // Ostensibly Skipping the Default Value.
                    foreach (var x in GetEnumValues<Month>().Skip(1))
                    {
                        yield return GetParameterTestCase(x, y => new MonthParameter(y), GetDefaultRenderParameterValueCallback<MonthParameter, Month>(RenderAnnotatedEnumValue)).ToArray();
                        yield return GetParameterTestCase(x, y => new AnnotatedMonthParameter(y), GetDefaultRenderParameterValueCallback<AnnotatedMonthParameter, Month>(RenderAnnotatedEnumValue, annotated_month)).ToArray();
                    }

                    yield return GetParameterTestCase(default, _ => new WeekdayParameter(), GetDefaultRenderParameterValueCallback<WeekdayParameter, AnnotatedWeekday>(RenderAnnotatedEnumValue)).ToArray();
                    yield return GetParameterTestCase(default, _ => new AnnotatedWeekdayParameter(), GetDefaultRenderParameterValueCallback<AnnotatedWeekdayParameter, AnnotatedWeekday>(RenderAnnotatedEnumValue, annotated_weekday)).ToArray();

                    // Ditto Enumerated Default Values...
                    foreach (var x in GetEnumValues<AnnotatedWeekday>().Skip(1))
                    {
                        yield return GetParameterTestCase(x, y => new WeekdayParameter(y), GetDefaultRenderParameterValueCallback<WeekdayParameter, AnnotatedWeekday>(RenderAnnotatedEnumValue)).ToArray();
                        yield return GetParameterTestCase(x, y => new AnnotatedWeekdayParameter(y), GetDefaultRenderParameterValueCallback<AnnotatedWeekdayParameter, AnnotatedWeekday>(RenderAnnotatedEnumValue, annotated_weekday)).ToArray();
                    }

                    const int precision = 3;

                    yield return GetParameterTestCase(default, _ => new DoubleParameter(), GetDefaultRenderParameterValueCallback<DoubleParameter, double>(RenderDoubleValue), precision).ToArray();
                    yield return GetParameterTestCase(default, _ => new AnnotatedDoubleParameter(), GetDefaultRenderParameterValueCallback<AnnotatedDoubleParameter, double>(RenderDoubleValue, annotated_double), precision).ToArray();

                    yield return GetParameterTestCase(NegativeInfinity, y => new DoubleParameter(y), GetDefaultRenderParameterValueCallback<DoubleParameter, double>(RenderDoubleValue), precision).ToArray();
                    yield return GetParameterTestCase(PositiveInfinity, y => new DoubleParameter(y), GetDefaultRenderParameterValueCallback<DoubleParameter, double>(RenderDoubleValue), precision).ToArray();
                    yield return GetParameterTestCase(NaN, y => new DoubleParameter(y), GetDefaultRenderParameterValueCallback<DoubleParameter, double>(RenderDoubleValue), precision).ToArray();

                    yield return GetParameterTestCase(NegativeInfinity, y => new AnnotatedDoubleParameter(y), GetDefaultRenderParameterValueCallback<AnnotatedDoubleParameter, double>(RenderDoubleValue, annotated_double), precision).ToArray();
                    yield return GetParameterTestCase(PositiveInfinity, y => new AnnotatedDoubleParameter(y), GetDefaultRenderParameterValueCallback<AnnotatedDoubleParameter, double>(RenderDoubleValue, annotated_double), precision).ToArray();
                    yield return GetParameterTestCase(NaN, y => new AnnotatedDoubleParameter(y), GetDefaultRenderParameterValueCallback<AnnotatedDoubleParameter, double>(RenderDoubleValue, annotated_double), precision).ToArray();

                    foreach (var x in GetRange<double>(1, 2))
                    {
                        yield return GetParameterTestCase(x, y => new DoubleParameter(y), GetDefaultRenderParameterValueCallback<DoubleParameter, double>(RenderDoubleValue), precision).ToArray();
                        yield return GetParameterTestCase(-x, y => new DoubleParameter(y), GetDefaultRenderParameterValueCallback<DoubleParameter, double>(RenderDoubleValue), precision).ToArray();

                        yield return GetParameterTestCase(x, y => new AnnotatedDoubleParameter(y), GetDefaultRenderParameterValueCallback<AnnotatedDoubleParameter, double>(RenderDoubleValue, annotated_double), precision).ToArray();
                        yield return GetParameterTestCase(-x, y => new AnnotatedDoubleParameter(y), GetDefaultRenderParameterValueCallback<AnnotatedDoubleParameter, double>(RenderDoubleValue, annotated_double), precision).ToArray();
                    }
                }

                IEnumerable<object[]> GetAllRepeatedParameters()
                {
                    var boolValues = GetRange(default, true).ToArray();

                    for (var i = 0; i <= boolValues.Length; ++i)
                    {
                        yield return GetRepeatedParameterTestCase(boolValues.Take(i).ToArray(), (x, y) => new BooleanRepeatedParameter(x, y), GetDefaultRenderRepeatedParameterValueCallback<BooleanRepeatedParameter, bool>(RenderRepeatedBoolean)).ToArray();
                        yield return GetRepeatedParameterTestCase(boolValues.Take(i).ToArray(), (x, y) => new AnnotatedBooleanRepeatedParameter(x, y), GetDefaultRenderRepeatedParameterValueCallback<AnnotatedBooleanRepeatedParameter, bool>(RenderRepeatedBoolean, annotated_boolean_repeated)).ToArray();
                    }

                    var intValues = GetRange(1, 2, 3).ToArray();

                    for (var i = 0; i <= intValues.Length; ++i)
                    {
                        yield return GetRepeatedParameterTestCase(intValues.Take(i).ToArray(), (x, y) => new IntegerRepeatedParameter(x, y), GetDefaultRenderRepeatedParameterValueCallback<IntegerRepeatedParameter, int>()).ToArray();
                        yield return GetRepeatedParameterTestCase(intValues.Take(i).ToArray(), (x, y) => new AnnotatedIntegerRepeatedParameter(x, y), GetDefaultRenderRepeatedParameterValueCallback<AnnotatedIntegerRepeatedParameter, int>(parameterName: annotated_integer_repeated)).ToArray();
                    }

                    var longValues = GetRange<long>(1, 2, 3).ToArray();

                    for (var i = 0; i <= longValues.Length; ++i)
                    {
                        yield return GetRepeatedParameterTestCase(longValues.Take(i).ToArray(), (x, y) => new LongRepeatedParameter(x, y), GetDefaultRenderRepeatedParameterValueCallback<LongRepeatedParameter, long>()).ToArray();
                        yield return GetRepeatedParameterTestCase(longValues.Take(i).ToArray(), (x, y) => new AnnotatedLongRepeatedParameter(x, y), GetDefaultRenderRepeatedParameterValueCallback<AnnotatedLongRepeatedParameter, long>(parameterName: annotated_long_repeated)).ToArray();
                    }

                    var monthValues = GetEnumValues<Month>().ToArray();

                    for (var i = 0; i <= monthValues.Length; ++i)
                    {
                        yield return GetRepeatedParameterTestCase(monthValues.Take(i).ToArray(), (x, y) => new MonthRepeatedParameter(x, y), GetDefaultRenderRepeatedParameterValueCallback<MonthRepeatedParameter, Month>(RenderAnnotatedEnumValues)).ToArray();
                        yield return GetRepeatedParameterTestCase(monthValues.Take(i).ToArray(), (x, y) => new AnnotatedMonthRepeatedParameter(x, y), GetDefaultRenderRepeatedParameterValueCallback<AnnotatedMonthRepeatedParameter, Month>(RenderAnnotatedEnumValues, annotated_month_repeated)).ToArray();
                    }

                    var weekdayValues = GetEnumValues<AnnotatedWeekday>().ToArray();

                    for (var i = 0; i <= weekdayValues.Length; ++i)
                    {
                        yield return GetRepeatedParameterTestCase(weekdayValues.Take(i).ToArray(), (x, y) => new WeekdayRepeatedParameter(x, y), GetDefaultRenderRepeatedParameterValueCallback<WeekdayRepeatedParameter, AnnotatedWeekday>(RenderAnnotatedEnumValues)).ToArray();
                        yield return GetRepeatedParameterTestCase(weekdayValues.Take(i).ToArray(), (x, y) => new AnnotatedWeekdayRepeatedParameter(x, y), GetDefaultRenderRepeatedParameterValueCallback<AnnotatedWeekdayRepeatedParameter, AnnotatedWeekday>(RenderAnnotatedEnumValues, annotated_weekday_repeated)).ToArray();
                    }

                    // TODO: TBD: next is to itemize the litany of Double cases...
                }

                return _privateCases ?? (_privateCases = GetAllSingularParameters().Concat(GetAllRepeatedParameters()).ToArray());
            }
        }

        protected override IEnumerable<object[]> Cases => PrivateCases;
    }
}
