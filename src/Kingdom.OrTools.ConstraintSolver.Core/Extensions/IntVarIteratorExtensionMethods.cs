using System.Collections.Generic;
using System.Linq;

namespace Kingdom.OrTools.ConstraintSolver
{
    using Google.OrTools.ConstraintSolver;

    /// <summary>
    /// Provides <see cref="IntVarIterator"/> extension methods. The iterators provided by the
    /// Google folks are nice, but aren't all that compliant with .NET isms. So this is provides
    /// a way of bridging that gap.
    /// </summary>
    public static class IntVarIteratorExtensionMethods
    {
        /// <summary>
        /// Returns an <see cref="IEnumerable{Int64}"/> representation of the <paramref name="iterator"/>.
        /// This is a one way trip purely for host language convenience.
        /// </summary>
        /// <param name="iterator"></param>
        /// <returns></returns>
        private static IEnumerable<long> ToEnumerable(this IntVarIterator iterator)
        {
            iterator.Init();

            while (iterator.Ok())
            {
                yield return iterator.Value();
                iterator.Next();
            }
        }

        /// <summary>
        /// Returns an <see cref="IList{Int64}"/> representation of the <paramref name="iterator"/>.
        /// This is a one way trip purely for host language convenience.
        /// </summary>
        /// <param name="iterator"></param>
        /// <returns></returns>
        /// <see cref="ToEnumerable(IntVarIterator)"/>
        public static IList<long> ToList(this IntVarIterator iterator)
        {
            return iterator.ToEnumerable().ToList();
        }

        /// <summary>
        /// Makes a <paramref name="solver"/> variable ranging over the values <paramref name="vmin"/>,
        /// <paramref name="vmax"/>, and the step <paramref name="vstep"/>. Returns the <see cref="IntVar"/>
        /// after it has been made.
        /// </summary>
        /// <param name="solver"></param>
        /// <param name="vmin"></param>
        /// <param name="vmax"></param>
        /// <param name="vstep"></param>
        /// <returns></returns>
        public static IntVar MakeIntVar(this Solver solver, long vmin, long vmax, long vstep)
        {
            var values = new CpInt64Vector();

            for (var v = vmin; v <= vmax; v += vstep)
                values.Add(v);

            return solver.MakeIntVar(values);
        }

        /// <summary>
        /// Makes a solver named variable ranging over the values <paramref name="vmin"/>,
        /// <paramref name="vmax"/>, and the step <paramref name="vstep"/>. Returns the
        /// <see cref="IntVar"/> after it has been made.
        /// </summary>
        /// <param name="solver"></param>
        /// <param name="vmin"></param>
        /// <param name="vmax"></param>
        /// <param name="vstep"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static IntVar MakeIntVar(this Solver solver, long vmin, long vmax, long vstep, string name)
        {
            var values = new CpInt64Vector();

            for (var v = vmin; v <= vmax; v += vstep)
                values.Add(v);

            return solver.MakeIntVar(values, name);
        }
    }
}
