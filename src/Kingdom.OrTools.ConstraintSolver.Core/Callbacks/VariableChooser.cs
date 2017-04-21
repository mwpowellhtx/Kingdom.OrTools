namespace Kingdom.OrTools.ConstraintSolver
{
    using Google.OrTools.ConstraintSolver;

    /// <summary>
    /// Provided an <paramref name="index"/> evaluates an appropriate qualitative result.
    /// The lowest numbers win.
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    /// <remarks>This became necessary because aliasing the OrTools callback is insufficient. We must
    /// have a first class symbol in order to minimize the risk to integrating applications.</remarks>
    public delegate long VariableChooserDelegate(long index);

    /// <summary>
    /// In C++ parlance, IndexEvaluator2 is defined as &lt;em&gt;typedef
    /// std::function&lt;int64(int64, int64)&gt;&lt;em&gt;, however, this is lost during the SWIG
    /// process, and appears only as <see cref="LongToLong"/>, which is not especially
    /// descriptive. <see cref="VariableChooser"/> provides an easier to comprehend entry point
    /// when deciding how to approach the issue from C-Sharp.
    /// </summary>
    /// <see cref="!:http://or-tools.googlecode.com/svn/trunk/documentation/reference_manual/or-tools/src/constraint_solver/classoperations__research_1_1Solver.html#fcf373cf1c769a34b1b89a5ba56fb473" />
    /// <see cref="!:http://or-tools.googlecode.com/svn/trunk/documentation/user_manual/manual/search_primitives/basic_working_phases.html#callbacks-to-the-rescue" />
    /// <see cref="!:http://or-tools.googlecode.com/svn/trunk/documentation/user_manual/manual/search_primitives/customized_search_primitives.html#customized-search-primitives" />
    /// <see cref="!:http://or-tools.blogspot.com/2015/08/getting-rid-of-callbacks.html" />
    public class VariableChooser : LongToLong
    {
        /// <summary>
        /// Chooser backing field.
        /// </summary>
        private readonly VariableChooserDelegate _chooser;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="chooser"></param>
        public VariableChooser(VariableChooserDelegate chooser)
        {
            //TODO: TBD: do something with swigCMemOwn (?) default is what?
            _chooser = chooser;
        }

        /// <summary>
        /// Evaluates the variable <paramref name="index"/>.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public override long Run(long index)
        {
            return _chooser(index);
        }

        /// <summary>
        /// Implicitly converts the chooser to the callback.
        /// </summary>
        /// <param name="chooser"></param>
        /// <returns></returns>
        public static implicit operator SwigDelegateLongToLong_0(VariableChooser chooser)
        {
            return chooser.Run;
        }
    }
}
