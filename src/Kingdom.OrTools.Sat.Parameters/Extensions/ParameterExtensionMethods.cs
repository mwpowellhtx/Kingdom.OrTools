using System;
using System.Collections.Generic;
using System.Linq;

namespace Kingdom.OrTools.Sat.Parameters
{
    using static Characters;
    using static String;

    /// <summary>
    /// Provides a useful set of <see cref="IParameter"/> extension methods.
    /// </summary>
    internal static class ParameterExtensionMethods
    {
        /// <summary>
        /// Renders the <paramref name="parameters"/> assuming a nominal set of
        /// <paramref name="options"/>.
        /// </summary>
        /// <param name="parameters"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static string RenderParameters(this IEnumerable<IParameter> parameters, IParameterValueRenderingOptions options = null)
            => Join($"{SemiColon}", parameters.Select(x => x.ToString(options)));
    }
}
