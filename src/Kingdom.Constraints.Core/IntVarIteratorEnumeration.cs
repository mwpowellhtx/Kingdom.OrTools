//using System.Collections;
//using System.Collections.Generic;
//using Google.OrTools.ConstraintSolver;

////TODO: the ToList extension is sufficient I think...
//namespace Football.Constraints
//{
//    public class IntVarIteratorEnumeration : IEnumerable<long>
//    {
//        /// <summary>
//        /// Iterator backing field.
//        /// </summary>
//        private readonly IntVarIterator _iterator;

//        /// <summary>
//        /// Constructor
//        /// </summary>
//        /// <param name="iterator"></param>
//        public IntVarIteratorEnumeration(IntVarIterator iterator)
//        {
//            _iterator = iterator;
//        }

//        public IEnumerator<long> GetEnumerator()
//        {
//            return new IntVarIteratorEnumerator(_iterator);
//        }

//        IEnumerator IEnumerable.GetEnumerator()
//        {
//            return GetEnumerator();
//        }
//    }
//}
