using System;

namespace Kingdom.Constraints
{
    using Google.OrTools.ConstraintSolver;

    /// <summary>
    /// This enum describes the strategy used to select the next branching variable at each node
    /// during the search.
    /// </summary>
    /// <remarks>The enum-ness about this is lost when the code passes through SWIG.</remarks>
    /// <see cref="!:http://github.com/google/or-tools/blob/792c1358a57469c9948edc004b07262348544f94/src/constraint_solver/constraint_solver.h" />
    public enum IntVarStrategy
    {
        /// <summary>
        /// The default behavior is CHOOSE_FIRST_UNBOUND.
        /// </summary>
        /// <see cref="Solver.INT_VAR_DEFAULT"/>
        IntVarDefault,

        /// <summary>
        /// The simple selection is CHOOSE_FIRST_UNBOUND.
        /// </summary>
        /// <see cref="Solver.INT_VAR_SIMPLE"/>
        IntVarSimple,

        /// <summary>
        /// Select the first unbound variable. Variables are considered in the order of the vector
        /// of IntVars used to create the selector.
        /// </summary>
        /// <see cref="Solver.CHOOSE_FIRST_UNBOUND"/>
        ChooseFirstUnbound,

        /// <summary>
        /// Randomly select one of the remaining unbound variables.
        /// </summary>
        /// <see cref="Solver.CHOOSE_RANDOM"/>
        ChooseRandom,

        /// <summary>
        /// Among unbound variables, select the variable with the smallest size, i.e. the smallest
        /// number of possible values. In case of tie, the selected variables is the one with the
        /// lowest min value. In case of tie, the first one is selected, first being defined by
        /// the order in the vector of IntVars used to create the selector.
        /// </summary>
        /// <see cref="Solver.CHOOSE_MIN_SIZE_LOWEST_MIN"/>
        ChooseMinSizeLowestMin,

        /// <summary>
        /// Among unbound variables, select the variable with the smallest size, i.e. the smallest
        /// number of possible values. In case of tie, the selected variables is the one with the
        /// highest min value. In case of tie, the first one is selected, first being defined by
        /// the order in the vector of IntVars used to create the selector.
        /// </summary>
        /// <see cref="Solver.CHOOSE_MIN_SIZE_HIGHEST_MIN"/>
        ChooseMinSizeHighestMin,

        /// <summary>
        /// Among unbound variables, select the variable with the smallest size, i.e. the smallest
        /// number of possible values. In case of tie, the selected variables is the one with the
        /// lowest max value. In case of tie, the first one is selected, first being defined by
        /// the order in the vector of IntVars used to create the selector.
        /// </summary>
        /// <see cref="Solver.CHOOSE_MIN_SIZE_LOWEST_MAX"/>
        ChooseMinSizeLowestMax,

        /// <summary>
        /// Among unbound variables, select the variable with the smallest size, i.e. the smallest
        /// number of possible values. In case of tie, the selected variables is the one with the
        /// highest max value. In case of tie, the first one is selected, first being defined by
        /// the order in the vector of IntVars used to create the selector.
        /// </summary>
        /// <see cref="Solver.CHOOSE_MIN_SIZE_HIGHEST_MAX"/>
        ChooseMinSizeHighestMax,

        /// <summary>
        /// Among unbound variables, select the variable with the smallest minimal value. In case
        /// of tie, the first one is selected, first being defined by the order in the vector of
        /// IntVars used to create the selector.
        /// </summary>
        /// <see cref="Solver.CHOOSE_LOWEST_MIN"/>
        ChooseLowestMin,

        /// <summary>
        /// Among unbound variables, select the variable with the highest maximal value. In case
        /// of tie, the first one is selected, first being defined by the order in the vector of
        /// IntVars used to create the selector.
        /// </summary>
        /// <see cref="Solver.CHOOSE_HIGHEST_MAX"/>
        ChooseHighestMax,

        /// <summary>
        /// Among unbound variables, select the variable with the smallest size. In case of tie,
        /// the first one is selected, first being defined by the order in the vector of IntVars
        /// used to create the selector.
        /// </summary>
        /// <see cref="Solver.CHOOSE_MIN_SIZE"/>
        ChooseMinSize,

        /// <summary>
        /// Among unbound variables, select the variable with the highest size. In case of tie,
        /// the first one is selected, first being defined by the order in the vector of IntVars
        /// used to create the selector.
        /// </summary>
        /// <see cref="Solver.CHOOSE_MAX_SIZE"/>
        ChooseMaxSize,

        /// <summary>
        /// Among unbound variables, select the variable with the largest gap between the first
        /// and the second values of the domain.
        /// </summary>
        /// <see cref="Solver.CHOOSE_MAX_REGRET_ON_MIN"/>
        ChooseMaxRegretOrMin,

        /// <summary>
        /// Selects the next unbound variable on a path, the path being defined by the variables:
        /// var[i] corresponds to the index of the next of i.
        /// </summary>
        /// <see cref="Solver.CHOOSE_PATH"/>
        ChoosePath,

        /// <summary>
        /// 
        /// </summary>
        [Obsolete("TODO (user)")]
        ChooseHighestMin,

        /// <summary>
        /// 
        /// </summary>
        [Obsolete("TODO (user)")]
        ChooseLowestMax,
    }

    /// <summary>
    /// Provides some helpful extensions adapting the enumerated values back to the
    /// <see cref="Solver"/>.
    /// </summary>
    public static partial class EnumExtensionMethods
    {
        /// <summary>
        /// Returns the <see cref="System.Int32"/> value corresponding to the
        /// <paramref name="value"/>.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static int ToInt(this IntVarStrategy value)
        {
            // ReSharper disable once SwitchStatementMissingSomeCases
            switch (value)
            {
#pragma warning disable 618
                case IntVarStrategy.ChooseHighestMin:
                case IntVarStrategy.ChooseLowestMax:
#pragma warning restore 618
                    throw new ArgumentException(string.Format("{0} not currently supported by or-tools", value), "value");
            }

            return (int) value;
        }
    }
}
