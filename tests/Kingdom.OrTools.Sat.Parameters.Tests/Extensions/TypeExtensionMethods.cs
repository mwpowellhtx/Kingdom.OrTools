using System;
using System.Collections.Generic;
using System.Linq;

namespace Kingdom.OrTools.Sat.Parameters
{
    using Xunit;
    using static Enum;

    internal static class TypeExtensionMethods
    {
        /// <summary>
        /// Returns the Values associated with the <paramref name="enumType"/>.
        /// </summary>
        /// <param name="enumType"></param>
        /// <returns></returns>
        private static IEnumerable<object> PrivateGetEnumValues(this Type enumType)
        {
            foreach (var x in GetValues(enumType.AssertNotNull().AssertTrue(x => x.IsEnum)).AssertNotNull())
            {
                yield return x.AssertNotNull();
            }
        }

        /// <summary>
        /// Returns the <see cref="IEnumerable{T}"/> of <typeparamref name="TUnderlying"/>
        /// values. Import inherent or implied verification is performed internally.
        /// </summary>
        /// <typeparam name="TUnderlying"></typeparam>
        /// <param name="enumType"></param>
        /// <returns></returns>
        public static IEnumerable<TUnderlying> GetEnumValues<TUnderlying>(this Type enumType)
            where TUnderlying : struct, IComparable
        {
            var underlyingType = typeof(TUnderlying);

            enumType.GetEnumUnderlyingType().AssertNotNull().AssertEqual(underlyingType);

            TUnderlying AsUnderlying(object value) => (TUnderlying) Convert.ChangeType(value, underlyingType);

            foreach (var x in enumType.PrivateGetEnumValues().AssertNotNull().AssertNotEmpty().Select(AsUnderlying))
            {
                yield return x;
            }
        }
    }
}
