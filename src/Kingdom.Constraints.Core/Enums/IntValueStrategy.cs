namespace Kingdom.Constraints
{
    using Google.OrTools.ConstraintSolver;

    /// <summary>
    /// This enum describes the strategy used to select the next variable value to set.
    /// </summary>
    /// <see cref="!:http://github.com/google/or-tools/blob/792c1358a57469c9948edc004b07262348544f94/src/constraint_solver/constraint_solver.h" />
    public enum IntValueStrategy
    {
        /// <summary>
        /// The default behavior is <see cref="AssignMinValue"/>.
        /// </summary>
        /// <see cref="Solver.INT_VALUE_DEFAULT"/>
        IntValueDefault,

        /// <summary>
        /// The simple selection is <see cref="AssignMinValue"/>.
        /// </summary>
        /// <see cref="Solver.INT_VALUE_SIMPLE"/>
        IntValueSimple,

        /// <summary>
        /// Selects the min value of the selected variable.
        /// </summary>
        /// <see cref="Solver.ASSIGN_MIN_VALUE"/>
        AssignMinValue,

        /// <summary>
        /// Selects the max value of the selected variable.
        /// </summary>
        /// <see cref="Solver.ASSIGN_MAX_VALUE"/>
        AssignMaxValue,

        /// <summary>
        /// Selects randomly one of the possible values of the selected variable.
        /// </summary>
        /// <see cref="Solver.ASSIGN_RANDOM_VALUE"/>
        AssignRandomValue,

        /// <summary>
        /// Selects the first possible value which is the closest to the center of the domain of
        /// the selected variable. The center is defined as (min+max)/2.
        /// </summary>
        /// <see cref="Solver.ASSIGN_CENTER_VALUE"/>
        AssignCenterValue,

        /// <summary>
        /// Split the domain in two around the center, and choose the lower part first.
        /// </summary>
        /// <see cref="Solver.SPLIT_LOWER_HALF"/>
        SplitLowerHalf,

        /// <summary>
        /// Split the domain in two around the center, and choose the lower part first.
        /// </summary>
        /// <see cref="Solver.SPLIT_UPPER_HALF"/>
        SplitUpperHalf
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
        public static int ToInt(this IntValueStrategy value)
        {
            return (int) value;
        }
    }
}
