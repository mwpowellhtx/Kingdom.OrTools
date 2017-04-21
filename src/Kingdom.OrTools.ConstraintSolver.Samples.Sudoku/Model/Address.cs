using System;

namespace Kingdom.OrTools.ConstraintSolver.Samples.Sudoku
{
    /// <summary>
    /// Sets up a basic addressable convention for purposes of addressing the grid.
    /// </summary>
    public class Address : IEquatable<Address>
    {
        /// <summary>
        /// Gets or sets the Row.
        /// </summary>
        public int Row { get; set; }

        /// <summary>
        /// Gets or sets the Column.
        /// </summary>
        public int Column { get; set; }

        /// <summary>
        /// Default Constructor
        /// </summary>
        public Address()
            : this(default(int), default(int))
        {
        }

        /// <summary>
        /// Copy Constructor
        /// </summary>
        /// <param name="other"></param>
        public Address(Address other)
            : this(other.Row, other.Column)
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="row"></param>
        /// <param name="column"></param>
        public Address(int row, int column)
        {
            Row = row;
            Column = column;
        }

        #region Equatable Members

        /// <summary>
        /// Returns whether <paramref name="a"/> Equals <paramref name="b"/>.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool Equals(Address a, Address b)
        {
            return ReferenceEquals(a, b)
                   || (!(a == null || b == null)
                       && a.Row == b.Row && a.Column == b.Column);
        }

        /// <summary>
        /// Returns whether this object Equals an <paramref name="other"/> one.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public virtual bool Equals(Address other)
        {
            return Equals(this, other);
        }

        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return 3*Row ^ 5*Column;
        }
    }
}
