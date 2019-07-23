using System;

namespace Kingdom.OrTools.Sat.Parameters
{
    internal static class ClassExtensionMethods
    {
        /// <summary>
        /// Initializes the <paramref name="instance"/> given an optional
        /// <paramref name="callback"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="instance"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        public static T Initialize<T>(this T instance, Action<T> callback = null)
        {
            callback?.Invoke(instance);
            return instance;
        }
    }
}
