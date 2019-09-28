namespace Kingdom.OrTools.Sat.Parameters
{
    /// <summary>
    /// Provides an Anchor class for internal use.
    /// We use this type to inform the unit tests as to the Parameters Assembly.
    /// </summary>
    /// <remarks>Predicate updates herewith based on whether there were changes from the most
    /// recent `sat_parameters.proto´ baseline. Such deltas may impact the code generation out
    /// workings, at best. Worst case, we may need to revisit the CG tooling itself, or even
    /// deeper into the actual Protocol Buffer ANTLR bits.</remarks>
    /// <see cref="!:https://github.com/google/or-tools/blob/stable/ortools/sat/sat_parameters.proto"/>
    internal static class Anchor
    {
    }
}
