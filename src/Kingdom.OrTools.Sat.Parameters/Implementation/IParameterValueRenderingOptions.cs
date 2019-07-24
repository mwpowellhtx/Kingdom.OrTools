using System;
using System.Collections.Generic;

namespace Kingdom.OrTools.Sat.Parameters
{
    /// <summary>
    /// Allows Parameter Rendering Options to be specified, starting with a nominal set of
    /// default options, including <see cref="int"/>, <see cref="long"/>, <see cref="bool"/>,
    /// and <see cref="double"/>. We also include default support for <see cref="Enum"/>.
    /// </summary>
    /// <inheritdoc />
    public interface IParameterValueRenderingOptions : IDictionary<Type, RenderParameterValueCallback>
    {
    }
}
