using System.Collections.Generic;

namespace Kingdom.OrTools.Sat.Parameters
{
    using static Characters;
    using IParameterCollectionType = ICollection<IParameter>;

    /// <inheritdoc />
    public interface IParameterCollection : IParameterCollectionType
    {
        /// <summary>
        /// Renders the Collection given a nominal set of <paramref name="options"/>.
        /// At a Collection level, Parameters are to be <see cref="Space"/> delimited.
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        /// <see cref="Space"/>
        /// <see cref="!:https://groups.google.com/forum/#!searchin/or-tools-discuss/sat$20parameter$20string%7Csort:date"/>
        /// <see cref="!:https://groups.google.com/forum/#!searchin/or-tools-discuss/sat$20parameter$20string%7Csort:date/or-tools-discuss/X4Y_ZpKIUp8/kz-xiKSYEAAJ"/>
        string ToString(IParameterValueRenderingOptions options);
    }
}
