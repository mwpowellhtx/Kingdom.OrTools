using System;
using System.Collections.Generic;
using Google.OrTools.ConstraintSolver;

namespace Kingdom.OrTools.ConstraintSolver
{
    using static Solver;
    using static IntValueStrategy;

    /// <summary>
    /// This enum describes the strategy used to select the next variable value to set.
    /// </summary>
    /// <see cref="!:http://github.com/google/or-tools/blob/792c1358a57469c9948edc004b07262348544f94/src/constraint_solver/constraint_solver.h" />
    public enum IntValueStrategy
    {
        /// <summary>
        /// The default behavior is <see cref="AssignMinValue"/>.
        /// </summary>
        /// <see cref="INT_VALUE_DEFAULT"/>
        IntValueDefault,

        /// <summary>
        /// The simple selection is <see cref="AssignMinValue"/>.
        /// </summary>
        /// <see cref="INT_VALUE_SIMPLE"/>
        IntValueSimple,

        /// <summary>
        /// Selects the min value of the selected variable.
        /// </summary>
        /// <see cref="ASSIGN_MIN_VALUE"/>
        AssignMinValue,

        /// <summary>
        /// Selects the max value of the selected variable.
        /// </summary>
        /// <see cref="ASSIGN_MAX_VALUE"/>
        AssignMaxValue,

        /// <summary>
        /// Selects randomly one of the possible values of the selected variable.
        /// </summary>
        /// <see cref="ASSIGN_RANDOM_VALUE"/>
        AssignRandomValue,

        /// <summary>
        /// Selects the first possible value which is the closest to the center of the domain of
        /// the selected variable. The center is defined as (min+max)/2.
        /// </summary>
        /// <see cref="ASSIGN_CENTER_VALUE"/>
        AssignCenterValue,

        /// <summary>
        /// Split the domain in two around the center, and choose the lower part first.
        /// </summary>
        /// <see cref="SPLIT_LOWER_HALF"/>
        SplitLowerHalf,

        /// <summary>
        /// Split the domain in two around the center, and choose the lower part first.
        /// </summary>
        /// <see cref="SPLIT_UPPER_HALF"/>
        SplitUpperHalf
    }

    /// <summary>
    /// Provides some helpful extensions adapting the enumerated values back to the
    /// <see cref="Solver"/>.
    /// </summary>
    public static partial class EnumExtensionMethods
    {
        private static readonly Lazy<IDictionary<IntValueStrategy, int>> LazyIntValueStrategyValues
            = new Lazy<IDictionary<IntValueStrategy, int>>(() =>
                new Dictionary<IntValueStrategy, int>
                {
                    {IntValueDefault, INT_VALUE_DEFAULT},
                    {IntValueSimple, INT_VALUE_SIMPLE},
                    {AssignMinValue, ASSIGN_MIN_VALUE},
                    {AssignMaxValue, ASSIGN_MAX_VALUE},
                    {AssignRandomValue, ASSIGN_RANDOM_VALUE},
                    {AssignCenterValue, ASSIGN_CENTER_VALUE},
                    {SplitLowerHalf, SPLIT_LOWER_HALF},
                    {SplitUpperHalf, SPLIT_UPPER_HALF},
                });

        /// <summary>
        /// Returns the <see cref="Int32"/> value corresponding to the <paramref name="value"/>.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static int ToInt(this IntValueStrategy value)
        {
            try
            {
                return LazyIntValueStrategyValues.Value[value];
            }
            catch (Exception ex)
            {
                throw new ArgumentException($"{value} not currently supported by Google Operational Research Tools.", nameof(value), ex);
            }
        }
    }
}
