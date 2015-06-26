using System.Collections;
using System.Collections.Generic;
using Google.OrTools.ConstraintSolver;

namespace Kingdom.Constraints
{
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

        /// <summary>
        /// Gets the Current <see cref="Assignment"/>.
        /// </summary>
        public Assignment Current
        {
            get { return _collector.Solution(_currentIndex); }
        }

        /// <summary>
        /// Gets the Current assignment.
        /// </summary>
        object IEnumerator.Current
        {
            get { return Current; }
        }

        /// <summary>
        /// Moves to the next <see cref="Assignment"/> in sequence.
        /// </summary>
        /// <returns></returns>
        public bool MoveNext()
        {
            if (_currentIndex >= _collector.SolutionCount())
                return false;
            _currentIndex++;
            return true;
        }

        /// <summary>
        /// Resets the enumerator to the beginning.
        /// </summary>
        public void Reset()
        {
            _currentIndex = 0;
        }

        /// <summary>
        /// Disposes the object.
        /// </summary>
        public void Dispose()
        {
            //Does not assume any ownership of the members.
        }
    }
}
