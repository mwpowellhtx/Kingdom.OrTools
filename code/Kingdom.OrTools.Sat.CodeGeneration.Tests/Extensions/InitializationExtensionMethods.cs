using System;

namespace Kingdom.OrTools.Sat.CodeGeneration
{
    internal static class InitializationExtensionMethods
    {
        public static T Initialize<T>(this T instance, Action<T> initialize = null)
        {
            initialize?.Invoke(instance);
            return instance;
        }
    }
}
