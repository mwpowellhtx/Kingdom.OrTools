using System;
using Google.OrTools.LinearSolver;

namespace Kingdom.OrTools.LinearSolver
{
    using OptimizationProblemType = Solver.OptimizationProblemType;
    using static Solver.OptimizationProblemType;

    /// <summary>
    /// Provides a C-Sharp friendly view of the <see cref="OptimizationProblemType"/> enumeration.
    /// </summary>
    public enum LinearOptimizationProblemType
    {
        /// <summary>
        /// COIN-OR linear programming.
        /// </summary>
        /// <see cref="!:http://www.coin-or.org/" />
        ClpLinearProgramming = CLP_LINEAR_PROGRAMMING,

        // ReSharper disable once IdentifierTypo
        /// <summary>
        /// GNU Linear Programming Kit Linear Programming (LP).
        /// </summary>
        /// <see cref="!:http://www.gnu.org/software/glpk/" />
        [Obsolete("No longer used.")]
        GlpkLinearProgramming = 1,

        /// <summary>
        /// Google Linear Optimization or Linear Programming.
        /// </summary>
        /// <see cref="!:http://developers.google.com/optimization/lp/glop" />
        GlopLinearProgramming = GLOP_LINEAR_PROGRAMMING,

        // ReSharper disable once CommentTypo, IdentifierTypo
        /// <summary>
        /// SCIP mixed integer programming.
        /// </summary>
        /// <see cref="!:http://scip.zib.de/" />
        [Obsolete("Obsolete in its current form without third part inclusion and custom build.")]
        ScipMixedIntegerProgramming = 3,

        // ReSharper disable once IdentifierTypo
        /// <summary>
        /// GNU Linear Programming Kit Mixed Integer Programming (MIP).
        /// </summary>
        /// <see cref="!:http://www.gnu.org/software/glpk/" />
        [Obsolete("No longer used.")]
        GlpkMixedIntegerProgramming = 4,

        /// <summary>
        /// COIN-OR Branch and Cut mixed integer programming. 
        /// </summary>
        /// <see cref="!:http://projects.coin-or.org/Cbc" />
        CbcMixedIntegerProgramming = CBC_MIXED_INTEGER_PROGRAMMING
    }
}
