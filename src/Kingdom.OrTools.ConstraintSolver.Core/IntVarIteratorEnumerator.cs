//using System.Collections;
//using System.Collections.Generic;
//using Google.OrTools.ConstraintSolver;

////TODO: ditto ToList extension
//namespace Football.Constraints
//{
//    public class IntVarIteratorEnumerator : IEnumerator<long>
//    {
//        /// <summary>
//        /// Iterator backing field.
//        /// </summary>
//        private readonly IntVarIterator _iterator;

//        /// <summary>
//        /// Constructor
//        /// </summary>
//        /// <param name="iterator"></param>
//        public IntVarIteratorEnumerator(IntVarIterator iterator)
//        {
//            _iterator = iterator;
//        }

//        public long Current
//        {
//            get { return _iterator.Value(); }
//        }

//        object IEnumerator.Current
//        {
//            get { return Current; }
//        }

//        public bool MoveNext()
//        {
//            _iterator.Next();
//            return true;
//        }

//        public void Reset()
//        {
//            _iterator.Init();
//        }

//        /// <summary>
//        /// Disposes the object.
//        /// </summary>
//        public void Dispose()
//        {
//        }
//    }
//}