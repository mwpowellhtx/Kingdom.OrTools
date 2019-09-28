using System;

namespace Kingdom.OrTools.Sat.Parameters
{
    using static Characters;

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
        /// <paramref name="options"/>. At a Parameter level, name value pairs are to be
        /// <see cref="Colon"/> delimited.
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        /// <see cref="Colon"/>
        /// <see cref="!:https://groups.google.com/forum/#!searchin/or-tools-discuss/sat$20parameter$20string%7Csort:date"/>
        /// <see cref="!:https://groups.google.com/forum/#!searchin/or-tools-discuss/sat$20parameter$20string%7Csort:date/or-tools-discuss/X4Y_ZpKIUp8/kz-xiKSYEAAJ"/>
        string ToString(IParameterValueRenderingOptions options);
    }
}
