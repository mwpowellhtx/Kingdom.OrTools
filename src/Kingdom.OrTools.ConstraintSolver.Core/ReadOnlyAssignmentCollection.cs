using System;
using System.Collections;
using System.Collections.Generic;

namespace Kingdom.OrTools.ConstraintSolver
{
    using Google.OrTools.ConstraintSolver;

    /// <inheritdoc />
    public class ReadOnlyAssignmentCollection : IReadOnlyList<Assignment>
    {
        /// <summary>
        /// Collector backing field.
        /// </summary>
        private readonly SolutionCollector _collector;

        /// <summary>
        /// Internal Constructor
        /// </summary>
        /// <param name="collector"></param>
        protected internal ReadOnlyAssignmentCollection(SolutionCollector collector) => _collector = collector;

        /// <summary>
        /// Gets whether the Collection Has a <see cref="SolutionCollector"/>.
        /// </summary>
        public virtual bool HasCollector => _collector != null;

        /// <inheritdoc />
        public Assignment this[int index]
        {
            get
            {
                //TODO: TBD: or decide on whether to risk 'IOOREX' from API?
                var count = _collector.SolutionCount();
                if (index < 0 || index >= count)
                {
                    var message = $@"Index {index} is out of range: SolutionCount: {count}";
                    throw new IndexOutOfRangeException(message);
                }
                var assignment = _collector.Solution(index);
                return assignment;
            }
        }

        /// <inheritdoc />
        public int Count => _collector.SolutionCount();

        /// <inheritdoc />
        public IEnumerator<Assignment> GetEnumerator() => new SolutionCollectorAssignmentEnumerator(_collector);

        /// <inheritdoc />
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
