using Google.OrTools.ConstraintSolver;

namespace Kingdom.Constraints
{
    //Alias the callback function for internal use.
    using VariableChooserFunc = LongResultCallback1.SwigDelegateLongResultCallback1_0;

    /// <summary>
    /// VariableChooser is a kind of Callback.
    /// </summary>
    /// <see cref="!:http://or-tools.googlecode.com/svn/trunk/documentation/reference_manual/or-tools/src/constraint_solver/classoperations__research_1_1Solver.html#fcf373cf1c769a34b1b89a5ba56fb473"/>
    /// <see cref="!:http://or-tools.googlecode.com/svn/trunk/documentation/user_manual/manual/search_primitives/basic_working_phases.html#callbacks-to-the-rescue"/>
    /// <see cref="!:http://or-tools.googlecode.com/svn/trunk/documentation/user_manual/manual/search_primitives/customized_search_primitives.html#customized-search-primitives"/>
    public class VariableChooser : LongResultCallback1
    {
        /// <summary>
        /// Chooser backing field.
        /// </summary>
        private readonly VariableChooserFunc _chooser;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="chooser"></param>
        public VariableChooser(VariableChooserFunc chooser)
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
    }
}
