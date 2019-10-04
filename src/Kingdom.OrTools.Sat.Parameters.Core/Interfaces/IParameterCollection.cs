using System.Collections.Generic;

namespace Kingdom.OrTools.Sat.Parameters
{
    using IParameterCollectionType = ICollection<IParameter>;

    /// <inheritdoc />
    public interface IParameterCollection : IParameterCollectionType
    {
        /// <summary>
        /// Renders the Collection given a nominal set of <paramref name="options"/>.
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        /// <see cref="!:https://groups.google.com/forum/#!searchin/or-tools-discuss/sat$20parameter$20string%7Csort:date"/>
        /// <see cref="!:https://groups.google.com/forum/#!searchin/or-tools-discuss/sat$20parameter$20string%7Csort:date/or-tools-discuss/X4Y_ZpKIUp8/kz-xiKSYEAAJ"/>
        string ToString(IParameterValueRenderingOptions options);
    }
}
