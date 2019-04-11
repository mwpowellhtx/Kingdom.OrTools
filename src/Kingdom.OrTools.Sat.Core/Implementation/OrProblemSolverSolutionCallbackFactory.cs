namespace Kingdom.OrTools.Sat
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TCallback"></typeparam>
    /// <returns></returns>
    public delegate TCallback OrProblemSolverSolutionCallbackFactory<out TCallback>()
        where TCallback : OrProblemSolverSolutionCallback;
}
