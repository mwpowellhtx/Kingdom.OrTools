using System;
using System.Collections.Generic;
using System.Linq;

namespace Kingdom.OrTools.Sudoku
{
    using static String;

    internal static class Domain
    {
        // ReSharper disable once UnusedMember.Local
        private static IEnumerable<T> GetRange<T>(params T[] values)
        {
            // ReSharper disable once LoopCanBeConvertedToQuery
            foreach (var x in values)
            {
                yield return x;
            }
        }

        private static IEnumerable<string> _stringProblems;

        /// <summary>
        /// 
        /// </summary>
        /// <see cref="!:http://www.sudoku.com/"/>
        public static IEnumerable<string> StringProblems
        {
            get
            {
                IEnumerable<string> GetAll()
                {
                    // Little bit different, more concise representation of the problem.
                    yield return Join(Empty, GetRange(
                        "905060340", "028000591", "000009600"
                        , "097630104", "000974000", "406028950"
                        , "009500000", "562000480", "083090205"
                    ));
                }

                return _stringProblems ?? (_stringProblems = GetAll().ToArray());
            }
        }

        private static IEnumerable<int[]> _integerArrayProblems;

        public static IEnumerable<int[]> IntegerArrayProblems
        {
            get
            {
                IEnumerable<int[]> GetAll()
                {
                    /* We can use GetRange,
                     * but I like this better from a style formatting perspective. */

                    // Published: Cape Gazette, Tue, Jun 23 - Thu, Jun 25, 2015, p. 21
                    yield return new[]
                    {
                        //====================================
                        0, 4, 0, /*|*/ 0, 7, 9, /*|*/ 0, 6, 0,
                        5, 0, 0, /*|*/ 0, 0, 0, /*|*/ 0, 0, 0,
                        0, 9, 8, /*|*/ 5, 0, 0, /*|*/ 2, 0, 0,
                        //====================================
                        0, 0, 0, /*|*/ 1, 0, 0, /*|*/ 9, 3, 0,
                        1, 0, 0, /*|*/ 4, 0, 0, /*|*/ 6, 0, 8,
                        0, 0, 9, /*|*/ 0, 0, 0, /*|*/ 0, 0, 1,
                        //====================================
                        0, 0, 0, /*|*/ 0, 2, 8, /*|*/ 0, 0, 3,
                        8, 0, 0, /*|*/ 0, 0, 0, /*|*/ 0, 7, 0,
                        4, 6, 0, /*|*/ 0, 0, 0, /*|*/ 0, 0, 0
                    };

                    // Published: Cape Gazette, Fri, Feb 13 - Mon, Feb 16, 2015, p. 101
                    yield return new[]
                    {
                        //====================================
                        0, 3, 0, /*|*/ 0, 0, 0, /*|*/ 5, 4, 0,
                        0, 0, 0, /*|*/ 0, 5, 0, /*|*/ 8, 0, 0,
                        7, 0, 0, /*|*/ 0, 3, 0, /*|*/ 1, 0, 0,
                        //====================================
                        0, 5, 9, /*|*/ 0, 0, 2, /*|*/ 4, 0, 0,
                        8, 0, 0, /*|*/ 0, 0, 0, /*|*/ 0, 9, 7,
                        0, 0, 0, /*|*/ 0, 9, 0, /*|*/ 0, 0, 0,
                        //====================================
                        0, 0, 4, /*|*/ 6, 0, 0, /*|*/ 2, 0, 0,
                        0, 8, 7, /*|*/ 0, 0, 0, /*|*/ 0, 0, 0,
                        5, 0, 6, /*|*/ 1, 0, 0, /*|*/ 0, 0, 0,
                    };

                    // Published: Cape Gazette, Fri, Feb 13 - Mon, Feb 16, 2015, p. 101
                    yield return new[]
                    {
                        //====================================
                        0, 0, 0, /*|*/ 0, 0, 5, /*|*/ 1, 9, 0,
                        0, 0, 9, /*|*/ 2, 6, 0, /*|*/ 0, 0, 7,
                        5, 6, 0, /*|*/ 0, 4, 9, /*|*/ 0, 8, 0,

                        //====================================
                        6, 1, 0, /*|*/ 0, 0, 8, /*|*/ 0, 0, 4,
                        0, 0, 0, /*|*/ 0, 0, 0, /*|*/ 0, 0, 0,
                        4, 0, 0, /*|*/ 9, 0, 0, /*|*/ 0, 6, 3,

                        //====================================
                        0, 2, 0, /*|*/ 8, 7, 0, /*|*/ 0, 4, 9,
                        9, 0, 0, /*|*/ 0, 5, 2, /*|*/ 7, 0, 0,
                        0, 7, 1, /*|*/ 6, 0, 0, /*|*/ 0, 0, 0,
                    };

                    // Published: Cape Gazette, Fri, Jun 26 - Mon, Jun 29, 2015, p. 107
                    yield return new[]
                    {
                        //====================================
                        0, 2, 1, /*|*/ 0, 0, 0, /*|*/ 0, 0, 0,
                        0, 0, 0, /*|*/ 0, 2, 3, /*|*/ 8, 0, 4,
                        0, 0, 0, /*|*/ 6, 0, 0, /*|*/ 0, 0, 0,
                        //====================================
                        1, 0, 0, /*|*/ 0, 8, 7, /*|*/ 0, 0, 0,
                        0, 9, 5, /*|*/ 2, 0, 0, /*|*/ 0, 0, 0,
                        8, 0, 0, /*|*/ 0, 0, 0, /*|*/ 9, 6, 0,
                        //====================================
                        0, 0, 9, /*|*/ 0, 0, 0, /*|*/ 0, 4, 3,
                        0, 0, 0, /*|*/ 5, 6, 0, /*|*/ 0, 0, 8,
                        6, 0, 4, /*|*/ 0, 7, 0, /*|*/ 2, 0, 0,
                    };
                }

                return _integerArrayProblems ?? (_integerArrayProblems = GetAll().ToArray());
            }
        }
    }
}
