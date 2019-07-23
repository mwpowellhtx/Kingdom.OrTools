using System;

namespace Kingdom.OrTools.Sat.Parameters
{
    using static Characters;
    using static Double;
    using static Names;
    using static String;
    using static AnnotatedWeekday;
    using static Month;
    using static StringComparison;

    internal class TestCaseDescriptor : IComparable<TestCaseDescriptor>
    {
        /// <summary>
        /// Gets an Id for Internal tracking.
        /// </summary>
        internal Guid Id { get; } = Guid.NewGuid();

        internal IParameter Instance { get; }

        internal virtual Type ValueType => Instance.ValueType;

        internal object Value => Instance.WeaklyTypedValue;

        protected internal string Rendered { get; protected set; }

        protected TestCaseDescriptor(IParameter instance)
        {
            Instance = instance;
            Rendered = null;
        }

        private static string RenderMonthValue(Month value)
        {
            switch (value)
            {
                case January:
                case February:
                case March:
                case April:
                case May:
                case June:
                case July:
                case August:
                case September:
                case October:
                case November:
                case December:
                    return $"{value}";

                default:
                    throw new InvalidOperationException($"`{typeof(Month).FullName}´ value `{value}´ is unsupported.");
            }
        }

        private static string RenderWeekdayValue(AnnotatedWeekday value)
        {
            switch (value)
            {
                case Monday:
                    return MONDAY;

                case Tuesday:
                    return TUESDAY;

                case Wednesday:
                    return WEDNESDAY;

                case Thursday:
                    return THURSDAY;

                case Friday:
                    return FRIDAY;

                default:
                    throw new InvalidOperationException($"`{typeof(AnnotatedWeekday).FullName}´ value `{value}´ is unsupported.");
            }
        }

        /// <summary>
        /// Returns the Rendered <paramref name="value"/> in a predictable manner.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        internal static string RenderValue(object value)
        {
            switch (value)
            {
                case bool _:
                    return $"{value}".ToLower();

                case int _:
                case long _:
                    return $"{value}";

                case double doubleValue:

                    string RenderBaseDouble() => $"{doubleValue}";

                    if (IsNaN(doubleValue) || IsPositiveInfinity(doubleValue))
                    {
                        // Same in either case.
                        return RenderBaseDouble().Substring(0, 3).ToLower();
                    }
                    else if (IsNegativeInfinity(doubleValue))
                    {
                        // Including Signage.
                        return RenderBaseDouble().Substring(0, 4).ToLower();
                    }

                    return $"{value:R}";

                case Month monthValue:
                    return RenderMonthValue(monthValue);

                case AnnotatedWeekday weekdayValue:
                    return RenderWeekdayValue(weekdayValue);

                case null:
                    return null;

                default:
                    throw new InvalidOperationException($"Value type `{value.GetType().FullName}´ is unsupported.");
            }
        }

        protected virtual int CompareTo(TestCaseDescriptor x, TestCaseDescriptor y)
        {
            const int lt = -1, gt = 1;

            // TODO: TBD: Descriptor Comparison, starting from the ValueType, if possible ...
            return x.ValueType == null
                ? lt
                : y.ValueType == null
                    ? gt
                    : Compare($"{x.ValueType.FullName}", $"{y.ValueType.FullName}", InvariantCulture);
        }

        /// <inheritdoc />
        public int CompareTo(TestCaseDescriptor other) => CompareTo(this, other);

        /// <summary>
        /// Returns the weakly typed Null instance of the Descriptor.
        /// </summary>
        internal static TestCaseDescriptor Null => Create(null);

        /// <summary>
        /// Returns a weakly typed <paramref name="instance"/> of the Descriptor.
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        internal static TestCaseDescriptor Create(IParameter instance)
            => new TestCaseDescriptor(instance);

        /// <summary>
        /// Returns a strongly typed instance of the Descriptor.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="instance"></param>
        /// <param name="parameterName"></param>
        /// <returns></returns>
        internal static TestCaseDescriptor Create<T>(IParameter<T> instance, string parameterName)
            where T : IComparable
            => new TestCaseDescriptor<T>(instance, parameterName);

        /// <summary>
        /// Returns a strongly typed instance of the Descriptor.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TParameter"></typeparam>
        /// <param name="instance"></param>
        /// <param name="parameterName"></param>
        /// <returns></returns>
        internal static TestCaseDescriptor Create<T, TParameter>(TParameter instance, string parameterName = null)
            where T : IComparable
            where TParameter : IParameter<T>
            => new TestCaseDescriptor<T, TParameter>(instance, parameterName);

        /// <summary>
        /// Returns a strongly typed instance of the Repeated Descriptor.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="instance"></param>
        /// <param name="parameterName"></param>
        /// <returns></returns>
        internal static TestCaseDescriptor CreateRepeated<T>(IRepeatedParameter<T> instance, string parameterName)
            where T : IComparable
            => new RepeatedTestCaseDescriptor<T>(instance, parameterName);

        /// <summary>
        /// Returns a strongly typed instance of the Repeated Descriptor.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TRepeated"></typeparam>
        /// <param name="instance"></param>
        /// <param name="parameterName"></param>
        /// <returns></returns>
        internal static TestCaseDescriptor CreateRepeated<T, TRepeated>(TRepeated instance, string parameterName = null)
            where T : IComparable
            where TRepeated : IRepeatedParameter<T>
            => new RepeatedTestCaseDescriptor<T, TRepeated>(instance, parameterName);
    }

    /// <inheritdoc />
    internal class TestCaseDescriptor<T> : TestCaseDescriptor
        where T : IComparable
    {
        /// <summary>
        /// Gets the strongly typed <see cref="IParameter{T}"/> Instance.
        /// </summary>
        internal new IParameter<T> Instance { get; }

        /// <summary>
        /// Gets the strongly typed <typeparamref name="T"/> Value.
        /// </summary>
        internal new T Value => Instance is null ? default : Instance.Value;

        /// <summary>
        /// Returns the Rendered <paramref name="instance"/>
        /// <see cref="IParameter.WeaklyTypedValue"/> in a predictable manner.
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        private static string RenderInstanceValue(IParameter instance) => RenderValue(instance?.WeaklyTypedValue);

        /// <summary>
        /// Protected Constructor.
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="parameterName"></param>
        internal TestCaseDescriptor(IParameter<T> instance, string parameterName)
            : base(instance)
        {
            Instance = instance;
            // TODO: TBD: Descriptor Rendering must include the Parameter as well, i.e. "parameter_name=rendered_value"
            Rendered = $"{parameterName}{Equal}{RenderInstanceValue(instance)}";
        }

        /// <inheritdoc />
        protected override int CompareTo(TestCaseDescriptor x, TestCaseDescriptor y)
        {
            int result;

            const int eq = 0;

            // TODO: TBD: may need/want the actual Parameter type comprehension here...
            // TODO: TBD: that is, over and above the Parameter ValueType ...
            switch (result = base.CompareTo(x, y))
            {
                case eq when x is TestCaseDescriptor<T> first && y is TestCaseDescriptor<T> second:
                    return first.Value.CompareTo(second.Value);

                default:
                    return result;
            }
        }
    }

    /// <inheritdoc />
    internal class TestCaseDescriptor<T, TParameter> : TestCaseDescriptor<T>
        where T : IComparable
        where TParameter : IParameter<T>
    {
        /// <summary>
        /// Gets the strongly typed <typeparamref name="TParameter"/> Instance.
        /// </summary>
        internal new TParameter Instance { get; }

        /// <summary>
        /// Internal Constructor.
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="parameterName">An optional ParameterName. Defaults to the Name of the
        /// <typeparamref name="TParameter"/>.</param>
        /// <inheritdoc />
        internal TestCaseDescriptor(TParameter instance, string parameterName = null)
            : base(instance, parameterName ?? typeof(TParameter).Name)
        {
            Instance = instance;
        }
    }
}
