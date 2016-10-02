using System;
using System.Collections;
using System.Collections.Generic;

namespace Kingdom.Constraints
{
    using Google.OrTools.ConstraintSolver;

    internal class ReadOnlyAssignmentCollection : IReadOnlyList<Assignment>
    {
        /// <summary>
        /// Collector backing field.
        /// </summary>
        private readonly SolutionCollector _collector;

        /// <summary>
        /// Internal Constructor
        /// </summary>
        /// <param name="collector"></param>
        internal ReadOnlyAssignmentCollection(SolutionCollector collector)
        {
            _collector = collector;
        }

        /// <summary>
        /// Gets whether the Collection Has a <see cref="SolutionCollector"/>.
        /// </summary>
        public virtual bool HasCollector
        {
            get { return _collector != null; }
        }

        /// <summary>
        /// Read-Only Indexer
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        /// <exception cref="IndexOutOfRangeException"></exception>
        public Assignment this[int index]
        {
            get
            {
                //TODO: TBD: or decide on whether to risk 'IOOREX' from API?
                var count = _collector.SolutionCount();
                if (index < 0 || index >= count)
                {
                    var message = string.Format(@"Index {0} is out of range: SolutionCount: {1}",
                        index, count);
                    throw new IndexOutOfRangeException(message);
                }
                var assignment = _collector.Solution(index);
                return assignment;
            }
        }

        /// <summary>
        /// Gets the <see cref="SolutionCollector.SolutionCount()"/>.
        /// </summary>
        public int Count
        {
            get { return _collector.SolutionCount(); }
        }

        /// <summary>
        /// Gets the <see cref="IEnumerator{Assignment}"/> corresponding to the
        /// <see cref="_collector"/>.
        /// </summary>
        /// <returns></returns>
        /// <see cref="SolutionCollectorAssignmentEnumerator"/>
        public IEnumerator<Assignment> GetEnumerator()
        {
            return new SolutionCollectorAssignmentEnumerator(_collector);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
