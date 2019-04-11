namespace Kingdom.OrTools.Sat
{
    using Google.OrTools.Sat;

    /// <summary>
    /// Derived in order to connect the dots with the Problem Solver.
    /// </summary>
    public abstract class OrProblemSolverSolutionCallback : SolutionCallback
    {
        /// <summary>
        /// Represents a simple Callback Callback.
        /// </summary>
        public delegate void CallbackCallback();

        /// <summary>
        /// Holds a simple <see cref="CallbackCallback"/>.
        /// </summary>
        public event CallbackCallback Callback;

        /// <summary>
        /// Protected <paramref name="callback"/> Constructor.
        /// </summary>
        /// <param name="callback"></param>
        protected OrProblemSolverSolutionCallback(CallbackCallback callback)
        {
            Callback += callback;
        }

        /// <inheritdoc />
        protected OrProblemSolverSolutionCallback()
            : this(() => { })
        {
        }

        /// <summary>
        /// Overridden in order to automatically connect the dots with the Problem Solver.
        /// </summary>
        public override void OnSolutionCallback()
        {
            base.OnSolutionCallback();
            Callback?.Invoke();
        }
    }
}
