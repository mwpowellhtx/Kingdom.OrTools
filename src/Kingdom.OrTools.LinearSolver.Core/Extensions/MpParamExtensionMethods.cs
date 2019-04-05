using Google.OrTools.LinearSolver;

namespace Kingdom.OrTools.LinearSolver
{
    using DoubleParam = MPSolverParameters.DoubleParam;
    using IntegerParam = MPSolverParameters.IntegerParam;
    // ReSharper disable once IdentifierTypo
    using IncrementalityValues = MPSolverParameters.IncrementalityValues;
    using LpAlgorithmValues = MPSolverParameters.LpAlgorithmValues;
    // ReSharper disable once IdentifierTypo
    using PresolveValues = MPSolverParameters.PresolveValues;
    using ScalingValues = MPSolverParameters.ScalingValues;

    internal static class MpParamExtensionMethods
    {
        public static DoubleParam ForSolver(this MpParamDoubleParam value)
            => value.ForSolver<MpParamDoubleParam, int, DoubleParam>();

        public static IntegerParam ForSolver(this MpParamIntegerParam value)
            => value.ForSolver<MpParamIntegerParam, int, IntegerParam>();

        public static IncrementalityValues ForSolver(this MpParamIncrementalityValues value)
            => value.ForSolver<MpParamIncrementalityValues, int, IncrementalityValues>();

        public static LpAlgorithmValues ForSolver(this LinearProgrammingAlgorithmValues value)
            => value.ForSolver<LinearProgrammingAlgorithmValues, int, LpAlgorithmValues>();

        public static PresolveValues ForSolver(this MpParamPresolveValues value)
            => value.ForSolver<MpParamPresolveValues, int, PresolveValues>();

        public static ScalingValues ForSolver(this MpParamScalingValues value)
            => value.ForSolver<MpParamScalingValues, int, ScalingValues>();
    }
}
