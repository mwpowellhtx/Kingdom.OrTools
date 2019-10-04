using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Kingdom.OrTools.Sat.Parameters
{
    using static Double;
    using static Rendered;
    using RenderParameterValueCallbackDictionary = Dictionary<Type, RenderParameterValueCallback>;
    // Not to be confused with the Options interface definition. There is a difference!
    using IRenderParameterValueCallbackDictionary = IDictionary<Type, RenderParameterValueCallback>;

    public class ParameterValueRenderingOptions : IParameterValueRenderingOptions
    {
        private readonly IDictionary<Type, RenderParameterValueCallback> _callbacks;

        /// <summary>
        /// Provides a nominal <paramref name="value"/> Rendering experience.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        private static string DefaultParameterValueCallback<T>(T value) => $"{value}";

        private static string RenderParameterValue<T>(T value, Func<T, string> callback = null)
            => (callback ?? DefaultParameterValueCallback).Invoke(value);

        /// <summary>
        /// Rendering an <see cref="Enum"/> <paramref name="value"/> is a bit of a special case,
        /// but it is not that special of a case given a bit of introspection exposed by the
        /// Parameters API.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private static string RenderEnumParameterValue(object value)
        {
            var valueType = value.GetType();

            if (!valueType.IsEnum)
            {
                throw new InvalidOperationException($"Unexpected enum type `{valueType.FullName}´.");
            }

            var defaultRendering = $"{value}";

            var fi = valueType.GetField(defaultRendering);

            // TODO: TBD: ditto test case comments... FieldName and not MemberName ...
            return fi.GetCustomAttribute<ParameterMemberNameAttribute>()?.MemberName ?? defaultRendering;
        }

        /// <summary>
        /// Renders <see cref="PositiveInfinity"/> or <see cref="NegativeInfinity"/>, to
        /// <see cref="inf"/>, having positive signage implicit, whereas negative signage
        /// is explicit. Also renders <see cref="NaN"/> to <see cref="nan"/>. Otherwise,
        /// renders the <see cref="double"/> <paramref name="value"/> as a Round Trip value.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        internal static string RenderDoubleValue(double value)
        {
            string RenderNegativeInfinity()
                => IsNegativeInfinity(value)
                    ? $"{value}".Substring(0, 4).ToLower()
                    : null;

            string RenderPositiveInfinityOrNaN()
                => IsPositiveInfinity(value) || IsNaN(value)
                    ? $"{value}".Substring(0, 3).ToLower()
                    : null;

            string RenderRoundTrip() => $"{value:R}";

            return RenderNegativeInfinity() ?? RenderPositiveInfinityOrNaN() ?? RenderRoundTrip();
        }

        // ReSharper disable PossibleMultipleEnumeration
        /// <summary>
        /// Filters the <paramref name="values"/> thereby precluding an Empty Range.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="values"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        internal static IEnumerable<T> FilterValues<T>(IEnumerable<T> values, T defaultValue = default)
            => values.Any() ? values : new [] {defaultValue};
        // ReSharper restore PossibleMultipleEnumeration

        /// <summary>
        /// Returns a Nominal set of Parameter Value Rendering Callbacks.
        /// </summary>
        /// <returns></returns>
        private static IRenderParameterValueCallbackDictionary GetDefaultRenderingCallbacks()
        {
            /* Primitive use cases are provided. The user may furnish his or her own
             * beyond that. In addition, we support Enum rendering in a special way. */

            return new RenderParameterValueCallbackDictionary
            {
                {typeof(int), x => RenderParameterValue((int) x)},
                {typeof(long), x => RenderParameterValue((long) x)},
                {typeof(bool), x => RenderParameterValue((bool) x).ToLower()},
                {typeof(double), x => RenderDoubleValue((double) x)}
            };
        }

        /// <summary>
        /// Gets the DefaultOptions useful for
        /// <see cref="IParameter.ToString(IParameterValueRenderingOptions)"/>.
        /// </summary>
        public static IParameterValueRenderingOptions DefaultOptions => new ParameterValueRenderingOptions();

        /// <inheritdoc /> 
        private ParameterValueRenderingOptions() : this(GetDefaultRenderingCallbacks())
        {
        }

        /// <summary>
        /// Public Constructor.
        /// </summary>
        /// <param name="callbacks"></param>
        public ParameterValueRenderingOptions(IRenderParameterValueCallbackDictionary callbacks)
        {
            _callbacks = callbacks;
        }

        private TResult DictionaryFunc<TResult>(Func<IRenderParameterValueCallbackDictionary, TResult> func) => func(_callbacks);

        private void DictionaryAction(Action<IRenderParameterValueCallbackDictionary> action) => action(_callbacks);

        /// <inheritdoc />
        public IEnumerator<KeyValuePair<Type, RenderParameterValueCallback>> GetEnumerator() => DictionaryFunc(x => x.GetEnumerator());

        /// <inheritdoc />
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        /// <inheritdoc />
        public void Add(KeyValuePair<Type, RenderParameterValueCallback> item) => DictionaryAction(x => x.Add(item));

        /// <inheritdoc />
        public void Clear() => DictionaryAction(x => x.Clear());

        /// <inheritdoc />
        public bool Contains(KeyValuePair<Type, RenderParameterValueCallback> item) => DictionaryFunc(x => x.Contains(item));

        /// <inheritdoc />
        public void CopyTo(KeyValuePair<Type, RenderParameterValueCallback>[] array, int arrayIndex) => DictionaryAction(x => x.CopyTo(array, arrayIndex));

        /// <inheritdoc />
        public bool Remove(KeyValuePair<Type, RenderParameterValueCallback> item) => DictionaryFunc(x => x.Remove(item));

        /// <inheritdoc />
        public int Count => DictionaryFunc(x => x.Count);

        /// <inheritdoc />
        public bool IsReadOnly => DictionaryFunc(x => x.IsReadOnly);

        /// <inheritdoc />
        public void Add(Type key, RenderParameterValueCallback value) => DictionaryAction(x => x.Add(key, value));

        /// <inheritdoc />
        public bool ContainsKey(Type key) => DictionaryFunc(x => x.ContainsKey(key) || key.IsEnum);

        /// <inheritdoc />
        public bool Remove(Type key) => DictionaryFunc(x => x.Remove(key) && !key.IsEnum);

        /// <inheritdoc />
        public bool TryGetValue(Type key, out RenderParameterValueCallback value)
        {
            // Enum Rendering is a bit of a special case.
            bool TryGetEnumRenderingCallback(out RenderParameterValueCallback callback)
                => (callback = key.IsEnum ? RenderEnumParameterValue : (RenderParameterValueCallback) null) != null;

            RenderParameterValueCallback actual = null;

            var result = DictionaryFunc(x => (actual = x.TryGetValue(key, out var y) ? y : null) != null)
                         || TryGetEnumRenderingCallback(out actual);

            return (value = actual) != null && result;
        }

        /// <inheritdoc />
        public RenderParameterValueCallback this[Type key]
        {
            // Again holding that Enum is a special case of sorts.
            get => key.IsEnum ? RenderEnumParameterValue : DictionaryFunc(x => x[key]);
            set => DictionaryAction(x => x[key] = value);
        }

        /// <inheritdoc />
        public ICollection<Type> Keys => DictionaryFunc(x => x.Keys);

        /// <inheritdoc />
        public ICollection<RenderParameterValueCallback> Values => DictionaryFunc(x => x.Values);
    }
}
