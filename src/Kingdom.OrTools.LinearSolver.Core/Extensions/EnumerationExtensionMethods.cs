using System;

namespace Kingdom.OrTools.LinearSolver
{
    internal static class EnumerationExtensionMethods
    {
        private static void VerifyIsEnum(this Type type)
        {
            const string typeParamName = nameof(type);

            if (type is null)
            {
                throw new ArgumentNullException(typeParamName);
            }

            if (type.IsEnum)
            {
                return;
            }

            throw new ArgumentException($"Expected type '{type.FullName}' to be an Enum.", typeParamName);
        }

        public static TResult ForSolver<T, TIntermediate, TResult>(this T value)
            where T : struct
            where TResult : struct
        {
            typeof(T).VerifyIsEnum();
            var resultType = typeof(TResult);
            resultType.VerifyIsEnum();
            var x = (TIntermediate) Convert.ChangeType(value, typeof(TIntermediate));
            return (TResult) Convert.ChangeType(x, resultType);
        }
    }
}
