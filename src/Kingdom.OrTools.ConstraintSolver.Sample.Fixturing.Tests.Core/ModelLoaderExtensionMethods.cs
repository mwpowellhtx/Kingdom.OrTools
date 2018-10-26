// ReSharper disable once IdentifierTypo
namespace Kingdom.Constraints.Sample.Fixturing.Tests
{
    using Google.OrTools.ConstraintSolver;

    internal static class ModelLoaderExtensionMethods
    {
        // TODO: TBD: pretty sure this one was an API before...
        /// <summary>
        /// 
        /// </summary>
        /// <param name="loader"></param>
        /// <param name="expressionName"></param>
        /// <returns></returns>
        /// <see cref="!:http://github.com/google/or-tools/issues/905">What happened to .NET ConstraintSolver CpModelLoader.IntegerExpressionByName</see>
        internal static IntExpr IntegerExpressionByName(this CpModelLoader loader, string expressionName)
        {
            var maxIntegerExpressions = loader.NumIntegerExpressions();
            for (var i = 0; i < maxIntegerExpressions; i++)
            {
                var expression = loader.IntegerExpression(i);
                if (expression.Name() == expressionName)
                {
                    return expression;
                }

                if (expression.Var().Name() == expressionName)
                {
                    return expression;
                }
            }

            return null;
        }
    }
}
