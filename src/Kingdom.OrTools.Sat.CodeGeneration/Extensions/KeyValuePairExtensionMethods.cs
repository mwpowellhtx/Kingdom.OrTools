using System;
using System.Collections.Generic;

namespace Kingdom.OrTools.Sat
{
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    internal static class KeyValuePairExtensionMethods
    {
        // ReSharper disable once UseDeconstructionOnParameter kind of redundant this is why we are here in the first place
        /// <summary>
        /// Deconstructs the <paramref name="pair"/> given <paramref name="key"/> and
        /// <paramref name="value"/>.
        /// </summary>
        /// <param name="pair"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public static void Deconstruct(this KeyValuePair<Guid, CompilationUnitSyntax> pair, out Guid key, out CompilationUnitSyntax value)
        {
            key = pair.Key;
            value = pair.Value;
        }
    }
}
