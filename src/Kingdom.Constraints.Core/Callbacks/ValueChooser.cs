using Google.OrTools.ConstraintSolver;

namespace Kingdom.Constraints
{
    /// <summary>
    /// Provided an <paramref name="index"/> evaluates an appropriate qualitative result.
    /// The lowest numbers win.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    /// <remarks>This became necessary because aliasing the OrTools callback is insufficient. We must
    /// have a first class symbol in order to minimize the risk to integrating applications.</remarks>
    public delegate long ValueChooserDelegate(long index, long value);

    /// <summary>
    /// ValueChooser is a kind of callback.
    /// </summary>
    /// <see cref="!:http://or-tools.googlecode.com/svn/trunk/documentation/reference_manual/or-tools/src/constraint_solver/classoperations__research_1_1Solver.html#487b521ee809f0f4ee397b9f6ea06f59" />
    /// <see cref="!:http://or-tools.googlecode.com/svn/trunk/documentation/user_manual/manual/search_primitives/basic_working_phases.html#callbacks-to-the-rescue" />
    /// <see cref="!:http://or-tools.googlecode.com/svn/trunk/documentation/user_manual/manual/search_primitives/customized_search_primitives.html#customized-search-primitives" />
    /// <see cref="!:http://or-tools.blogspot.com/2015/08/getting-rid-of-callbacks.html" />
    public class ValueChooser : LongLongToLong
    {
        /// <summary>
        /// Chooser backing field.
        /// </summary>
        private readonly ValueChooserDelegate _chooser;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="chooser"></param>
        public ValueChooser(ValueChooserDelegate chooser)
        {
            //TODO: TBD: do something with swigCMemOwn (?) default is what?
            _chooser = chooser;
        }

        /// <summary>
        /// Evaluates the <paramref name="value"/> given the <paramref name="index"/>.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public override long Run(long index, long value)
        {
            return _chooser(index, value);
        }

        /// <summary>
        /// Implicitly converts the chooser to the callback.
        /// </summary>
        /// <param name="chooser"></param>
        /// <returns></returns>
        public static implicit operator SwigDelegateLongLongToLong_0(ValueChooser chooser)
        {
            return chooser.Run;
        }
    }
}
