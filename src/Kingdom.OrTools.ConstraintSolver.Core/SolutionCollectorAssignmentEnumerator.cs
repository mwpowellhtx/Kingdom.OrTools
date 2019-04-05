using System.Collections;
using System.Collections.Generic;

namespace Kingdom.OrTools.ConstraintSolver
{
    using Google.OrTools.ConstraintSolver;

    /// <inheritdoc />
    internal class SolutionCollectorAssignmentEnumerator : IEnumerator<Assignment>
    {
        private int _currentIndex;

        /// <summary>
        /// Collector backing field.
        /// </summary>
        private readonly SolutionCollector _collector;

        /// <summary>
        /// Internal Constructor.
        /// </summary>
        /// <param name="collector"></param>
        internal SolutionCollectorAssignmentEnumerator(SolutionCollector collector)
        {
            _currentIndex = 0;
            _collector = collector;
        }

        /// <inheritdoc />
        public Assignment Current => _collector.Solution(_currentIndex);

        /// <inheritdoc />
        object IEnumerator.Current => Current;

        /// <inheritdoc />
        public bool MoveNext()
        {
            if (_currentIndex >= _collector.SolutionCount())
            {
                return false;
            }

            _currentIndex++;
            return true;
        }

        /// <inheritdoc />
        public void Reset() => _currentIndex = 0;

        /// <inheritdoc />
        public void Dispose()
        {
            //Does not assume any ownership of the members.
        }
    }
}
