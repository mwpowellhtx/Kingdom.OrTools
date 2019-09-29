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
        /// Returns the <see cref="long"/> Values Ranging from <paramref name="min"/>
        /// to <paramref name="max"/> incrementing by <paramref name="step"/>.
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <param name="step"></param>
        /// <returns></returns>
        private static IEnumerable<long> GetValues(long min, long max, long step)
        {
            for (var x = min; x < max + 1; x += step)
            {
                yield return x;
            }
        }

        /// <summary>
        /// Makes a <paramref name="solver"/> variable ranging over the values <paramref name="min"/>,
        /// <paramref name="max"/>, and the step <paramref name="step"/>. Returns the <see cref="IntVar"/>
        /// after it has been made.
        /// </summary>
        /// <param name="solver"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <param name="step"></param>
        /// <returns></returns>
        /// <see cref="GetValues"/>
        public static IntVar MakeIntVar(this Solver solver, long min, long max, long step)
            => solver.MakeIntVar(GetValues(min, max, step).ToArray());

        /// <summary>
        /// Makes a solver named variable ranging over the values <paramref name="min"/>,
        /// <paramref name="max"/>, and the step <paramref name="step"/>. Returns the
        /// <see cref="IntVar"/> after it has been made.
        /// </summary>
        /// <param name="solver"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <param name="step"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static IntVar MakeIntVar(this Solver solver, long min, long max, long step, string name)
            => solver.MakeIntVar(GetValues(min, max, step).ToArray(), name);
    }
}
