using System;
using System.Collections.Generic;

namespace Kingdom.OrTools.Sat.CodeGeneration
{
    using Xunit;

    internal static class AssertionExtensionMethods
    {
        public static bool AssertTrue(this bool x)
        {
            Assert.True(x);
            // ReSharper disable once ConditionIsAlwaysTrueOrFalse
            return x;
        }

        public static bool AssertFalse(this bool x)
        {
            Assert.False(x);
            // ReSharper disable once ConditionIsAlwaysTrueOrFalse
            return x;
        }

        public static T AssertTrue<T>(this T obj, Func<T, bool> callback)
        {
            Assert.True(callback(obj));
            return obj;
        }

        public static T AssertFalse<T>(this T obj, Func<T, bool> callback)
        {
            Assert.False(callback(obj));
            return obj;
        }

        public static IEnumerable<T> AssertAll<T>(this IEnumerable<T> values, Action<T> action)
        {
            // ReSharper disable once PossibleMultipleEnumeration
            Assert.All(values, action);
            // ReSharper disable once PossibleMultipleEnumeration
            return values;
        }

        /// <summary>
        /// Asserts that the <paramref name="obj"/> is <see cref="Assert.NotNull"/>
        /// Optionally performs an additional <paramref name="verify"/> step.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="verify"></param>
        /// <returns></returns>
        public static T AssertNotNull<T>(this T obj, Action<T> verify = null)
        {
            Assert.NotNull(obj);
            verify?.Invoke(obj);
            return obj;
        }

        public static T AssertIsAssignableFrom<T>(this object obj) => Assert.IsAssignableFrom<T>(obj.AssertNotNull());
    }
}
