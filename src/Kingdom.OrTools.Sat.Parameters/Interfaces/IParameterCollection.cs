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
        string ToString(IParameterValueRenderingOptions options);
    }
}
