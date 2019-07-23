using System;
using System.Collections.Generic;
using System.Linq;

namespace Kingdom.OrTools.Sat.Parameters
{
    using static Characters;
    using static String;

    /// <inheritdoc />
    internal class RepeatedTestCaseDescriptor<T> : TestCaseDescriptor
        where T : IComparable
    {
        /// <summary>
        /// Gets the ItemType for Internal use.
        /// </summary>
        internal Type ItemType { get; }

        /// <inheritdoc />
        internal override Type ValueType { get; }

        /// <summary>
        /// Gets the strongly typed <see cref="IRepeatedParameter{T}"/> Instance.
        /// </summary>
        internal new IRepeatedParameter<T> Instance { get; }

        /// <summary>
        /// Gets the strongly typed <see cref="IRepeatedParameter{T}.Value"/>.
        /// </summary>
        internal new ICollection<T> Value => Instance.Value;

        /// <inheritdoc />
        internal RepeatedTestCaseDescriptor(IRepeatedParameter<T> instance, string parameterName)
            : base(instance)
        {
            Instance = instance;
            ItemType = typeof(T);
            ValueType = typeof(ICollection<T>);
            Rendered = $"{parameterName}{Equal}{RenderRepeatedValues(instance?.Value?.ToArray())}";
        }

        /// <summary>
        /// Returns the <paramref name="values"/> Rendered for Repeated use.
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        private static string RenderRepeatedValues(IEnumerable<T> values)
            => Join($"{Comma}"
                , (values ?? Array.Empty<T>()).Select(x => (object) x).Select(RenderValue)
            );
    }

    /// <inheritdoc />
    internal class RepeatedTestCaseDescriptor<T, TRepeated> : RepeatedTestCaseDescriptor<T>
        where T : IComparable
        where TRepeated : IRepeatedParameter<T>
    {
        /// <summary>
        /// Gets the strongly typed <typeparamref name="TRepeated"/> Instance.
        /// </summary>
        internal new TRepeated Instance { get; }

        /// <inheritdoc />
        internal RepeatedTestCaseDescriptor(TRepeated instance, string parameterName = null)
            : base(instance, parameterName ?? typeof(TRepeated).Name)
        {
            Instance = instance;
        }
    }
}
