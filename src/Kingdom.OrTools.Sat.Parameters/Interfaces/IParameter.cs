using System;

namespace Kingdom.OrTools.Sat.Parameters
{
    /// <summary>
    /// Represents all Parameter assets.
    /// </summary>
    public interface IParameter
    {
        /// <summary>
        /// Gets the Parameter Ordinal value.
        /// </summary>
        long Ordinal { get; }

        /// <summary>
        /// Gets the ParameterName.
        /// </summary>
        string ParameterName { get; }

        /// <summary>
        /// Gets the WeaklyTypedValue.
        /// </summary>
        object WeaklyTypedValue { get; }

        /// <summary>
        /// Gets the ValueType.
        /// </summary>
        Type ValueType { get; }

        /// <summary>
        /// Renders the <see cref="IParameter"/> assuming a nominal set of
        /// <paramref name="options"/>.
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        string ToString(IParameterValueRenderingOptions options);
    }
}
